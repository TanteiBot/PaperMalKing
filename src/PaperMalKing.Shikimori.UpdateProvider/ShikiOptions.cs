﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using PaperMalKing.Common.Options;

namespace PaperMalKing.Shikimori.UpdateProvider;

public sealed class ShikiOptions : ITimerOptions<ShikiUpdateProvider>
{
	public const string Shikimori = Constants.NAME;

	public required string ShikimoriAppName { get; init; }

	public required int DelayBetweenChecksInMilliseconds { get; init; }
}