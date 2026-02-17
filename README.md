# Hue3D - 3D Renk Puzzle Oyunu

"I Love Hue" oyununun 3D versiyonu. Karışık renkleri doğru sıraya dizerek gradient oluşturun!

## Oyun Mekaniği

1. **Amaç**: Karışık küpleri tıklayarak yer değiştirin ve renk uyumlu (gradient) bir düzen oluşturun
2. **Kontroller**:
   - Sol tık: Küp seç
   - İkinci sol tık: Seçili küple yer değiştir
   - Sağ tık + sürükle: Kamerayı döndür
   - Mouse tekerleği: Yakınlaştır/Uzaklaştır
3. **Sabit Küpler**: Bazı köşe küpleri sabittir ve ipucu olarak kullanılır

## Kurulum

### Hızlı Kurulum

1. Unity'de sahneyi aç (SampleScene)
2. Boş bir GameObject oluştur (Create Empty)
3. `SceneSetup` scriptini bu objeye ekle
4. Play tuşuna bas

### Manuel Kurulum

1. **GameManager** objesi oluştur ve `GameManager.cs` ekle
2. **PuzzleGenerator** objesi oluştur ve `PuzzleGenerator.cs` ekle
3. **Main Camera**'ya `CameraController.cs` ekle
4. (Opsiyonel) **UIManager** objesi oluştur ve `UIManager.cs` ekle
5. (Opsiyonel) **AudioManager** objesi oluştur ve `AudioManager.cs` ekle

## Script Açıklamaları

### Cube.cs
Tek bir 3D küpü temsil eder.
- Renk yönetimi
- Tıklama algılama
- Seçim animasyonları
- Hover efektleri

### PuzzleGenerator.cs
Puzzle şeklini ve renk gradientlerini oluşturur.
- Organik 3D şekil üretimi
- Gradient renk hesaplama
- Renk paletleri desteği
- Renk karıştırma

### GameManager.cs
Ana oyun mantığı.
- Küp seçimi ve değiştirme
- Kazanma kontrolü
- Level yönetimi
- Skor takibi

### CameraController.cs
İzometrik kamera kontrolü.
- Orbit döndürme (sağ tık)
- Zoom (tekerlek)
- Touch desteği (mobil)
- Puzzle'a otomatik odaklanma

### ColorPalette.cs
Renk paleti sistemi.
- 6 hazır palet: Ocean, Sunset, Forest, Aurora, Cosmic, Monochrome
- HSV gradient desteği
- Özel palet oluşturma (ScriptableObject)

### UIManager.cs
UI yönetimi ve animasyonları.
- HUD (level, hamle sayısı, ilerleme)
- Kazanma paneli
- Panel animasyonları

### AudioManager.cs
Ses efektleri.
- Prosedürel ses oluşturma
- Ses ayarları kaydetme
- SFX ve müzik kontrolü

## Level Sistemi

Level zorluk seviyesi arttıkça:
- Daha fazla küp eklenir
- Daha karmaşık şekiller oluşur
- Renk geçişleri daha ince olur

## Özelleştirme

### Yeni Renk Paleti Ekleme

```csharp
// ColorPalette.cs'e yeni palet ekle
public static readonly Color[] MyPalette = new Color[]
{
    new Color(1f, 0f, 0f),   // Kırmızı
    new Color(1f, 0.5f, 0f), // Turuncu
    new Color(1f, 1f, 0f),   // Sarı
};
```

### Küp Boyutunu Değiştirme

GameManager'da `cubeSpacing` değerini değiştirin (varsayılan: 1.1)

### Sabit Küp Oranını Değiştirme

PuzzleGenerator -> IsCornerPosition metodunda `0.3f` değerini ayarlayın

## Teknik Notlar

- URP (Universal Render Pipeline) ile uyumlu
- Mobil touch kontrolü destekler
- PlayerPrefs ile ayarlar kaydedilir
- Prosedürel ses efektleri (harici dosya gerektirmez)

## Gereksinimler

- Unity 2022.3 LTS veya üstü
- Universal Render Pipeline (URP)

## Lisans

Bu proje eğitim amaçlıdır.
