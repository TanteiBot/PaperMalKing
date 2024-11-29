// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Text.Json.Serialization;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Interfaces;
using PaperMalKing.Common.Json;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class Studio : ISiteUrlable, IIdentifiable
{
	[JsonConverter(typeof(StringPoolingJsonConverter))]
	public required string Name { get; init; }

	[JsonPropertyName("siteUrl")]
	public required string Url { get; init; }

	public Connection<Media> Media { get; init; } = Connection<Media>.Empty;

	public bool IsAnimationStudio { get; init; }

	public uint Id { get; init; }
}