// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

namespace PaperMalKing.MyAnimeList.Wrapper.Abstractions.Models.List.Official;

public sealed class Picture
{
	public string? Large { get; init; }

	public required string Medium { get; init; }
}