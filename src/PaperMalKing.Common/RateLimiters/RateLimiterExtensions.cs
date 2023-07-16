﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N
using System;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaperMalKing.Common.Options;

namespace PaperMalKing.Common.RateLimiters;

public static class RateLimiterExtensions
{
	public static RateLimiterHttpMessageHandler ToHttpMessageHandler(this RateLimiter rateLimiter) =>
		new(rateLimiter);

	public static RateLimiter<T> ToRateLimiter<T>(this RateLimit rateLimit) =>
		RateLimiterFactory.Create<T>(rateLimit);

	public static RateLimiter<T> ConfigurationLambda<TO, T>(IServiceProvider servicesProvider)
		where TO : class, IRateLimitOptions<T>
	{
		var options = servicesProvider.GetRequiredService<IOptions<TO>>();
		return options.Value.ToRateLimiter();
	}
}