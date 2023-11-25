#region Copyright (c) Pixeval/Pixeval
// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2023 Pixeval/IllustratorViewModel.cs
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Pixeval.Controls.Illustrate;
using Pixeval.Controls.MarkupExtensions;
using Pixeval.CoreApi.Global.Enum;
using Pixeval.CoreApi.Model;
using Pixeval.CoreApi.Net;
using Pixeval.CoreApi.Net.Response;
using Pixeval.Options;
using Pixeval.Util;
using Pixeval.Util.IO;
using Pixeval.Util.Threading;
using Pixeval.Util.UI;
using Pixeval.Utilities;
using AppContext = Pixeval.AppManagement.AppContext;

namespace Pixeval.Controls;

public sealed partial class IllustratorViewModel : IllustrateViewModel<User>
{
    // Dominant color of the "No Image" image
    public static readonly SolidColorBrush DefaultAvatarBorderColorBrush = new(UiHelper.ParseHexColor("#D6DEE5"));

    private readonly TaskCompletionSource<SoftwareBitmapSource[]> _bannerImageTaskCompletionSource;

    public IllustratorViewModel(User user) : base(user)
    {
        _bannerImageTaskCompletionSource = new();
        IsFollowed = Illustrate.UserInfo?.IsFollowed ?? false;

        SetAvatarAsync().Discard();
        SetBannerSourceAsync().Discard();

        InitializeCommands();
    }

    public PixivSingleUserResponse? UserDetail { get; private set; }

    public Task<SoftwareBitmapSource[]> BannerImageTask => _bannerImageTaskCompletionSource.Task;

    [ObservableProperty]
    private ImageSource? _avatarSource;

    [ObservableProperty]
    private Brush? _avatarBorderBrush;

    public string Username => Illustrate.UserInfo?.Name ?? "";

    public string UserId => Illustrate.UserInfo?.Id.ToString() ?? "";

    [ObservableProperty]
    private bool _isFollowed;

    public XamlUICommand FollowCommand { get; set; }

    public XamlUICommand ShareCommand { get; set; }

    public string GetIllustrationToolTipSubtitleText(User? user)
    {
        return user?.UserInfo?.Comment ?? IllustratorProfileResources.UserHasNoComment;
    }

    private async Task SetAvatarAsync()
    {
        var avatar = Illustrate.UserInfo?.ProfileImageUrls?.Medium?.Let(url => App.AppViewModel.MakoClient.DownloadBitmapImageResultAsync(url, 100)) ?? Task.FromResult(Result<ImageSource>.OfFailure());
        AvatarSource = await avatar.GetOrElseAsync(await AppContext.GetPixivNoProfileImageAsync());
    }

    private async Task SetBannerSourceAsync()
    {
        var client = App.AppViewModel.MakoClient.GetMakoHttpClient(MakoApiKind.ImageApi);
        if (Illustrate.Illusts?.Take(3).ToArray() is { Length: > 0 } illustrations && illustrations.SelectNotNull(c => c.GetThumbnailUrl(ThumbnailUrlOption.SquareMedium)).ToArray() is { Length: > 0 } urls)
        {
            var tasks = await Task.WhenAll(urls.Select(u => client.DownloadAsIRandomAccessStreamAsync(u)));
            if (tasks is [Result<IRandomAccessStream>.Success(var first), ..])
            {
                var dominantColor = await UiHelper.GetDominantColorAsync(first.AsStreamForRead(), false);
                AvatarBorderBrush = new SolidColorBrush(dominantColor);
            }

            var result = (await Task.WhenAll(tasks.SelectNotNull(r => r.BindAsync(s => s.GetSoftwareBitmapSourceAsync(true)))))
                .SelectNotNull(res => res.GetOrElse(null)).ToArray();
            _ = _bannerImageTaskCompletionSource.TrySetResult(result);
            return;
        }

        UserDetail = await App.AppViewModel.MakoClient.GetUserFromIdAsync(Illustrate.UserInfo?.Id.ToString() ?? string.Empty, App.AppViewModel.AppSetting.TargetFilter);
        if (UserDetail.UserProfile?.BackgroundImageUrl is { } url && await client.DownloadAsIRandomAccessStreamAsync(url) is Result<IRandomAccessStream>.Success(var stream))
        {
            var managedStream = stream.AsStreamForRead();
            var dominantColor = await UiHelper.GetDominantColorAsync(managedStream, false);
            AvatarBorderBrush = new SolidColorBrush(dominantColor);
            var result = Enumerates.ArrayOf(await stream.GetSoftwareBitmapSourceAsync(true));
            _ = _bannerImageTaskCompletionSource.TrySetResult(result);
            return;
        }

        AvatarBorderBrush = DefaultAvatarBorderColorBrush;
        _ = _bannerImageTaskCompletionSource.TrySetResult(Enumerates.ArrayOf(await AppContext.GetPixivNoProfileImageAsync()));
    }

    // private void SetMetrics()
    // {
    //     var followings = UserDetail!.UserProfile?.TotalFollowUsers ?? 0;
    //     var myPixivUsers = UserDetail.UserProfile?.TotalMyPixivUsers ?? 0;
    //     var illustrations = UserDetail.UserProfile?.TotalIllusts ?? 0;
    //     Metrics = new UserMetrics(followings, myPixivUsers, illustrations);
    // }

    private string FollowText => IsFollowed ? IllustratorProfileResources.Unfollow : IllustratorProfileResources.Follow;

    private void InitializeCommands()
    {
        FollowCommand = FollowText.GetCommand(MakoHelper.GetFollowButtonIcon(IsFollowed));
        ShareCommand = IllustratorProfileResources.Share.GetCommand(FontIconSymbols.ShareE72D);

        FollowCommand.ExecuteRequested += FollowCommandOnExecuteRequested;
        // TODO: ShareCommand
    }

    private void FollowCommandOnExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        SwitchFollowState();
        FollowCommand.Label = IsFollowed ? IllustratorProfileResources.Unfollow : IllustratorProfileResources.Follow;
        FollowCommand.IconSource = MakoHelper.GetFollowButtonIcon(IsFollowed);
    }

    private void SwitchFollowState()
    {
        if (IsFollowed)
            Unfollow();
        else
            Follow();
    }

    private void Follow()
    {
        IsFollowed = true;
        _ = App.AppViewModel.MakoClient.PostFollowUserAsync(UserId, PrivacyPolicy.Public);
    }

    private void Unfollow()
    {
        IsFollowed = false;
        _ = App.AppViewModel.MakoClient.RemoveFollowUserAsync(UserId);
    }

    public override void Dispose()
    {
        _ = _bannerImageTaskCompletionSource.Task.ContinueWith(s => s.Dispose());
        _ = BannerImageTask.ContinueWith(i => i.Dispose());
    }
}
