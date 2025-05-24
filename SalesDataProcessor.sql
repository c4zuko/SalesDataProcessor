CREATE TABLE Customers (
    CustomerId VARCHAR(100) PRIMARY KEY,
    CustomerName VARCHAR(255),
    CustomerEmail VARCHAR(255),
    CustomerAddress TEXT
);

CREATE TABLE Products (
    ProductId VARCHAR(100) PRIMARY KEY,
    ProductName VARCHAR(255),
    Category VARCHAR(100)
);

CREATE TABLE Orders (
    OrderId VARCHAR(100) PRIMARY KEY,
    Region VARCHAR(100),
    DateOfSale DATETIME,
    PaymentMethod VARCHAR(100),
    CustomerId VARCHAR(100),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);

CREATE TABLE OrderItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId VARCHAR(100),
    ProductId VARCHAR(100),
    QuantitySold INT,
    UnitPrice DECIMAL(10,2),
    Discount DECIMAL(5,2),
    ShippingCost DECIMAL(10,2),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE DataRefreshLogs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    RefreshTime DATETIME,
    Success BOOLEAN,
    Message TEXT
);
