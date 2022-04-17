// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2022 N0D4N

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tantei.Core.Models.Users;

namespace Tantei.Data.ModelConfigurations;

internal class DiscordUserConfiguration : IEntityTypeConfiguration<DiscordUser>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<DiscordUser> builder)
	{
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).IsRequired();
		builder.Property(x => x.Id).ValueGeneratedNever();
	}
}