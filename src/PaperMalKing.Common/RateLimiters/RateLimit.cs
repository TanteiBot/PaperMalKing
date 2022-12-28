﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N
using System;

namespace PaperMalKing.Common.RateLimiters;

#pragma warning disable CA1724
public sealed class RateLimit
#pragma warning restore
{
	public static readonly RateLimit Empty = new();

	public int AmountOfRequests { get; }

	public int PeriodInMilliseconds { get; }

	public RateLimit(int amountOfRequests, int periodInMilliseconds)
	{
		if (amountOfRequests <= 0)
			throw new ArgumentException("Amount of requests must be a number bigger than 0", nameof(amountOfRequests));
		if (periodInMilliseconds <= 0)
			throw new ArgumentException("Period of time in milliseconds must be bigger than 0", nameof(periodInMilliseconds));
		this.AmountOfRequests = amountOfRequests;
		this.PeriodInMilliseconds = periodInMilliseconds;
	}

	public RateLimit(int amountOfRequests, TimeSpan period) : this(amountOfRequests, (int)period.TotalMilliseconds)
	{ }

	private RateLimit()
	{
		this.PeriodInMilliseconds = 0;
		this.AmountOfRequests = 0;
	}

	public override string ToString()
	{
		return $"{this.AmountOfRequests}r per {this.PeriodInMilliseconds}ms";
	}
}