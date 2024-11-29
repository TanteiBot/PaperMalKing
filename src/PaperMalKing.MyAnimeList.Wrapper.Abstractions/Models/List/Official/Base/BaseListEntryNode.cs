// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PaperMalKing.Common.Json;

namespace PaperMalKing.MyAnimeList.Wrapper.Abstractions.Models.List.Official.Base;

public abstract class BaseListEntryNode<TMediaType, TStatus>
	where TMediaType : unmanaged, Enum
	where TStatus : unmanaged, Enum
{
	public required uint Id { get; init; }

	[JsonConverter(typeof(ClearableStringPoolingJsonConverter))]
	public required string Title { get; init; }

	[JsonPropertyName("main_picture")]
	public Picture? Picture { get; init; }

	public string? Synopsis { get; init; }

	public IReadOnlyList<Genre>? Genres { get; init; }

	[JsonPropertyName("media_type")]
	public required TMediaType MediaType { get; init; }

	public required TStatus Status { get; init; }

	public abstract string Url { get; }

	public abstract uint TotalSubEntries { get; }
}