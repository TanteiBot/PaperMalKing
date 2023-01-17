// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using CommunityToolkit.HighPerformance.Buffers;

namespace PaperMalKing.Common.Json;

/// <summary>
/// This string converter should be applied to strings, that are expected to not be present in incoming data in some time
/// For example, media titles, that are constantly returned by update provider, but can be not present in response if user updated their list
/// And so on.
/// As well as <see cref="StringPoolingJsonConverter"/> it should be applied on small strings only, see docs on <see cref="StringPoolingJsonConverter"/>
/// </summary>
public sealed class ClearableStringPoolingJsonConverter : JsonConverter<string>
{
	private static readonly StringPool _stringPool = new();

	[SuppressMessage("Performance", "CA1823:Avoid unused private fields")]
	private static readonly Timer _timer = new Timer(_ => _stringPool.Reset(), null, dueTime: TimeSpan.Zero, TimeSpan.FromHours(3));

	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return StringPoolingJsonConverter.ReadStringOrGetFromPool(ref reader, _stringPool);
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{ }
}