﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;
using PaperMalKing.AniList.Wrapper.Abstractions.Models;

namespace PaperMalKing.AniList.UpdateProvider.CombinedResponses;

internal sealed class CombinedInitialInfoResponse
{
	public uint? UserId { get; set; }

	public List<IdentifiableFavourite> Favourites1 { get; } = [];

	public void Add(User user)
	{
		this.UserId ??= user.Id;

		this.Favourites1.AddRange(user.Favourites.AllFavourites);
	}
}