# 5. Fonksiyonel Gereksinimler

| ID | Modül | Gereksinim | Öncelik |
| --- | --- | --- | --- |
| FR-01 | Tedarikçi | Tedarikçi listelenebilir, filtrelenebilir | Must |
| FR-02 | Tedarikçi | Yeni tedarikçi oluşturulabilir | Must |
| FR-03 | Tedarikçi | Tedarikçi güncellenebilir (durum dahil) | Must |
| FR-04 | Ürün | Ürünler tedarikçiye göre listelenir | Must |
| FR-05 | Ürün | Yeni ürün yalnızca aktif tedarikçiye eklenir | Must |
| FR-06 | Ürün | Stok ve min. sipariş adedi güncellenebilir | Must |
| FR-07 | Müşteri | Aktif müşteriler listelenir | Must |
| FR-08 | Sipariş | Taslak sipariş oluşturulur | Must |
| FR-09 | Sipariş | Sipariş gönderilir / onaylanır / reddedilir / sevk edilir / iptal edilir | Must |
| FR-10 | Sipariş | Sipariş no otomatik üretilir | Must |
| FR-11 | Entegrasyon | API Key ile inbound sipariş alınır | Must |
| FR-12 | Entegrasyon | İstekler success/fail olarak loglanır | Must |
| FR-13 | Hata | Standart hata JSON'u döner | Must |
| FR-14 | Rapor | Siparişler durum ve tedarikçiye göre filtrelenir | Should |
| FR-15 | UI | Admin paneli: katalog, sipariş, log ekranları | Should |

Kabul kriterleri her FR için `docs/10-test-senaryolari.md` ve Postman koleksiyonunda karşılanır.
