// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Text.Json.Serialization;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Interfaces;
using PaperMalKing.Common.Json;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class User : ISiteUrlable, IImageble
{
	public uint Id { get; init; }

	[JsonConverter(typeof(ClearableStringPoolingJsonConverter))]
	public string? Name { get; init; }

	[JsonPropertyName("siteUrl")]
	public required string Url { get; init; }

	public Image? Image { get; init; }

	public UserOptions Options { get; init; } = null!;

	public MediaListOptions? MediaListOptions { get; init; }

	public Favourites Favourites { get; init; } = Favourites.Empty;
}