// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using PaperMalKing.MyAnimeList.Wrapper.Abstractions.Converters;

namespace PaperMalKing.MyAnimeList.Wrapper.Abstractions.Models.List.Official.Base;

public abstract class BaseListEntryStatus<TListStatus>
	where TListStatus : unmanaged, Enum
{
	public byte GetStatusAsUnderlyingType()
	{
		Debug.Assert(Enum.GetUnderlyingType(typeof(TListStatus)) == typeof(byte), $"{nameof(TListStatus)} must be a {nameof(Byte)}");
		var status = this.Status;
		return Unsafe.As<TListStatus, byte>(ref status);
	}

	public required TListStatus Status { get; init; }

	public required byte Score { get; init; }

	public abstract ulong ProgressedSubEntries { get; }

	public abstract bool IsReprogressing { get; }

	public abstract ulong ReprogressTimes { get; }

	public IReadOnlyList<string>? Tags { get; init; }

	public string? Comments { get; init; }

	[JsonPropertyName("updated_at")]
	public required DateTimeOffset UpdatedAt { get; init; }

	[JsonPropertyName("start_date")]
	[JsonConverter(typeof(DateOnlyFromMalConverter))]
	public DateOnly? StartDate { get; init; }

	[JsonPropertyName("finish_date")]
	[JsonConverter(typeof(DateOnlyFromMalConverter))]
	public DateOnly? FinishDate { get; init; }
}