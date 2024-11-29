// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using DSharpPlus.Entities;

namespace PaperMalKing.AniList.UpdateProvider;

internal static class ProviderConstants
{
	public const string Name = "AniList";

	public const string Url = "https://anilist.co";

	public const string IconUrl = "https://anilist.co/img/icons/android-chrome-512x512.png";

	/// <remarks>
	/// Completed.
	/// </remarks>
	public static DiscordColor AniListGreen { get; } = new("#7bd555");

	/// <remarks>
	/// Planned.
	/// </remarks>
	public static DiscordColor AniListOrange { get; } = new("#f79a63");

	/// <remarks>
	/// Dropped.
	/// </remarks>
	public static DiscordColor AniListRed { get; } = new("#e85d75");

	/// <remarks>
	/// Paused.
	/// </remarks>
	public static DiscordColor AniListPeach { get; } = new("#fa7a7a");

	/// <remarks>
	/// Current.
	/// </remarks>
	public static DiscordColor AniListBlue { get; } = new("#3db4f2");
}