﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaperMalKing.Database.Models.Shikimori;

public sealed class ShikiUser
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public ulong Id { get; init; }

	public ulong LastHistoryEntryId { get; set; }

	public ulong DiscordUserId { get; init; }

	public ShikiUserFeatures Features { get; set; }

	public required DiscordUser DiscordUser { get; init; }

	public required List<ShikiFavourite> Favourites { get; set; }
}