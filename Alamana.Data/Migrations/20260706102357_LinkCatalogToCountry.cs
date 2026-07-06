using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkCatalogToCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // تنظيف مخطط Countries القديم إن وُجد (من migrations سابقة)
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.CategoryCountries', N'U') IS NOT NULL
    DROP TABLE dbo.CategoryCountries;

IF OBJECT_ID(N'dbo.CountryAdvertisements', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryAdvertisements_Countries_CountriesId')
        ALTER TABLE dbo.CountryAdvertisements DROP CONSTRAINT FK_CountryAdvertisements_Countries_CountriesId;
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryAdvertisements_Country_CountryId')
        ALTER TABLE dbo.CountryAdvertisements DROP CONSTRAINT FK_CountryAdvertisements_Country_CountryId;
    IF COL_LENGTH('dbo.CountryAdvertisements', 'CountriesId') IS NOT NULL
        EXEC sp_rename N'dbo.CountryAdvertisements.CountriesId', N'CountryId', N'COLUMN';
END

IF OBJECT_ID(N'dbo.CountryProducts', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryProducts_Countries_CountriesId')
        ALTER TABLE dbo.CountryProducts DROP CONSTRAINT FK_CountryProducts_Countries_CountriesId;
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryProducts_Country_CountryId')
        ALTER TABLE dbo.CountryProducts DROP CONSTRAINT FK_CountryProducts_Country_CountryId;
    IF COL_LENGTH('dbo.CountryProducts', 'CountriesId') IS NOT NULL
        EXEC sp_rename N'dbo.CountryProducts.CountriesId', N'CountryId', N'COLUMN';
END

IF OBJECT_ID(N'dbo.Countries', N'U') IS NOT NULL
    DROP TABLE dbo.Countries;
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.CountryCategories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CountryCategories (
        Id int NOT NULL IDENTITY,
        CategoryId int NOT NULL,
        CountryId int NOT NULL,
        CONSTRAINT PK_CountryCategories PRIMARY KEY (Id),
        CONSTRAINT FK_CountryCategories_Category_CategoryId FOREIGN KEY (CategoryId) REFERENCES dbo.Category (Id) ON DELETE CASCADE,
        CONSTRAINT FK_CountryCategories_Country_CountryId FOREIGN KEY (CountryId) REFERENCES dbo.Country (Id) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX IX_CountryCategories_CategoryId_CountryId ON dbo.CountryCategories (CategoryId, CountryId);
    CREATE INDEX IX_CountryCategories_CountryId ON dbo.CountryCategories (CountryId);
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.CountryProducts', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CountryProducts (
        Id int NOT NULL IDENTITY,
        ProductId int NOT NULL,
        CountryId int NOT NULL,
        Stock int NOT NULL,
        Price decimal(18,2) NOT NULL,
        Discount decimal(18,2) NOT NULL,
        IsNew bit NOT NULL,
        IsHeroProduct bit NOT NULL,
        CONSTRAINT PK_CountryProducts PRIMARY KEY (Id),
        CONSTRAINT FK_CountryProducts_Product_ProductId FOREIGN KEY (ProductId) REFERENCES dbo.Product (Id) ON DELETE CASCADE,
        CONSTRAINT FK_CountryProducts_Country_CountryId FOREIGN KEY (CountryId) REFERENCES dbo.Country (Id) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX IX_CountryProducts_ProductId_CountryId ON dbo.CountryProducts (ProductId, CountryId);
    CREATE INDEX IX_CountryProducts_CountryId ON dbo.CountryProducts (CountryId);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryProducts_Country_CountryId')
        ALTER TABLE dbo.CountryProducts ADD CONSTRAINT FK_CountryProducts_Country_CountryId
            FOREIGN KEY (CountryId) REFERENCES dbo.Country (Id) ON DELETE NO ACTION;
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryProducts_Product_ProductId')
        ALTER TABLE dbo.CountryProducts ADD CONSTRAINT FK_CountryProducts_Product_ProductId
            FOREIGN KEY (ProductId) REFERENCES dbo.Product (Id) ON DELETE CASCADE;
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.CountryAdvertisements', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CountryAdvertisements (
        Id int NOT NULL IDENTITY,
        AdvertisementId int NOT NULL,
        CountryId int NOT NULL,
        CONSTRAINT PK_CountryAdvertisements PRIMARY KEY (Id),
        CONSTRAINT FK_CountryAdvertisements_Advertisements_AdvertisementId FOREIGN KEY (AdvertisementId) REFERENCES dbo.Advertisements (Id) ON DELETE CASCADE,
        CONSTRAINT FK_CountryAdvertisements_Country_CountryId FOREIGN KEY (CountryId) REFERENCES dbo.Country (Id) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX IX_CountryAdvertisements_AdvertisementId_CountryId ON dbo.CountryAdvertisements (AdvertisementId, CountryId);
    CREATE INDEX IX_CountryAdvertisements_CountryId ON dbo.CountryAdvertisements (CountryId);
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryAdvertisements_Country_CountryId')
        ALTER TABLE dbo.CountryAdvertisements ADD CONSTRAINT FK_CountryAdvertisements_Country_CountryId
            FOREIGN KEY (CountryId) REFERENCES dbo.Country (Id) ON DELETE NO ACTION;
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CountryAdvertisements_Advertisements_AdvertisementId')
        ALTER TABLE dbo.CountryAdvertisements ADD CONSTRAINT FK_CountryAdvertisements_Advertisements_AdvertisementId
            FOREIGN KEY (AdvertisementId) REFERENCES dbo.Advertisements (Id) ON DELETE CASCADE;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CountryCategories");
            migrationBuilder.DropTable(name: "CountryAdvertisements");
            migrationBuilder.DropTable(name: "CountryProducts");
        }
    }
}
