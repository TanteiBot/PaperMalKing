﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Humanizer;
using PaperMalKing.Common;
using PaperMalKing.Common.Enums;
using PaperMalKing.Database.Models.Shikimori;
using PaperMalKing.Shikimori.Wrapper;
using PaperMalKing.Shikimori.Wrapper.Models;

namespace PaperMalKing.Shikimori.UpdateProvider;

internal static class Extensions
{
	private static readonly DiscordEmbedBuilder.EmbedFooter ShikiUpdateProviderFooter = new()
	{
		Text = "Shikimori",
		IconUrl = Constants.ICON_URL
	};

	private static readonly Dictionary<ProgressType, DiscordColor> Colors = new()
	{
		{ProgressType.Completed, Constants.ShikiGreen},
		{ProgressType.Dropped, Constants.ShikiRed},
		{ProgressType.InPlans, Constants.ShikiBlue},
		{ProgressType.InProgress, Constants.ShikiBlue},
		{ProgressType.OnHold, Constants.ShikiGrey}
	};

	public static DiscordEmbedBuilder WithShikiAuthor(this DiscordEmbedBuilder builder, UserInfo user) =>
		builder.WithAuthor(user.Nickname, user.Url, user.ImageUrl);

	public static async Task<List<History>> GetAllUserHistoryAfterEntryAsync(this ShikiClient client, ulong userId, ulong limitHistoryEntryId,
																			 ShikiUserFeatures features,
																			 CancellationToken cancellationToken = default)
	{
		uint page = 1;
		byte limit = 10;
		var options = features switch
		{
			_ when features.HasFlag(ShikiUserFeatures.AnimeList) && features.HasFlag(ShikiUserFeatures.MangaList) => HistoryRequestOptions.Any,
			_ when features.HasFlag(ShikiUserFeatures.AnimeList) && !features.HasFlag(ShikiUserFeatures.MangaList) => HistoryRequestOptions.Anime,
			_ when features.HasFlag(ShikiUserFeatures.MangaList) && !features.HasFlag(ShikiUserFeatures.AnimeList) => HistoryRequestOptions.Manga,
			_ => throw new ArgumentOutOfRangeException(nameof(features), features, null)
		};

		var (data, hasNextPage) = await client.GetUserHistoryAsync(userId, page, limit, options, cancellationToken).ConfigureAwait(false);
		var unpaginatedRes = data.Where(e => e.Id > limitHistoryEntryId).ToList();
		if (unpaginatedRes.Count != data.Length || !hasNextPage)
			return unpaginatedRes;

		var acc = new List<History>(50);
		var hnp = true;
		var isLimitReached = false;
		for (page = 1, limit = 100; hnp && !isLimitReached; page++)
		{
			var (paginatedData, paginatedHasNextPage) =
				await client.GetUserHistoryAsync(userId, page, limit, options, cancellationToken).ConfigureAwait(false);
			hnp = paginatedHasNextPage;
			var toAcc = paginatedData.Where(e => e.Id > limitHistoryEntryId).ToArray();
			isLimitReached = paginatedData.Length == toAcc.Length;
			acc.AddRange(toAcc);
		}

		return acc;
	}

	public static List<List<History>> GroupSimilarHistoryEntries(this List<History> source)
	{
		source.Sort((h1, h2) => Comparer<ulong>.Default.Compare(h1.Id, h2.Id));
		var res = new List<List<History>>(5);
		var group = new List<History>(1);
		foreach (var he in source)
		{
			if (!group.Any() || group.TrueForAll(hge => hge.Target?.Id == he.Target?.Id))
				group.Add(he);
			else
			{
				res.Add(group);
				group = new(1)
				{
					he
				};
			}
		}

		if (group.Any())
			res.Add(group);
		return res;
	}

