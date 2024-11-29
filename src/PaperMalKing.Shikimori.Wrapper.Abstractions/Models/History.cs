// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Text.Json.Serialization;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models;

public sealed class History
{
	public uint Id { get; init; }

	[JsonPropertyName("created_at")]
	public DateTimeOffset CreatedAt { get; init; }

	public required string Description { get; init; }

	public HistoryTarget? Target { get; init; }
}