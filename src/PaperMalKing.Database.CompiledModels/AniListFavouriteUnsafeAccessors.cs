﻿// <auto-generated />
using System;
using System.Runtime.CompilerServices;
using PaperMalKing.Database.Models.AniList;

#pragma warning disable 219, 612, 618
#nullable disable

namespace PaperMalKing.Database.CompiledModels
{
    public static class AniListFavouriteUnsafeAccessors
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Id>k__BackingField")]
        public static extern ref uint Id(AniListFavourite @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<FavouriteType>k__BackingField")]
        public static extern ref FavouriteType FavouriteType(AniListFavourite @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<UserId>k__BackingField")]
        public static extern ref uint UserId(AniListFavourite @this);

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<User>k__BackingField")]
        public static extern ref AniListUser User(AniListFavourite @this);
    }
}
