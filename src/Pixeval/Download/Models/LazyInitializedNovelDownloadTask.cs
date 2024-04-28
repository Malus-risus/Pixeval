#region Copyright

// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2024 Pixeval/LazyInitializedNovelDownloadTask.cs
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

using System.Threading.Tasks;
using Pixeval.Database;

namespace Pixeval.Download.Models;

public class LazyInitializedNovelDownloadTask(DownloadHistoryEntry entry)
    : NovelDownloadTask(entry, null!, null!, null!), ILazyLoadDownloadTask
{
    private readonly long _novelId = entry.Id;

    public override async Task DownloadAsync(Downloader downloadStreamAsync)
    {
        await LazyLoadAsync(_novelId);

        await base.DownloadAsync(downloadStreamAsync);
    }

    public async Task LazyLoadAsync(long id)
    {
        NovelItemViewModel ??= new(await App.AppViewModel.MakoClient.GetNovelFromIdAsync(id));
        NovelContent ??= await App.AppViewModel.MakoClient.GetNovelContentAsync(id);
        DocumentViewModel ??= new(NovelContent);
    }
}