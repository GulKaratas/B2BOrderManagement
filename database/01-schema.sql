# B2BOrderManagement şema (referans DDL)
# API açılışında EF Core EnsureCreated aynı modeli üretir.

IF DB_ID('B2BOrderManagement') IS NULL
    CREATE DATABASE B2BOrderManagement;
GO
USE B2BOrderManagement;
GO

CREATE TABLE Suppliers (
    Id            INT IDENTITY PRIMARY KEY,
    Code          NVARCHAR(20)  NOT NULL UNIQUE,
    CompanyName   NVARCHAR(200) NOT NULL,
    TaxNumber     NVARCHAR(20)  NOT NULL UNIQUE,
    Email         NVARCHAR(200) NOT NULL,
    Phone         NVARCHAR(30)  NOT NULL,
    City          NVARCHAR(80)  NOT NULL,
    Country       NVARCHAR(2)   NOT NULL,
    Status        NVARCHAR(20)  NOT NULL,
    ApiKey        NVARCHAR(80)  NOT NULL UNIQUE,
    CreatedAt     DATETIME2     NOT NULL,
    UpdatedAt     DATETIME2     NULL
);

CREATE TABLE Products (
    Id               INT IDENTITY PRIMARY KEY,
    SupplierId       INT            NOT NULL,
    Sku              NVARCHAR(40)   NOT NULL UNIQUE,
    Name             NVARCHAR(200)  NOT NULL,
    Description      NVARCHAR(1000) NULL,
    Category         NVARCHAR(80)   NOT NULL,
    UnitPrice        DECIMAL(18,2)  NOT NULL,
    Currency         NVARCHAR(3)    NOT NULL,
    StockQuantity    INT            NOT NULL,
    MinOrderQuantity INT            NOT NULL,
    Status           NVARCHAR(20)   NOT NULL,
    CreatedAt        DATETIME2      NOT NULL,
    UpdatedAt        DATETIME2      NULL,
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
);

CREATE TABLE Customers (
    Id          INT IDENTITY PRIMARY KEY,
    Code        NVARCHAR(20)  NOT NULL UNIQUE,
    CompanyName NVARCHAR(200) NOT NULL,
    Email       NVARCHAR(200) NOT NULL,
    City        NVARCHAR(80)  NOT NULL,
    Status      NVARCHAR(20)  NOT NULL,
    CreatedAt   DATETIME2     NOT NULL
);

CREATE TABLE Orders (
    Id           INT IDENTITY PRIMARY KEY,
    OrderNumber  NVARCHAR(30)  NOT NULL UNIQUE,
    CustomerId   INT           NOT NULL,
    SupplierId   INT           NOT NULL,
    Status       NVARCHAR(20)  NOT NULL,
    TotalAmount  DECIMAL(18,2) NOT NULL,
    Currency     NVARCHAR(3)   NOT NULL,
    Source       NVARCHAR(30)  NOT NULL,
    Notes        NVARCHAR(500) NULL,
    OrderedAt    DATETIME2     NOT NULL,
    SubmittedAt  DATETIME2     NULL,
    ConfirmedAt  DATETIME2     NULL,
    CancelledAt  DATETIME2     NULL,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_Orders_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id)
);

CREATE TABLE OrderItems (
    Id        INT IDENTITY PRIMARY KEY,
    OrderId   INT           NOT NULL,
    ProductId INT           NOT NULL,
    Quantity  INT           NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE IntegrationLogs (
    Id              INT IDENTITY PRIMARY KEY,
    Direction       NVARCHAR(20)   NOT NULL,
    EventType       NVARCHAR(50)   NOT NULL,
    ReferenceNumber NVARCHAR(50)   NULL,
    Payload         NVARCHAR(4000) NOT NULL,
    Status          NVARCHAR(20)   NOT NULL,
    ErrorMessage    NVARCHAR(1000) NULL,
    CreatedAt       DATETIME2      NOT NULL
);
