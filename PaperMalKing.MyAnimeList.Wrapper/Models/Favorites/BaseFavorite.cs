﻿#region LICENSE
// PaperMalKing.
// Copyright (C) 2021-2022 N0D4N
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

namespace PaperMalKing.MyAnimeList.Wrapper.Models.Favorites
{
	internal class BaseFavorite
	{
		internal MalUrl Url { get; init; }

		internal string Name { get; init; }

		internal string? ImageUrl { get; init; }

		internal BaseFavorite(MalUrl url, string name, string? imageUrl)
		{
			this.Url = url;
			this.Name = name;
			this.ImageUrl = imageUrl;
		}

		internal BaseFavorite(BaseFavorite other) : this(other.Url, other.Name, other.ImageUrl)
		{ }
	}
}