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
    
    // 0: Sunset - Sarı → Kırmızı → Turuncu → Mor (sıcak kontrast)
    public static readonly Color[] SunsetWarmth = new Color[]
    {
        new Color(1.00f, 0.90f, 0.20f),  // topLeft:     Pure yellow
        new Color(0.95f, 0.20f, 0.30f),  // topRight:    Vivid red
        new Color(1.00f, 0.55f, 0.10f),  // bottomLeft:  Orange
        new Color(0.70f, 0.15f, 0.70f),  // bottomRight: Purple
    };
    
    // 1: Ocean - Cyan → Lacivert → Yeşil → Mor (soğuk derin)
    public static readonly Color[] OceanDepths = new Color[]
    {
        new Color(0.10f, 0.90f, 0.95f),  // topLeft:     Bright cyan
        new Color(0.15f, 0.25f, 0.90f),  // topRight:    Deep blue
        new Color(0.10f, 0.80f, 0.40f),  // bottomLeft:  Sea green
        new Color(0.55f, 0.20f, 0.85f),  // bottomRight: Violet
    };
    
    // 2: Spring - Yeşil → Sarı → Turkuaz → Pembe (taze canlı)
    public static readonly Color[] SpringMeadow = new Color[]
    {
        new Color(0.30f, 0.92f, 0.20f),  // topLeft:     Vivid green
        new Color(1.00f, 0.95f, 0.15f),  // topRight:    Bright yellow
        new Color(0.10f, 0.85f, 0.75f),  // bottomLeft:  Teal
        new Color(0.98f, 0.35f, 0.55f),  // bottomRight: Hot pink
    };
    
    // 3: Berry - Pembe → Mor → Kırmızı → Mavi (canlı kontrast)
    public static readonly Color[] BerryGarden = new Color[]
    {
        new Color(0.98f, 0.30f, 0.60f),  // topLeft:     Hot pink
        new Color(0.50f, 0.15f, 0.90f),  // topRight:    Vivid purple
        new Color(0.92f, 0.15f, 0.20f),  // bottomLeft:  Cherry red
        new Color(0.20f, 0.45f, 0.95f),  // bottomRight: Bright blue
    };
    
    // 4: Desert - Altın → Pembe → Yeşil → Turuncu (sıcak toprak)
    public static readonly Color[] DesertDusk = new Color[]
    {
        new Color(0.98f, 0.80f, 0.15f),  // topLeft:     Gold
        new Color(0.92f, 0.35f, 0.50f),  // topRight:    Rose
        new Color(0.55f, 0.78f, 0.25f),  // bottomLeft:  Olive green
        new Color(0.95f, 0.50f, 0.15f),  // bottomRight: Deep orange
    };
    
    // 5: Aurora - Yeşil → Mavi → Mor → Pembe (neon aurora)
    public static readonly Color[] NorthernLights = new Color[]
    {
        new Color(0.15f, 0.95f, 0.40f),  // topLeft:     Neon green
        new Color(0.15f, 0.50f, 0.98f),  // topRight:    Electric blue
        new Color(0.70f, 0.10f, 0.90f),  // bottomLeft:  Neon violet
        new Color(0.98f, 0.30f, 0.65f),  // bottomRight: Magenta
    };
    
    // 6: Autumn - Kırmızı → Sarı → Yeşil → Turuncu (sonbahar yaprak)
    public static readonly Color[] AutumnForest = new Color[]
    {
        new Color(0.90f, 0.15f, 0.10f),  // topLeft:     Fire red
        new Color(1.00f, 0.88f, 0.10f),  // topRight:    Bright yellow
        new Color(0.20f, 0.70f, 0.20f),  // bottomLeft:  Forest green
        new Color(0.98f, 0.55f, 0.08f),  // bottomRight: Amber
    };
    
    // 7: Tropical - Limon → Karpuz → Mango → Turkuaz (tropik meyve)
    public static readonly Color[] TropicalSunrise = new Color[]
    {
        new Color(1.00f, 0.95f, 0.15f),  // topLeft:     Lemon
        new Color(0.98f, 0.20f, 0.45f),  // topRight:    Watermelon
        new Color(1.00f, 0.60f, 0.08f),  // bottomLeft:  Mango
        new Color(0.08f, 0.90f, 0.85f),  // bottomRight: Turquoise
    };
    
    // 8: Lavender - Mor → Pembe → Mavi → Yeşil (pastel canlı)
    public static readonly Color[] LavenderFields = new Color[]
    {
        new Color(0.72f, 0.30f, 0.95f),  // topLeft:     Rich purple
        new Color(0.98f, 0.45f, 0.60f),  // topRight:    Rose pink
        new Color(0.20f, 0.45f, 0.92f),  // bottomLeft:  Royal blue
        new Color(0.30f, 0.90f, 0.50f),  // bottomRight: Mint green
    };
    
    // 9: Midnight - Lacivert → Mor → Turuncu → Kırmızı (gece şehir)
    public static readonly Color[] MidnightCity = new Color[]
    {
        new Color(0.10f, 0.18f, 0.75f),  // topLeft:     Dark blue
        new Color(0.65f, 0.12f, 0.70f),  // topRight:    Deep magenta
        new Color(0.98f, 0.65f, 0.10f),  // bottomLeft:  Amber gold
        new Color(0.92f, 0.20f, 0.35f),  // bottomRight: Crimson
    };
    
    // 10: Coral - Mercan → Turkuaz → Pembe → Mavi (deniz kontrast)
    public static readonly Color[] CoralReef = new Color[]
    {
        new Color(0.98f, 0.35f, 0.28f),  // topLeft:     Bright coral
        new Color(0.08f, 0.92f, 0.85f),  // topRight:    Aqua
        new Color(0.95f, 0.55f, 0.70f),  // bottomLeft:  Salmon
        new Color(0.15f, 0.50f, 0.95f),  // bottomRight: Azure
    };
    
    // 11: Golden - Altın → Kırmızı → Yeşil → Turuncu (zengin sıcak)
    public static readonly Color[] GoldenHour = new Color[]
    {
        new Color(1.00f, 0.88f, 0.15f),  // topLeft:     Rich gold
        new Color(0.92f, 0.18f, 0.15f),  // topRight:    Red
        new Color(0.30f, 0.85f, 0.25f),  // bottomLeft:  Lime green
        new Color(0.98f, 0.60f, 0.10f),  // bottomRight: Deep amber
    };
    
    // 12: Arctic - Buz mavi → Beyaz → Mor → Turkuaz (buz kristal)
    public static readonly Color[] ArcticIce = new Color[]
    {
        new Color(0.45f, 0.78f, 0.98f),  // topLeft:     Ice blue
        new Color(0.92f, 0.88f, 0.98f),  // topRight:    Frost
        new Color(0.50f, 0.25f, 0.85f),  // bottomLeft:  Violet
        new Color(0.15f, 0.88f, 0.72f),  // bottomRight: Cool teal
    };
    
    // 13: Volcanic - Lava → Alev → Kükürt → Obsidyen (ateş)
    public static readonly Color[] VolcanicAsh = new Color[]
    {
        new Color(0.92f, 0.10f, 0.08f),  // topLeft:     Lava red
        new Color(1.00f, 0.60f, 0.05f),  // topRight:    Flame orange
        new Color(1.00f, 0.92f, 0.15f),  // bottomLeft:  Sulfur yellow
        new Color(0.35f, 0.15f, 0.50f),  // bottomRight: Obsidian
    };
    
    // 14: Sakura - Pembe → Krem → Mor → Yeşil (bahar çiçek)
    public static readonly Color[] CherryBlossom = new Color[]
    {
        new Color(0.98f, 0.50f, 0.68f),  // topLeft:     Sakura pink
        new Color(0.98f, 0.92f, 0.78f),  // topRight:    Cream
        new Color(0.70f, 0.20f, 0.78f),  // bottomLeft:  Plum purple
        new Color(0.35f, 0.88f, 0.38f),  // bottomRight: Leaf green
    };
    
    // 15: Emerald - Zümrüt → Safir → Sarı → Ametist (mücevher)
    public static readonly Color[] EmeraldCave = new Color[]
    {
        new Color(0.05f, 0.82f, 0.38f),  // topLeft:     Emerald
        new Color(0.12f, 0.40f, 0.95f),  // topRight:    Sapphire
        new Color(0.90f, 0.92f, 0.15f),  // bottomLeft:  Chartreuse
        new Color(0.68f, 0.18f, 0.82f),  // bottomRight: Amethyst
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
        new Color(0.98f, 0.92f, 0.90f),  // 0 Sunset - warm peach
        new Color(0.90f, 0.94f, 0.98f),  // 1 Ocean - cool blue
        new Color(0.92f, 0.98f, 0.92f),  // 2 Spring - fresh green
        new Color(0.98f, 0.90f, 0.94f),  // 3 Berry - rosy pink
        new Color(0.98f, 0.96f, 0.90f),  // 4 Desert - warm sand
        new Color(0.90f, 0.96f, 0.96f),  // 5 Northern - icy teal
        new Color(0.98f, 0.94f, 0.88f),  // 6 Autumn - warm amber
        new Color(0.98f, 0.92f, 0.90f),  // 7 Tropical - coral
        new Color(0.94f, 0.90f, 0.98f),  // 8 Lavender - purple
        new Color(0.90f, 0.90f, 0.96f),  // 9 Midnight - deep blue
        new Color(0.90f, 0.97f, 0.97f),  // 10 Coral - aqua
        new Color(0.98f, 0.96f, 0.88f),  // 11 Golden - gold
        new Color(0.92f, 0.96f, 0.99f),  // 12 Arctic - frost
        new Color(0.98f, 0.92f, 0.88f),  // 13 Volcanic - warm
        new Color(0.98f, 0.92f, 0.94f),  // 14 Cherry - blush
        new Color(0.90f, 0.98f, 0.94f),  // 15 Emerald - jade
    };
    
    // ============ GRADIENT ARKA PLAN ÇİFTLERİ ============
    // Üst açık, alt koyu
    
    // Aydınlık gradient: Üst saf beyaz, alt belirgin renkli ton
    public static readonly Color[][] BackgroundGradients = new Color[][]
    {
        // 0 Sunset: Beyaz → sıcak turuncu-pembe
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.90f, 0.68f, 0.60f) },
        // 1 Ocean: Beyaz → canlı mavi
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.55f, 0.70f, 0.92f) },
        // 2 Spring: Beyaz → canlı yeşil
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.60f, 0.85f, 0.62f) },
        // 3 Berry: Beyaz → canlı pembe-mor
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.82f, 0.55f, 0.75f) },
        // 4 Desert: Beyaz → sıcak kum-altın
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.92f, 0.80f, 0.55f) },
        // 5 Northern: Beyaz → canlı turkuaz
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.50f, 0.82f, 0.78f) },
        // 6 Autumn: Beyaz → sıcak turuncu
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.92f, 0.70f, 0.48f) },
        // 7 Tropical: Beyaz → canlı mercan
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.95f, 0.62f, 0.55f) },
        // 8 Lavender: Beyaz → canlı mor
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.75f, 0.60f, 0.90f) },
        // 9 Midnight: Beyaz → koyu mavi-mor
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.58f, 0.52f, 0.78f) },
        // 10 Coral: Beyaz → canlı aqua-turkuaz
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.48f, 0.82f, 0.82f) },
        // 11 Golden: Beyaz → sıcak altın-sarı
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.92f, 0.82f, 0.45f) },
        // 12 Arctic: Beyaz → soğuk buz mavisi
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.62f, 0.80f, 0.95f) },
        // 13 Volcanic: Beyaz → sıcak kırmızı-turuncu
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.90f, 0.58f, 0.48f) },
        // 14 Cherry: Beyaz → canlı pembe
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.92f, 0.62f, 0.70f) },
        // 15 Emerald: Beyaz → canlı zümrüt
        new Color[] { new Color(1.00f, 1.00f, 1.00f), new Color(0.48f, 0.82f, 0.60f) },
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
    
    // ============ I LOVE HUE - BİLİNEAR İNTERPOLATİON ============
    
    /// <summary>
    /// I Love Hue tarzı bilinear interpolation - 4 köşe renginden 2D gradient
    /// palette[0]=topLeft, palette[1]=topRight, palette[2]=bottomLeft, palette[3]=bottomRight
    /// u = normalize X (0=sol, 1=sağ), v = normalize Y (0=alt, 1=üst)
    /// </summary>
    public static Color GetBilinearColor(Color[] palette, float u, float v)
    {
        if (palette == null || palette.Length < 4)
            return GetGradientFromPalette(palette, u); // Fallback to 1D
        
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        
        // Lineer geçiş - keskin, net renk farkları (SmoothStep kaldırıldı)
        
        Color topLeft = palette[0];
        Color topRight = palette[1];
        Color bottomLeft = palette[2];
        Color bottomRight = palette[3];
        
        // Bilinear interpolation
        Color top = Color.Lerp(topLeft, topRight, u);
        Color bottom = Color.Lerp(bottomLeft, bottomRight, u);
        return Color.Lerp(bottom, top, v);
    }
    
    /// <summary>
    /// 3D bilinear - Z ekseni için hafif renk kayması ekler
    /// </summary>
    public static Color GetBilinearColor3D(Color[] palette, float u, float v, float w)
    {
        Color baseColor = GetBilinearColor(palette, u, v);
        
        // Z ekseni: hafif brightness/saturation shift (derinlik hissi)
        w = Mathf.Clamp01(w);
        Color.RGBToHSV(baseColor, out float h, out float s, out float val);
        
        // Arka plana yakın küpler biraz daha koyu, öne yakın olanlar biraz daha açık
        val = Mathf.Lerp(val * 0.88f, val * 1.05f, w);
        s = Mathf.Lerp(s * 1.08f, s * 0.92f, w);
        val = Mathf.Clamp01(val);
        s = Mathf.Clamp01(s);
        
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
