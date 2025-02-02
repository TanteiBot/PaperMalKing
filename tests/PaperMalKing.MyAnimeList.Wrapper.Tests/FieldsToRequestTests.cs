// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using PaperMalKing.MyAnimeList.Wrapper.Abstractions;

namespace PaperMalKing.MyAnimeList.Wrapper.Tests;

public sealed class FieldsToRequestTests
{
	[Fact]
	public void MangaFieldsToRequestAndAnimeFieldsToRequestHaveSameStartingValues()
	{
		const int enumStart = 4;
		var aftr = Enum.GetNames<AnimeFieldsToRequest>();
		var mftr = Enum.GetNames<MangaFieldsToRequest>();
		Assert.True(aftr.AsSpan()[..enumStart].SequenceEqual(mftr.AsSpan()[..enumStart]));
	}

	[Fact]
	public void FieldsToRequestEnumsHaveByteAsUnderlyingType()
	{
		static void Check(Type t)
		{
			Assert.Equal(typeof(byte), Enum.GetUnderlyingType(t));
		}

		Check(typeof(MangaFieldsToRequest));
		Check(typeof(AnimeFieldsToRequest));
	}
}