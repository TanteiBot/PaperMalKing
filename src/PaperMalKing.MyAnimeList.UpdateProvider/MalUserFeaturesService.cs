﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaperMalKing.Database;
using PaperMalKing.Database.Models.MyAnimeList;
using PaperMalKing.MyAnimeList.Wrapper.Abstractions;
using PaperMalKing.MyAnimeList.Wrapper.Abstractions.Models;
using PaperMalKing.UpdatesProviders.Base.Exceptions;
using PaperMalKing.UpdatesProviders.Base.Features;

namespace PaperMalKing.MyAnimeList.UpdateProvider;

internal sealed class MalUserFeaturesService : BaseUserFeaturesService<MalUser, MalUserFeatures>
{
	private readonly IMyAnimeListClient _client;

	public MalUserFeaturesService(IMyAnimeListClient client, ILogger<MalUserFeaturesService> logger,
								  IDbContextFactory<DatabaseContext> dbContextFactory) : base(dbContextFactory, logger)
	{
		this._client = client;
	}

	public override async Task EnableFeaturesAsync(MalUserFeatures feature, ulong userId)
	{
		using var db = this.DbContextFactory.CreateDbContext();
		var dbUser = db.MalUsers.TagWith("Query user for enabling feature").TagWithCallSite().FirstOrDefault(u => u.DiscordUser.DiscordUserId == userId);
		if (dbUser is null)
			throw new UserFeaturesException("You must register first before enabling features");
		if (dbUser.Features.HasFlag(feature))
		{
			throw new UriFormatException("You already have this feature enabled");
		}

		User? user = null;
		dbUser.Features |= feature;
		var now = DateTimeOffset.UtcNow;

		switch (feature)
		{
			case MalUserFeatures.AnimeList:
			{
				user = await this._client.GetUserAsync(dbUser.Username, dbUser.Features.ToParserOptions(), CancellationToken.None)
								 .ConfigureAwait(false);
				dbUser.LastAnimeUpdateHash = user.LatestAnimeUpdateHash ?? "";
				dbUser.LastUpdatedAnimeListTimestamp = now;
				break;
			}
			case MalUserFeatures.MangaList:
			{
				user = await this._client.GetUserAsync(dbUser.Username, dbUser.Features.ToParserOptions(), CancellationToken.None)
								 .ConfigureAwait(false);
				dbUser.LastMangaUpdateHash = user.LatestMangaUpdateHash ?? "";
				dbUser.LastUpdatedMangaListTimestamp = now;
				break;
			}
			case MalUserFeatures.Favorites:
			{
				user = await this._client.GetUserAsync(dbUser.Username, dbUser.Features.ToParserOptions(), CancellationToken.None)
								 .ConfigureAwait(false);
				dbUser.FavoriteAnimes = user.Favorites.FavoriteAnime.Select(x => x.ToMalFavoriteAnime(dbUser)).ToList();
				dbUser.FavoriteMangas = user.Favorites.FavoriteManga.Select(x => x.ToMalFavoriteManga(dbUser)).ToList();
				dbUser.FavoriteCharacters = user.Favorites.FavoriteCharacters.Select(x => x.ToMalFavoriteCharacter(dbUser)).ToList();
				dbUser.FavoritePeople = user.Favorites.FavoritePeople.Select(x => x.ToMalFavoritePerson(dbUser)).ToList();
				dbUser.FavoriteCompanies = user.Favorites.FavoriteCompanies.Select(x => x.ToMalFavoriteCompany(dbUser)).ToList();
				break;
			}
			default:
			{
				// No additional work needed
				break;
			}
		}

		await db.SaveChangesAndThrowOnNoneAsync(CancellationToken.None).ConfigureAwait(false);
	}

	protected override ValueTask DisableFeatureCleanupAsync(DatabaseContext db, MalUser user, MalUserFeatures featureToDisable)
	{
		if (featureToDisable == MalUserFeatures.Favorites)
		{
			db.BaseMalFavorites.TagWith("Remove user's favorites when Favourites feature gets disabled").TagWithCallSite().Where(x => x.UserId == user.UserId).ExecuteDelete();
		}

		return ValueTask.CompletedTask;
	}
}