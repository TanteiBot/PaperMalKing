﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace PaperMalKing.Database.CompiledModels
{
    public partial class DatabaseContextModel
    {
        private DatabaseContextModel()
            : base(skipDetectChanges: false, modelId: new Guid("f66843e2-82bf-4c5a-ad00-b7e223290643"), entityTypeCount: 19)
        {
        }

        partial void Initialize()
        {
            var discordGuildDiscordUser = DiscordGuildDiscordUserEntityType.Create(this);
            var aniListFavourite = AniListFavouriteEntityType.Create(this);
            var aniListUser = AniListUserEntityType.Create(this);
            var customUpdateColor = CustomUpdateColorEntityType.Create(this);
            var botUser = BotUserEntityType.Create(this);
            var discordGuild = DiscordGuildEntityType.Create(this);
            var discordUser = DiscordUserEntityType.Create(this);
            var baseMalFavorite = BaseMalFavoriteEntityType.Create(this);
            var malUser = MalUserEntityType.Create(this);
            var customUpdateColor0 = CustomUpdateColor0EntityType.Create(this);
            var shikiDbAchievement = ShikiDbAchievementEntityType.Create(this);
            var shikiFavourite = ShikiFavouriteEntityType.Create(this);
            var shikiUser = ShikiUserEntityType.Create(this);
            var customUpdateColor1 = CustomUpdateColor1EntityType.Create(this);
            var malFavoriteAnime = MalFavoriteAnimeEntityType.Create(this, baseMalFavorite);
            var malFavoriteCharacter = MalFavoriteCharacterEntityType.Create(this, baseMalFavorite);
            var malFavoriteCompany = MalFavoriteCompanyEntityType.Create(this, baseMalFavorite);
            var malFavoriteManga = MalFavoriteMangaEntityType.Create(this, baseMalFavorite);
            var malFavoritePerson = MalFavoritePersonEntityType.Create(this, baseMalFavorite);

            DiscordGuildDiscordUserEntityType.CreateForeignKey1(discordGuildDiscordUser, discordGuild);
            DiscordGuildDiscordUserEntityType.CreateForeignKey2(discordGuildDiscordUser, discordUser);
            AniListFavouriteEntityType.CreateForeignKey1(aniListFavourite, aniListUser);
            AniListUserEntityType.CreateForeignKey1(aniListUser, discordUser);
            CustomUpdateColorEntityType.CreateForeignKey1(customUpdateColor, aniListUser);
            DiscordUserEntityType.CreateForeignKey1(discordUser, botUser);
            MalUserEntityType.CreateForeignKey1(malUser, discordUser);
            CustomUpdateColor0EntityType.CreateForeignKey1(customUpdateColor0, malUser);
            ShikiDbAchievementEntityType.CreateForeignKey1(shikiDbAchievement, shikiUser);
            ShikiFavouriteEntityType.CreateForeignKey1(shikiFavourite, shikiUser);
            ShikiUserEntityType.CreateForeignKey1(shikiUser, discordUser);
            CustomUpdateColor1EntityType.CreateForeignKey1(customUpdateColor1, shikiUser);
            MalFavoriteAnimeEntityType.CreateForeignKey1(malFavoriteAnime, malUser);
            MalFavoriteCharacterEntityType.CreateForeignKey1(malFavoriteCharacter, malUser);
            MalFavoriteCompanyEntityType.CreateForeignKey1(malFavoriteCompany, malUser);
            MalFavoriteMangaEntityType.CreateForeignKey1(malFavoriteManga, malUser);
            MalFavoritePersonEntityType.CreateForeignKey1(malFavoritePerson, malUser);

            DiscordGuildEntityType.CreateSkipNavigation1(discordGuild, discordUser, discordGuildDiscordUser);
            DiscordUserEntityType.CreateSkipNavigation1(discordUser, discordGuild, discordGuildDiscordUser);

            DiscordGuildDiscordUserEntityType.CreateAnnotations(discordGuildDiscordUser);
            AniListFavouriteEntityType.CreateAnnotations(aniListFavourite);
            AniListUserEntityType.CreateAnnotations(aniListUser);
            CustomUpdateColorEntityType.CreateAnnotations(customUpdateColor);
            BotUserEntityType.CreateAnnotations(botUser);
            DiscordGuildEntityType.CreateAnnotations(discordGuild);
            DiscordUserEntityType.CreateAnnotations(discordUser);
            BaseMalFavoriteEntityType.CreateAnnotations(baseMalFavorite);
            MalUserEntityType.CreateAnnotations(malUser);
            CustomUpdateColor0EntityType.CreateAnnotations(customUpdateColor0);
            ShikiDbAchievementEntityType.CreateAnnotations(shikiDbAchievement);
            ShikiFavouriteEntityType.CreateAnnotations(shikiFavourite);
            ShikiUserEntityType.CreateAnnotations(shikiUser);
            CustomUpdateColor1EntityType.CreateAnnotations(customUpdateColor1);
            MalFavoriteAnimeEntityType.CreateAnnotations(malFavoriteAnime);
            MalFavoriteCharacterEntityType.CreateAnnotations(malFavoriteCharacter);
            MalFavoriteCompanyEntityType.CreateAnnotations(malFavoriteCompany);
            MalFavoriteMangaEntityType.CreateAnnotations(malFavoriteManga);
            MalFavoritePersonEntityType.CreateAnnotations(malFavoritePerson);

            AddAnnotation("ProductVersion", "9.0.0");
        }
    }
}
