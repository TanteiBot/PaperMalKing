﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaperMalKing.AniList.Wrapper;
using PaperMalKing.Common.Attributes;
using PaperMalKing.Database;
using PaperMalKing.Database.Models.AniList;
using PaperMalKing.UpdatesProviders.Base.Exceptions;
using PaperMalKing.UpdatesProviders.Base.Features;

namespace PaperMalKing.AniList.UpdateProvider;

public sealed class AniListUserFeaturesService : IUserFeaturesService<AniListUserFeatures>
{
	private readonly AniListClient _client;
	private readonly ILogger<AniListUserFeaturesService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly Dictionary<AniListUserFeatures, (string, string)> Descriptions = new();

	public AniListUserFeaturesService(AniListClient client, ILogger<AniListUserFeaturesService> logger, IServiceProvider serviceProvider)
	{
		this._client = client;
		this._logger = logger;
		this._serviceProvider = serviceProvider;

		var t = typeof(AniListUserFeatures);
		var ti = t.GetTypeInfo();
		var values = Enum.GetValues(t).Cast<AniListUserFeatures>().Where(v => v != AniListUserFeatures.None);

		foreach (var enumVal in values)
		{
			var name = enumVal.ToString();
			var fieldVal = ti.DeclaredMembers.First(xm => xm.Name == name);
			var attribute = fieldVal.GetCustomAttribute<FeatureDescriptionAttribute>()!;

			this.Descriptions[enumVal] = (attribute.Description, attribute.Summary);
		}
	}

	IReadOnlyDictionary<AniListUserFeatures, (string, string)> IUserFeaturesService<AniListUserFeatures>.Descriptions => this.Descriptions;

	public async Task EnableFeaturesAsync(IReadOnlyList<AniListUserFeatures> features, ulong userId)
	{
		using var scope = this._serviceProvider.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
		var dbUser = db.AniListUsers.Include(u => u.Favourites).FirstOrDefault(u => u.DiscordUserId == userId);
		if (dbUser == null)
			throw new UserFeaturesException("You must register first before enabling features");
		var total = features.Aggregate((acc, next) => acc | next);
		dbUser.Features |= total;
		var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		for (var i = 0; i < features.Count; i++)
		{
			var feature = features[i];
			switch (feature)
			{
				case AniListUserFeatures.AnimeList:
				case AniListUserFeatures.MangaList:
					{
						dbUser.LastActivityTimestamp = now;
						break;
					}
				case AniListUserFeatures.Favourites:
					{
						var fr = await this._client.GetAllRecentUserUpdatesAsync(dbUser, AniListUserFeatures.Favourites, CancellationToken.None)
										   .ConfigureAwait(false);
						dbUser.Favourites.Clear();
						dbUser.Favourites.AddRange(fr.Favourites.Select(f => new AniListFavourite { Id = f.Id, FavouriteType = (FavouriteType)f.Type })
													 .ToList());
						break;
					}
				case AniListUserFeatures.Reviews:
					{
						dbUser.LastReviewTimestamp = now;
						break;
					}
			}
		}


		db.AniListUsers.Update(dbUser);
		await db.SaveChangesAndThrowOnNoneAsync(CancellationToken.None).ConfigureAwait(false);
	}

	public async Task DisableFeaturesAsync(IReadOnlyList<AniListUserFeatures> features, ulong userId)
	{
		using var scope = this._serviceProvider.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
		var dbUser = db.AniListUsers.Include(su => su.Favourites).FirstOrDefault(su => su.DiscordUserId == userId);
		if (dbUser == null)
			throw new UserFeaturesException("You must register first before disabling features");

		var total = features.Aggregate((acc, next) => acc | next);

		dbUser.Features &= ~total;
		if (features.Any(x => x == AniListUserFeatures.Favourites))
			dbUser.Favourites.Clear();

		db.AniListUsers.Update(dbUser);
		await db.SaveChangesAndThrowOnNoneAsync(CancellationToken.None).ConfigureAwait(false);
	}

	public ValueTask<string> EnabledFeaturesAsync(ulong userId)
	{
		using var scope = this._serviceProvider.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
		var dbUser = db.AniListUsers.AsNoTrackingWithIdentityResolution().FirstOrDefault(su => su.DiscordUserId == userId);
		if (dbUser == null)
			throw new UserFeaturesException("You must register first before checking for enabled features");

		return ValueTask.FromResult(dbUser.Features.Humanize());
	}
}