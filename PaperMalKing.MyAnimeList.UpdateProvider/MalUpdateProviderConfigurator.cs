﻿#region LICENSE
// PaperMalKing.
// Copyright (C) 2021-2022 N0D4N
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaperMalKing.Common.RateLimiters;
using PaperMalKing.Database.Models.MyAnimeList;
using PaperMalKing.MyAnimeList.Wrapper;
using PaperMalKing.UpdatesProviders.Base;
using PaperMalKing.UpdatesProviders.Base.Features;
using PaperMalKing.UpdatesProviders.Base.UpdateProvider;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace PaperMalKing.UpdatesProviders.MyAnimeList
{
	public sealed class MalUpdateProviderConfigurator : IUpdateProviderConfigurator<MalUpdateProvider>
	{
		public static void Configure(IConfiguration configuration, IServiceCollection serviceCollection)
		{
			serviceCollection.AddOptions<MalOptions>().Bind(configuration.GetSection(Constants.Name));
			serviceCollection.AddSingleton<RateLimiter<MyAnimeListClient>>(RateLimiterExtensions.ConfigurationLambda<MalOptions, MyAnimeListClient>);

			var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
												  .OrResult(message => message.StatusCode == HttpStatusCode.TooManyRequests)
												  .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(10), 5));
			serviceCollection.AddHttpClient(MalOptions.MyAnimeList).AddPolicyHandler(retryPolicy).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
			{
				UseCookies = true,
				CookieContainer = new()
			}).AddHttpMessageHandler(provider =>
			{
				var rl = provider.GetRequiredService<RateLimiter<MyAnimeListClient>>();
				return rl.ToHttpMessageHandler();
			}).ConfigureHttpClient(client =>
			{
				client.DefaultRequestHeaders.UserAgent.Clear();
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:76.0) Gecko/20100101 Firefox/76.0");
			});
			serviceCollection.AddSingleton<MyAnimeListClient>(provider =>
			{
				var factory = provider.GetRequiredService<IHttpClientFactory>();
				var logger = provider.GetRequiredService<ILogger<MyAnimeListClient>>();
				return new(logger, factory.CreateClient(MalOptions.MyAnimeList));
			});
			serviceCollection.AddSingleton<IExecuteOnStartupService, MalExecuteOnStartupService>();
			serviceCollection.AddSingleton<IUserFeaturesService<MalUserFeatures>,MalUserFeaturesService>();
			serviceCollection.AddSingleton<MalUserService>();
			serviceCollection.AddSingleton<IUpdateProvider, MalUpdateProvider>();
		}
	}
}