﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using PaperMalKing.Common;

namespace PaperMalKing.UpdatesProviders.Base.Colors;

public sealed class ColorsChoiceProvider<T> : IEnumChoiceProvider<T>
	where T : unmanaged, Enum, IComparable, IConvertible, IFormattable
{
	public static Task<IEnumerable<DiscordApplicationCommandOptionChoice>> CreateChoicesAsync()
	{
		return Task.FromResult(UpdateTypesHelper<T>.UpdateTypes.Select(static ut => ut.ToDiscordApplicationCommandOptionChoice()).ToArray().AsEnumerable());
	}
}