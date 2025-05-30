﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameValueToAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Transactions",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "Value");
        }
    }
}
