﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace PaperMalKing.Common;

public static class Helpers
{
	public static string ToDiscordMention(ulong id) => $"<@!{id}>";

	public static string FavoritesHash(ReadOnlySpan<FavoriteIdType> ids)
	{
		Span<byte> shaHashDestination = stackalloc byte[SHA512.HashSizeInBytes];
		Span<byte> buffer = stackalloc byte[sizeof(uint) + 1 + sizeof(byte) + 1];
		buffer[^3] = (byte)',';
		buffer[^1] = (byte)')';

		using var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);
		for (var i = 0; i < ids.Length; i++)
		{
			var id = ids[i];
			BinaryPrimitives.WriteUInt32LittleEndian(buffer,id.Id);

			buffer[^2] = id.Type;

			incrementalHash.AppendData(buffer);
		}

		incrementalHash.GetCurrentHash(shaHashDestination);
		return Convert.ToHexString(shaHashDestination);
	}
}