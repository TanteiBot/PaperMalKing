// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;

namespace PaperMalKing.Common;

public abstract class BotCommandsModule : ApplicationCommandModule
{
	protected abstract bool IsResponseVisibleOnlyForRequester { get; }

	public override async Task<bool> BeforeSlashExecutionAsync(InteractionContext ctx)
	{
		var responseBuilder = new DiscordInteractionResponseBuilder
		{
			IsEphemeral = this.IsResponseVisibleOnlyForRequester,
		};

		await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, responseBuilder).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
		return true;
	}

	protected static IDisposable? CreateLoggerScope(ILogger<BotCommandsModule> logger, InteractionContext ctx)
		=> logger.ExecutingCommandByUserScope(ctx.QualifiedName, ctx.Member.ToString(), ctx.Guild.ToString());
}