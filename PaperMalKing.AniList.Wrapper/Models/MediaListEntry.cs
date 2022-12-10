﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using PaperMalKing.AniList.Wrapper.Models.Enums;

namespace PaperMalKing.AniList.Wrapper.Models;

public sealed class MediaListEntry
{
	private readonly Dictionary<ScoreFormat, byte> _scores = new(4);

	[JsonPropertyName("status")]
	public MediaListStatus Status { get; init; }

	[JsonPropertyName("repeat")]
	public ushort Repeat { get; init; }

	[JsonPropertyName("notes")]
	public string? Notes { get; init; }

	[JsonPropertyName("advancedScores")]
	public Dictionary<string, float>? AdvancedScores { get; init; }

	[JsonPropertyName("point100Score")]
	public byte Point100Score
	{
		[Obsolete("", true), EditorBrowsable(EditorBrowsableState.Never)]
		get => throw new NotSupportedException();
		init => this._scores.Add(ScoreFormat.POINT_100, value);
	}

	[JsonPropertyName("point10Score")]
	public byte Point10Score
	{
		[Obsolete("", true), EditorBrowsable(EditorBrowsableState.Never)]
		get => throw new NotSupportedException();
		init => this._scores.Add(ScoreFormat.POINT_10, value);
	}

	[JsonPropertyName("point5Score")]
	public byte Point5Score
	{
		[Obsolete("", true), EditorBrowsable(EditorBrowsableState.Never)]
		get => throw new NotSupportedException();
		init => this._scores.Add(ScoreFormat.POINT_5, value);
	}

	[JsonPropertyName("point3Score")]
	public byte Point3Score
	{
		[Obsolete("", true), EditorBrowsable(EditorBrowsableState.Never)]
		get => throw new NotSupportedException();
		init => this._scores.Add(ScoreFormat.POINT_3, value);
	}

	[JsonPropertyName("id")]
	public ulong Id { get; init; }

	[JsonPropertyName("customLists")]
	public IReadOnlyList<CustomList>? CustomLists { get; init; } = Array.Empty<CustomList>();

	public string GetScore(ScoreFormat scoreFormat)
	{
		if (this._scores.Values.All(s => s == 0))
			return "";
		return scoreFormat switch
		{
			ScoreFormat.POINT_100 => $"{this._scores[scoreFormat]}/100",
			ScoreFormat.POINT_10_DECIMAL => $"{(this._scores[ScoreFormat.POINT_100] * 1.0d / 10).ToString(CultureInfo.InvariantCulture)}/10",
			ScoreFormat.POINT_10 => $"{this._scores[scoreFormat]}/10",
			ScoreFormat.POINT_5 => $"{this._scores[scoreFormat]}/5",
			ScoreFormat.POINT_3 => this._scores[scoreFormat] switch
			{
				1 => ":(",
				2 => ":|",
				3 => ":)",
				_ => throw new ArgumentOutOfRangeException(nameof(scoreFormat), scoreFormat, null)
			},
			_ => throw new ArgumentOutOfRangeException(nameof(scoreFormat), scoreFormat, null)
		};
	}
}