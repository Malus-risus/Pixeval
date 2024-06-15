#region Copyright

// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2024 Pixeval/IntrinsicNovelDownloadTask.cs
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
using Pixeval.Controls;
using Pixeval.Database;

namespace Pixeval.Download.Models;

public sealed partial class IntrinsicNovelDownloadTask : NovelDownloadTask
{
    public IntrinsicNovelDownloadTask(DownloadHistoryEntry entry, NovelItemViewModel novel, DocumentViewerViewModel viewModel) :
        base(entry, novel, viewModel.NovelContent, viewModel) =>
        Report(90);

    public override async Task DownloadAsync(Downloader downloadStreamAsync)
    {
        await ManageResult();
    }
}
