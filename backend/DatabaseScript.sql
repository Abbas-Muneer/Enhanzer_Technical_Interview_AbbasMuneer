IF DB_ID('EnhanzerPurchaseBillDb') IS NULL
BEGIN
    CREATE DATABASE EnhanzerPurchaseBillDb;
END;
GO

USE EnhanzerPurchaseBillDb;
GO

IF OBJECT_ID('dbo.Locations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Locations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Code NVARCHAR(100) NOT NULL UNIQUE,
        Name NVARCHAR(200) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NULL
    );
END;
GO

IF OBJECT_ID('dbo.Items', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Items (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL UNIQUE,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF OBJECT_ID('dbo.PurchaseBillHeaders', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseBillHeaders (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        BillNumber NVARCHAR(50) NOT NULL UNIQUE,
        PurchaseDate DATETIME2 NOT NULL,
        SupplierName NVARCHAR(150) NOT NULL,
        ReferenceNo NVARCHAR(100) NOT NULL DEFAULT '',
        Notes NVARCHAR(1000) NOT NULL DEFAULT '',
        SyncStatus NVARCHAR(20) NOT NULL,
        OfflineClientId NVARCHAR(100) NULL UNIQUE,
        TotalItems INT NOT NULL,
        TotalQuantity DECIMAL(18,2) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        TotalCost DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF OBJECT_ID('dbo.PurchaseBillLines', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PurchaseBillLines (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PurchaseBillHeaderId INT NOT NULL,
        ItemName NVARCHAR(100) NOT NULL,
        BatchCode NVARCHAR(100) NOT NULL,
        Cost DECIMAL(18,2) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Quantity DECIMAL(18,2) NOT NULL,
        DiscountPercentage DECIMAL(18,2) NOT NULL,
        TotalCost DECIMAL(18,2) NOT NULL,
        TotalSelling DECIMAL(18,2) NOT NULL,
        LineOrder INT NOT NULL,
        CONSTRAINT FK_PurchaseBillLines_PurchaseBillHeaders
            FOREIGN KEY (PurchaseBillHeaderId) REFERENCES dbo.PurchaseBillHeaders(Id)
            ON DELETE CASCADE
    );
END;
GO

IF OBJECT_ID('dbo.Audit_Logs', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Audit_Logs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Entity NVARCHAR(100) NOT NULL,
        Action NVARCHAR(50) NOT NULL,
        OldValue NVARCHAR(MAX) NULL,
        NewValue NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;
GO

MERGE dbo.Locations AS target
USING (VALUES
    ('LOC001', 'Warehouse A'),
    ('LOC002', 'Warehouse B'),
    ('LOC003', 'Main Store')
) AS source (Code, Name)
ON target.Code = source.Code
WHEN MATCHED THEN
    UPDATE SET target.Name = source.Name, target.UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (Code, Name) VALUES (source.Code, source.Name);
GO

MERGE dbo.Items AS target
USING (VALUES
    ('Mango'),
    ('Apple'),
    ('Banana'),
    ('Orange'),
    ('Grapes'),
    ('Kiwi'),
    ('Strawberry')
) AS source (Name)
ON target.Name = source.Name
WHEN NOT MATCHED THEN
    INSERT (Name) VALUES (source.Name);
GO
