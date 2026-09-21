using B2BOrderManagement.Api.Domain;
using B2BOrderManagement.Api.Domain.Entities;

namespace B2BOrderManagement.Api.Data;

public static class SeedData
{
    public static async Task EnsureSeededAsync(AppDbContext db)
    {
        if (db.Suppliers.Any())
        {
            return;
        }

        var now = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc);

        var nordic = new Supplier
        {
            Code = "SUP-NORDIC",
            CompanyName = "Nordic Components A.Ş.",
            TaxNumber = "1234567890",
            Email = "entegrasyon@nordic.example",
            Phone = "+90 212 555 0101",
            City = "İstanbul",
            Country = "TR",
            Status = SupplierStatus.Active,
            ApiKey = "sup-nordic-demo-key-2026",
            CreatedAt = now
        };

        var anadolu = new Supplier
        {
            Code = "SUP-ANADOLU",
            CompanyName = "Anadolu Endüstriyel Ltd.",
            TaxNumber = "9876543210",
            Email = "siparis@anadolu.example",
            Phone = "+90 312 555 0202",
            City = "Ankara",
            Country = "TR",
            Status = SupplierStatus.Active,
            ApiKey = "sup-anadolu-demo-key-2026",
            CreatedAt = now
        };

        var ege = new Supplier
        {
            Code = "SUP-EGE",
            CompanyName = "Ege Ambalaj Sanayi",
            TaxNumber = "1122334455",
            Email = "info@egeambalaj.example",
            Phone = "+90 232 555 0303",
            City = "İzmir",
            Country = "TR",
            Status = SupplierStatus.Inactive,
            ApiKey = "sup-ege-demo-key-2026",
            CreatedAt = now
        };

        db.Suppliers.AddRange(nordic, anadolu, ege);
        await db.SaveChangesAsync();

        db.Products.AddRange(
            new Product { SupplierId = nordic.Id, Sku = "NRD-MTR-001", Name = "Endüstriyel Motor 1.5kW", Category = "Motor", UnitPrice = 12500m, StockQuantity = 40, MinOrderQuantity = 1, Status = ProductStatus.Active, CreatedAt = now },
            new Product { SupplierId = nordic.Id, Sku = "NRD-SNS-014", Name = "Sıcaklık Sensörü PT100", Category = "Sensör", UnitPrice = 850m, StockQuantity = 200, MinOrderQuantity = 5, Status = ProductStatus.Active, CreatedAt = now },
            new Product { SupplierId = nordic.Id, Sku = "NRD-CBL-220", Name = "Güç Kablosu 3x2.5mm", Category = "Kablo", UnitPrice = 45.50m, StockQuantity = 8, MinOrderQuantity = 10, Status = ProductStatus.Active, CreatedAt = now },
            new Product { SupplierId = anadolu.Id, Sku = "AND-VLV-007", Name = "Pnömatik Vana 1/2\"", Category = "Vana", UnitPrice = 320m, StockQuantity = 120, MinOrderQuantity = 2, Status = ProductStatus.Active, CreatedAt = now },
            new Product { SupplierId = anadolu.Id, Sku = "AND-FLT-003", Name = "Hidrolik Filtre", Category = "Filtre", UnitPrice = 175m, StockQuantity = 60, MinOrderQuantity = 1, Status = ProductStatus.Active, CreatedAt = now },
            new Product { SupplierId = anadolu.Id, Sku = "AND-OLD-099", Name = "Eski Model Rulman", Category = "Rulman", UnitPrice = 90m, StockQuantity = 15, MinOrderQuantity = 1, Status = ProductStatus.Discontinued, CreatedAt = now },
            new Product { SupplierId = ege.Id, Sku = "EGE-BOX-010", Name = "Karton Koli 40x40", Category = "Ambalaj", UnitPrice = 12m, StockQuantity = 500, MinOrderQuantity = 50, Status = ProductStatus.Inactive, CreatedAt = now }
        );

        db.Customers.AddRange(
            new Customer { Code = "CUS-METAL", CompanyName = "MetalTech Üretim A.Ş.", Email = "satin-alma@metaltech.example", City = "Bursa", Status = CustomerStatus.Active, CreatedAt = now },
            new Customer { Code = "CUS-AUTO", CompanyName = "AutoLine Yan Sanayi", Email = "orders@autoline.example", City = "Kocaeli", Status = CustomerStatus.Active, CreatedAt = now }
        );

        await db.SaveChangesAsync();

        var motor = db.Products.Single(x => x.Sku == "NRD-MTR-001");
        var sensor = db.Products.Single(x => x.Sku == "NRD-SNS-014");
        var valve = db.Products.Single(x => x.Sku == "AND-VLV-007");
        var metal = db.Customers.Single(x => x.Code == "CUS-METAL");
        var auto = db.Customers.Single(x => x.Code == "CUS-AUTO");

        var draft = new Order
        {
            OrderNumber = "ORD-20260901-0001",
            CustomerId = metal.Id,
            SupplierId = nordic.Id,
            Status = OrderStatus.Draft,
            Currency = "TRY",
            Source = "Portal",
            Notes = "Bakım dönemi için taslak sipariş",
            OrderedAt = now.AddDays(5),
            Items =
            {
                new OrderItem { ProductId = motor.Id, Quantity = 2, UnitPrice = motor.UnitPrice, LineTotal = 25000m }
            },
            TotalAmount = 25000m
        };

        var submitted = new Order
        {
            OrderNumber = "ORD-20260910-0002",
            CustomerId = auto.Id,
            SupplierId = nordic.Id,
            Status = OrderStatus.Submitted,
            Currency = "TRY",
            Source = "Integration",
            Notes = "ERP üzerinden gelen sipariş",
            OrderedAt = now.AddDays(9),
            SubmittedAt = now.AddDays(9),
            Items =
            {
                new OrderItem { ProductId = sensor.Id, Quantity = 20, UnitPrice = sensor.UnitPrice, LineTotal = 17000m }
            },
            TotalAmount = 17000m
        };

        var confirmed = new Order
        {
            OrderNumber = "ORD-20260912-0003",
            CustomerId = metal.Id,
            SupplierId = anadolu.Id,
            Status = OrderStatus.Confirmed,
            Currency = "TRY",
            Source = "Portal",
            OrderedAt = now.AddDays(11),
            SubmittedAt = now.AddDays(11),
            ConfirmedAt = now.AddDays(12),
            Items =
            {
                new OrderItem { ProductId = valve.Id, Quantity = 10, UnitPrice = valve.UnitPrice, LineTotal = 3200m }
            },
            TotalAmount = 3200m
        };

        db.Orders.AddRange(draft, submitted, confirmed);

        db.IntegrationLogs.AddRange(
            new IntegrationLog
            {
                Direction = "Inbound",
                EventType = "CreateOrder",
                ReferenceNumber = "ORD-20260910-0002",
                Payload = "{\"customerCode\":\"CUS-AUTO\",\"sku\":\"NRD-SNS-014\",\"quantity\":20}",
                Status = IntegrationStatus.Success,
                CreatedAt = now.AddDays(9)
            },
            new IntegrationLog
            {
                Direction = "Inbound",
                EventType = "CreateOrder",
                ReferenceNumber = "EXT-FAIL-001",
                Payload = "{\"customerCode\":\"UNKNOWN\",\"sku\":\"INVALID\"}",
                Status = IntegrationStatus.Failed,
                ErrorMessage = "Müşteri kodu bulunamadı.",
                CreatedAt = now.AddDays(10)
            }
        );

        await db.SaveChangesAsync();
    }
}
