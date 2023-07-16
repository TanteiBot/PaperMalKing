﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using System.Text.Json.Serialization;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class StaffEdge
{
	[JsonPropertyName("role")]
	public required string Role { get; init; }

	[JsonPropertyName("node")]
	public required Staff Staff { get; init; }
}