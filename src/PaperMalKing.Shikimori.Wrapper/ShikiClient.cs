﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaperMalKing.Common.Enums;
using PaperMalKing.Shikimori.Wrapper.Abstractions;
using PaperMalKing.Shikimori.Wrapper.Abstractions.Models;
using PaperMalKing.Shikimori.Wrapper.Abstractions.Models.Media;

namespace PaperMalKing.Shikimori.Wrapper;

public sealed class ShikiClient : IShikiClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ShikiClient> _logger;

	public ShikiClient(HttpClient httpClient, ILogger<ShikiClient> logger)
	{
		this._httpClient = httpClient;
		this._logger = logger;
	}

	public async Task<UserInfo> GetUserAsync(string nickname, CancellationToken cancellationToken = default)
	{
		this._logger.LogDebug("Requesting {@Nickname} profile", nickname);

		nickname = WebUtility.UrlEncode(nickname);
		var url = $"{Constants.BASE_USERS_API_URL}/{nickname}";

		using var rm = new HttpRequestMessage(HttpMethod.Get, url)
		{
			Content = new MultipartFormDataContent()
			{
				#pragma warning disable CA2000
				{ new StringContent("1"), "is_nickname" },
				#pragma warning restore CA2000
			},
		};

		using var response = await this._httpClient.SendAsync(rm, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
		return (await response.Content.ReadFromJsonAsync(JsonSGContext.Default.UserInfo, cancellationToken).ConfigureAwait(false))!;
	}

	public async Task<Favourites> GetUserFavouritesAsync(uint userId, CancellationToken cancellationToken = default)
	{
		this._logger.LogDebug("Requesting {@UserId} favourites", userId);
		var url = $"{Constants.BASE_USERS_API_URL}/{userId}/favourites";
		var favs = await this._httpClient.GetFromJsonAsync(url, JsonSGContext.Default.Favourites, cancellationToken).ConfigureAwait(false);
		return favs!;
	}

	public async Task<Paginatable<History[]>> GetUserHistoryAsync(uint userId, uint page, byte limit, HistoryRequestOptions options,
																  CancellationToken cancellationToken = default)
	{
		var url = $"{Constants.BASE_USERS_API_URL}/{userId}/history";
		limit = Constants.HISTORY_LIMIT < limit ? Constants.HISTORY_LIMIT : limit;
		this._logger.LogDebug("Requesting {@UserId} history. Page {@Page}", userId, page);

		#pragma warning disable CA2000
		using var content = new MultipartFormDataContent
		{
			{ new StringContent(page.ToString(CultureInfo.InvariantCulture)), "page" },
			{ new StringContent(limit.ToString(CultureInfo.InvariantCulture)), "limit" },
		};
		if (options != HistoryRequestOptions.Any) content.Add(new StringContent(options.ToString()), "target_type");
		#pragma warning restore CA2000

		using var rm = new HttpRequestMessage(HttpMethod.Get, url)
		{
			Content = content,
		};
		using var response = await this._httpClient.SendAsync(rm, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

		var data = (await response.Content.ReadFromJsonAsync(JsonSGContext.Default.HistoryArray, cancellationToken).ConfigureAwait(false))!;
		var hasNextPage = data.Length == limit + 1;
		return new(data, hasNextPage);
	}

	public Task<TMedia?> GetMediaAsync<TMedia>(ulong id, ListEntryType type, CancellationToken cancellationToken = default) where TMedia : BaseMedia
	{
		var url = BuildUrlForRequestingMedia(id, type);
		this._logger.LogInformation("Requesting media with id: {Id}, and type: {Type}", id, type);
		return this._httpClient.GetFromJsonAsync<TMedia>(url, cancellationToken);
	}

	private static string BuildUrlForRequestingMedia(ulong id, ListEntryType type) =>
		$"{Constants.BASE_API_URL}/{(type == ListEntryType.Anime ? "animes" : "mangas")}/{id}";

	public async Task<IReadOnlyList<Role>> GetMediaStaffAsync(ulong id, ListEntryType type, CancellationToken cancellationToken = default)
	{
		var url = $"{BuildUrlForRequestingMedia(id, type)}/roles";
		this._logger.LogInformation("Requesting staff for media with id: {Id}, and type: {Type}", id, type);
		var roles = await this._httpClient.GetFromJsonAsync(url, JsonSGContext.Default.ListRole, cancellationToken).ConfigureAwait(false);
		roles!.RemoveAll(x => x.Person is null);
		roles.TrimExcess();
		return roles;
	}

	public Task<UserInfo> GetUserInfoAsync(uint userId, CancellationToken cancellationToken = default)
	{
		var url = $"{Constants.BASE_USERS_API_URL}/{userId}/info";
		return this._httpClient.GetFromJsonAsync(url, JsonSGContext.Default.UserInfo, cancellationToken)!;
	}

	public async Task<IReadOnlyList<UserAchievement>> GetUserAchievementsAsync(uint userId, CancellationToken cancellationToken = default)
	{
		var url = $"{Constants.BASE_API_URL}/achievements";
		using var rm = new HttpRequestMessage(HttpMethod.Get, url)
		{
			Content = new MultipartFormDataContent()
			{
				#pragma warning disable CA2000
				{ new StringContent(userId.ToString("D", CultureInfo.InvariantCulture)), "user_id" },
				#pragma warning restore CA2000
			},
		};
		using var response = await this._httpClient.SendAsync(rm, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
		var achievements = (await response.Content.ReadFromJsonAsync(JsonSGContext.Default.UserAchievementArray, cancellationToken)
										  .ConfigureAwait(false))!;
		var r = new List<UserAchievement>(achievements.Length);
		foreach (var userAchievement in achievements.Where(x=>x is { Level: > 0}).GroupBy(x => x.Id, StringComparer.Ordinal))
		{
			r.Add(new UserAchievement(userAchievement.Key, userAchievement.Max(x => x.Level)));
		}

		return r;
	}
}