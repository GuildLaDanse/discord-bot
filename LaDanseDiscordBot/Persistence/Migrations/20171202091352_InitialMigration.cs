using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace LaDanseDiscordBot.Persistence.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordUsers",
                columns: table => new
                {
                    DiscordUserId = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUsers", x => x.DiscordUserId);
                });

            migrationBuilder.CreateTable(
                name: "AccessTokenMappings",
                columns: table => new
                {
                    AccessTokenMappingId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<long>(nullable: false),
                    DiscordUserId = table.Column<ulong>(nullable: true),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokenMappings", x => x.AccessTokenMappingId);
                    table.ForeignKey(
                        name: "FK_AccessTokenMappings_DiscordUsers_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthSessions",
                columns: table => new
                {
                    AuthSessionId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<long>(nullable: false),
                    DiscordUserId = table.Column<ulong>(nullable: true),
                    Nonce = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthSessions", x => x.AuthSessionId);
                    table.ForeignKey(
                        name: "FK_AuthSessions_DiscordUsers_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokenMappings_DiscordUserId",
                table: "AccessTokenMappings",
                column: "DiscordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthSessions_DiscordUserId",
                table: "AuthSessions",
                column: "DiscordUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokenMappings");

            migrationBuilder.DropTable(
                name: "AuthSessions");

            migrationBuilder.DropTable(
                name: "DiscordUsers");
        }
    }
}
