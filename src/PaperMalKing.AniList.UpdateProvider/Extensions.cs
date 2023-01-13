﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using PaperMalKing.AniList.UpdateProvider.CombinedResponses;
using PaperMalKing.AniList.Wrapper;
using PaperMalKing.AniList.Wrapper.Models;
using PaperMalKing.AniList.Wrapper.Models.Enums;
using PaperMalKing.Common;
using PaperMalKing.Database.Models.AniList;

namespace PaperMalKing.AniList.UpdateProvider;

internal static partial class Extensions
{
	[GeneratedRegex(@"([\s\S][Ss]ource: .*)", RegexOptions.Compiled | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 30000/*30s*/)]
	internal static partial Regex SourceRemovalRegex();

	[GeneratedRegex(@"(^\s+$[\r\n])|(\n{2,})", RegexOptions.Compiled | RegexOptions.Multiline, matchTimeoutMilliseconds: 30000/*30s*/)]
	internal static partial Regex EmptyLinesRemovalRegex();

	private static readonly string[] IgnoredStartWithRoles = {
		"Touch-Up",
		"Touch Up",
		"Illustrat",
		"Collaborat",
		"Color",
		"Digital Coloring",
		"Cooking Supervisor",
		"Letter",   //Letterer and Lettering
		"Translat", //Translator and Translation
	};

	private static readonly string[] IgnoredContainsRoles = { "Assist", "Edit", "Insert", "Consultant", "Cooperation" };

	private static readonly Dictionary<MediaListStatus, DiscordColor> Colors = new()
	{
		{MediaListStatus.PAUSED, Constants.AniListPeach},
		{MediaListStatus.CURRENT, Constants.AniListBlue},
		{MediaListStatus.REPEATING, Constants.AniListBlue},
		{MediaListStatus.DROPPED, Constants.AniListRed},
		{MediaListStatus.PLANNING, Constants.AniListOrange},
		{MediaListStatus.COMPLETED, Constants.AniListGreen}
	};

	private static readonly DiscordEmbedBuilder.EmbedFooter AniListFooter = new()
	{
		Text = Constants.NAME,
		IconUrl = Constants.ICON_URL
	};

	private const int InlineFieldValueMaxLength = 30;


	public static async Task<CombinedRecentUpdatesResponse> GetAllRecentUserUpdatesAsync(
		this AniListClient client, AniListUser user, AniListUserFeatures features,
		CancellationToken cancellationToken)
	{
		const ushort initialPerChunkValue = 50;
		const ushort extendedPerChunkValue = 500;
		var hasNextPage = true;
		var perChunk = initialPerChunkValue;
		ushort chunk = 1;
		var result = new CombinedRecentUpdatesResponse();
		var options = (RequestOptions)features;
		for (byte page = 1; hasNextPage; page++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var response = await client.CheckForUpdatesAsync(user.Id, page, user.LastActivityTimestamp, perChunk, chunk, options,
				cancellationToken).ConfigureAwait(false);
			result.Add(response);
			hasNextPage = response.HasNextPage;
			if (perChunk == initialPerChunkValue)
			{
				perChunk = extendedPerChunkValue;
			}
			else
			{
				chunk++;
			}
		}

		return result;
	}

	public static async Task<CombinedInitialInfoResponse> GetCompleteUserInitialInfoAsync(this AniListClient client, string username,
																						  CancellationToken cancellationToken = default)
	{
		var hasNextPage = true;
		var result = new CombinedInitialInfoResponse();
		for (byte page = 1; hasNextPage; page++)
		{
			var response = await client.GetInitialUserInfoAsync(username, page, cancellationToken).ConfigureAwait(false);
			result.Add(response.User);
			hasNextPage = response.User.Favourites.HasNextPage;
		}

		return result;
	}

	public static string? GetEmbedFormat(this Media media)
	{
		static string? DefaultFormatting(Media media) => media.Format?.ToString().ToLowerInvariant().Humanize(LetterCasing.Sentence);

		switch (media.CountryOfOrigin)
		{
			case "CN":
				switch (media.Format)
				{
					case MediaFormat.TV:
					case MediaFormat.TV_SHORT:
					case MediaFormat.MOVIE:
					case MediaFormat.SPECIAL:
					case MediaFormat.OVA:
					case MediaFormat.ONA:
						return "Donghua";
					case MediaFormat.MANGA:
					case MediaFormat.ONE_SHOT:
						return "Manhua";
					default:
						return DefaultFormatting(media);
				}
			case "KR" when media.Format is MediaFormat.MANGA or MediaFormat.ONE_SHOT:
				{
					return "Manhwa";
				}
			default:
				return DefaultFormatting(media);
		}
	}

