#region Copyright (c) Pixeval/Pixeval
// GPL v3 License
// 
// Pixeval/Pixeval
// Copyright (c) 2023 Pixeval/MangaIndexMacro.cs
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

using Pixeval.Controls;
using Pixeval.Download.MacroParser;

namespace Pixeval.Download.Macros;

[MetaPathMacro<IWorkViewModel>]
public class PicSetIndexMacro : ITransducer<IWorkViewModel>, ILastSegment
{
    public const string NameConst = "pic_set_index";

    public const string NameConstToken = $"<{NameConst}>";

    public string Name => NameConst;

    public string Substitute(IWorkViewModel context)
    {
        // 下载单张漫画的时候，MangaIndex 不为 -1
        // 下载多张漫画或者单张插画的时候，为 -1
        return context is IllustrationItemViewModel { IsManga: true, MangaIndex: not -1 } i ? i.MangaIndex.ToString() : $"<{Name}>";
    }
}
