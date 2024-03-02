using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Yaginx.DomainModels.MonitorModels;

#nullable disable

namespace Yaginx.DataStore.PostgreSQLStore.Migrations
{
    /// <inheritdoc />
    public partial class AddTraffic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "resource_monitor_info",
                columns: table => new
                {
                    resource_uuid = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    session_key = table.Column<string>(type: "text", nullable: true),
                    data = table.Column<List<MonitorInfo>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resource_monitor_info", x => new { x.resource_uuid, x.timestamp });
                });

            migrationBuilder.CreateTable(
                name: "resource_report",
                columns: table => new
                {
                    resource_uuid = table.Column<string>(type: "text", nullable: false),
                    cycle_type = table.Column<int>(type: "integer", nullable: false),
                    report_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    request_qty = table.Column<long>(type: "bigint", nullable: false),
                    status_code = table.Column<Dictionary<string, long>>(type: "jsonb", nullable: true),
                    spider = table.Column<Dictionary<string, long>>(type: "jsonb", nullable: true),
                    browser = table.Column<Dictionary<string, long>>(type: "jsonb", nullable: true),
                    os = table.Column<Dictionary<string, long>>(type: "jsonb", nullable: true),
                    duration = table.Column<Dictionary<string, long>>(type: "jsonb", nullable: true),
                    create_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resource_report", x => new { x.resource_uuid, x.cycle_type, x.report_time });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resource_monitor_info");

            migrationBuilder.DropTable(
                name: "resource_report");
        }
    }
}
