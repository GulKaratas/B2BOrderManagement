# B2B Integration & Order Management System

B2B tedarikçi, ürün ve sipariş süreçlerini destekleyen REST tabanlı entegrasyon sistemi.

Bu proje bir **iş analizi + REST API** çalışmasıdır: gereksinimler, iş kuralları, kullanıcı akışları, ilişkisel veri modeli, UML diyagramları, Postman test koleksiyonu ve çalışan ASP.NET Core Web API birlikte teslim edilir.

## Ne içerir?

| Katman | Çıktı |
| --- | --- |
| İş analizi | `docs/` altında BRD, FR, iş kuralları, kullanıcı akışları, Jira backlog |
| Veri modeli | SQL Server şeması + EF Core modeli + seed data |
| REST API | Tedarikçi, ürün, müşteri, sipariş ve entegrasyon endpointleri |
| Test | Postman koleksiyonu (başarılı + hata senaryoları) |
| UI | `ui-mockups/` admin paneli ekran tasarımları (Figma'ya aktarılabilir) |
| UML | Sipariş durumu, entegrasyon ve ER diyagramları |

## Teknolojiler

- ASP.NET Core Web API (.NET 10)
- SQL Server LocalDB / SQL Server
- Entity Framework Core
- REST + OpenAPI
- Postman
- Mermaid (UML / iş akışı)

## Çalıştırma

```powershell
cd src/B2BOrderManagement.Api
dotnet run
```

API varsayılan adres: `http://localhost:5187`

İlk açılışta LocalDB üzerinde `B2BOrderManagement` veritabanı oluşturulur ve örnek veriler yüklenir.

OpenAPI: `http://localhost:5187/openapi/v1.json`

## Postman

1. `postman/B2B-Order-Management.postman_collection.json` dosyasını import et
2. `postman/B2B-Local.postman_environment.json` ortamını import et
3. Collection'ı **Run** ile uçtan uca çalıştır

Demo entegrasyon anahtarı: `sup-nordic-demo-key-2026`

## Örnek iş kuralları

- Pasif tedarikçiden sipariş alınamaz
- Ürün stok ve minimum sipariş adedi kontrol edilir
- Sipariş durumu: Draft → Submitted → Confirmed → Shipped
- B2B partner siparişi `X-Api-Key` ile `/api/integrations/orders` üzerinden gelir
- Hatalı entegrasyonlar `IntegrationLogs` tablosuna yazılır

## Klasör yapısı

```
docs/          İş analizi ve UML
database/      SQL şema ve seed scriptleri
postman/       API test koleksiyonu
ui-mockups/    Admin paneli ekranları
src/           ASP.NET Core Web API
```
