using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Yaginx.DomainModels;

#nullable disable

namespace Yaginx.DataStore.PostgreSQLStore.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "host_traffic",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    host_name = table.Column<string>(type: "text", nullable: true),
                    period = table.Column<long>(type: "bigint", nullable: false),
                    request_counts = table.Column<long>(type: "bigint", nullable: false),
                    inbound_bytes = table.Column<long>(type: "bigint", nullable: false),
                    outbound_bytes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_host_traffic", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: true),
                    password = table.Column<string>(type: "text", nullable: true),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    password_salt = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "web_page",
                columns: table => new
                {
                    page_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_web_page", x => x.page_id);
                });

            migrationBuilder.CreateTable(
                name: "website",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    specifications = table.Column<WebsiteSpecifications>(type: "jsonb", nullable: true),
                    hosts = table.Column<string[]>(type: "jsonb", nullable: true),
                    proxy_rules = table.Column<List<WebsiteProxyRuleItem>>(type: "jsonb", nullable: true),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    proxy_transforms = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: true),
                    simple_responses = table.Column<SimpleResponseItem[]>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_website", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "host_traffic");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "web_page");

            migrationBuilder.DropTable(
                name: "website");
        }
    }
}
