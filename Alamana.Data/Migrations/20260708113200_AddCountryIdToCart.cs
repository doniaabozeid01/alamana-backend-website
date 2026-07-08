using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryIdToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server بيكompile الـ batch كله قبل التنفيذ،
            // فاستخدام العمود الجديد مباشرة بعد ADD بيفشل بـ Invalid column name.
            // الحل: الـ DDL/DML بعد الإضافة يتنفّذ بـ dynamic SQL.
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Cart', 'CountryId') IS NULL
BEGIN
    ;WITH ranked AS (
        SELECT Id, ROW_NUMBER() OVER (PARTITION BY userId ORDER BY Id) AS rn
        FROM Cart
    )
    DELETE FROM CartItems WHERE cartId IN (SELECT Id FROM ranked WHERE rn > 1);

    ;WITH ranked AS (
        SELECT Id, ROW_NUMBER() OVER (PARTITION BY userId ORDER BY Id) AS rn
        FROM Cart
    )
    DELETE FROM Cart WHERE Id IN (SELECT Id FROM ranked WHERE rn > 1);

    DECLARE @defaultCountryId INT = NULL;

    IF COL_LENGTH('dbo.Country', 'IsDefault') IS NOT NULL
        SET @defaultCountryId = (SELECT TOP 1 Id FROM Country WHERE IsDefault = 1 ORDER BY Id);

    IF @defaultCountryId IS NULL
        SET @defaultCountryId = (SELECT TOP 1 Id FROM Country ORDER BY Id);

    IF @defaultCountryId IS NULL
        THROW 50001, 'No Country rows exist. Insert a country before adding Cart.CountryId.', 1;

    ALTER TABLE Cart ADD CountryId INT NULL;

    EXEC sp_executesql N'
        UPDATE Cart SET CountryId = @id;
        ALTER TABLE Cart ALTER COLUMN CountryId INT NOT NULL;
        CREATE INDEX IX_Cart_CountryId ON Cart (CountryId);
        CREATE UNIQUE INDEX IX_Cart_userId_CountryId ON Cart (userId, CountryId);
        ALTER TABLE Cart WITH CHECK ADD CONSTRAINT FK_Cart_Country_CountryId
            FOREIGN KEY (CountryId) REFERENCES Country (Id);
        ALTER TABLE Cart CHECK CONSTRAINT FK_Cart_Country_CountryId;
    ', N'@id INT', @id = @defaultCountryId;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Cart', 'CountryId') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Cart_Country_CountryId')
        ALTER TABLE Cart DROP CONSTRAINT FK_Cart_Country_CountryId;

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Cart_userId_CountryId' AND object_id = OBJECT_ID('dbo.Cart'))
        DROP INDEX IX_Cart_userId_CountryId ON Cart;

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Cart_CountryId' AND object_id = OBJECT_ID('dbo.Cart'))
        DROP INDEX IX_Cart_CountryId ON Cart;

    ALTER TABLE Cart DROP COLUMN CountryId;
END
");
        }
    }
}
