﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Text.Json.Serialization;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Enums;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Interfaces;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class Review : ISiteUrlable
{
	[JsonPropertyName("createdAt")]
	public long CreatedAtTimeStamp { get; init; }

	[JsonPropertyName("siteUrl")]
	public required string Url { get; init; }

	public string? Summary { get; init; }

	public required Media Media { get; init; }

	public MediaFormat Format { get; init; }
}