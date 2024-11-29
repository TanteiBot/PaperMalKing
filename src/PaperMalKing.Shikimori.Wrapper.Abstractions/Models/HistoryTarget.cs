﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Text.Json.Serialization;
using PaperMalKing.Common.Enums;
using PaperMalKing.Common.Json;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models;

public sealed class HistoryTarget : IMultiLanguageName
{
	private readonly string _url = null!;

	public ListEntryType Type { get; init; }

	[JsonConverter(typeof(StringPoolingJsonConverter))]
	public required string Status { get; init; }

	public ulong Id { get; init; }

	public string Url
	{
		get => this._url;
		init
		{
			this._url = Constants.BaseUrl + value;
			this.Type = value.Contains("/animes", StringComparison.OrdinalIgnoreCase) ? ListEntryType.Anime : ListEntryType.Manga;
			var entryType = this.Type == ListEntryType.Anime ? "animes" : "mangas";
			this.ImageUrl = Utils.GetImageUrl(entryType, this.Id);
		}
	}

	public uint? Episodes { get; init; }

	[JsonPropertyName("episodes_aired")]
	public uint? EpisodesAired { get; init; }

	public uint? Volumes { get; init; }

	public uint? Chapters { get; init; }

	[JsonConverter(typeof(StringPoolingJsonConverter))]
	public string? Kind { get; init; }

	public required string Name { get; init; }

	[JsonPropertyName("russian")]
	public string? RussianName { get; init; }

	[JsonIgnore]
	public string ImageUrl { get; private init; } = null!;
}