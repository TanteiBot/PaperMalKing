// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "We follow AniListNaming")]
public sealed class MediaListCollection
{
	public static MediaListCollection Empty { get; } = new();

	public IReadOnlyList<MediaListGroup> Lists { get; init; } = [];
}