﻿// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Collections.Generic;
using System.ComponentModel;
using PaperMalKing.AniList.Wrapper.Abstractions.Models.Enums;
using PaperMalKing.Common;

namespace PaperMalKing.AniList.Wrapper.Abstractions.Models;

public sealed class Favourites
{
	public static Favourites Empty { get; } = new() { HasNextPage = false };

	public bool HasNextPage { get; init; }

	private readonly List<IdentifiableFavourite> _allFavourites = new(4);

	public IReadOnlyList<IdentifiableFavourite> AllFavourites => this._allFavourites;

	public Connection<IdentifiableFavourite> Anime
	{
		[Obsolete("This property is used only for JSON deserialization", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		get => ThrowNotSupportedException();
		init
		{
			if (value.PageInfo!.HasNextPage)
			{
				this.HasNextPage = value.PageInfo.HasNextPage;
			}

			value.Nodes.ForEach(static fav => fav.Type = FavouriteType.Anime);
			this._allFavourites.AddRange(value.Nodes);
		}
	}

	public Connection<IdentifiableFavourite> Manga
	{
		[Obsolete("This property is used only for JSON deserialization", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		get => ThrowNotSupportedException();
		init
		{
			if (value.PageInfo!.HasNextPage)
			{
				this.HasNextPage = value.PageInfo.HasNextPage;
			}

			value.Nodes.ForEach(static fav => fav.Type = FavouriteType.Manga);
			this._allFavourites.AddRange(value.Nodes);
		}
	}

	public Connection<IdentifiableFavourite> Characters
	{
		[Obsolete("This property is used only for JSON deserialization", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		get => ThrowNotSupportedException();
		init
		{
			if (value.PageInfo!.HasNextPage)
			{
				this.HasNextPage = value.PageInfo.HasNextPage;
			}

			value.Nodes.ForEach(static fav => fav.Type = FavouriteType.Characters);
			this._allFavourites.AddRange(value.Nodes);
		}
	}

	public Connection<IdentifiableFavourite> Staff
	{
		[Obsolete("This property is used only for JSON deserialization", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		get => ThrowNotSupportedException();
		init
		{
			if (value.PageInfo!.HasNextPage)
			{
				this.HasNextPage = value.PageInfo.HasNextPage;
			}

			value.Nodes.ForEach(static fav => fav.Type = FavouriteType.Staff);
			this._allFavourites.AddRange(value.Nodes);
		}
	}

	public Connection<IdentifiableFavourite> Studios
	{
		[Obsolete("This property is used only for JSON deserialization", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		get => ThrowNotSupportedException();
		init
		{
			if (value.PageInfo!.HasNextPage)
			{
				this.HasNextPage = value.PageInfo.HasNextPage;
			}

			value.Nodes.ForEach(static fav => fav.Type = FavouriteType.Studios);
			this._allFavourites.AddRange(value.Nodes);
		}
	}

	private static Connection<IdentifiableFavourite> ThrowNotSupportedException()
	{
		throw new NotSupportedException("You shouldn't access this property");
	}
}