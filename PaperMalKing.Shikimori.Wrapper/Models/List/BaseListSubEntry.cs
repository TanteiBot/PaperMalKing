﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N
using System.Text.Json.Serialization;

namespace PaperMalKing.Shikimori.Wrapper.Models.List;

internal abstract class BaseListSubEntry
{
	protected abstract string Type { get; }

	public abstract string TotalAmount { get; }

	[JsonPropertyName("id")]
	public ulong Id { get; init; }

	[JsonPropertyName("name")]
	public required string Name { get; init; }

	[JsonPropertyName("kind")]
	public required string Kind { get; init; }

	[JsonPropertyName("status")]
	public SubEntryReleasingStatus Status { get; init; }

	public string Url => Utils.GetUrl(this.Type, this.Id);

	public string ImageUrl => Utils.GetImageUrl(this.Type, this.Id);
}