// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Text.Json.Serialization;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models.Media;

public sealed class Person : IMultiLanguageName
{
	public required uint Id { get; init; }

	public required string Name { get; init; }

	[JsonPropertyName("russian")]
	public required string RussianName { get; init; }

	public bool IsMangaka { get; init; }

	public string Url => Utils.GetUrl("people", this.Id);
}