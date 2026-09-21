# 2. Teknik Gereksinimler

## 2.1 Mimari

Monolitik REST API. İş kuralları servis katmanında, veri erişimi EF Core ile SQL Server üzerindedir.

```
Postman / Admin UI / Tedarikçi ERP
                |
 trest        REST/JSON
                |
        ASP.NET Core Web API
                |
          EF Core + SQL Server
```

## 2.2 Teknoloji kararları

| İhtiyaç | Seçim | Gerekçe |
| --- | --- | --- |
| API | ASP.NET Core Web API | Kurumsal .NET yığını, kolay OpenAPI |
| Veri | SQL Server | İlişkisel sipariş modeli, unique constraint |
| ORM | EF Core | Code-first + seed |
| Test | Postman | İstek / yanıt / hata senaryoları |
| Sözleşme | REST + JSON | Partner entegrasyonu için standart |

## 2.3 Niteliksel gereksinimler (NFR)

| ID | Gereksinim | Ölçüt |
| --- | --- | --- |
| NFR-01 | Kullanılabilirlik | Liste endpointleri 2 sn altında |
| NFR-02 | Tutarlılık | Stok düşümü sipariş onayı ile aynı transaction |
| NFR-03 | İzlenebilirlik | Her inbound istek IntegrationLogs'a yazılır |
| NFR-04 | Güvenlik | Entegrasyon `X-Api-Key` olmadan 401 |
| NFR-05 | Doğrulama | Geçersiz body 400 Problem benzeri JSON |
| NFR-06 | Açıklık | OpenAPI dokümanı yayınlanır |

## 2.4 Hata sözleşmesi

Tüm iş kuralı hataları şu JSON ile döner:

```json
{
  "code": "STOCK_INSUFFICIENT",
  "message": "Ürün stok miktarı yetersiz.",
  "details": { "sku": "NRD-CBL-220", "requested": 50, "available": 8 }
}
```

| HTTP | Kullanım |
| --- | --- |
| 400 | Validasyon (eksik alan, min adet) |
| 401 | API Key yok / hatalı |
| 404 | Kayıt bulunamadı |
| 409 | İş kuralı çatışması (stok, durum geçişi, duplicate) |
| 422 | İş kuralı ihlali (genel) |
| 500 | Beklenmeyen hata |

## 2.5 Güvenlik (v1)

- Entegrasyon endpointi: `X-Api-Key` header, tedarikçi kaydındaki anahtar ile eşleşir
- İç CRUD endpointleri demo amaçlı açıktır
- CORS development için açıktır
