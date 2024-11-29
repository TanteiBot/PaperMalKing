// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models.Media;

public sealed class MangaMedia : BaseMedia
{
	public IReadOnlyList<Publisher> Publishers { get; init; } = [];

	protected override string Type => "mangas";
}