﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaperMalKing.Database.Models.AniList;

public sealed class AniListUser
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public ulong Id { get; init; }

	public long LastActivityTimestamp { get; set; }

	public long LastReviewTimestamp { get; set; }

	public ulong DiscordUserId { get; init; }

	public AniListUserFeatures Features { get; set; }

	public required DiscordUser DiscordUser { get; init; }

	public required List<AniListFavourite> Favourites { get; init; }
}