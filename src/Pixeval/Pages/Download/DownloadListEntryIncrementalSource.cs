#region Copyright (c) Pixeval/Pixeval
// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2023 Pixeval/DownloadListEntryIncrementalSource.cs
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pixeval.CoreApi.Model;
using Pixeval.Download;
using Pixeval.Misc;
using Pixeval.Utilities;

namespace Pixeval.Pages.Download;

public class DownloadListEntryIncrementalSource(IEnumerable<ObservableDownloadTask> source) : FetchEngineIncrementalSource<Illustration, DownloadListEntryViewModel>(null!)
{
    protected override long Identifier(Illustration entity) => throw new System.NotImplementedException();

    protected override DownloadListEntryViewModel Select(Illustration entity) => throw new System.NotImplementedException();

    public override async Task<IEnumerable<DownloadListEntryViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = new CancellationToken())
    {
        return await source
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(async o => new DownloadListEntryViewModel(o, await App.AppViewModel.MakoClient.GetIllustrationFromIdAsync(o.Id!)))
            .WhenAll();
    }
}
