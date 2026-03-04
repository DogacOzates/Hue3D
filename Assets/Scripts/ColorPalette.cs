using UnityEngine;

/// <summary>
/// Önceden tanımlanmış renk paletleri - ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "ColorPalette", menuName = "Hue3D/Color Palette")]
public class ColorPalette : ScriptableObject
{
    public string paletteName = "New Palette";
    public Color primaryColor = Color.cyan;
    public Color secondaryColor = Color.blue;
    public Color accentColor = Color.magenta;
    public Color backgroundColor = Color.white;
    
    /// <summary>
    /// İki renk arasında gradient hesaplar
    /// </summary>
    public Color GetGradientColor(float t)
    {
        return Color.Lerp(primaryColor, secondaryColor, t);
    }
    
    /// <summary>
    /// Üç renk gradient'i
    /// </summary>
    public Color GetTriColorGradient(float t)
    {
        if (t < 0.5f)
        {
            return Color.Lerp(primaryColor, secondaryColor, t * 2f);
        }
        else
        {
            return Color.Lerp(secondaryColor, accentColor, (t - 0.5f) * 2f);
        }
    }
}

/// <summary>
/// I Love Hue tarzı renk paletleri - 4 köşe renk + bilinear interpolation
/// Her palette 4 köşe renginden oluşur: [topLeft, topRight, bottomLeft, bottomRight]
/// Grid üzerinde x,y pozisyonuna göre 2D gradient oluşturulur.
/// </summary>
public static class ColorPalettes
{
    // ============ I LOVE HUE - 4 KÖŞE RENK PALETLERİ ============
    // Her palette: [topLeft, topRight, bottomLeft, bottomRight]
    // Bilinear interpolation ile 2D gradient oluşturulur
    // Yüksek kontrast köşeler = net renk geçişi
    
    // ============ HER PALETTE: 4 KÖŞE FARKLI HUE QUADRANT'DA (270°+ YAYILIM) ============
    // Bu sayede 50+ küplük gridlerde bile her küp ayırt edilebilir renk alır.
    // HSV unwrapped bilinear interpolation ile kullanılmak üzere tasarlandı.
    
    // 0: Sunset - Kırmızı(5°) → Sarı(55°) → Teal(175°) → Mor(280°) = 275° yayılım
    public static readonly Color[] SunsetWarmth = new Color[]
    {
        new Color(0.97f, 0.25f, 0.20f),  // topLeft:     Vivid red
        new Color(0.97f, 0.85f, 0.12f),  // topRight:    Bright yellow
        new Color(0.12f, 0.85f, 0.75f),  // bottomLeft:  Teal
        new Color(0.60f, 0.18f, 0.90f),  // bottomRight: Purple
    };
    
    // 1: Ocean - Cyan(180°) → Lacivert(230°) → Lime(85°) → Mercan(10°) = 270° yayılım
    public static readonly Color[] OceanDepths = new Color[]
    {
        new Color(0.10f, 0.92f, 0.88f),  // topLeft:     Cyan
        new Color(0.20f, 0.32f, 0.92f),  // topRight:    Navy blue
        new Color(0.55f, 0.92f, 0.15f),  // bottomLeft:  Lime green
        new Color(0.95f, 0.42f, 0.30f),  // bottomRight: Coral
    };
    
    // 2: Spring - Yeşil(100°) → Altın(50°) → Magenta(310°) → GökMavi(195°) = 260° yayılım
    public static readonly Color[] SpringMeadow = new Color[]
    {
        new Color(0.40f, 0.92f, 0.18f),  // topLeft:     Lime green
        new Color(0.95f, 0.80f, 0.12f),  // topRight:    Gold
        new Color(0.88f, 0.18f, 0.75f),  // bottomLeft:  Magenta
        new Color(0.12f, 0.72f, 0.92f),  // bottomRight: Sky blue
    };
    
