# Admin paneli — Figma aktarımı

Kaynak dosya: `ui-mockups/index.html`

CV'deki "Figma kullanarak admin paneli ve kullanıcı akışlarının arayüz tasarımlarını oluşturdum" maddesini bu ekranlarla somutlaştırabilirsin.

## Ekranlar

1. Dashboard — KPI + sipariş akışı
2. Tedarikçiler — master data tablosu
3. Ürünler — stok / min adet / discontinued
4. Siparişler — Draft / Submitted / Confirmed
5. Entegrasyon logları — success / fail

## Figma adımları

1. `ui-mockups/index.html` dosyasını tarayıcıda aç
2. Her menü için ekran görüntüsü al
3. Figma'da 1440x900 frame'ler oluştur: `Dashboard`, `Suppliers`, `Products`, `Orders`, `Logs`
4. Görselleri yerleştir, üzerine Auto Layout ile buton/tablo component'leri çiz
5. Prototype'da soldaki menü tıklanınca ilgili frame'e geç
6. Ayrı bir flow: `Sipariş oluştur → Gönder → Onayla → Sevk et`

Renkler: arka plan `#0F172A`, kart `#1F2937`, vurgu `#38BDF8`.
