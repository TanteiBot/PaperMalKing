// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Net.Http;
using JikanDotNet;
using JikanDotNet.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaperMalKing.Common.RateLimiters;
using PaperMalKing.Database.Models.MyAnimeList;
using PaperMalKing.MyAnimeList.Wrapper;
using PaperMalKing.MyAnimeList.Wrapper.Abstractions;
using PaperMalKing.UpdatesProviders.Base.Features;
using PaperMalKing.UpdatesProviders.Base.UpdateProvider;
using Polly;

namespace PaperMalKing.MyAnimeList.UpdateProvider.Installer;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMyAnimeList(this IServiceCollection serviceCollection)
	{
		const int malHttpRetries = 3;

		serviceCollection.AddOptions<MalOptions>().BindConfiguration(Constants.Name).ValidateDataAnnotations().ValidateOnStart();
		serviceCollection.AddSingleton(RateLimiterExtensions.ConfigurationLambda<MalOptions, IMyAnimeListClient>);

		serviceCollection.AddHttpClient(Constants.UnOfficialApiHttpClientName, client =>
						{
							client.Timeout = TimeSpan.FromSeconds(120L);
							client.DefaultRequestHeaders.UserAgent.Clear();
							client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);
						}).ConfigurePrimaryHttpMessageHandler(_ => HttpClientHandlerFactory()).AddResilienceHandler("parser-mal", (builder, rbc) =>
						{
							builder.AddRetry(new HttpRetryStrategyOptions
							{
								MaxRetryAttempts = malHttpRetries,
							});

							var rateLimiter = rbc.ServiceProvider.GetRequiredService<RateLimiter<IMyAnimeListClient>>();
							builder.AddRateLimiter(rateLimiter);
						});
		serviceCollection.AddHttpClient(Constants.OfficialApiHttpClientName).ConfigurePrimaryHttpMessageHandler(_ => HttpClientHandlerFactory())
						 .ConfigureHttpClient((provider, client) =>
						 {
							 var options = provider.GetRequiredService<IOptions<MalOptions>>().Value;
							 client.DefaultRequestHeaders.Add(Constants.OfficialApiHeaderName, options.ClientId);
						 }).AddResilienceHandler("official-mal", (builder, rbc) =>
						 {
							 builder.AddRetry(new HttpRetryStrategyOptions
							 {
								 MaxRetryAttempts = malHttpRetries,
							 });

							 var rateLimiter = rbc.ServiceProvider.GetRequiredService<RateLimiter<IMyAnimeListClient>>();
							 builder.AddRateLimiter(rateLimiter);
						 });
		serviceCollection.AddHttpClient(Constants.JikanHttpClientName, client => client.BaseAddress = new(Constants.JikanApiUrl))
						 .ConfigurePrimaryHttpMessageHandler(_ => HttpClientHandlerFactory())
						 .AddResilienceHandler("jikan", builder =>
						 {
							 var rpmRl = new RateLimitValue(60, TimeSpan.FromMinutes(1, 12)); // 60rpm with 0.2 as inaccuracy
							 builder.AddRateLimiter(RateLimiterFactory.Create<IJikan>(rpmRl));

							 var rpsRl = new RateLimitValue(3, TimeSpan.FromSeconds(1, 500)); // 3rps with 0.5 as inaccuracy
							 builder.AddRateLimiter(RateLimiterFactory.Create<IJikan>(rpsRl));
						 });

		serviceCollection.AddSingleton<IJikan>(provider => new Jikan(
			new()
			{
				SuppressException = false,
				LimiterConfigurations = TaskLimiterConfiguration.None, // We use System.Threading.RateLimiting
			},
			provider.GetRequiredService<IHttpClientFactory>().CreateClient(Constants.JikanHttpClientName)));

		serviceCollection.AddSingleton<IMyAnimeListClient, MyAnimeListClient>(provider =>
		{
			var factory = provider.GetRequiredService<IHttpClientFactory>();
			var logger = provider.GetRequiredService<ILogger<MyAnimeListClient>>();
			var jikan = provider.GetRequiredService<IJikan>();
			return new(logger, _unofficialApiHttpClient: factory.CreateClient(Constants.UnOfficialApiHttpClientName),
				_officialApiHttpClient: factory.CreateClient(Constants.OfficialApiHttpClientName), _jikanClient: jikan);
		});
		serviceCollection.AddSingleton<BaseUserFeaturesService<MalUser, MalUserFeatures>, MalUserFeaturesService>();
		serviceCollection.AddSingleton<MalUserService>();

		serviceCollection.AddSingleton<MalUpdateProvider>();
		serviceCollection.AddSingleton<IUpdateProvider>(f => f.GetRequiredService<MalUpdateProvider>());
		serviceCollection.AddHostedService(f => f.GetRequiredService<MalUpdateProvider>());

		return serviceCollection;
	}

	private static SocketsHttpHandler HttpClientHandlerFactory() => new()
	{
		UseCookies = true,
		CookieContainer = new(),
		PooledConnectionLifetime = TimeSpan.FromMinutes(15),
	};
}