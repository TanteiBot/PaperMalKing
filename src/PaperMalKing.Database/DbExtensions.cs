﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PaperMalKing.Database.Models.AniList;
using PaperMalKing.Database.Models.MyAnimeList;
using PaperMalKing.Database.Models.Shikimori;

namespace PaperMalKing.Database;

public static class DbExtensions
{
	public static async Task<int> SaveChangesAndThrowOnNoneAsync(this DbContext context, CancellationToken cancellationToken = default)
	{
		var rows = await context.TrySaveChangesUntilDatabaseIsUnlockedAsync(cancellationToken).ConfigureAwait(false);
		if (rows <= 0)
			throw new NoChangesSavedException(context);
		return rows;
	}

	public static async Task<int> TrySaveChangesUntilDatabaseIsUnlockedAsync(this DbContext context, CancellationToken cancellationToken = default)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			try
			{
				return context.SaveChanges();
			}
			catch (SqliteException ex) when (ex.SqliteErrorCode == 5) // Database is locked
			{
				await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
			}
		}
		throw new TaskCanceledException("Saving changes were cancelled");
	}
}