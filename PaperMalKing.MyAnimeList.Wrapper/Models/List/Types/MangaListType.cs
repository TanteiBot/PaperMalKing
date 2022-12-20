﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PaperMalKing.Common.Enums;
using static PaperMalKing.MyAnimeList.Wrapper.MangaFieldsToRequest;


namespace PaperMalKing.MyAnimeList.Wrapper.Models.List.Types;

internal abstract class MangaListType : IListType
{
	public static ListEntryType ListEntryType => ListEntryType.Manga;

	public static string LatestUpdatesUrl<TRequestOptions>(string username, TRequestOptions options) where TRequestOptions : unmanaged, Enum
	{
		Debug.Assert(typeof(TRequestOptions) == typeof(MangaFieldsToRequest));
		var fields = Unsafe.As<TRequestOptions, MangaFieldsToRequest>(ref options);
		return
			$"/users/{username}/mangalist?fields=list_status{{status,score,num_volumes_read,num_chapters_read,is_rereading,num_times_reread,updated_at{(fields.Has(Tags) ? ",tags" : "")}{(fields.Has(Comments) ? ",comments" : "")}}},id,title,main_picture,media_type,status,num_volumes,num_chapters{(fields.Has(Synopsis) ? ",synopsis" : "")}{(fields.Has(Genres) ? ",genres" : "")}{(fields.Has(Authors) ? ",authors{first_name,last_name}" : "")}&limit=1000&sort=list_updated_at&nsfw=true";
	}
}