	public static DiscordEmbedBuilder WithMediaTitle(this DiscordEmbedBuilder eb, Media media, TitleLanguage titleLanguage,
													 AniListUserFeatures features)
	{
		var strings = new List<string> { media.Title.GetTitle(titleLanguage) };
		if ((features & AniListUserFeatures.MediaFormat) != 0)
		{
			var format = media.GetEmbedFormat();
			if (!string.IsNullOrEmpty(format))
				strings.Add($" ({format})");
		}
		if ((features & AniListUserFeatures.MediaStatus) != 0)
			strings.Add($" [{media.Status.ToString().ToLowerInvariant().Humanize(LetterCasing.Sentence)}]");
		var sb = new StringBuilder(256);
		foreach (var titlePart in strings)
		{
			if (sb.Length + titlePart.Length <= 256)
				sb.Append(titlePart);
			else
				break;
		}

		return eb.WithTitle(sb.ToString());
	}

	public static DiscordEmbedBuilder WithAniListAuthor(this DiscordEmbedBuilder embedBuilder, User user) =>
		embedBuilder.WithAuthor(user.Name, user.Url, user.Image?.ImageUrl);

	public static DiscordEmbedBuilder ToDiscordEmbedBuilder(this Review review, User user)
	{
		return new DiscordEmbedBuilder()
			   .WithAniListAuthor(user)
			   .WithTitle($"New review on {review.Media.Title.GetTitle(user.Options.TitleLanguage)} ({review.Media.Format?.Humanize(LetterCasing.Sentence)})")
			   .WithThumbnail(review.Media.Image?.ImageUrl).WithUrl(review.Url)
			   .WithTimestamp(DateTimeOffset.FromUnixTimeSeconds(review.CreatedAtTimeStamp))
			   .WithDescription(review.Summary);
	}

	public static DiscordEmbedBuilder ToDiscordEmbedBuilder(this ListActivity activity, MediaListEntry mediaListEntry, User user,
															AniListUserFeatures features)
	{
		var isAnime = activity.Media.Type == ListType.ANIME;
		var isHiddenProgressPresent = !string.IsNullOrEmpty(activity.Progress) && (mediaListEntry.Status == MediaListStatus.PAUSED ||
																				   mediaListEntry.Status == MediaListStatus.DROPPED ||
																				   mediaListEntry.Status == MediaListStatus.COMPLETED);
		var desc = isHiddenProgressPresent switch
		{
			true => $"{(isAnime ? "Watched episode" : "Read chapter")} {activity.Progress} and {mediaListEntry.Status.Humanize(LetterCasing.LowerCase)} it",
			_ => $"{activity.Status.Humanize(LetterCasing.Sentence)} {activity.Progress}"
		};
		var isAdvancedScoringEnabled =
			(isAnime
				? user.MediaListOptions!.AnimeListOptions.IsAdvancedScoringEnabled
				: user.MediaListOptions!.MangaListOptions.IsAdvancedScoringEnabled) &&
			mediaListEntry.AdvancedScores?.Values.Any(s => s != 0) == true;
		var eb = new DiscordEmbedBuilder()
				 .WithAniListAuthor(user)
				 .WithTimestamp(DateTimeOffset.FromUnixTimeSeconds(activity.CreatedAtTimestamp))
				 .WithUrl(activity.Media.Url)
				 .WithMediaTitle(activity.Media, user.Options.TitleLanguage, features)
				 .WithDescription(desc)
				 .WithColor(Colors[mediaListEntry.Status])
				 .WithThumbnail(activity.Media.Image?.ImageUrl);
		var score = mediaListEntry.GetScore(user.MediaListOptions.ScoreFormat);
		if (!string.IsNullOrEmpty(score))
			eb.AddField("Score", score, true);

		if (isAdvancedScoringEnabled)
		{
			var sb = new StringBuilder();
			foreach (var (key, value) in mediaListEntry.AdvancedScores?.Where(e => e.Value != 0f) ??
										 Array.Empty<KeyValuePair<string, float>>())
				sb.AppendLine($"{key}: {value:0.#}");
			eb.AddField("Advanced scoring", sb.ToString(), true);
		}

		eb.WithTotalSubEntries(activity.Media);
		if (mediaListEntry.Repeat != 0) eb.AddField($"{(isAnime ? "Rewatched" : "Reread")} times", mediaListEntry.Repeat.ToString(), true);
		if (!string.IsNullOrEmpty(mediaListEntry.Notes)) eb.AddField("Notes", mediaListEntry.Notes.Truncate(1023), true);
		if ((features & AniListUserFeatures.CustomLists) != 0 && mediaListEntry.CustomLists?.Any(x => x.Enabled) == true)
		{
			eb.AddField("Custom lists", string.Join(", ", mediaListEntry.CustomLists.Where(x=>x.Enabled).Select(x => x.Name)), true);
		}
		return eb.EnrichWithMediaInfo(activity.Media, user, features);
	}

