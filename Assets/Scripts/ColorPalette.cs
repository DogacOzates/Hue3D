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
    
    // 0: Sunset Warmth - Sarı → Pembe → Turuncu → Mor geçişi
    public static readonly Color[] SunsetWarmth = new Color[]
    {
        new Color(0.98f, 0.92f, 0.35f),  // topLeft:     Bright yellow
        new Color(0.92f, 0.35f, 0.55f),  // topRight:    Hot pink
        new Color(0.98f, 0.65f, 0.25f),  // bottomLeft:  Tangerine orange
        new Color(0.65f, 0.30f, 0.72f),  // bottomRight: Purple
    };
    
    // 1: Ocean Depths - Turkuaz → Lacivert → Yeşil → Mor
    public static readonly Color[] OceanDepths = new Color[]
    {
        new Color(0.30f, 0.85f, 0.82f),  // topLeft:     Bright turquoise
        new Color(0.20f, 0.32f, 0.78f),  // topRight:    Royal blue
        new Color(0.35f, 0.75f, 0.45f),  // bottomLeft:  Sea green
        new Color(0.48f, 0.28f, 0.68f),  // bottomRight: Deep violet
    };
    
    // 2: Spring Meadow - Açık yeşil → Sarı → Turkuaz → Pembe
    public static readonly Color[] SpringMeadow = new Color[]
    {
        new Color(0.60f, 0.90f, 0.40f),  // topLeft:     Lime green
        new Color(0.98f, 0.88f, 0.30f),  // topRight:    Sunny yellow
        new Color(0.25f, 0.78f, 0.72f),  // bottomLeft:  Teal
        new Color(0.95f, 0.55f, 0.65f),  // bottomRight: Rose pink
    };
    
    // 3: Berry Garden - Pembe → Mor → Kırmızı → Mavi
    public static readonly Color[] BerryGarden = new Color[]
    {
        new Color(0.95f, 0.55f, 0.70f),  // topLeft:     Bubblegum pink
        new Color(0.55f, 0.30f, 0.78f),  // topRight:    Grape purple
        new Color(0.88f, 0.25f, 0.35f),  // bottomLeft:  Cherry red
        new Color(0.35f, 0.45f, 0.85f),  // bottomRight: Cornflower blue
    };
    
    // 4: Desert Dusk - Turuncu → Pembe → Sarı → Mor
    public static readonly Color[] DesertDusk = new Color[]
    {
        new Color(0.95f, 0.62f, 0.28f),  // topLeft:     Deep orange
        new Color(0.85f, 0.42f, 0.62f),  // topRight:    Dusty rose
        new Color(0.95f, 0.88f, 0.45f),  // bottomLeft:  Gold
        new Color(0.58f, 0.35f, 0.65f),  // bottomRight: Plum
    };
    
    // 5: Northern Lights - Yeşil → Mavi → Mor → Pembe
    public static readonly Color[] NorthernLights = new Color[]
    {
        new Color(0.30f, 0.88f, 0.55f),  // topLeft:     Aurora green
        new Color(0.35f, 0.55f, 0.92f),  // topRight:    Electric blue
        new Color(0.62f, 0.30f, 0.82f),  // bottomLeft:  Violet
        new Color(0.90f, 0.50f, 0.72f),  // bottomRight: Orchid pink
    };
    
    // 6: Autumn Forest - Kırmızı → Sarı → Yeşil → Turuncu
    public static readonly Color[] AutumnForest = new Color[]
    {
        new Color(0.82f, 0.22f, 0.18f),  // topLeft:     Fire red
        new Color(0.95f, 0.85f, 0.25f),  // topRight:    Bright yellow
        new Color(0.28f, 0.62f, 0.32f),  // bottomLeft:  Forest green
        new Color(0.92f, 0.58f, 0.18f),  // bottomRight: Amber orange
    };
    
    // 7: Tropical Sunrise - Sarı → Pembe → Turuncu → Turkuaz
    public static readonly Color[] TropicalSunrise = new Color[]
    {
        new Color(0.98f, 0.95f, 0.40f),  // topLeft:     Lemon yellow
        new Color(0.95f, 0.38f, 0.58f),  // topRight:    Tropical pink
        new Color(0.98f, 0.55f, 0.20f),  // bottomLeft:  Mango orange
        new Color(0.22f, 0.82f, 0.78f),  // bottomRight: Tropical teal
    };
    
    // 8: Lavender Fields - Mor → Pembe → Mavi → Yeşil
    public static readonly Color[] LavenderFields = new Color[]
    {
        new Color(0.72f, 0.45f, 0.88f),  // topLeft:     Rich lavender
        new Color(0.95f, 0.62f, 0.72f),  // topRight:    Soft pink
        new Color(0.38f, 0.52f, 0.85f),  // bottomLeft:  Periwinkle blue
        new Color(0.55f, 0.82f, 0.62f),  // bottomRight: Sage green
    };
    
    // 9: Midnight City - Mavi → Mor → Turuncu → Pembe
    public static readonly Color[] MidnightCity = new Color[]
    {
        new Color(0.18f, 0.28f, 0.62f),  // topLeft:     Deep navy
        new Color(0.55f, 0.22f, 0.58f),  // topRight:    Dark magenta
        new Color(0.88f, 0.55f, 0.25f),  // bottomLeft:  Warm amber
        new Color(0.82f, 0.38f, 0.52f),  // bottomRight: Berry pink
    };
    
    // 10: Coral Reef - Mercan → Turkuaz → Pembe → Mavi
    public static readonly Color[] CoralReef = new Color[]
    {
        new Color(0.98f, 0.48f, 0.42f),  // topLeft:     Coral red
        new Color(0.25f, 0.85f, 0.78f),  // topRight:    Bright turquoise
        new Color(0.92f, 0.60f, 0.72f),  // bottomLeft:  Blush pink
        new Color(0.28f, 0.55f, 0.88f),  // bottomRight: Ocean blue
    };
    
    // 11: Golden Hour - Sarı → Kırmızı → Yeşil → Turuncu
    public static readonly Color[] GoldenHour = new Color[]
    {
        new Color(0.98f, 0.90f, 0.35f),  // topLeft:     Bright gold
        new Color(0.88f, 0.30f, 0.28f),  // topRight:    Crimson
        new Color(0.48f, 0.78f, 0.35f),  // bottomLeft:  Grass green
        new Color(0.95f, 0.62f, 0.22f),  // bottomRight: Deep amber
    };
    
    // 12: Arctic Ice - Açık mavi → Beyaz → Mor → Yeşil
    public static readonly Color[] ArcticIce = new Color[]
    {
        new Color(0.65f, 0.85f, 0.98f),  // topLeft:     Ice blue
        new Color(0.92f, 0.90f, 0.95f),  // topRight:    Snow white
        new Color(0.55f, 0.42f, 0.78f),  // bottomLeft:  Frozen violet
        new Color(0.40f, 0.82f, 0.72f),  // bottomRight: Glacial teal
    };
    
    // 13: Volcanic Ash - Kırmızı → Turuncu → Sarı → Siyahımsı
    public static readonly Color[] VolcanicAsh = new Color[]
    {
        new Color(0.82f, 0.18f, 0.15f),  // topLeft:     Lava red
        new Color(0.95f, 0.58f, 0.18f),  // topRight:    Molten orange
        new Color(0.95f, 0.88f, 0.30f),  // bottomLeft:  Sulfur yellow
        new Color(0.35f, 0.25f, 0.42f),  // bottomRight: Dark obsidian
    };
    
    // 14: Cherry Blossom - Pembe → Beyaz → Mor → Yeşil
    public static readonly Color[] CherryBlossom = new Color[]
    {
        new Color(0.95f, 0.62f, 0.72f),  // topLeft:     Sakura pink
        new Color(0.95f, 0.92f, 0.88f),  // topRight:    Cream
        new Color(0.68f, 0.35f, 0.72f),  // bottomLeft:  Plum purple
        new Color(0.55f, 0.82f, 0.50f),  // bottomRight: Spring green
    };
    
    // 15: Emerald Cave - Yeşil → Mavi → Sarı → Mor
    public static readonly Color[] EmeraldCave = new Color[]
    {
        new Color(0.18f, 0.72f, 0.42f),  // topLeft:     Emerald green
        new Color(0.25f, 0.48f, 0.85f),  // topRight:    Sapphire blue
        new Color(0.82f, 0.88f, 0.28f),  // bottomLeft:  Chartreuse
        new Color(0.62f, 0.28f, 0.72f),  // bottomRight: Amethyst purple
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
        new Color(0.95f, 0.93f, 0.97f),  // Sunset - ultra soft lavender
        new Color(0.93f, 0.95f, 0.98f),  // Ocean - ultra soft blue
        new Color(0.94f, 0.97f, 0.94f),  // Spring - ultra soft mint
        new Color(0.98f, 0.93f, 0.95f),  // Berry - ultra soft pink
        new Color(0.98f, 0.96f, 0.93f),  // Desert - ultra soft cream
        new Color(0.94f, 0.95f, 0.98f),  // Northern - ultra soft ice blue
        new Color(0.98f, 0.95f, 0.92f),  // Autumn - ultra soft peach
        new Color(0.98f, 0.94f, 0.93f),  // Tropical - ultra soft coral
        new Color(0.95f, 0.93f, 0.98f),  // Lavender - ultra soft lilac
        new Color(0.93f, 0.94f, 0.97f),  // Midnight - ultra soft slate
        new Color(0.93f, 0.97f, 0.97f),  // Coral - ultra soft aqua
        new Color(0.98f, 0.97f, 0.93f),  // Golden - ultra soft gold
        new Color(0.94f, 0.97f, 0.99f),  // Arctic - ultra soft frost
        new Color(0.98f, 0.94f, 0.93f),  // Volcanic - ultra soft warm
        new Color(0.99f, 0.94f, 0.95f),  // Cherry - ultra soft rose
        new Color(0.93f, 0.98f, 0.95f),  // Emerald - ultra soft jade
    };
    
    // ============ GRADIENT ARKA PLAN ÇİFTLERİ ============
    // Üst açık, alt koyu
    
    // EYKA tarzı: Üst beyaza yakın, alt daha belirgin pastel
    public static readonly Color[][] BackgroundGradients = new Color[][]
    {
        // Sunset: Beyaz → lavender
        new Color[] { new Color(0.99f, 0.98f, 1.00f), new Color(0.68f, 0.58f, 0.82f) },
        // Ocean: Beyaz → blue
        new Color[] { new Color(0.98f, 0.99f, 1.00f), new Color(0.58f, 0.70f, 0.90f) },
        // Spring: Beyaz → mint
        new Color[] { new Color(0.98f, 1.00f, 0.98f), new Color(0.60f, 0.84f, 0.68f) },
        // Berry: Beyaz → pink
        new Color[] { new Color(1.00f, 0.97f, 0.98f), new Color(0.88f, 0.60f, 0.70f) },
        // Desert: Beyaz → cream
        new Color[] { new Color(1.00f, 0.99f, 0.97f), new Color(0.88f, 0.78f, 0.60f) },
        // Northern: Beyaz → ice blue
        new Color[] { new Color(0.98f, 0.99f, 1.00f), new Color(0.62f, 0.72f, 0.90f) },
        // Autumn: Beyaz → peach
        new Color[] { new Color(1.00f, 0.98f, 0.97f), new Color(0.90f, 0.72f, 0.60f) },
        // Tropical: Beyaz → coral
        new Color[] { new Color(1.00f, 0.98f, 0.97f), new Color(0.90f, 0.66f, 0.62f) },
        // Lavender: Beyaz → lilac
        new Color[] { new Color(0.99f, 0.98f, 1.00f), new Color(0.72f, 0.62f, 0.86f) },
        // Midnight: Beyaz → slate
        new Color[] { new Color(0.98f, 0.98f, 1.00f), new Color(0.64f, 0.66f, 0.82f) },
        // Coral: Beyaz → aqua
        new Color[] { new Color(0.98f, 1.00f, 1.00f), new Color(0.58f, 0.82f, 0.82f) },
        // Golden: Beyaz → gold
        new Color[] { new Color(1.00f, 0.99f, 0.97f), new Color(0.88f, 0.80f, 0.55f) },
        // Arctic: Beyaz → frost
        new Color[] { new Color(0.98f, 0.99f, 1.00f), new Color(0.65f, 0.82f, 0.92f) },
        // Volcanic: Beyaz → rose
        new Color[] { new Color(1.00f, 0.98f, 0.97f), new Color(0.84f, 0.62f, 0.58f) },
        // Cherry: Beyaz → blush
        new Color[] { new Color(1.00f, 0.98f, 0.98f), new Color(0.88f, 0.65f, 0.72f) },
        // Emerald: Beyaz → jade
        new Color[] { new Color(0.98f, 1.00f, 0.98f), new Color(0.58f, 0.82f, 0.68f) },
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
        
        // Smooth easing for more perceptually uniform gradient (I Love Hue style)
        u = SmoothStep(u);
        v = SmoothStep(v);
        
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
