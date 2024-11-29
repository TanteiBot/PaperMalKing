// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Enums;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class MediaListEntry
{
	public MediaListStatus Status { get; init; }

	public ushort Repeat { get; init; }

	public string? Notes { get; init; }

	public Dictionary<string, float>? AdvancedScores { get; init; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public byte Point100Score { get; init; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public byte Point10Score { get; init; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public byte Point5Score { get; init; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public byte Point3Score { get; init; }

	public uint Id { get; init; }

	public IReadOnlyList<CustomList>? CustomLists { get; init; } = [];

	[SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Obvious from usage")]
	public string GetScore(ScoreFormat scoreFormat)
	{
		if (this.Point3Score is 0 && this.Point5Score is 0 && this.Point10Score is 0 && this.Point100Score is 0)
		{
			return "";
		}

		return scoreFormat switch
		{
			ScoreFormat.POINT_100 => $"{this.Point100Score}/100",
			ScoreFormat.POINT_10_DECIMAL => $"{(this.Point100Score * 1.0d / 10).ToString(CultureInfo.InvariantCulture)}/10",
			ScoreFormat.POINT_10 => $"{this.Point10Score}/10",
			ScoreFormat.POINT_5 => $"{this.Point5Score}/5",
			ScoreFormat.POINT_3 => this.Point3Score switch
			{
				1 => ":(",
				2 => ":|",
				3 => ":)",
				_ => throw new UnreachableException("Invalid data in AniList Point 3 score format"),
			},
			_ => throw new UnreachableException("Invalid score format"),
		};
	}
}