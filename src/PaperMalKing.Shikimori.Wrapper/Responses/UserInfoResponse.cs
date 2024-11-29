// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using PaperMalKing.Shikimori.Wrapper.Abstractions.Models;

namespace PaperMalKing.Shikimori.Wrapper.Responses;

internal sealed class UserInfoResponse
{
	public required UserInfo[] Users { get; init; }
}