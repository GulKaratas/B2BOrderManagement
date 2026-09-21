# 9. Jira Backlog (örnek)

Proje anahtarı: `B2B`

Epic'ler:

- `B2B-E1` Katalog yönetimi
- `B2B-E2` Sipariş yaşam döngüsü
- `B2B-E3` B2B entegrasyon
- `B2B-E4` Analiz ve dokümantasyon

| Key | Tip | Özet | Epic | Story point |
| --- | --- | --- | --- | --- |
| B2B-1 | Story | Tedarikçi CRUD ve unique kod/vergi no | E1 | 5 |
| B2B-2 | Story | Ürün kataloğu ve stok alanları | E1 | 5 |
| B2B-3 | Story | Taslak sipariş oluşturma | E2 | 8 |
| B2B-4 | Story | Durum geçişleri (submit/confirm/ship/cancel) | E2 | 8 |
| B2B-5 | Story | Stok rezervasyonu onay anında | E2 | 5 |
| B2B-6 | Story | API Key ile inbound sipariş | E3 | 8 |
| B2B-7 | Story | Integration log listesi | E3 | 3 |
| B2B-8 | Task | Postman happy path + hata senaryoları | E3 | 5 |
| B2B-9 | Task | BRD, FR, iş kuralları, UML | E4 | 5 |
| B2B-10 | Task | Admin paneli ekran tasarımları | E4 | 5 |

## Örnek user story

**B2B-5**  
*Bir operasyon uzmanı olarak siparişi onayladığımda stoğun düşmesini istiyorum ki fazla satış olmasın.*

Kabul kriterleri:

- Confirm yalnızca `Submitted` siparişte çalışır
- Stok yetersizse `STOCK_INSUFFICIENT` ve 409
- Başarılı onayda `StockQuantity` azalır
- İptalde stok iade edilir
