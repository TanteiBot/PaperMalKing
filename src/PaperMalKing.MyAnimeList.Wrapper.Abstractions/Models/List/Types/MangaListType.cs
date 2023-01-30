﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using PaperMalKing.Common.Enums;
using static PaperMalKing.MyAnimeList.Wrapper.Abstractions.MangaFieldsToRequest;


namespace PaperMalKing.MyAnimeList.Wrapper.Abstractions.Models.List.Types;

public abstract class MangaListType : IListType
{
	public static ListEntryType ListEntryType => ListEntryType.Manga;

	public static string LatestUpdatesUrl<TRequestOptions>(string username, TRequestOptions options) where TRequestOptions : unmanaged, Enum
	{
		Debug.Assert(typeof(TRequestOptions) == typeof(MangaFieldsToRequest));
		var fields = Unsafe.As<TRequestOptions, MangaFieldsToRequest>(ref options);
		return
			$"/users/{username}/mangalist?fields=list_status{{status,score,num_volumes_read,num_chapters_read,is_rereading,num_times_reread,updated_at{(fields.Has(Tags) ? ",tags" : "")}{(fields.Has(Comments) ? ",comments" : "")}{(fields.Has(Dates) ? ",start_date,finish_date" : "")}}},id,title,main_picture,media_type,status,num_volumes,num_chapters{(fields.Has(Synopsis) ? ",synopsis" : "")}{(fields.Has(Genres) ? ",genres{name}" : "")}{(fields.Has(Authors) ? ",authors{first_name,last_name}" : "")}&limit=100&sort=list_updated_at&nsfw=true";
	}
}