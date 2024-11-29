// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PaperMalKing.Shikimori.Wrapper.Abstractions.Models;

public sealed class Favourites : IJsonOnDeserialized
{
	private const string PeopleType = "people";
	private const string MangasType = "mangas";

	public static Favourites Empty { get; } = new();

	private readonly List<FavouriteEntry> _allFavourites = new(20);

	public IReadOnlyList<FavouriteEntry> AllFavourites => this._allFavourites;

	public IReadOnlyList<FavouriteEntry> Animes
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, "animes", "Anime");
	}

	public IReadOnlyList<FavouriteEntry> Mangas
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, MangasType, "Manga");
	}

	public IReadOnlyList<FavouriteEntry> Characters
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, "characters", "Character");
	}

	public IReadOnlyList<FavouriteEntry> People
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, PeopleType, "Person");
	}

	public IReadOnlyList<FavouriteEntry> Mangakas
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, PeopleType, "Mangaka");
	}

	public IReadOnlyList<FavouriteEntry> Seyu
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, PeopleType, "Seyu");
	}

	public IReadOnlyList<FavouriteEntry> Producers
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, PeopleType, "Producer");
	}

	public IReadOnlyList<FavouriteEntry> Ranobe
	{
		get => ThrowNotSupportedException();
		init => this.SetTypesThenAddToAll(value, MangasType, "Ranobe");
	}

	private void SetTypesThenAddToAll(IReadOnlyList<FavouriteEntry> entries, string genericType, string? specificType)
	{
		foreach (var entry in entries)
		{
			entry.GenericType = genericType;
			entry.SpecificType = specificType;
		}

		this._allFavourites.AddRange(entries);
	}

	void IJsonOnDeserialized.OnDeserialized()
	{
		this._allFavourites.Sort();
	}

	[DoesNotReturn]
	private static IReadOnlyList<FavouriteEntry> ThrowNotSupportedException() => throw new NotSupportedException("Used only for json serializer");
}