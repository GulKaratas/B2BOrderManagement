USE B2BOrderManagement;
GO

-- Örnek sorgular (iş analizi / demo)

-- Aktif tedarikçiler ve ürün sayısı
SELECT s.Code, s.CompanyName, s.Status, COUNT(p.Id) AS ProductCount
FROM Suppliers s
LEFT JOIN Products p ON p.SupplierId = s.Id
GROUP BY s.Code, s.CompanyName, s.Status;

-- Stok kritik ürünler
SELECT Sku, Name, StockQuantity, MinOrderQuantity
FROM Products
WHERE StockQuantity < 20 AND Status = 'Active';

-- Duruma göre sipariş özeti
SELECT Status, COUNT(*) AS OrderCount, SUM(TotalAmount) AS TotalAmount
FROM Orders
GROUP BY Status;

-- Entegrasyon başarı oranı
SELECT Status, COUNT(*) AS Cnt
FROM IntegrationLogs
GROUP BY Status;
