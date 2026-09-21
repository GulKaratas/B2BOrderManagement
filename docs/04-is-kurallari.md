# 4. İş Kuralları

| ID | Kural | Hata kodu |
| --- | --- | --- |
| BR-01 | Tedarikçi kodu ve vergi numarası tekildir | `DUPLICATE_CODE`, `DUPLICATE_TAX_NUMBER` |
| BR-02 | SKU tekildir | `DUPLICATE_SKU` |
| BR-03 | Yalnızca `Active` tedarikçiye ürün eklenir | `SUPPLIER_INACTIVE` |
| BR-04 | Yalnızca `Active` tedarikçiden sipariş alınır | `SUPPLIER_INACTIVE` |
| BR-05 | Yalnızca `Active` müşteri sipariş açabilir | `CUSTOMER_INACTIVE` |
| BR-06 | Sipariş kalemindeki ürün, seçilen tedarikçiye ait olmalıdır | `PRODUCT_SUPPLIER_MISMATCH` |
| BR-07 | `Inactive` / `Discontinued` ürün sipariş edilemez | `PRODUCT_INACTIVE` |
| BR-08 | Adet, ürünün `MinOrderQuantity` değerinden küçük olamaz | `VALIDATION_ERROR` |
| BR-09 | Adet, mevcut stoktan büyük olamaz | `STOCK_INSUFFICIENT` |
| BR-10 | Siparişte en az bir kalem olmalıdır | `VALIDATION_ERROR` |
| BR-11 | Sipariş numarası `ORD-yyyyMMdd-####` formatında ve tekildir | sistem |
| BR-12 | Durum geçişleri tanımlı matrise uymalıdır | `INVALID_STATUS_TRANSITION` |
| BR-13 | Stok yalnızca `Confirmed` anında düşülür | sistem |
| BR-14 | `Confirmed` sipariş iptal edilirse stok iade edilir | sistem |
| BR-15 | Entegrasyon istekleri API Key olmadan kabul edilmez | `UNAUTHORIZED` |
| BR-16 | Başarısız entegrasyon da loglanır | sistem |

## Durum geçiş matrisi

| Mevcut | İzinli sonraki |
| --- | --- |
| Draft | Submitted, Cancelled |
| Submitted | Confirmed, Rejected, Cancelled |
| Confirmed | Shipped, Cancelled |
| Rejected | — |
| Shipped | — |
| Cancelled | — |
