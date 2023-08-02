﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2023 N0D4N

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.Entities;

namespace PaperMalKing.Shikimori.UpdateProvider;

internal sealed class ShikiAchievementsService
{
	private readonly FrozenDictionary<(string Id, byte Level), ShikiAchievement> _achievements;
	public ShikiAchievementsService(NekoFileJson neko)
	{
		this._achievements = neko.Achievements.ToDictionary(item => (item.Id, item.Level),
			item => new ShikiAchievement(item.Id, item.Level,
				new Uri(Wrapper.Abstractions.Constants.BASE_URL + item.Image, UriKind.Absolute),
				item.BorderColor is not null ? new(item.BorderColor) : DiscordColor.None, item.TitleRussian, item.TextRussian, item.TitleEnglish,
				item.TextEnglish, neko.HumanNames.GetValueOrDefault(item.Id))).ToFrozenDictionary(optimizeForReading: true);
	}

	public ShikiAchievement? GetAchievementOrNull(string id, byte level) => this._achievements.GetValueOrDefault((id, level));
}