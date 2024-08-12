namespace Stocks.DataAccess.Migration.Migrations;

using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

[ExcludeFromCodeCoverage]
[Migration(001)]
public class AddUsersTable_001 : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.UsersTableName)
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("EmailAddress").AsString()
            .WithColumn("PasswordHash").AsString();
    }

    public override void Down()
    {
        Delete.Table(TableNames.UsersTableName);
    }
}