    // 3: Berry - Pembe(335°) → İndigo(255°) → Turuncu(28°) → Teal(165°) = 307° yayılım
    public static readonly Color[] BerryGarden = new Color[]
    {
        new Color(0.95f, 0.22f, 0.55f),  // topLeft:     Hot pink
        new Color(0.38f, 0.18f, 0.92f),  // topRight:    Indigo
        new Color(0.95f, 0.55f, 0.12f),  // bottomLeft:  Orange
        new Color(0.12f, 0.88f, 0.68f),  // bottomRight: Teal
    };
    
    // 4: Desert - Altın(48°) → Kırmızı(0°) → Teal(170°) → Violet(275°) = 275° yayılım
    public static readonly Color[] DesertDusk = new Color[]
    {
        new Color(0.95f, 0.78f, 0.12f),  // topLeft:     Gold
        new Color(0.92f, 0.20f, 0.18f),  // topRight:    Crimson red
        new Color(0.15f, 0.85f, 0.70f),  // bottomLeft:  Teal
        new Color(0.58f, 0.20f, 0.88f),  // bottomRight: Violet
    };
    
    // 5: Aurora - Yeşil(135°) → Mavi(225°) → Gül(345°) → Limon(60°) = 285° yayılım
    public static readonly Color[] NorthernLights = new Color[]
    {
        new Color(0.15f, 0.92f, 0.38f),  // topLeft:     Vivid green
        new Color(0.22f, 0.38f, 0.95f),  // topRight:    Royal blue
        new Color(0.95f, 0.28f, 0.48f),  // bottomLeft:  Rose
        new Color(0.95f, 0.92f, 0.18f),  // bottomRight: Lemon yellow
    };
    
    // 6: Autumn - Kırmızı(5°) → Amber(42°) → YeşilOrman(140°) → KoyuMor(285°) = 280° yayılım
    public static readonly Color[] AutumnForest = new Color[]
    {
        new Color(0.92f, 0.18f, 0.12f),  // topLeft:     Fire red
        new Color(0.95f, 0.68f, 0.10f),  // topRight:    Amber
        new Color(0.12f, 0.78f, 0.28f),  // bottomLeft:  Forest green
        new Color(0.65f, 0.15f, 0.88f),  // bottomRight: Deep purple
    };
    
    // 7: Tropical - Limon(58°) → Karpuz(350°) → Turkuaz(175°) → Violet(270°) = 292° yayılım
    public static readonly Color[] TropicalSunrise = new Color[]
    {
        new Color(0.98f, 0.92f, 0.15f),  // topLeft:     Lemon
        new Color(0.95f, 0.25f, 0.42f),  // topRight:    Watermelon
        new Color(0.10f, 0.88f, 0.82f),  // bottomLeft:  Turquoise
        new Color(0.55f, 0.18f, 0.90f),  // bottomRight: Violet
    };
    
    // 8: Lavender - Mor(278°) → Mercan(8°) → Yeşil(140°) → Azure(210°) = 270° yayılım
    public static readonly Color[] LavenderFields = new Color[]
    {
        new Color(0.68f, 0.25f, 0.92f),  // topLeft:     Rich purple
        new Color(0.95f, 0.40f, 0.32f),  // topRight:    Coral
        new Color(0.22f, 0.88f, 0.45f),  // bottomLeft:  Spring green
        new Color(0.15f, 0.52f, 0.92f),  // bottomRight: Azure blue
    };
    
    // 9: Midnight - KoyuMavi(235°) → Magenta(305°) → Amber(42°) → Teal(170°) = 263° yayılım
    public static readonly Color[] MidnightCity = new Color[]
    {
        new Color(0.18f, 0.22f, 0.88f),  // topLeft:     Deep blue
        new Color(0.82f, 0.15f, 0.68f),  // topRight:    Deep magenta
        new Color(0.95f, 0.72f, 0.12f),  // bottomLeft:  Amber gold
        new Color(0.12f, 0.85f, 0.72f),  // bottomRight: Teal
    };
    
