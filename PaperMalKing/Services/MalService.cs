﻿/*
 * Cleaned up a bit should look nice now
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaperMalKing.Data;
using PaperMalKing.MyAnimeList;
using PaperMalKing.MyAnimeList.Exceptions;
using PaperMalKing.MyAnimeList.FeedReader;
using PaperMalKing.MyAnimeList.Jikan;
using PaperMalKing.MyAnimeList.Jikan.Data.Interfaces;
using PaperMalKing.Utilities;

namespace PaperMalKing.Services
{
	public sealed class MalService
	{
		/// <summary>
		/// Is currently bot checking for updates
		/// </summary>
		public bool Updating { get; private set; }

		/// <summary>
		/// Regex that is used to get Id from urls
		/// </summary>
		private readonly Regex _regex = new Regex(@"(?<=\/)(\d*?)(?=\/)", RegexOptions.Compiled);

		/// <summary>
		/// Channels where updates will be sent
		/// </summary>
		private readonly ConcurrentDictionary<long, DiscordChannel> _channels;

		/// <summary>
		/// Bots' config
		/// </summary>
		private readonly BotConfig _config;

		/// <summary>
		/// Client for interacting with Discord
		/// </summary>
		private readonly DiscordClient _discordClient;

		/// <summary>
		/// Timer that handles delays between checks for updates
		/// </summary>
		private readonly Timer _timer;

		/// <summary>
		/// Client for interacting with Jikan REST API
		/// </summary>
		private readonly JikanClient _jikanClient;

		private const string LogName = "MalService";

		/// <summary>
		/// Delay between checks for updates
		/// </summary>
		private readonly TimeSpan _timerDelay;

		/// <summary>
		/// MyAnimeList RSS feeds reader
		/// </summary>
		private readonly FeedReader _rssReader;

		private readonly ClockService _clock;
		private readonly HttpClient _httpClient;
		private readonly IServiceProvider _services;

		private delegate Task UpdateFoundHandler(ListUpdateEntry update);

		private event UpdateFoundHandler UpdateFound;

		public MalService(BotConfig config, DiscordClient discordClient, FeedReader reader, ClockService clock,
						  HttpClient httpClient, IServiceProvider services)
		{
			this._rssReader = reader;
			this._clock = clock;
			this._httpClient = httpClient;
			this._services = services;
			this._channels = new ConcurrentDictionary<long, DiscordChannel>();
			this._config = config;
			this._discordClient = discordClient;
			discordClient.Ready += this.Client_Ready;
			this.UpdateFound += this.MalService_UpdateFound;
			this._jikanClient = services.GetRequiredService<JikanClient>();
			this._timerDelay = TimeSpan.FromMilliseconds(config.MyAnimeList.DelayBetweenUpdateChecks);
			this._timer = new Timer(async (e) =>
			{
				try
				{
					await this.Timer_Tick();
				}
				catch (Exception ex)
				{
					this._discordClient.DebugLogger.LogMessage(LogLevel.Error, LogName,
						"Exception occured in Timer_Tick method", this._clock.Now, ex);
				}
			}, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
		}

		public void RestartTimer()
		{
			if (!this.Updating)
				this._timer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
		}

		public async Task AddUserAsync(DiscordMember member, string username, DatabaseContext db)
		{
			var userId = (long) member.Id;
			var user = db.Users.FirstOrDefault(x => x.DiscordId == userId);
			if (user == null) //User is adding himself in the first time
			{
				var feeds = new[]
				{
					$"https://myanimelist.net/rss.php?type=rw&u={username}",
					$"https://myanimelist.net/rss.php?type=rm&u={username}"
				};
				foreach (var feed in feeds)
				{
					RssLoadResult res;
					try
					{
						res = await this._rssReader.GetRssFeedLoadResult(feed);
					}
					catch (ServerSideException ex)
					{
						this._discordClient.DebugLogger.LogMessage(LogLevel.Warning, LogName, ex.Message,
							this._clock.Now);
						throw new Exception("Mal is having some issues. Try again later.");
					}

					if (res == RssLoadResult.NotFound)
						throw new Exception($"Can't find MyAnimeList account with username '{username}'.");
					if (res == RssLoadResult.Forbidden)
						throw new Exception("One of your lists isn't public, make it public and try again.");
					if (res == RssLoadResult.Unknown) throw new Exception("Unhandled exception happened.");
				}

				var guildId = (long) member.Guild.Id;
				var guild = db.Guilds.FirstOrDefault(x => x.GuildId == guildId);
				if (guild == null)
				{
					guild = new PmkGuild
					{
						ChannelId = null,
						GuildId = guildId,
						Users = null
					};
					await db.Guilds.AddAsync(guild);
				}

				var guilds = new List<GuildUsers>
				{
					new GuildUsers
					{
						DiscordId = userId,
						GuildId = guildId
					}
				};

				var pmkUser = new PmkUser
				{
					DiscordId = userId,
					LastUpdateDate = this._clock.Now.ToUniversalTime(),
					MalUsername = username,
					Guilds = guilds
				};

				await db.Users.AddAsync(pmkUser);
				this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
					$"Added new user '{username}'({member})", this._clock.Now);
			}
			else // User is already saved in another guilds
			{
				var guildId = (long) member.Guild.Id;
				var guild = db.Guilds.FirstOrDefault(x => x.GuildId == guildId);
				if (guild == null)
				{
					guild = new PmkGuild
					{
						ChannelId = null,
						GuildId = guildId,
						Users = null
					};
					await db.Guilds.AddAsync(guild);
				}

				if (user.Guilds?.All(x => x.GuildId != guildId) == true)
				{
					user.Guilds.Add(new GuildUsers
					{
						DiscordId = user.DiscordId,
						GuildId = guildId
					});
					db.Update(user);
					this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
						$"Added ({member}) in guild '{guildId}'", this._clock.Now);
				}
				else
					throw new Exception("You are already registered in this guild.");
			}

			var rowChanged = await db.SaveChangesAsync();

			if (rowChanged == 0)
				throw new Exception("Couldn't save in database. Try again later.");
		}

		public async Task AddUserHereAsync(DiscordMember member, DatabaseContext db)
		{
			var userId = (long) member.Id;
			var user = db.Users.FirstOrDefault(x => x.DiscordId == userId);
			if (user == null)
				throw new Exception("You must add username in this or other guild first");
			var guildId = (long) member.Guild.Id;
			if (user.Guilds?.All(x => x.GuildId != guildId) == true)
			{
				user.Guilds.Add(new GuildUsers
				{
					DiscordId = user.DiscordId,
					GuildId = guildId
				});
				db.Update(user);
				this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
					$"Added ({member}) in guild '{guildId}'", this._clock.Now);
				var rowChanged = await db.SaveChangesAsync();
				if (rowChanged > 0)
					return;
			}

			throw new Exception("You are already added in this guild");
		}


		public async Task RemoveUserEverywhereAsync(DiscordMember member, DatabaseContext db)
		{
			var userId = (long) member.Id;
			var user = db.Users.FirstOrDefault(x => x.DiscordId == userId);
			if (user == null)
				throw new ArgumentException("Such user does not exist in database", nameof(user));
			db.Users.Remove(user);
			var rowsChanged = await db.SaveChangesAsync();
			if (rowsChanged == 0)
				throw new Exception("Couldn't save changes in database. Try again later");
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Successfully removed user '{user.MalUsername}'({member}) from all guilds", this._clock.Now);
		}

		public async Task RemoveUserHereAsync(DiscordMember member, DatabaseContext db)
		{
			var userId = (long) member.Id;
			var guildId = (long) member.Guild.Id;
			var user = db.Users.FirstOrDefault(x => x.DiscordId == userId);
			if (user == null)
				throw new ArgumentException("Such user does not exist in database", nameof(user));
			if (user.Guilds.Count == 1)
			{
				db.Users.Remove(user);
			}
			else
			{
				var guild = user.Guilds.FirstOrDefault(x => x.GuildId == guildId);
				if (guild == null)
					throw new Exception("You are not found in this guild(??), try again later.");
				user.Guilds.Remove(guild);
				db.Users.Update(user);
			}

			var rowsChanged = await db.SaveChangesAsync();
			if (rowsChanged == 0)
				throw new Exception("Couldn't save changes in database. Try again later");
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Successfully removed user '{user.MalUsername}'({member}) from {member.Guild}", this._clock.Now);
		}

		public async Task UpdateUserAsync(long userId, string newUsername, DatabaseContext db)
		{
			var user = db.Users.FirstOrDefault(x => x.DiscordId == userId);
			if (user == null)
				throw new ArgumentException("Such user does not exist in database", nameof(user));
			if (user.MalUsername == newUsername)
				throw new ArgumentException("New username can't be the same as the old one");
			var oldUsername = user.MalUsername;
			user.MalUsername = newUsername;
			var feeds = new[] {user.AnimeRssFeed, user.MangaRssFeed};
			foreach (var feed in feeds)
			{
				RssLoadResult res;
				try
				{
					res = await this._rssReader.GetRssFeedLoadResult(feed);
				}
				catch (ServerSideException ex)
				{
					this._discordClient.DebugLogger.LogMessage(LogLevel.Warning, LogName, ex.Message, this._clock.Now);
					throw new Exception("Mal is having some issues. Try again later.");
				}

				if (res == RssLoadResult.NotFound)
					throw new Exception($"Can't find MyAnimeList account with username '{newUsername}'.");
				if (res == RssLoadResult.Forbidden)
					throw new Exception("One of your lists isn't public, make it public and try again.");
				if (res == RssLoadResult.Unknown) throw new Exception("Unhandled exception happened.");
			}

			db.Users.Update(user);
			var rowChanges = await db.SaveChangesAsync();
			if (rowChanges == 0)
				throw new Exception("Couldn't save update in database. Try again later");
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Updated user with id'{userId}' from '{oldUsername}' to '{newUsername}'", this._clock.Now);
		}

		public async Task AddChannelAsync(long guildId, long channelId, DatabaseContext db)
		{
			var uChannelId = (ulong) channelId;
			var channel = await this._discordClient.GetChannelAsync(uChannelId);


			var guild = db.Guilds.FirstOrDefault(x => x.GuildId == guildId);
			if (guild == null)
			{
				guild = new PmkGuild
				{
					GuildId = guildId,
					ChannelId = channelId
				};

				await db.Guilds.AddAsync(guild);
			}
			else if (guild.ChannelId == null)
			{
				guild.ChannelId = channelId;
			}
			else
				throw new Exception(
					"Guild with channel is already in database. Use ChannelUpdate command instead of ChannelAdd");

			await db.SaveChangesAsync();

			this._channels.TryAdd(guildId, channel);
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Successfully added channel in guild with id '{guildId}'", this._clock.Now);
		}

		public async Task UpdateChannelAsync(long guildId, long channelId, DatabaseContext db)
		{
			var uChannelId = (ulong) channelId;
			var channel = await this._discordClient.GetChannelAsync(uChannelId);

			var guild = db.Guilds.FirstOrDefault(x => x.GuildId == guildId);
			if (guild == null)
				throw new Exception("Channel is not saved in database try to add it instead of updating it");
			if (guild.ChannelId == channelId)
				throw new Exception("New channel can't be the same as the old one");
			guild.ChannelId = channelId;
			db.Guilds.Update(guild);
			await db.SaveChangesAsync();

			this._channels[guildId] = channel;
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Successfully updated channel in guild with id '{guildId}'", this._clock.Now);
		}

		public async Task RemoveChannelAsync(long guildId, DatabaseContext db)
		{
			var guild = db.Guilds.FirstOrDefault(x => x.GuildId == guildId);
			if (guild != null)
			{
				this._channels.TryRemove(guildId, out _);
				if (guild.ChannelId == null) return;
				guild.ChannelId = null;
				db.Guilds.Update(guild);
				await db.SaveChangesAsync();
			}

			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Successfully removed channel in guild with id '{guildId}'", this._clock.Now);
		}

		private async Task<IMalEntity> GetMalEntityAsync(EntityType type, FeedItem feedItem, PmkUser pmkUser)
		{
			var malUnparsedId = this._regex.Matches(feedItem.Link)
									.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Value))?.Value;

			if (!long.TryParse(malUnparsedId, out long malId))
			{
				this._discordClient.DebugLogger.LogMessage(LogLevel.Error, LogName, $"Couldn't parse {malUnparsedId}",
					this._clock.Now);
				return null;
			}

			if (type == EntityType.Anime)
			{
				var animeListEntry = pmkUser.RecentlyUpdatedAnime.FirstOrDefault(x => x.MalId == malId);
				if (animeListEntry != null)
					return animeListEntry;
			}
			else
			{
				var mangaListEntry = pmkUser.RecentlyUpdatedManga.FirstOrDefault(x => x.MalId == malId);
				if (mangaListEntry != null)
					return mangaListEntry;
			}

			if (feedItem.IsPlanToCheck)
			{
				if (type == EntityType.Anime)
					return await this._jikanClient.GetAnimeAsync(malId);
				return await this._jikanClient.GetMangaAsync(malId);
			}

			var index = feedItem.Title.LastIndexOf(" - ", StringComparison.InvariantCulture);
			var query = feedItem.Title.Remove(index).Trim();
			if (type == EntityType.Anime)
			{
				var userAl = await this._jikanClient.GetUserAnimeListAsync(pmkUser.MalUsername, query);
				if (userAl?.Anime?.Any() != true)
				{
					this._discordClient.DebugLogger.LogMessage(LogLevel.Error, LogName,
						$"Couldn't load '{query}' from '{pmkUser.MalUsername}'s animelist", this._clock.Now);
					return await this._jikanClient.GetAnimeAsync(malId);
				}

				return userAl.Anime.FirstOrDefault(x => x.MalId == malId);
			}

			var userMl = await this._jikanClient.GetUserMangaList(pmkUser.MalUsername, query);
			if (userMl?.Manga?.Any() != true)
			{
				this._discordClient.DebugLogger.LogMessage(LogLevel.Error, LogName,
					$"Couldn't load '{query}' from '{pmkUser.MalUsername}'s mangalist", this._clock.Now);
				return await this._jikanClient.GetMangaAsync(malId);
			}

			return userMl.Manga.FirstOrDefault(x => x.MalId == malId);
		}

		private async Task MalService_UpdateFound(ListUpdateEntry update)
		{
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
				$"Sending update for {update.Entry.Title} in {update.User.MalUsername} MAL", this._clock.Now);

			var embed = update.CreateEmbed();

			foreach (var guildId in update.User.Guilds.Select(x => x.GuildId))
			{
				if (!this._channels.TryGetValue(guildId, out var channel)) continue;
				try
				{
					await channel.SendMessageAsync(embed: embed);
				}
				catch
				{
					// ignored
				}
			}
		}

		private async Task Client_Ready(ReadyEventArgs e)
		{
			using (var scope = this._services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
				foreach (var guild in db.Guilds)
				{
					try
					{
						if (guild.ChannelId == null)
							continue;
						var channelId = (ulong) guild.ChannelId.Value;
						var channel = await this._discordClient.GetChannelAsync(channelId);
						this._channels.TryAdd(guild.GuildId, channel);
						e.Client.DebugLogger.LogMessage(LogLevel.Info, LogName,
							$"Successfully loaded channel for guild with id '{guild.GuildId}'", this._clock.Now);
					}
					catch (Exception ex)
					{
						e.Client.DebugLogger.LogMessage(LogLevel.Critical, LogName,
							$"Channel wasn't loaded successfully in guild with id '{guild.GuildId}'", this._clock.Now,
							ex);
					}
				}
			}

			e.Client.DebugLogger.LogMessage(LogLevel.Info, LogName, "Loaded channels for all guilds", this._clock.Now);

			this.RestartTimer();

			this._discordClient.Ready -= this.Client_Ready;
		}

		// Cleaned up a bit should look better now
		private async Task Timer_Tick()
		{
			this.Updating = true;
			this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName, "Starting checking for updates",
				this._clock.Now);

			var scope = this._services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
			try
			{
				var users = db.Users.Include(ug => ug.Guilds).ThenInclude(g => g.Guild).ToArray().Shuffle();
				foreach (var user in users)
				{
					this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
						$"Starting to checking updates for {user.MalUsername}", this._clock.Now);
					var feeds = new Feed[2];
					var rssUrls = new[] {user.AnimeRssFeed, user.MangaRssFeed};

					for (int i = 0; i <= 1; i++)
					{
						try
						{
							feeds[i] = await this._rssReader.ReadFeedAsync(rssUrls[i]);
						}
						catch (MalRssException ex)
						{
							this._discordClient.DebugLogger.LogMessage(LogLevel.Warning, LogName,
								$"Couldn't read {ex.ListType}list because {ex.Reason}", this._clock.Now);
						}
					}

					var updateItems = new List<(FeedItem, EntityType)>();

					foreach (var feed in feeds)
					{
						updateItems.AddRange(
							feed?.Items.Where(x => DateTime.Compare(x.PublishingDateTime, user.LastUpdateDate) > 0)
								.Select(x => (x, feed.GetEntitiesType())) ??
							Enumerable.Empty<(FeedItem, EntityType)>());
					}

					if (!updateItems.Any())
						continue;

					if (!user.MalId.HasValue)
					{
						var malUser = await this._jikanClient.GetUserProfileAsync(user.MalUsername);
						if (malUser == null)
						{
							this._discordClient.DebugLogger.LogMessage(LogLevel.Error, LogName,
								$"Couldn't load MyAnimeList user from username '{user.MalUsername}' (DiscordId '{user.DiscordId}'",
								this._clock.Now);
							continue;
						}

						user.MalId = malUser.UserId;
						db.Users.Update(user);
					}

					updateItems.Sort((x, y) => DateTime.Compare(x.Item1.PublishingDateTime,
						y.Item1.PublishingDateTime));
					var latestUpdateDate = user.LastUpdateDate;
					if (updateItems.Any(x => x.Item2 == EntityType.Anime))
						user.RecentlyUpdatedAnime =
							(await this._jikanClient.GetUserRecentlyUpdatedAnimeAsync(user.MalUsername)).Anime;
					if (updateItems.Any(x => x.Item2 == EntityType.Manga))
						user.RecentlyUpdatedManga =
							(await this._jikanClient.GetUserRecentlyUpdatedMangaAsync(user.MalUsername)).Manga;

					try
					{
						foreach (var (feedItem, entityType) in updateItems)
						{
							var malEntity = await this.GetMalEntityAsync(entityType, feedItem, user);
							if (malEntity == null)
								continue;
							var actionString = feedItem.Description.Split(" - ")[0];
							var status = feedItem.Description;
							if (string.IsNullOrWhiteSpace(actionString))
							{
								if (entityType == EntityType.Anime)
									status = "Re-watching" + status;
								else
									status = "Re-reading" + status;
							}

							var listUpdateEntry = new ListUpdateEntry(user, malEntity,
								status, feedItem.PublishingDateTime);
							await this.UpdateFound?.Invoke(listUpdateEntry);
							latestUpdateDate = feedItem.PublishingDateTime;
						}
					}
					finally
					{
						if (user.LastUpdateDate != latestUpdateDate)
						{
							user.LastUpdateDate = latestUpdateDate.ToUniversalTime();
							db.Users.Update(user);
						}
					}
				}
			}
			catch (ServerSideException ex)
			{
				this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName,
					"Skipping planned check for updates because Mal or Jikan are having some issues",
					this._clock.Now, ex);
				return;
			}
			finally
			{
				await db.SaveChangesAsync();
				scope.Dispose();
				this.Updating = false;
				this._discordClient.DebugLogger.LogMessage(LogLevel.Info, LogName, "Ended checking for updates",
					this._clock.Now);
				this._timer.Change(this._timerDelay, TimeSpan.FromMilliseconds(-1));
			}
		}
	}
}