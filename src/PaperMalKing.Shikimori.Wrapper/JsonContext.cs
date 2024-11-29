// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2021-2024 N0D4N

using System.Text.Json.Serialization;
using PaperMalKing.Shikimori.Wrapper.Abstractions.Models;

namespace PaperMalKing.Shikimori.Wrapper;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, IgnoreReadOnlyProperties = true, RespectNullableAnnotations = true,
	RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(Favourites))]
[JsonSerializable(typeof(History[]))]
[JsonSerializable(typeof(UserAchievement[]))]
internal sealed partial class JsonContext : JsonSerializerContext;
