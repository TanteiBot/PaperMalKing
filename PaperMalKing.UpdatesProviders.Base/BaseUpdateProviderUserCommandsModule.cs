﻿#region LICENSE
// PaperMalKing.
// Copyright (C) 2021 N0D4N
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Diagnostics.CodeAnalysis;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using PaperMalKing.Common;
using PaperMalKing.UpdatesProviders.Base.Exceptions;

namespace PaperMalKing.UpdatesProviders.Base;

[SuppressMessage("Microsoft.Design", "CA1051")]
public abstract class BaseUpdateProviderUserCommandsModule : BaseCommandModule
{
	protected readonly ILogger<BaseUpdateProviderUserCommandsModule> Logger;
	protected readonly IUpdateProviderUserService UserService;

	/// <inheritdoc />
	protected BaseUpdateProviderUserCommandsModule(IUpdateProviderUserService userService, ILogger<BaseUpdateProviderUserCommandsModule> logger)
	{
		this.UserService = userService;
		this.Logger = logger;
	}

	public virtual async Task AddUserCommand(CommandContext ctx, [RemainingText] [Description("Your username")]
											 string username)
	{
		this.Logger.LogInformation("Trying to add {ProviderUsername} {Member} to {Name} update provider", username, ctx.Member, UserService.Name);
		BaseUser user;
		try
		{
			user = await this.UserService.AddUserAsync(username, ctx.Member.Id, ctx.Member.Guild.Id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			var embed = ex is UserProcessingException ? EmbedTemplate.ErrorEmbed(ctx, ex.Message) : EmbedTemplate.UnknownErrorEmbed(ctx);
			await ctx.RespondAsync(embed: embed.Build()).ConfigureAwait(false);
			this.Logger.LogError(ex, "Failed to add {ProviderUsername} {Member} to {Name} update provider", username, ctx.Member, UserService.Name);
			throw;
		}

		this.Logger.LogInformation("Successfully added {ProviderUsername} {Member} to {Name} update provider", username, ctx.Member, UserService.Name);

		await ctx.RespondAsync(embed: EmbedTemplate.SuccessEmbed(ctx,
			$"Successfully added {user.Username} to {this.UserService.Name} update checker")).ConfigureAwait(false);
	}


	public virtual async Task RemoveUserInGuildCommand(CommandContext ctx)
	{
		this.Logger.LogInformation("Trying to remove {Member} from {Name} update provider", ctx.Member, UserService.Name);
		BaseUser user;
		try
		{
			user = await this.UserService.RemoveUserAsync(ctx.User.Id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			var embed = ex is UserProcessingException ? EmbedTemplate.ErrorEmbed(ctx, ex.Message) : EmbedTemplate.UnknownErrorEmbed(ctx);
			await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
			this.Logger.LogError(ex, "Failed to remove {Member} from {Name} update provider", ctx.Member, UserService.Name);

			throw;
		}
		this.Logger.LogInformation("Successfully removed {Member} from {Name} update provider", ctx.Member, UserService.Name);

		await ctx.RespondAsync(embed: EmbedTemplate.SuccessEmbed(ctx,
			$"Successfully removed {user.Username} from {this.UserService.Name} update checker")).ConfigureAwait(false);
	}

	public virtual async Task RemoveUserHereCommand(CommandContext ctx)
	{
		try
		{
			await this.UserService.RemoveUserHereAsync(ctx.User.Id, ctx.Guild.Id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			var embed = ex is UserProcessingException ? EmbedTemplate.ErrorEmbed(ctx, ex.Message) : EmbedTemplate.UnknownErrorEmbed(ctx);
			await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
			throw;
		}

		await ctx.RespondAsync(embed: EmbedTemplate.SuccessEmbed(ctx, $"Now your updates won't appear in this server")).ConfigureAwait(false);
	}

	public virtual async Task ListUsersCommand(CommandContext ctx)
	{
		var sb = new StringBuilder();
		try
		{
			var i = 1;
			await foreach (var user in this.UserService.ListUsersAsync(ctx.Guild.Id))
			{
				if (sb.Length + user.Username.Length > 2048)
				{
					if (sb.Length + "...".Length > 2048)
						break;

					sb.Append("...");
					break;
				}

				sb.AppendLine(
					$"{i++.ToString()}{user.Username} {(user.DiscordUser == null ? "" : Helpers.ToDiscordMention(user.DiscordUser.DiscordUserId))}");
			}
		}
		catch (Exception ex)
		{
			var embed = ex is UserProcessingException ? EmbedTemplate.ErrorEmbed(ctx, ex.Message) : EmbedTemplate.UnknownErrorEmbed(ctx);
			await ctx.RespondAsync(embed: embed).ConfigureAwait(false);
			throw;
		}

		await ctx.RespondAsync(embed: EmbedTemplate.SuccessEmbed(ctx, "Users").WithDescription(sb.ToString())).ConfigureAwait(false);
	}
}