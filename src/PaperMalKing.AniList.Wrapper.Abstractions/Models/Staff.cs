// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;
using System.Text.Json.Serialization;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Interfaces;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class Staff : IImageble, ISiteUrlable, IIdentifiable
{
	public required GenericName Name { get; init; }

	[JsonPropertyName("siteUrl")]
	public required string Url { get; init; }

	public Image? Image { get; init; }

	public uint Id { get; init; }

	public string? Description { get; init; }

	public Connection<Media> StaffMedia { get; init; } = Connection<Media>.Empty;

	public IReadOnlyList<string> PrimaryOccupations { get; init; } = [];
}