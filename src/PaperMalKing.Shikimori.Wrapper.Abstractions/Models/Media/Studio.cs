// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Text.Json.Serialization;
using PaperMalKing.Common.Json;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models.Media;

public sealed class Studio
{
	public required uint Id { get; init; }

	[JsonConverter(typeof(StringPoolingJsonConverter))]
	public required string Name { get; init; }

	public string Url => Utils.GetUrl("animes/studio", this.Id);

	public string Image => Utils.GetImageUrl("studios", this.Id);
}