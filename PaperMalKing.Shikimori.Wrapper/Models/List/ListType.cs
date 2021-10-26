﻿#region LICENSE
// PaperMalKing.
// Copyright (C) 2021 N0D4N
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using PaperMalKing.Common.Enums;

namespace PaperMalKing.Shikimori.Wrapper.Models.List;

internal readonly struct AnimeListType : IListType
{
	/// <inheritdoc />
	public string ListType => "anime_rates";

	/// <inheritdoc />
	public ListEntryType ListEntryType => ListEntryType.Anime;
}

internal readonly struct MangaListType : IListType
{
	/// <inheritdoc />
	public string ListType => "manga_rates";

	/// <inheritdoc />
	public ListEntryType ListEntryType => ListEntryType.Manga;
}

internal interface IListType
{
	string ListType { get; }
	ListEntryType ListEntryType { get; }
}