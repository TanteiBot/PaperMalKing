﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N
using System.Text.Json.Serialization;

namespace PaperMalKing.AniList.Wrapper.Models;

public sealed class MediaListTypeOptions
{
	[JsonPropertyName("advancedScoringEnabled")]
	public bool IsAdvancedScoringEnabled { get; init; }
}