	public static DiscordEmbedBuilder ToDiscordEmbed(this List<History> history, UserInfo user, ShikiUserFeatures features)
	{
		var first = history[0];
		var ruCulture = CultureInfo.GetCultureInfo("ru-RU");
		var eb = new DiscordEmbedBuilder().WithTimestamp(first.CreatedAt).WithShikiAuthor(user).WithColor(Constants.ShikiBlue);
		var desc = string.Join("; ", history.Select(h => h.Description)).StripHtml().ToSentenceCase(ruCulture)!;
		eb.WithDescription(desc);

		var dict = new Dictionary<int, ProgressType>();
		dict.TryAdd(desc.LastIndexOf("смотрю", StringComparison.OrdinalIgnoreCase), ProgressType.InProgress);
		dict.TryAdd(desc.LastIndexOf("пересматриваю", StringComparison.OrdinalIgnoreCase), ProgressType.InProgress);
		dict.TryAdd(desc.LastIndexOf("запланировано", StringComparison.OrdinalIgnoreCase), ProgressType.InPlans);
		dict.TryAdd(desc.LastIndexOf("брошено", StringComparison.OrdinalIgnoreCase), ProgressType.Dropped);
		dict.TryAdd(desc.LastIndexOf("просмотрен", StringComparison.OrdinalIgnoreCase), ProgressType.Completed);
		dict.TryAdd(desc.LastIndexOf("отложено", StringComparison.OrdinalIgnoreCase), ProgressType.OnHold);

		dict.TryAdd(desc.LastIndexOf("прочитан", StringComparison.OrdinalIgnoreCase), ProgressType.Completed);
		dict.TryAdd(desc.LastIndexOf("перечитываю", StringComparison.OrdinalIgnoreCase), ProgressType.InProgress);
		var progress = dict.MaxBy(kvp => kvp.Key).Value;

		eb.WithColor(Colors[progress]);
		var firstTarget = first?.Target;
		if (firstTarget is null)
			return eb;

		var titleSb = new StringBuilder();
		if ((features & ShikiUserFeatures.Russian) != 0)
			titleSb.Append(firstTarget.RussianName ?? firstTarget.Name);
		else
			titleSb.Append(firstTarget.Name);
		if ((features & ShikiUserFeatures.MediaFormat) != 0)
			titleSb.Append($" ({(firstTarget.Kind ?? "Unknown").Humanize(LetterCasing.Sentence)})");
		if ((features & ShikiUserFeatures.MediaStatus) != 0)
			titleSb.AppendLine($" [{firstTarget.Status.Humanize(LetterCasing.Sentence)}]");
		eb.WithTitle(titleSb.ToString())
		  .WithUrl(firstTarget.Url).WithThumbnail(firstTarget.ImageUrl);

		if (firstTarget.Chapters.HasValue && firstTarget.Chapters != 0)
		{
			eb.AddField("Total", $"{firstTarget.Chapters.Value} ch. {firstTarget.Volumes.GetValueOrDefault()} v.", true);
		}
		else if (firstTarget.Episodes.HasValue)
		{
			var episodes = firstTarget switch
			{
				_ when firstTarget.Episodes      != 0 => firstTarget.Episodes.Value,
				_ when firstTarget.EpisodesAired != 0 => firstTarget.EpisodesAired.GetValueOrDefault(),
				_                                     => 0
			};
			if (episodes != 0)
				eb.AddField("Total", $"{episodes} ep.", true);
		}

		return eb;
	}

	public static DiscordEmbedBuilder ToDiscordEmbed(this FavouriteEntry favouriteEntry, UserInfo user, bool added, ShikiUserFeatures features)
	{
		var favouriteName = (features & ShikiUserFeatures.Russian) != 0 ? favouriteEntry.RussianName ?? favouriteEntry.Name : favouriteEntry.Name;
		return new DiscordEmbedBuilder
		{
			Url = favouriteEntry.Url,
			Title = $"{favouriteName} [{(favouriteEntry.GenericType ?? favouriteEntry.SpecificType).Humanize(LetterCasing.Sentence)}]"
		}.WithThumbnail(favouriteEntry.ImageUrl).WithDescription($"{(added ? "Added" : "Removed")} favourite").WithShikiAuthor(user)
			 .WithColor(added ? Constants.ShikiGreen : Constants.ShikiRed);
	}

	public static DiscordEmbedBuilder WithShikiUpdateProviderFooter(this DiscordEmbedBuilder eb)
	{
		eb.Footer = ShikiUpdateProviderFooter;
		return eb;
	}
}