	public static DiscordEmbedBuilder EnrichWithMediaInfo(this DiscordEmbedBuilder eb, Media media, User user, AniListUserFeatures features)
	{
		var isAnime = media.Type == ListType.ANIME;

		if (isAnime)
		{
			if ((features & AniListUserFeatures.Studio) != 0)
			{
				var text = string.Join(", ", media.Studios.Nodes.Where(s => s.IsAnimationStudio)
												  .Select(studio => Formatter.MaskedUrl(studio.Name, new Uri(studio.Url))));
				if (!string.IsNullOrEmpty(text))
					eb.AddField("Made by", text, true);
			}
			if ((features & AniListUserFeatures.Director) != 0)
			{
				var director = Array.Find(media.Staff.Nodes, x => x.Role.Equals("Director", StringComparison.Ordinal));
				if (director is not null)
				{
					eb.AddField("Director", Formatter.MaskedUrl(director.Staff.Name.GetName(user.Options.TitleLanguage), new(director.Staff.Url)), inline: true);
				}
			}

			if ((features & AniListUserFeatures.Seyu) != 0)
			{
				var seyus = string.Join(", ", media.Characters.Nodes.Where(x => x.VoiceActors.Count > 0).Select(x =>
				{
					var seyu = x.VoiceActors[0];
					return Formatter.MaskedUrl(seyu.Name.GetName(user.Options.TitleLanguage), new(seyu.Url));
				}));
				if (!string.IsNullOrEmpty(seyus))
				{
					eb.AddField("Seyu", seyus);
				}
			}
		}
		else // If not anime then its mange
		{
			if ((features & AniListUserFeatures.Mangaka) != 0)
			{
				var text = string.Join(", ",
					media.Staff.Nodes
						 .Where(edge => IgnoredStartWithRoles.All(r =>
							 !edge.Role.StartsWith(r, StringComparison.OrdinalIgnoreCase) &&
							 IgnoredContainsRoles.All(r => !edge.Role.Contains(r, StringComparison.OrdinalIgnoreCase)))).Take(7)
						 .Select(edge =>
							 $"{Formatter.MaskedUrl(edge.Staff.Name.GetName(user.Options.TitleLanguage), new(edge.Staff.Url))} - {edge.Role}"));
				if (!string.IsNullOrEmpty(text))
					eb.AddField("Made by", text, true);
			}
		}


		if ((features & AniListUserFeatures.Genres) != 0 && media.Genres.Any())
		{
			var fieldVal = string.Join(", ", media.Genres);
			eb.AddField("Genres", fieldVal, fieldVal.Length <= InlineFieldValueMaxLength);
		}

		if ((features & AniListUserFeatures.Tags) != 0 && media.Tags.Any())
		{
			var fieldVal = string.Join(", ",
				media.Tags.OrderByDescending(t => t.Rank).Take(7).Select(t => t.IsSpoiler ? $"||{t.Name}||" : t.Name));
			eb.AddField("Tags", fieldVal, fieldVal.Length <= InlineFieldValueMaxLength);
		}


		if ((features & AniListUserFeatures.MediaDescription) != 0 && !string.IsNullOrEmpty(media.Description))
		{
			var mediaDescription = media.Description.StripHtml();
			mediaDescription = SourceRemovalRegex().Replace(mediaDescription, string.Empty);
			mediaDescription = EmptyLinesRemovalRegex().Replace(mediaDescription, string.Empty);
			mediaDescription = Formatter.Strip(mediaDescription).Trim().Truncate(350);
			if (!string.IsNullOrEmpty(mediaDescription))
				eb.AddField("Description", mediaDescription, mediaDescription.Length <= InlineFieldValueMaxLength);
		}

		return eb;
	}

	public static DiscordEmbedBuilder WithTotalSubEntries(this DiscordEmbedBuilder eb, Media media)
	{
		var episodes = media.Episodes.GetValueOrDefault();
		var chapters = media.Chapters.GetValueOrDefault();
		var volumes = media.Volumes.GetValueOrDefault();
		if (episodes == 0 && chapters == 0 && volumes == 0) return eb;
		var fieldVal = new List<string>(2);
		if (episodes != 0) fieldVal.Add($"{episodes} ep.");
		if (chapters != 0) fieldVal.Add($"{chapters} ch");
		if (volumes != 0) fieldVal.Add($"{volumes} v.");
		if (fieldVal.Count != 0)
			eb.AddField("Total", string.Join(", ", fieldVal), inline: true);

		return eb;
	}

	public static DiscordEmbedBuilder WithAniListFooter(this DiscordEmbedBuilder eb)
	{
		eb.Footer = AniListFooter;
		return eb;
	}
}