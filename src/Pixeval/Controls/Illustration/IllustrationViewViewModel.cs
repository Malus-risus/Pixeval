#region Copyright (c) Pixeval/Pixeval
// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2023 Pixeval/IllustrationViewViewModel.cs
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

using System.Collections.Generic;
using Pixeval.CoreApi.Model;
using IllustrationViewDataProvider = Pixeval.Controls.SharableViewDataProvider<
    Pixeval.CoreApi.Model.Illustration,
    Pixeval.Controls.IllustrationItemViewModel>;

namespace Pixeval.Controls;

public sealed partial class IllustrationViewViewModel : SortableEntryViewViewModel<Illustration, IllustrationItemViewModel>
{
    public IllustrationViewViewModel(IllustrationViewViewModel viewModel) : this(viewModel.DataProvider.CloneRef(), viewModel.BlockedTags)
    {
        Filter = viewModel.Filter;
        DataProvider.View.Range = viewModel.DataProvider.View.Range;
    }

    public IllustrationViewViewModel() : this(new IllustrationViewDataProvider(), App.AppViewModel.AppSettings.BlockedTags)
    {

    }

    private IllustrationViewViewModel(IllustrationViewDataProvider dataProvider, HashSet<string> blockedTags) : base(blockedTags)
    {
        DataProvider = dataProvider;
        dataProvider.View.Filter = DefaultFilter;
        dataProvider.View.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasNoItem));
    }

    public override IllustrationViewDataProvider DataProvider { get; }

    protected override void OnFilterChanged() => DataProvider.View.RaiseFilterChanged();
}
