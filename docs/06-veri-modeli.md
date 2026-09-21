# 6. Veri Modeli

## Varlıklar

- **Suppliers:** B2B tedarikçi master data + entegrasyon API Key
- **Products:** Tedarikçiye bağlı katalog ve stok
- **Customers:** Sipariş veren B2B müşteri
- **Orders / OrderItems:** Sipariş başlık ve kalem
- **IntegrationLogs:** Inbound/outbound olay kaydı

## İlişkiler

- Supplier 1—N Product
- Supplier 1—N Order
- Customer 1—N Order
- Order 1—N OrderItem
- Product 1—N OrderItem

## Kısıtlar

- Unique: `Suppliers.Code`, `Suppliers.TaxNumber`, `Suppliers.ApiKey`
- Unique: `Products.Sku`, `Orders.OrderNumber`, `Customers.Code`
- FK Restrict: ürün ve sipariş silinince tedarikçi silinmez
- Enum alanlar string olarak saklanır (`Active`, `Draft`, ...)

Detaylı DDL: `database/01-schema.sql`
