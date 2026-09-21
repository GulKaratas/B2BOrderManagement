# 7. API Tasarımı

Base URL: `http://localhost:5187`

## Tedarikçiler

| Method | Path | Açıklama |
| --- | --- | --- |
| GET | `/api/suppliers?status=&page=&pageSize=` | Liste |
| GET | `/api/suppliers/{id}` | Detay |
| POST | `/api/suppliers` | Oluştur |
| PUT | `/api/suppliers/{id}` | Güncelle |

## Ürünler

| Method | Path | Açıklama |
| --- | --- | --- |
| GET | `/api/products?supplierId=&status=` | Liste |
| GET | `/api/products/{id}` | Detay |
| POST | `/api/products` | Oluştur |
| PUT | `/api/products/{id}` | Güncelle |

## Müşteriler

| Method | Path | Açıklama |
| --- | --- | --- |
| GET | `/api/customers` | Liste |

## Siparişler

| Method | Path | Açıklama |
| --- | --- | --- |
| GET | `/api/orders?status=&supplierId=` | Liste |
| GET | `/api/orders/{id}` | Detay |
| POST | `/api/orders` | Taslak oluştur |
| POST | `/api/orders/{id}/submit` | Gönder |
| POST | `/api/orders/{id}/confirm` | Onayla (stok düş) |
| POST | `/api/orders/{id}/reject` | Reddet |
| POST | `/api/orders/{id}/ship` | Sevk et |
| POST | `/api/orders/{id}/cancel` | İptal et |

## Entegrasyon

| Method | Path | Header | Açıklama |
| --- | --- | --- | --- |
| POST | `/api/integrations/orders` | `X-Api-Key` | Tedarikçi ERP siparişi |
| GET | `/api/integrations/logs` | — | Log listesi |

## Örnek: entegrasyon siparişi

```json
{
  "customerCode": "CUS-METAL",
  "externalOrderNo": "ERP-10045",
  "items": [{ "sku": "NRD-SNS-014", "quantity": 10 }],
  "notes": "ERP kaynaklı acil sipariş"
}
```
