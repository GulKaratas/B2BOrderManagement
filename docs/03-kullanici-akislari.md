# 3. Kullanıcı Akışları

## 3.1 Aktörler

- **Operasyon uzmanı:** katalog ve sipariş yönetimi
- **Satın alma:** taslak sipariş oluşturur
- **Tedarikçi ERP:** API Key ile sipariş gönderir
- **Sistem:** stok ve log kayıtlarını günceller

## 3.2 Akış A — Portalden sipariş

1. Kullanıcı aktif tedarikçiyi seçer
2. Ürünleri sepete ekler (min adet ve stok kontrolü)
3. Sistem taslak sipariş üretir (`Draft`)
4. Kullanıcı siparişi gönderir (`Submitted`)
5. Operasyon stoku tekrar kontrol ederek onaylar (`Confirmed`) → stok düşer
6. Sevkiyat yapılır (`Shipped`)

Alternatif:

- 5a. Operasyon reddeder (`Rejected`)
- 4a/5b. Sipariş iptal edilir (`Cancelled`); onaylıysa stok iade edilir

## 3.3 Akış B — B2B entegrasyon siparişi

1. Tedarikçi ERP, `POST /api/integrations/orders` çağırır
2. Sistem API Key ile tedarikçiyi doğrular
3. Müşteri kodu ve SKU'lar çözümlenir
4. İş kuralları uygulanır
5. Başarılıysa `Submitted` sipariş + success log
6. Başarısızsa sipariş yazılmaz, failed log yazılır

## 3.4 Akış C — Katalog bakımı

1. Tedarikçi kaydı oluşturulur (kod ve vergi no tekil)
2. Aktif tedarikçiye ürün eklenir (SKU tekil)
3. Ürün `Discontinued` yapılırsa yeni siparişe giremez
4. Tedarikçi `Inactive` olursa yeni sipariş ve yeni ürün engellenir