    // 10: Coral - Mercan(8°) → Aqua(178°) → Violet(272°) → Yeşilsarı(82°) = 264° yayılım
    public static readonly Color[] CoralReef = new Color[]
    {
        new Color(0.95f, 0.38f, 0.28f),  // topLeft:     Bright coral
        new Color(0.10f, 0.90f, 0.85f),  // topRight:    Aqua
        new Color(0.58f, 0.20f, 0.92f),  // bottomLeft:  Violet
        new Color(0.60f, 0.92f, 0.18f),  // bottomRight: Chartreuse
    };
    
    // 11: Golden - Altın(50°) → Kiraz(355°) → Safir(220°) → Zümrüt(145°) = 305° yayılım
    public static readonly Color[] GoldenHour = new Color[]
    {
        new Color(0.98f, 0.85f, 0.15f),  // topLeft:     Rich gold
        new Color(0.90f, 0.18f, 0.22f),  // topRight:    Cherry red
        new Color(0.18f, 0.38f, 0.92f),  // bottomLeft:  Sapphire blue
        new Color(0.18f, 0.88f, 0.38f),  // bottomRight: Emerald green
    };
    
    // 12: Arctic - BuzMavi(195°) → Lavanta(275°) → Altın(50°) → Nane(150°) = 225° yayılım
    public static readonly Color[] ArcticIce = new Color[]
    {
        new Color(0.28f, 0.78f, 0.95f),  // topLeft:     Ice blue
        new Color(0.58f, 0.25f, 0.88f),  // topRight:    Lavender purple
        new Color(0.95f, 0.82f, 0.22f),  // bottomLeft:  Pale gold
        new Color(0.22f, 0.88f, 0.42f),  // bottomRight: Mint green
    };
    
    // 13: Volcanic - Lava(2°) → AlevTuruncu(28°) → DerinTeal(180°) → ObsidyanMor(260°) = 258° yayılım
    public static readonly Color[] VolcanicAsh = new Color[]
    {
        new Color(0.95f, 0.15f, 0.10f),  // topLeft:     Lava red
        new Color(0.95f, 0.55f, 0.08f),  // topRight:    Flame orange
        new Color(0.08f, 0.85f, 0.85f),  // bottomLeft:  Deep teal
        new Color(0.35f, 0.18f, 0.85f),  // bottomRight: Obsidian purple
    };
    
    // 14: Cherry - Pembe(335°) → Altın(52°) → Mavi(225°) → BaharYeşili(145°) = 283° yayılım
    public static readonly Color[] CherryBlossom = new Color[]
    {
        new Color(0.95f, 0.45f, 0.62f),  // topLeft:     Sakura pink
        new Color(0.95f, 0.85f, 0.22f),  // topRight:    Pale gold
        new Color(0.22f, 0.35f, 0.92f),  // bottomLeft:  Royal blue
        new Color(0.25f, 0.88f, 0.42f),  // bottomRight: Spring green
    };
    
    // 15: Emerald - Zümrüt(145°) → Safir(225°) → Altın(50°) → Magenta(315°) = 265° yayılım
    public static readonly Color[] EmeraldCave = new Color[]
    {
        new Color(0.10f, 0.85f, 0.38f),  // topLeft:     Deep emerald
        new Color(0.18f, 0.38f, 0.92f),  // topRight:    Sapphire
        new Color(0.92f, 0.85f, 0.15f),  // bottomLeft:  Chartreuse gold
        new Color(0.85f, 0.18f, 0.72f),  // bottomRight: Magenta
    };
    
    // Tüm paletler (her biri 4 köşe rengi)
    public static readonly Color[][] AllPalettes = new Color[][]
    {
        SunsetWarmth, OceanDepths, SpringMeadow, BerryGarden, DesertDusk, NorthernLights,
        AutumnForest, TropicalSunrise, LavenderFields, MidnightCity, CoralReef,
        GoldenHour, ArcticIce, VolcanicAsh, CherryBlossom, EmeraldCave
    };
    
