// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;
using System.Text.Json.Serialization;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Enums;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Interfaces;
using PaperMalKing.Common.Json;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;
#pragma warning disable CA1724, S4041

// The type name Media conflicts in whole or in part with the namespace name 'System.Media' defined in the .NET Framework. Rename the type to eliminate the conflict.
public sealed class Media : IImageble, ISiteUrlable, IIdentifiable
#pragma warning restore
{
	public uint Id { get; init; }

	public required MediaTitle Title { get; init; }

	public ListType Type { get; init; }

	[JsonPropertyName("siteUrl")]
	public required string Url { get; init; }

	public MediaFormat? Format { get; init; }

	[JsonConverter(typeof(StringPoolingJsonConverter))]
	public string? CountryOfOrigin { get; init; }

	public MediaStatus Status { get; init; }

	public ushort? Episodes { get; init; }

	public ushort? Chapters { get; init; }

	public ushort? Volumes { get; init; }

	public Image? Image { get; init; }

	public string? Description { get; init; }

	/// <remarks>
	/// Apply <see cref="StringPoolingJsonConverter"/> when https://github.com/dotnet/runtime/issues/54189 gets closed
	/// Currently we cant apply custom converter for collection item.
	/// </remarks>
	public IReadOnlyList<string> Genres { get; init; } = [];

	public IReadOnlyList<MediaTag> Tags { get; init; } = [];

	public Connection<Studio> Studios { get; init; } = Connection<Studio>.Empty;

	public Connection<CharacterEdge> Characters { get; init; } = Connection<CharacterEdge>.Empty;

	public Connection<StaffEdge> Staff { get; init; } = Connection<StaffEdge>.Empty;
}