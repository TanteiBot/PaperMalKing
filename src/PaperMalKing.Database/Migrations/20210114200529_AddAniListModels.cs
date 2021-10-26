﻿#region LICENSE
// PaperMalKing.
// Copyright (C) 2021 N0D4N
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using Microsoft.EntityFrameworkCore.Migrations;

namespace PaperMalKing.Database.Migrations
{
	public partial class AddAniListModels : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "AniListUsers",
				columns: table => new
				{
					Id = table.Column<long>(type: "INTEGER", nullable: false),
					LastActivityTimestamp = table.Column<long>(type: "INTEGER", nullable: false),
					LastReviewTimestamp = table.Column<long>(type: "INTEGER", nullable: false),
					DiscordUserId = table.Column<long>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AniListUsers", x => x.Id);
					table.ForeignKey(
						name: "FK_AniListUsers_DiscordUsers_DiscordUserId",
						column: x => x.DiscordUserId,
						principalTable: "DiscordUsers",
						principalColumn: "DiscordUserId",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "AniListFavourites",
				columns: table => new
				{
					Id = table.Column<long>(type: "INTEGER", nullable: false),
					FavouriteType = table.Column<byte>(type: "INTEGER", nullable: false),
					UserId = table.Column<long>(type: "INTEGER", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_AniListFavourites", x => new { x.Id, x.FavouriteType, x.UserId });
					table.ForeignKey(
						name: "FK_AniListFavourites_AniListUsers_UserId",
						column: x => x.UserId,
						principalTable: "AniListUsers",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_AniListFavourites_UserId",
				table: "AniListFavourites",
				column: "UserId");

			migrationBuilder.CreateIndex(
				name: "IX_AniListUsers_DiscordUserId",
				table: "AniListUsers",
				column: "DiscordUserId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "AniListFavourites");

			migrationBuilder.DropTable(
				name: "AniListUsers");
		}
	}
}
