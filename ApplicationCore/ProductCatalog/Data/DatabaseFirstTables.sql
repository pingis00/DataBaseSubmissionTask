CREATE TABLE Brands (
    Id INT PRIMARY KEY,
    BrandName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY,
    CategoryName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Products (
    Id INT PRIMARY KEY,
    ArticleNumber INT NOT NULL UNIQUE,
    Title NVARCHAR(200) NOT NULL,
    BrandId INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (BrandId) REFERENCES Brands(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE Inventory (
    ProductId INT PRIMARY KEY,
    QuantityAvailable INT NOT NULL,
    Price MONEY NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE ProductReviews (
    Id INT PRIMARY KEY,
    ReviewerName NVARCHAR(100) NOT NULL,
    ReviewText NVARCHAR(1000) NOT NULL,
    ProductId INT NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
