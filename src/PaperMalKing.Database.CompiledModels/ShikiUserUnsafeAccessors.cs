﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PaperMalKing.Database.Models;
using PaperMalKing.Database.Models.Shikimori;

#pragma warning disable 219, 612, 618
#nullable disable

namespace PaperMalKing.Database.CompiledModels
{
    public static class ShikiUserUnsafeAccessors
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Id>k__BackingField")]
        public static extern ref uint Id(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<DiscordUserId>k__BackingField")]
        public static extern ref ulong DiscordUserId(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<FavouritesIdHash>k__BackingField")]
        public static extern ref string FavouritesIdHash(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Features>k__BackingField")]
        public static extern ref ShikiUserFeatures Features(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<LastHistoryEntryId>k__BackingField")]
        public static extern ref uint LastHistoryEntryId(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Achievements>k__BackingField")]
        public static extern ref List<ShikiDbAchievement> Achievements(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Colors>k__BackingField")]
        public static extern ref List<CustomUpdateColor> Colors(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<DiscordUser>k__BackingField")]
        public static extern ref DiscordUser DiscordUser(ShikiUser @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Favourites>k__BackingField")]
        public static extern ref IList<ShikiFavourite> Favourites(ShikiUser @this);
    }
}
