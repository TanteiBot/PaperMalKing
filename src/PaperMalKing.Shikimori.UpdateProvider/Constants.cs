// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using DSharpPlus.Entities;

namespace PaperMalKing.Shikimori.UpdateProvider;

internal static class Constants
{
	public const string Name = "Shikimori";

	public const string IconUrl = "https://shikimori.me/favicons/opera-icon-228x228.png";

	public static DiscordColor ShikiGreen { get; } = new("#419541");

	public static DiscordColor ShikiRed { get; } = new("#FC575E");

	public static DiscordColor ShikiGrey { get; } = new("#7b8084");

	public static DiscordColor ShikiBlue { get; } = new("#176093");
}