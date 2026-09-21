# 10. Test Senaryoları

Postman koleksiyonunda assertion’larla koştuğum senaryolar:

| ID | Senaryo | Beklenen |
| --- | --- | --- |
| TC-01 | Tedarikçi listesi | 200, en az 3 kayıt |
| TC-02 | Aynı kodla tedarikçi | 409 `DUPLICATE_CODE` |
| TC-03 | Olmayan ürün | 404 `NOT_FOUND` |
| TC-04 | Pasif tedarikçiye ürün | 409 `SUPPLIER_INACTIVE` |
| TC-05 | Taslak sipariş | 201, status Draft |
| TC-06 | Min adet altı sipariş | 400 |
| TC-07 | Stok üstü sipariş | 409 `STOCK_INSUFFICIENT` |
| TC-08 | Draft → submit → confirm | 200, stok azalır |
| TC-09 | Shipped siparişi iptal | 409 `INVALID_STATUS_TRANSITION` |
| TC-10 | API Key yok | 401 |
| TC-11 | Hatalı API Key | 401 |
| TC-12 | Geçerli entegrasyon siparişi | 201 Submitted + success log |
| TC-13 | Bilinmeyen müşteri kodu | 404 + failed log |
