# 1. İş Gereksinimleri (BRD)

## 1.1 Proje özeti

**B2B Integration & Order Management System**, üretici / distribütör şirketin tedarikçilerle ürün kataloğu ve sipariş süreçlerini dijital olarak yönetmesini sağlar. Müşteri siparişleri hem iç portal üzerinden hem de tedarikçi ERP sistemlerinden REST entegrasyonu ile alınır.

## 1.2 İş problemi

Bugün siparişler e-posta ve Excel ile ilerliyor. Sonuç:

- Stok bilgisi güncel değil, fazla satış (over-order) yaşanıyor
- Tedarikçi onayı geç geliyor
- Hatalı SKU / adet ile sipariş oluşuyor
- Entegrasyon hataları izlenemiyor

## 1.3 Hedefler

1. Tedarikçi, ürün ve müşteri master datasını tek yerde tutmak
2. Sipariş yaşam döngüsünü standart duruma bağlamak
3. Dış sistemlerden güvenli REST sipariş alımı sağlamak
4. Tüm entegrasyon isteklerini loglamak
5. Admin kullanıcısının siparişi onay / red / iptal edebilmesi

## 1.4 Kapsam

**Dahil:**

- Tedarikçi ve ürün kataloğu
- Sipariş oluşturma, gönderme, onay, red, sevkiyat, iptal
- Stok ve minimum sipariş adedi kuralları
- B2B inbound sipariş entegrasyonu (API Key)
- Entegrasyon logları
- Admin paneli ekran ihtiyaçları

**Hariç (v1):**

- Ödeme / faturalama
- Kargo takip numarası entegrasyonu
- Çoklu para birimi kur servisi
- Gerçek zamanlı stok push (webhook) — v2'ye bırakıldı

## 1.5 Paydaşlar

| Paydaş | Rol | Beklenti |
| --- | --- | --- |
| Satın alma | İç kullanıcı | Katalog ve sipariş oluşturma |
| Tedarikçi | Dış sistem | Kendi ürünlerine sipariş almak |
| Operasyon | Admin | Onay, red, sevkiyat |
| IT / Entegrasyon | Teknik | API, log, hata izleme |
| Yönetim | Sponsor | Süreç şeffaflığı |

## 1.6 Başarı kriterleri

- Sipariş, stok yetersizse oluşmaz
- Entegrasyon hatalarının %100'ü loglanır
- Sipariş numarası tekildir (`ORD-YYYYMMDD-####`)
- Durum geçişleri tanımlı matrise aykırı olamaz

## 1.7 Varsayımlar ve kısıtlar

- Her ürün tek tedarikçiye aittir
- Para birimi v1'de TRY'dir
- Kimlik doğrulama v1'de entegrasyon için API Key, iç API için açık (demo)
- SQL Server LocalDB / SQL Server kullanılır