    public static readonly string[] PaletteNames = new string[]
    {
        "SunsetWarmth", "OceanDepths", "SpringMeadow", "BerryGarden", "DesertDusk", "NorthernLights",
        "AutumnForest", "TropicalSunrise", "LavenderFields", "MidnightCity", "CoralReef",
        "GoldenHour", "ArcticIce", "VolcanicAsh", "CherryBlossom", "EmeraldCave"
    };
    
    // ============ ARKA PLAN RENKLERİ (Her palette için) - EYKA tarzı çok soft ============
    
    public static readonly Color[] BackgroundColors = new Color[]
    {
        new Color(0.97f, 0.93f, 0.91f),  // 0 Sunset - warm peach
        new Color(0.91f, 0.95f, 0.97f),  // 1 Ocean - cool mist
        new Color(0.93f, 0.97f, 0.91f),  // 2 Spring - fresh green
        new Color(0.97f, 0.91f, 0.94f),  // 3 Berry - rosy
        new Color(0.97f, 0.95f, 0.91f),  // 4 Desert - warm sand
        new Color(0.91f, 0.97f, 0.95f),  // 5 Aurora - cool mint
        new Color(0.97f, 0.94f, 0.90f),  // 6 Autumn - amber
        new Color(0.97f, 0.95f, 0.91f),  // 7 Tropical - sunny
        new Color(0.94f, 0.91f, 0.97f),  // 8 Lavender - purple
        new Color(0.91f, 0.91f, 0.96f),  // 9 Midnight - deep
        new Color(0.97f, 0.93f, 0.91f),  // 10 Coral - warm
        new Color(0.97f, 0.95f, 0.90f),  // 11 Golden - gold
        new Color(0.92f, 0.95f, 0.97f),  // 12 Arctic - frost
        new Color(0.97f, 0.92f, 0.90f),  // 13 Volcanic - warm
        new Color(0.97f, 0.93f, 0.94f),  // 14 Cherry - blush
        new Color(0.91f, 0.97f, 0.93f),  // 15 Emerald - jade
    };
    
    // ============ GRADIENT ARKA PLAN ÇİFTLERİ ============
    // Üst açık, alt koyu
    
