﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N
namespace PaperMalKing.MyAnimeList.Wrapper.Abstractions.Models.Favorites;

public sealed class FavoritePerson : BaseFavorite
{
	public FavoritePerson(BaseFavorite other) : base(other)
	{ }
}