// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class CustomList
{
	public required string Name { get; init; }

	public bool Enabled { get; init; }
}