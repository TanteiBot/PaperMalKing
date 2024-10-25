// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaperMalKing.Common.RateLimiters;
using PaperMalKing.Database.Models.Shikimori;
using PaperMalKing.Shikimori.UpdateProvider.Achievements;
using PaperMalKing.Shikimori.Wrapper;
using PaperMalKing.Shikimori.Wrapper.Abstractions;
using PaperMalKing.UpdatesProviders.Base.Features;
using PaperMalKing.UpdatesProviders.Base.UpdateProvider;
using Polly;

namespace PaperMalKing.Shikimori.UpdateProvider.Installer;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddShikimori(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddOptions<ShikiOptions>().BindConfiguration(Constants.Name).ValidateDataAnnotations().ValidateOnStart();

		serviceCollection.AddHttpClient(Constants.Name).ConfigureHttpClient((provider, client) =>
		{
			client.DefaultRequestHeaders.UserAgent.Clear();
			client.DefaultRequestHeaders.UserAgent.ParseAdd(provider.GetRequiredService<IOptions<ShikiOptions>>().Value.ShikimoriAppName);
			client.BaseAddress = new(Wrapper.Abstractions.Constants.BaseUrl);
		}).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
		{
			PooledConnectionLifetime = TimeSpan.FromMinutes(15),
		}).AddResilienceHandler("shiki", builder =>
		{
			const int shikiHttpRetryAttempts = 5;

			builder.AddRetry(new HttpRetryStrategyOptions
			{
				MaxRetryAttempts = shikiHttpRetryAttempts,
			});

			var rpmRl = new RateLimitValue(50, TimeSpan.FromMinutes(1, 5)); // 90rpm with .05 as inaccuracy
			builder.AddRateLimiter(RateLimiterFactory.Create<ShikiClient>(rpmRl));

			var rpsRl = new RateLimitValue(3, TimeSpan.FromSeconds(1, 200)); // 5rps with .05 as inaccuracy
			builder.AddRateLimiter(RateLimiterFactory.Create<ShikiClient>(rpsRl));
		});

		serviceCollection.AddSingleton<IShikiClient, ShikiClient>(provider =>
		{
			var factory = provider.GetRequiredService<IHttpClientFactory>();
			var logger = provider.GetRequiredService<ILogger<ShikiClient>>();
			return new(factory.CreateClient(Constants.Name), logger);
		});
		serviceCollection.AddSingleton<BaseUserFeaturesService<ShikiUser, ShikiUserFeatures>, ShikiUserFeaturesService>();
		serviceCollection.AddSingleton<ShikiUserService>();

		serviceCollection.AddOptions<NekoFileJson>().BindConfiguration("ShikimoriNeko").ValidateDataAnnotations().ValidateOnStart();
		serviceCollection.AddSingleton<ShikiAchievementsService>();

		serviceCollection.AddSingleton<ShikiUpdateProvider>();
		serviceCollection.AddSingleton<IUpdateProvider>(f => f.GetRequiredService<ShikiUpdateProvider>());
		serviceCollection.AddHostedService(f => f.GetRequiredService<ShikiUpdateProvider>());

		return serviceCollection;
	}
}