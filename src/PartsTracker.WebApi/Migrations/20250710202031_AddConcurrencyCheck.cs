﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsTracker.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrencyCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Parts",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Parts");
        }
    }
}
