// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models.Media;

public abstract class BaseMedia
{
	public IReadOnlyList<Genre> Genres { get; init; } = [];

	public string? Description { get; init; }

	public IReadOnlyList<Role> PersonRoles { get; init; } = [];

	protected abstract string Type { get; }
}