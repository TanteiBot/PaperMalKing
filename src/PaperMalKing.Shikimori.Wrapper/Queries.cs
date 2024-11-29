// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using GraphQL;
using PaperMalKing.Shikimori.Wrapper.Abstractions;

namespace PaperMalKing.Shikimori.Wrapper;

internal static class Queries
{
	[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Not applicable to multiline strings")]
	public static string GetUserByIdQuery(uint userId)
		=> string.Create(CultureInfo.InvariantCulture, $$"""
														query {
															users (ids: [{{userId}}], limit: 1) {
																id,
																nickname
															}
														}
														""");

	public static GraphQLQuery UserByNicknameQuery { get; } = new(
		"""
		query ($nickname: String) {
			users (search: $nickname, limit: 1) {
				id,
				nickname
			}
		}
		""");

	private const string GenresSubquery = "genres { name russian }";

	private const string DescriptionSubquery = "description";

	public static string GetMangaQuery(RequestOptions options)
	{
		return $$"""
				query ($ids: String) {
					media: mangas (ids: $ids) {
						{{((options & RequestOptions.Publisher) != 0 ? " publishers { name, id } " : "")}}
						{{((options & RequestOptions.Mangaka) != 0 ? " personRoles { roles_russian: rolesRu, roles: rolesEn, person { name, russian, id, isMangaka } } " : "")}}
						{{((options & RequestOptions.Genres) != 0 ? GenresSubquery : "")}}
						{{((options & RequestOptions.Description) != 0 ? DescriptionSubquery : "")}}
					} 
				}
				""";
	}

	public static string GetAnimeQuery(RequestOptions options)
	{
		return $$"""
				query ($ids: String) {
					media: animes (ids: $ids) {
						{{((options & RequestOptions.Studio) != 0 ? "studios { name, id }" : "")}}
						{{((options & RequestOptions.Director) != 0 ? "personRoles { roles_russian: rolesRu, roles: rolesEn, person { name, russian, id } }" : "")}}
						{{((options & RequestOptions.Genres) != 0 ? GenresSubquery : "")}}
						{{((options & RequestOptions.Description) != 0 ? DescriptionSubquery : "")}}
					}
				}
				""";
	}
}