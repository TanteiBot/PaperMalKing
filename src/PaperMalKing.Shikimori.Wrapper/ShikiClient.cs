﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PaperMalKing.Common.Enums;
using PaperMalKing.Shikimori.Wrapper.Models;
using PaperMalKing.Shikimori.Wrapper.Models.Media;

namespace PaperMalKing.Shikimori.Wrapper;

internal sealed class ShikiClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<ShikiClient> _logger;

	public ShikiClient(HttpClient httpClient, ILogger<ShikiClient> logger)
	{
		this._httpClient = httpClient;
		this._logger = logger;
	}

	public async Task<User> GetUserAsync(string nickname, CancellationToken cancellationToken = default)
	{
		this._logger.LogDebug("Requesting {@Nickname} profile", nickname);

		nickname = WebUtility.UrlEncode(nickname);
		var url = $"{Constants.BASE_USERS_API_URL}/{nickname}";

		using var stringContent = new StringContent("1");
		using var rm = new HttpRequestMessage(HttpMethod.Get, url)
		{
			Content = new MultipartFormDataContent()
			{
				{ stringContent, "is_nickname" }
			}
		};

		using var response = await this._httpClient.SendAsync(rm, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
		return (await response.Content.ReadFromJsonAsync<User>(JsonSerializerOptions.Default, cancellationToken).ConfigureAwait(false))!;
	}

	public async Task<Favourites> GetUserFavouritesAsync(uint userId, CancellationToken cancellationToken = default)
	{
		this._logger.LogDebug("Requesting {@UserId} favourites", userId);
		var url = $"{Constants.BASE_USERS_API_URL}/{userId}/favourites";
		var favs = await this._httpClient.GetFromJsonAsync<Favourites>(url, cancellationToken).ConfigureAwait(false);
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
			{ new StringContent(page.ToString()), "page" },
			{ new StringContent(limit.ToString()), "limit" }
		};
		if (options != HistoryRequestOptions.Any) content.Add(new StringContent(options.ToString()), "target_type");
		#pragma warning restore CA2000

		using var rm = new HttpRequestMessage(HttpMethod.Get, url)
		{
			Content = content
		};
		using var response = await this._httpClient.SendAsync(rm, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

		var data = (await response.Content.ReadFromJsonAsync<History[]>(JsonSerializerOptions.Default, cancellationToken).ConfigureAwait(false))!;
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
		var roles = await this._httpClient.GetFromJsonAsync<List<Role>>(url, cancellationToken).ConfigureAwait(false);
		roles!.RemoveAll(x => x.Person is null);
		roles.TrimExcess();
		return roles;
	}

	public Task<UserInfo> GetUserInfoAsync(uint userId, CancellationToken cancellationToken = default)
	{
		var url = $"{Constants.BASE_USERS_API_URL}/{userId}/info";
		return this._httpClient.GetFromJsonAsync<UserInfo>(url, cancellationToken)!;
	}
}