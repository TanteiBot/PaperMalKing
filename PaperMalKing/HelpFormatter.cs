﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2022 N0D4N

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Humanizer;
using PaperMalKing.Common.Attributes;

namespace PaperMalKing;

/// <summary>
/// Custom help formatter for CommandsNext
/// </summary>
public sealed class HelpFormatter : BaseHelpFormatter
{
	public DiscordEmbedBuilder EmbedBuilder { get; }
	private Command Command { get; set; } = null!;

	/// <summary>
	/// Creates a new help formatter.
	/// </summary>
	/// <param name="ctx">Context in which this formatter is being invoked.</param>
	public HelpFormatter(CommandContext ctx) : base(ctx)
	{
		this.EmbedBuilder = new DiscordEmbedBuilder().WithTitle("Help").WithColor(DiscordColor.Azure);
	}

	/// <summary>
	/// Sets the command this help message will be for.
	/// </summary>
	/// <param name="command">Command for which the help message is being produced.</param>
	/// <returns>This help formatter.</returns>
	public override BaseHelpFormatter WithCommand(Command command)
	{
		this.Command = command;

		this.EmbedBuilder.WithDescription($"{Formatter.InlineCode(command.Name)}: {command.Description ?? "No description provided."}");

		if (command is CommandGroup { IsExecutableWithoutSubcommands: true })
			this.EmbedBuilder.WithDescription($"{this.EmbedBuilder.Description}\n\nThis group can be executed as a standalone command.");

		if (command.Aliases?.Any() == true)
			this.EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);

		if (command.Overloads?.Any() == true)
		{
			var sb = new StringBuilder();

#pragma warning disable S3267
			foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
#pragma warning restore S3267
			{
				sb.Append('`').Append(command.QualifiedName);

				foreach (var arg in ovl.Arguments)
					sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "…" : "")
					  .Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

				sb.Append("`\n");

				foreach (var arg in ovl.Arguments)
					sb.Append('`').Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ")
					  .Append(arg.Description ?? "No description provided.").Append('\n');

				sb.Append('\n');
			}

			this.EmbedBuilder.AddField("Arguments", sb.ToString().Trim(), false);
		}

		var exChecks = this.GetExecutionChecks();

		if (!string.IsNullOrWhiteSpace(exChecks))
			this.EmbedBuilder.AddField("Command requirements", exChecks, false);

		return this;
	}

	/// <summary>
	/// Sets the subcommands for this command, if applicable. This method will be called with filtered data.
	/// </summary>
	/// <param name="subcommands">Subcommands for this command group.</param>
	/// <returns>This help formatter.</returns>
	public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
	{
		if (this.Command is not null)
			this.EmbedBuilder.AddField("Subcommands", string.Join(", ", subcommands.Select(x => Formatter.InlineCode(x.Name))), false);
		else
		{
			var cmdList = new List<Command>();
			foreach (var cmd in subcommands)
			{
				if (cmd is CommandGroup cGroup)
				{
					if (cGroup.Parent is not null)
						continue;
					var chs = cGroup.Children.ToList();

					if (cGroup.IsExecutableWithoutSubcommands)
						chs.Add(cGroup);
					var shortestAlias = cGroup.Aliases.Any() ? $"({cGroup.Aliases.MinBy(alias => alias.Length)})" : "";
					this.EmbedBuilder.AddField($"{cGroup.Name.Humanize(LetterCasing.Sentence)} {shortestAlias} commands",
						string.Join(", ", chs.Select(x => Formatter.InlineCode(x.Name))), false);
				}
				else
					cmdList.Add(cmd);
			}

			if (cmdList.Any())
				this.EmbedBuilder.AddField("Ungrouped commands", string.Join(", ", cmdList.Select(x => Formatter.InlineCode(x.Name))), false);
		}


		return this;
	}

	/// <summary>
	/// Construct the help message.
	/// </summary>
	/// <returns>Data for the help message.</returns>
	public override CommandHelpMessage Build()
	{
		if (this.Command is null)
			this.EmbedBuilder.WithDescription("List of all commands.");

		return new CommandHelpMessage(embed: this.EmbedBuilder.Build());
	}

	/// <summary>
	/// Get a string representation of all execution checks for command and it's parents.
	/// </summary>
	/// <returns>String representation of all execution checks for command</returns>
	private string GetExecutionChecks()
	{
		var cmd = this.Command;
		var exChecksSb = new StringBuilder();

		while (cmd is not null)
		{
			if (cmd.ExecutionChecks?.Any() == true)
			{
				if (cmd.ExecutionChecks.Any(x => x is RequireOwnerAttribute))
					exChecksSb.AppendLine("To execute this command you need to be the owner of the bot.");

				if (cmd.ExecutionChecks.SingleOrDefault(x => x is OwnerOrPermissionsAttribute) is OwnerOrPermissionsAttribute ownerOrPerms)
					exChecksSb.AppendLine(
						$"To execute this command you need to be the owner of the bot or have this permissions {Formatter.InlineCode(ownerOrPerms.Permissions.ToPermissionString())}.");
			}

			cmd = cmd.Parent;
		}

		return exChecksSb.ToString().Trim();
	}
}