    // Aydınlık gradient: Üst saf beyaz, alt belirgin renkli ton
    public static readonly Color[][] BackgroundGradients = new Color[][]
    {
        // 0 Sunset: Beyaz → sıcak pembe-turuncu
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.88f, 0.70f, 0.62f) },
        // 1 Ocean: Beyaz → okyanus mavisi
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.55f, 0.72f, 0.88f) },
        // 2 Spring: Beyaz → taze yeşil
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.62f, 0.85f, 0.58f) },
        // 3 Berry: Beyaz → pembe-mor
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.80f, 0.55f, 0.72f) },
        // 4 Desert: Beyaz → sıcak altın-kum
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.90f, 0.78f, 0.55f) },
        // 5 Aurora: Beyaz → yeşil-turkuaz
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.52f, 0.82f, 0.72f) },
        // 6 Autumn: Beyaz → sıcak turuncu
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.90f, 0.70f, 0.48f) },
        // 7 Tropical: Beyaz → güneşli sarı-turuncu
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.92f, 0.75f, 0.50f) },
        // 8 Lavender: Beyaz → mor
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.72f, 0.58f, 0.88f) },
        // 9 Midnight: Beyaz → koyu mavi-mor
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.55f, 0.50f, 0.78f) },
        // 10 Coral: Beyaz → mercan-somon
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.88f, 0.65f, 0.58f) },
        // 11 Golden: Beyaz → altın sarısı
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.90f, 0.80f, 0.48f) },
        // 12 Arctic: Beyaz → buz mavisi
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.60f, 0.78f, 0.92f) },
        // 13 Volcanic: Beyaz → lav kırmızısı
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.88f, 0.58f, 0.48f) },
        // 14 Cherry: Beyaz → çiçek pembesi
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.90f, 0.65f, 0.70f) },
        // 15 Emerald: Beyaz → zümrüt yeşili
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.50f, 0.82f, 0.60f) },
    };
    
    /// <summary>
    /// Palette index'e göre arka plan rengi döndürür
    /// </summary>
    public static Color GetBackgroundColor(int paletteIndex)
    {
        paletteIndex = paletteIndex % BackgroundColors.Length;
        return BackgroundColors[paletteIndex];
    }
    
    /// <summary>
    /// Palette index'e göre gradient arka plan renkleri döndürür (üst, alt)
    /// </summary>
    public static (Color top, Color bottom) GetBackgroundGradient(int paletteIndex)
    {
        paletteIndex = paletteIndex % BackgroundGradients.Length;
        return (BackgroundGradients[paletteIndex][0], BackgroundGradients[paletteIndex][1]);
    }
    
    // ============ I LOVE HUE - UNWRAPPED HSV BİLİNEAR İNTERPOLATİON ============
    
    /// <summary>
    /// Hue farkını en kısa yoldan hesaplar (signed, -0.5..+0.5 arası)
    /// </summary>
    private static float ShortHueDist(float from, float to)
    {
        float diff = to - from;
        while (diff > 0.5f) diff -= 1f;
        while (diff < -0.5f) diff += 1f;
        return diff;
    }
    
    /// <summary>
    /// Unwrapped HSV bilinear interpolation - 4 köşe renginden 2D gradient.
    /// Tüm hue değerlerini h0'a göre "açar" (unwrap), standard bilinear yapar,
    /// sonra geri sarar. Bu sayede iki aşamalı circular lerp'ün neden olduğu
    /// "center hue jumps to wrong side" hatası önlenir.
    /// 270°+ hue yayılımlı paletlerle birlikte, 50+ küplük gridlerde bile
    /// her küp kolayca ayırt edilebilir renk alır.
    /// </summary>
    public static Color GetBilinearColor(Color[] palette, float u, float v)
    {
        if (palette == null || palette.Length < 4)
            return GetGradientFromPalette(palette, u);
        
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        
        // 4 köşeyi HSV'ye çevir
        Color.RGBToHSV(palette[0], out float h0, out float s0, out float v0); // topLeft
        Color.RGBToHSV(palette[1], out float h1, out float s1, out float v1); // topRight
        Color.RGBToHSV(palette[2], out float h2, out float s2, out float v2); // bottomLeft
        Color.RGBToHSV(palette[3], out float h3, out float s3, out float v3); // bottomRight
        
        // Hue: h0'a göre unwrap (tüm hue'ları sürekli eksene aç)
        float uh0 = h0;
        float uh1 = h0 + ShortHueDist(h0, h1);
        float uh2 = h0 + ShortHueDist(h0, h2);
        float uh3 = h0 + ShortHueDist(h0, h3);
        
        // Standard bilinear interpolation (unwrapped hue üzerinde)
        float uhTop = Mathf.Lerp(uh0, uh1, u);
        float uhBot = Mathf.Lerp(uh2, uh3, u);
        float h = Mathf.Lerp(uhBot, uhTop, v);
        
        // [0,1] aralığına geri sar
        h = h % 1f;
        if (h < 0f) h += 1f;
        
        // Saturation: standard bilinear
        float sTop = Mathf.Lerp(s0, s1, u);
        float sBot = Mathf.Lerp(s2, s3, u);
        float s = Mathf.Lerp(sBot, sTop, v);
        
        // Value: standard bilinear
        float valTop = Mathf.Lerp(v0, v1, u);
        float valBot = Mathf.Lerp(v2, v3, u);
        float val = Mathf.Lerp(valBot, valTop, v);
        
        // Hafif satürasyon boost - interpolation'da renk solmasını önle
        s = Mathf.Clamp01(s * 1.1f);
        
        return Color.HSVToRGB(h, s, val);
    }
    
    /// <summary>
    /// 3D Unwrapped HSV bilinear - Z ekseni hue'yu kaydırarak derinlik katmanlarını ayırır
    /// </summary>
    public static Color GetBilinearColor3D(Color[] palette, float u, float v, float w)
    {
        Color baseColor = GetBilinearColor(palette, u, v);
        
        Color.RGBToHSV(baseColor, out float h, out float s, out float val);
        
        // Z ekseni: hue'yu kaydır → farklı derinlik katmanları farklı tonlar
        w = Mathf.Clamp01(w);
        h = (h + w * 0.05f) % 1f;
        if (h < 0f) h += 1f;
        
        // Z ekseni: value farkı → arka daha koyu, ön daha parlak
        val = Mathf.Lerp(val * 0.90f, val, w);
        val = Mathf.Clamp01(val);
        
        return Color.HSVToRGB(h, s, val);
    }
    
    /// <summary>
    /// Palette index'e göre bilinear renk döndürür
    /// </summary>
    public static Color GetBilinearColorByIndex(int paletteIndex, float u, float v)
    {
        paletteIndex = Mathf.Abs(paletteIndex) % AllPalettes.Length;
        return GetBilinearColor(AllPalettes[paletteIndex], u, v);
    }
    
    /// <summary>
    /// Smooth step fonksiyonu - daha yumuşak geçişler
    /// </summary>
    private static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }
    
    /// <summary>
    /// Palette dizisinden gradient renk alır - 1D Smooth interpolation (geriye uyumluluk)
    /// </summary>
    public static Color GetGradientFromPalette(Color[] palette, float t)
    {
        if (palette == null || palette.Length == 0)
            return Color.white;
        
        if (palette.Length == 1)
            return palette[0];
        
        t = Mathf.Clamp01(t);
        
        // 4 köşeli palette ise diagonal gradient yap (1D fallback)
        if (palette.Length == 4)
        {
            // topLeft'ten bottomRight'a diagonal
            Color topDiag = Color.Lerp(palette[0], palette[1], t);
            Color botDiag = Color.Lerp(palette[2], palette[3], t);
            return Color.Lerp(topDiag, botDiag, t);
        }
        
        // Smooth easing (cubic)
        t = t * t * (3f - 2f * t);
        
        float scaledT = t * (palette.Length - 1);
        int index = Mathf.FloorToInt(scaledT);
        float remainder = scaledT - index;
        
        // Smooth step for remainder too
        remainder = remainder * remainder * (3f - 2f * remainder);
        
        if (index >= palette.Length - 1)
            return palette[palette.Length - 1];
        
        return Color.Lerp(palette[index], palette[index + 1], remainder);
    }
    
    /// <summary>
    /// Rastgele palette seçer
    /// </summary>
    public static Color[] GetRandomPalette()
    {
        return AllPalettes[Random.Range(0, AllPalettes.Length)];
    }
    
    /// <summary>
    /// İsme göre palette döndürür
    /// </summary>
    public static Color[] GetPaletteByName(string name)
    {
        for (int i = 0; i < PaletteNames.Length; i++)
        {
            if (PaletteNames[i].ToLower() == name.ToLower())
            {
                return AllPalettes[i];
            }
        }
        return SunsetWarmth; // Default
    }
    
    /// <summary>
    /// HSV renk alanında gradient oluşturur
    /// </summary>
    public static Color HSVGradient(float hueStart, float hueEnd, float t, float saturation = 0.7f, float value = 0.9f)
    {
        float hue = Mathf.Lerp(hueStart, hueEnd, t);
        // Hue 360 derece döngüsel
        if (hue < 0) hue += 1f;
        if (hue > 1f) hue -= 1f;
        
        return Color.HSVToRGB(hue, saturation, value);
    }
    
    /// <summary>
    /// Gökkuşağı gradient'i
    /// </summary>
    public static Color RainbowGradient(float t)
    {
        return HSVGradient(0f, 1f, t, 0.8f, 0.95f);
    }
}
