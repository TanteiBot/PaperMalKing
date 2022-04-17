// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2022 N0D4N

namespace Tantei.Core.Models.Users;

public interface IUpdateProviderUser
{
	ulong Id { get; }

	ulong BotUserId { get; }

	BotUser BotUser { get; }
}