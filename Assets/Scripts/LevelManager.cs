using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Level verilerini ve şekil tanımlarını içerir
/// </summary>
[System.Serializable]
public class LevelData
{
    public string levelName;
    public int levelNumber;
    public int colorPaletteIndex;
    public float fixedCubeRatio;
    public string description;
    // Custom shape - her level kendi pozisyonlarını tanımlar
    public List<Vector3Int> customPositions;
    // Sabit küplerin pozisyonları (değiştirilemez küpler)
    public List<Vector3Int> fixedPositions;
}

/// <summary>
/// Tüm levelleri yöneten statik sınıf
/// </summary>
public static class LevelManager
{
    private static LevelData[] levels;
    private static LevelData[] sortedLevels;  // Küp sayısına göre sıralanmış
    private static int maxJsonLevel = 0;
    
    /// <summary>
    /// Hardcoded level sayısı
    /// </summary>
    public static int TotalLevels => levels?.Length ?? 0;
    
    /// <summary>
    /// En yüksek level numarası (hardcoded ve JSON dahil)
    /// </summary>
    public static int MaxLevelNumber
    {
        get
        {
            // JSON level'ları kontrol et
            RefreshMaxJsonLevel();
            return Mathf.Max(TotalLevels, maxJsonLevel);
        }
    }
    
    /// <summary>
    /// JSON level'ların en yükseğini bul
    /// </summary>
    private static void RefreshMaxJsonLevel()
    {
        var jsonLevels = LevelJsonManager.LoadAllLevels();
        foreach (var level in jsonLevels)
        {
            if (level.levelNumber > maxJsonLevel)
            {
                maxJsonLevel = level.levelNumber;
            }
        }
    }
    
    static LevelManager()
    {
        CreateLevels();
    }
    
    /// <summary>
    /// Level'leri yeniden yükler (Editor'den değişiklik yapıldığında)
    /// </summary>
    public static void ReloadLevels()
    {
        CreateLevels();
        sortedLevels = null;  // Sıralamayı yeniden yap
        Debug.Log($"LevelManager: {TotalLevels} levels reloaded.");
    }
    
    /// <summary>
    /// I Love Hue 3D - 50 level: Asimetrik, yaratıcı 3D şekiller
    /// Bölüm 1 (1-10): 2D giriş - basit gridler + ilk şekiller
    /// Bölüm 2 (11-20): Şekilli 2D + 3D giriş
    /// Bölüm 3 (21-30): Yaratıcı 3D yapılar
    /// Bölüm 4 (31-40): Karmaşık asimetrik yapılar
    /// Bölüm 5 (41-50): Epik dev yapılar
    /// </summary>
    private static void CreateLevels()
    {
        levels = new LevelData[]
        {
            // ============ TUTORIAL ============
            
            // Level 0: Tutorial - 5 küp yan yana (en basit)
            CreateLevelWithFixed(0, "Tutorial", 0, "Learn to play",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), 
                    new Vector3Int(3,0,0), new Vector3Int(4,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0) }),

            // ============ BÖLÜM 1: GİRİŞ (1-10) ============

            // Level 1: 3x3 grid - öğretici (9 küp)
            CreateLevelWithFixed(1, "First Touch", 0, "3x3 intro",
                GenerateGrid(3, 3, 1),
                GetCorners(3, 3, 1)),

            // Level 2: 4x3 grid (12 küp)
            CreateLevelWithFixed(2, "Canvas", 1, "4x3 rectangle",
                GenerateGrid(4, 3, 1),
                GetCorners(4, 3, 1)),

            // Level 3: 5x4 grid (20 küp)
            CreateLevelWithFixed(3, "Board", 2, "5x4 wide rectangle",
                GenerateGrid(5, 4, 1),
                GetCorners(5, 4, 1)),

            // Level 4: Elmas (13 küp) - ilk asimetrik
            CreateLevelWithFixed(4, "Diamond", 3, "Diamond shape",
                new Vector3Int[] {
                    new Vector3Int(3,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(3,4,0)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(1,2,0), new Vector3Int(5,2,0), new Vector3Int(3,4,0) }),

            // Level 5: Ok işareti (15 küp)
            CreateLevelWithFixed(5, "Arrow", 4, "Right arrow",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    // Uç üçgen
                    new Vector3Int(4,1,0), new Vector3Int(4,2,0), new Vector3Int(4,3,0), new Vector3Int(4,4,0),
                    new Vector3Int(5,2,0), new Vector3Int(5,3,0),
                    new Vector3Int(6,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(0,3,0), new Vector3Int(6,2,0), new Vector3Int(4,4,0) }),

            // Level 6: Hilal (14 küp)
            CreateLevelWithFixed(6, "Crescent", 5, "Crescent shape",
                new Vector3Int[] {
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(0,2,0),
                    new Vector3Int(0,3,0),
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0),
                    new Vector3Int(1,5,0), new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    new Vector3Int(0,5,0), new Vector3Int(0,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0), new Vector3Int(0,3,0), new Vector3Int(3,5,0) }),

            // Level 7: Yıldırım (16 küp)
            CreateLevelWithFixed(7, "Lightning", 0, "Lightning shape",
                new Vector3Int[] {
                    new Vector3Int(2,5,0), new Vector3Int(3,5,0), new Vector3Int(4,5,0),
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0),
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(2,5,0), new Vector3Int(4,5,0) }),

            // Level 8: Spiral 2D (18 küp)
            CreateLevelWithFixed(8, "Spiral", 1, "Flat spiral",
                new Vector3Int[] {
                    // Dış kenar saat yönü
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(4,1,0), new Vector3Int(4,2,0), new Vector3Int(4,3,0),
                    new Vector3Int(3,3,0), new Vector3Int(2,3,0), new Vector3Int(1,3,0),
                    new Vector3Int(1,2,0),
                    // İç spiral
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    new Vector3Int(3,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,3,0), new Vector3Int(2,2,0) }),

            // Level 9: Kalp (19 küp)
            CreateLevelWithFixed(9, "Heart", 2, "Heart shape",
                new Vector3Int[] {
                    // Alt uç
                    new Vector3Int(3,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(5,3,0), new Vector3Int(6,3,0),
                    // Üst çukurluk
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0), new Vector3Int(2,4,0),
                    new Vector3Int(4,4,0), new Vector3Int(5,4,0), new Vector3Int(6,4,0),
                    new Vector3Int(3,2,0)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(0,4,0), new Vector3Int(6,4,0), new Vector3Int(3,2,0) }),

            // Level 10: Yıldız (22 küp)
            CreateLevelWithFixed(10, "Star", 3, "5-pointed star",
                new Vector3Int[] {
                    // Merkez
                    new Vector3Int(3,2,0), new Vector3Int(3,3,0), new Vector3Int(2,2,0), new Vector3Int(4,2,0), new Vector3Int(2,3,0), new Vector3Int(4,3,0),
                    // Üst kol
                    new Vector3Int(3,4,0), new Vector3Int(3,5,0),
                    // Alt kol
                    new Vector3Int(3,1,0), new Vector3Int(3,0,0),
                    // Sol kol
                    new Vector3Int(1,3,0), new Vector3Int(0,3,0),
                    // Sağ kol
                    new Vector3Int(5,2,0), new Vector3Int(6,2,0),
                    // Çapraz kollar
                    new Vector3Int(1,4,0), new Vector3Int(0,5,0),
                    new Vector3Int(5,4,0), new Vector3Int(6,5,0),
                    new Vector3Int(1,1,0), new Vector3Int(0,0,0),
                    new Vector3Int(5,1,0), new Vector3Int(6,0,0)
                },
                new Vector3Int[] { new Vector3Int(3,5,0), new Vector3Int(3,0,0), new Vector3Int(0,5,0), new Vector3Int(6,5,0), new Vector3Int(0,0,0), new Vector3Int(6,0,0) }),

            // ============ BÖLÜM 2: ŞEKİLLİ 2D + 3D GİRİŞ (11-20) ============

            // Level 11: L şekli 3D (18 küp) - ilk 3D asimetrik
            CreateLevelWithFixed(11, "3D L", 4, "L shape two layers",
                new Vector3Int[] {
                    // Alt L
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    new Vector3Int(1,1,0), new Vector3Int(1,2,0),
                    // Üst katman L
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(0,1,1), new Vector3Int(0,2,1), new Vector3Int(0,3,1),
                    new Vector3Int(1,1,1), new Vector3Int(1,2,1)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(0,3,0), new Vector3Int(3,0,1), new Vector3Int(0,3,1) }),

            // Level 12: 3D Basamak (20 küp)
            CreateLevelWithFixed(12, "Step", 5, "3D staircase",
                new Vector3Int[] {
                    // Basamak 1
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    // Basamak 2
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(4,1,1),
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1),
                    // Basamak 3
                    new Vector3Int(3,2,2), new Vector3Int(4,2,2), new Vector3Int(5,2,2), new Vector3Int(6,2,2),
                    new Vector3Int(4,3,2), new Vector3Int(5,3,2),
                    // Ek bağlantılar
                    new Vector3Int(2,1,0), new Vector3Int(4,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0), new Vector3Int(6,2,2), new Vector3Int(5,3,2) }),

            // Level 13: Çapraz zigzag (22 küp)
            CreateLevelWithFixed(13, "Zigzag", 0, "Diagonal zigzag",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(4,2,0), new Vector3Int(5,2,0), new Vector3Int(6,2,0),
                    new Vector3Int(6,3,0), new Vector3Int(7,3,0), new Vector3Int(8,3,0),
                    // Alt kalınlık
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(3,2,0), new Vector3Int(4,3,0), new Vector3Int(5,3,0),
                    new Vector3Int(7,4,0), new Vector3Int(8,4,0),
                    // 3D derinlik
                    new Vector3Int(0,0,1), new Vector3Int(4,2,1), new Vector3Int(8,4,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(0,1,0), new Vector3Int(8,3,0), new Vector3Int(8,4,1) }),

            // Level 14: Halka / Ring (20 küp)
            CreateLevelWithFixed(14, "Ring", 1, "Flat ring shape",
                new Vector3Int[] {
                    // Üst kenar
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    // Sağ kenar
                    new Vector3Int(4,1,0), new Vector3Int(4,2,0), new Vector3Int(4,3,0),
                    // Alt kenar
                    new Vector3Int(1,4,0), new Vector3Int(2,4,0), new Vector3Int(3,4,0),
                    // Sol kenar
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    // Köşeler
                    new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,4,0), new Vector3Int(4,4,0),
                    // İç derinlik
                    new Vector3Int(2,2,1),
                    new Vector3Int(1,1,0), new Vector3Int(3,1,0), new Vector3Int(1,3,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,4,0), new Vector3Int(4,4,0), new Vector3Int(2,2,1) }),

            // Level 15: T şekli 3D (24 küp)
            CreateLevelWithFixed(15, "3D T", 2, "T shape two layers",
                new Vector3Int[] {
                    // Üst çubuk z=0
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0), new Vector3Int(5,3,0),
                    // Gövde z=0
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    // Üst çubuk z=1
                    new Vector3Int(0,3,1), new Vector3Int(1,3,1), new Vector3Int(2,3,1), new Vector3Int(3,3,1), new Vector3Int(4,3,1), new Vector3Int(5,3,1),
                    // Gövde z=1
                    new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(2,2,1), new Vector3Int(3,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(5,3,0), new Vector3Int(2,0,0), new Vector3Int(3,0,1) }),

            // Level 16: 3D Bumerang (22 küp)
            CreateLevelWithFixed(16, "Boomerang", 3, "Twisted boomerang",
                new Vector3Int[] {
                    // Sol kol z=0
                    new Vector3Int(0,4,0), new Vector3Int(1,3,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(1,2,0),
                    // Merkez
                    new Vector3Int(4,2,0), new Vector3Int(4,2,1),
                    new Vector3Int(3,2,1), new Vector3Int(5,2,0),
                    // Sağ kol z=1
                    new Vector3Int(5,2,1), new Vector3Int(6,3,1), new Vector3Int(7,4,1), new Vector3Int(8,5,1),
                    new Vector3Int(6,2,1), new Vector3Int(7,3,1), new Vector3Int(8,4,1),
                    // Kalınlık
                    new Vector3Int(0,4,1), new Vector3Int(1,3,1), new Vector3Int(2,2,1),
                    new Vector3Int(7,5,1), new Vector3Int(8,5,0)
                },
                new Vector3Int[] { new Vector3Int(0,4,0), new Vector3Int(0,4,1), new Vector3Int(8,5,0), new Vector3Int(8,5,1) }),

            // Level 17: Piramit 2 kat (22 küp)
            CreateLevelWithFixed(17, "Pyramid", 4, "Stepped pyramid",
                new Vector3Int[] {
                    // Taban 5x5
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    // Orta 3x3
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    // Tepe
                    new Vector3Int(2,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,0,2), new Vector3Int(4,0,2), new Vector3Int(2,2,1) }),

            // Level 18: Yılan 3D (25 küp)
            CreateLevelWithFixed(18, "Snake", 5, "Winding snake",
                new Vector3Int[] {
                    // Kafa
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    // Gövde 1 (sağa)
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    // Dönüş yukarı
                    new Vector3Int(4,1,0), new Vector3Int(4,2,0),
                    // Gövde 2 (sola, z=1)
                    new Vector3Int(4,2,1), new Vector3Int(3,2,1), new Vector3Int(2,2,1), new Vector3Int(1,2,1),
                    // Dönüş yukarı
                    new Vector3Int(1,3,1), new Vector3Int(1,4,1),
                    // Gövde 3 (sağa, z=2)
                    new Vector3Int(1,4,2), new Vector3Int(2,4,2), new Vector3Int(3,4,2), new Vector3Int(4,4,2),
                    // Kuyruk ucu
                    new Vector3Int(5,4,2), new Vector3Int(5,5,2), new Vector3Int(5,5,3),
                    // Kalınlık
                    new Vector3Int(2,0,1), new Vector3Int(3,4,1), new Vector3Int(4,4,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(1,1,0), new Vector3Int(5,5,2), new Vector3Int(5,5,3) }),

            // Level 19: Çatal 3D (26 küp)
            CreateLevelWithFixed(19, "Fork", 0, "Branching fork",
                new Vector3Int[] {
                    // Sap
                    new Vector3Int(3,0,0), new Vector3Int(3,1,0), new Vector3Int(3,2,0), new Vector3Int(3,3,0),
                    new Vector3Int(4,0,0), new Vector3Int(4,1,0), new Vector3Int(4,2,0), new Vector3Int(4,3,0),
                    // Sol dal
                    new Vector3Int(2,3,0), new Vector3Int(1,4,0), new Vector3Int(0,5,0), new Vector3Int(0,6,0),
                    new Vector3Int(1,5,0), new Vector3Int(2,4,0),
                    // Sağ dal
                    new Vector3Int(5,3,0), new Vector3Int(6,4,0), new Vector3Int(7,5,0), new Vector3Int(7,6,0),
                    new Vector3Int(6,5,0), new Vector3Int(5,4,0),
                    // 3D derinlik
                    new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(3,3,1), new Vector3Int(4,3,1),
                    new Vector3Int(0,6,1), new Vector3Int(7,6,1)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(4,0,1), new Vector3Int(0,6,0), new Vector3Int(7,6,1) }),

            // Level 20: Asimetrik ada (28 küp)
            CreateLevelWithFixed(20, "Island", 1, "Asymmetric island",
                new Vector3Int[] {
                    // Sol kıyı (alçak)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    // Orta alan
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0), new Vector3Int(5,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0),
                    // Tepe (yüksek)
                    new Vector3Int(3,1,1), new Vector3Int(4,1,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1),
                    new Vector3Int(3,1,2), new Vector3Int(4,1,2),
                    // Sağ uzantı
                    new Vector3Int(5,0,0), new Vector3Int(6,0,0), new Vector3Int(6,1,0),
                    // Kuzey uzantı
                    new Vector3Int(1,2,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    // Plaj
                    new Vector3Int(7,0,0), new Vector3Int(5,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(7,0,0), new Vector3Int(2,3,0), new Vector3Int(4,1,2) }),

            // ============ BÖLÜM 3: YARATICI 3D (21-30) ============

            // Level 21: 3D Spiral merdiven (28 küp)
            CreateLevelWithFixed(21, "Helix", 2, "Spiral staircase",
                new Vector3Int[] {
                    // Kat 0 - güney kenar
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    // Kat 0 - doğu kenar
                    new Vector3Int(3,1,0), new Vector3Int(3,2,0),
                    // Kat 1 - kuzey kenar
                    new Vector3Int(3,3,1), new Vector3Int(2,3,1), new Vector3Int(1,3,1), new Vector3Int(0,3,1),
                    new Vector3Int(3,2,1),
                    // Kat 1 - batı kenar
                    new Vector3Int(0,2,1), new Vector3Int(0,1,1),
                    // Kat 2 - güney kenar
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2),
                    new Vector3Int(0,1,2),
                    // Kat 2 - doğu kenar
                    new Vector3Int(3,1,2), new Vector3Int(3,2,2),
                    // Kat 3 - kuzey kenar
                    new Vector3Int(3,3,3), new Vector3Int(2,3,3), new Vector3Int(1,3,3), new Vector3Int(0,3,3),
                    new Vector3Int(3,2,3),
                    // Kat 3 - batı kenar
                    new Vector3Int(0,2,3), new Vector3Int(0,1,3),
                    new Vector3Int(0,0,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0), new Vector3Int(0,0,3), new Vector3Int(0,3,3) }),

            // Level 22: Köprü yapısı (30 küp)
            CreateLevelWithFixed(22, "Bridge", 3, "Rocky bridge",
                new Vector3Int[] {
                    // Sol kaya (geniş taban, daralan)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(0,2,1),
                    new Vector3Int(0,3,0),
                    // Sağ kaya (farklı boyut - asimetrik)
                    new Vector3Int(6,0,0), new Vector3Int(7,0,0), new Vector3Int(7,0,1), new Vector3Int(7,0,2),
                    new Vector3Int(6,1,0), new Vector3Int(7,1,0), new Vector3Int(7,1,1), new Vector3Int(7,1,2),
                    new Vector3Int(7,2,0), new Vector3Int(7,2,1),
                    new Vector3Int(7,3,0),
                    // Köprü tablası (asimetrik eğri)
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(5,3,0), new Vector3Int(6,3,0),
                    // Köprü derinlik
                    new Vector3Int(3,4,1), new Vector3Int(4,4,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(7,0,2), new Vector3Int(0,3,0), new Vector3Int(7,3,0), new Vector3Int(3,4,1) }),

            // Level 23: Deniz yıldızı (30 küp)
            CreateLevelWithFixed(23, "Starfish", 4, "Asymmetric starfish",
                new Vector3Int[] {
                    // Merkez gövde (düzensiz)
                    new Vector3Int(3,3,0), new Vector3Int(4,3,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(3,3,1), new Vector3Int(4,4,1),
                    // Kol 1 - kuzey (uzun)
                    new Vector3Int(3,5,0), new Vector3Int(3,6,0), new Vector3Int(3,7,0), new Vector3Int(2,7,0),
                    // Kol 2 - kuzeydoğu (kısa, 3D)
                    new Vector3Int(5,4,0), new Vector3Int(6,5,0), new Vector3Int(6,5,1),
                    // Kol 3 - güneydoğu (orta)
                    new Vector3Int(5,3,0), new Vector3Int(6,2,0), new Vector3Int(7,1,0), new Vector3Int(7,2,0),
                    // Kol 4 - güney (uzun, eğri)
                    new Vector3Int(4,2,0), new Vector3Int(4,1,0), new Vector3Int(3,0,0), new Vector3Int(5,0,0), new Vector3Int(4,0,0),
                    // Kol 5 - batı (kalın)
                    new Vector3Int(2,3,0), new Vector3Int(1,3,0), new Vector3Int(0,3,0), new Vector3Int(0,4,0),
                    new Vector3Int(1,4,0), new Vector3Int(2,4,0),
                    // Asimetrik uzantılar
                    new Vector3Int(0,3,1), new Vector3Int(7,1,1)
                },
                new Vector3Int[] { new Vector3Int(2,7,0), new Vector3Int(7,1,0), new Vector3Int(0,4,0), new Vector3Int(6,5,1), new Vector3Int(3,0,0) }),

            // Level 24: Hortum / Tornado (32 küp)
            CreateLevelWithFixed(24, "Tornado", 5, "Rising spinning tornado",
                new Vector3Int[] {
                    // Taban (geniş, düzensiz)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    // Bel (dönerek daralıyor)
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(2,1,2), new Vector3Int(3,1,2),
                    // Orta
                    new Vector3Int(2,2,2), new Vector3Int(3,2,2),
                    new Vector3Int(3,2,3),
                    // Üst (genişleyerek, yana kayarak)
                    new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(4,3,4),
                    new Vector3Int(4,4,4), new Vector3Int(5,4,4), new Vector3Int(5,4,5),
                    new Vector3Int(5,5,5), new Vector3Int(6,5,5), new Vector3Int(6,5,6),
                    new Vector3Int(6,6,6)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,0,2), new Vector3Int(6,6,6) }),

            // Level 25: Kristal oluşumu (35 küp)
            CreateLevelWithFixed(25, "Crystal", 0, "Natural crystal cluster",
                new Vector3Int[] {
                    // Ana kristal (uzun dikey, eğik)
                    new Vector3Int(3,0,2), new Vector3Int(3,1,2), new Vector3Int(3,2,2), new Vector3Int(3,3,2),
                    new Vector3Int(3,4,2), new Vector3Int(3,5,2), new Vector3Int(3,6,2),
                    new Vector3Int(4,0,2), new Vector3Int(4,1,2), new Vector3Int(4,2,2),
                    // Sol kristal (kısa, eğri)
                    new Vector3Int(1,0,1), new Vector3Int(1,1,1), new Vector3Int(1,2,1), new Vector3Int(1,3,1),
                    new Vector3Int(0,0,1), new Vector3Int(0,1,0),
                    new Vector3Int(2,0,1), new Vector3Int(2,1,1),
                    // Sağ kristal (orta, farklı açı)
                    new Vector3Int(5,0,3), new Vector3Int(5,1,3), new Vector3Int(5,2,3), new Vector3Int(5,3,4),
                    new Vector3Int(5,4,4), new Vector3Int(6,0,3), new Vector3Int(6,1,4),
                    // Ön küçük kristal
                    new Vector3Int(3,0,0), new Vector3Int(3,1,0), new Vector3Int(3,2,0),
                    new Vector3Int(4,0,0),
                    // Taban kayası
                    new Vector3Int(2,0,2), new Vector3Int(4,0,3), new Vector3Int(2,0,3),
                    new Vector3Int(1,0,2), new Vector3Int(5,0,2), new Vector3Int(4,0,1)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(3,6,2), new Vector3Int(5,4,4), new Vector3Int(6,1,4), new Vector3Int(3,2,0) }),

            // Level 26: Kaktüs (34 küp)
            CreateLevelWithFixed(26, "Cactus", 1, "Armed cactus",
                new Vector3Int[] {
                    // Ana gövde (dikey)
                    new Vector3Int(3,0,1), new Vector3Int(3,1,1), new Vector3Int(3,2,1), new Vector3Int(3,3,1),
                    new Vector3Int(3,4,1), new Vector3Int(3,5,1), new Vector3Int(3,6,1), new Vector3Int(3,7,1),
                    new Vector3Int(4,0,1), new Vector3Int(4,1,1), new Vector3Int(4,2,1), new Vector3Int(4,3,1),
                    // Sol kol (yukarı kıvrılıyor)
                    new Vector3Int(2,3,1), new Vector3Int(1,3,1), new Vector3Int(0,3,1),
                    new Vector3Int(0,4,1), new Vector3Int(0,5,1), new Vector3Int(0,6,1),
                    new Vector3Int(1,6,1),
                    // Sağ kol (farklı yükseklik, 3D)
                    new Vector3Int(5,4,1), new Vector3Int(6,4,1), new Vector3Int(7,4,1),
                    new Vector3Int(7,5,1), new Vector3Int(7,5,2), new Vector3Int(7,6,2),
                    new Vector3Int(7,7,2),
                    // Saksı
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    new Vector3Int(5,0,1)
                },
                new Vector3Int[] { new Vector3Int(2,0,0), new Vector3Int(5,0,2), new Vector3Int(3,7,1), new Vector3Int(1,6,1), new Vector3Int(7,7,2) }),

            // Level 27: Uçan ada (36 küp)
            CreateLevelWithFixed(27, "Floating Island", 2, "Floating island",
                new Vector3Int[] {
                    // Ana ada (düzensiz platform)
                    new Vector3Int(1,3,1), new Vector3Int(2,3,1), new Vector3Int(3,3,1), new Vector3Int(4,3,1), new Vector3Int(5,3,1),
                    new Vector3Int(1,3,2), new Vector3Int(2,3,2), new Vector3Int(3,3,2), new Vector3Int(4,3,2), new Vector3Int(5,3,2), new Vector3Int(6,3,2),
                    new Vector3Int(2,3,3), new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(5,3,3),
                    // Tepe (asimetrik dağ)
                    new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(3,5,2), new Vector3Int(4,4,3),
                    new Vector3Int(3,6,2),
                    // Alt (damlayan kayalar)
                    new Vector3Int(2,2,2), new Vector3Int(3,2,2), new Vector3Int(4,2,2),
                    new Vector3Int(3,1,2), new Vector3Int(3,0,2),
                    new Vector3Int(4,2,3),
                    // Ek platformlar
                    new Vector3Int(0,3,1), new Vector3Int(7,3,2),
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    // Küçük uçan kaya
                    new Vector3Int(7,4,4), new Vector3Int(8,4,4),
                    new Vector3Int(0,5,0), new Vector3Int(0,5,1),
                    new Vector3Int(1,3,3), new Vector3Int(6,3,3)
                },
                new Vector3Int[] { new Vector3Int(0,3,1), new Vector3Int(8,4,4), new Vector3Int(3,6,2), new Vector3Int(3,0,2), new Vector3Int(0,5,0) }),

            // Level 28: Dalgakıran (38 küp)
            CreateLevelWithFixed(28, "Breakwater", 3, "Coastal breakwater",
                new Vector3Int[] {
                    // Dalgalı su yüzeyi (z=0)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,1,0), new Vector3Int(3,0,0), new Vector3Int(4,1,0),
                    new Vector3Int(5,0,0), new Vector3Int(6,1,0), new Vector3Int(7,0,0), new Vector3Int(8,1,0), new Vector3Int(9,0,0),
                    // İkinci dalga (z=1, y yükseliyor)
                    new Vector3Int(0,1,1), new Vector3Int(1,1,1), new Vector3Int(2,2,1), new Vector3Int(3,1,1), new Vector3Int(4,2,1),
                    new Vector3Int(5,1,1), new Vector3Int(6,2,1), new Vector3Int(7,1,1),
                    // Kıran duvar
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2), new Vector3Int(2,3,2), new Vector3Int(3,2,2),
                    new Vector3Int(4,3,2), new Vector3Int(5,2,2), new Vector3Int(6,3,2),
                    // Kıran üst
                    new Vector3Int(0,3,2), new Vector3Int(1,3,2), new Vector3Int(2,4,2),
                    // Bağlantılar
                    new Vector3Int(0,1,0), new Vector3Int(0,2,1), new Vector3Int(9,1,0),
                    new Vector3Int(8,2,1), new Vector3Int(9,1,1),
                    // Köpük
                    new Vector3Int(3,3,2), new Vector3Int(5,3,2),
                    new Vector3Int(1,2,1), new Vector3Int(7,2,1),
                    new Vector3Int(2,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(9,0,0), new Vector3Int(0,3,2), new Vector3Int(2,4,2) }),

            // Level 29: Mantar (35 küp)
            CreateLevelWithFixed(29, "Mushroom", 4, "Giant mushroom",
                new Vector3Int[] {
                    // Sap (ince, eğri)
                    new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(3,0,3), new Vector3Int(4,0,3),
                    new Vector3Int(3,1,2), new Vector3Int(4,1,2), new Vector3Int(3,1,3),
                    new Vector3Int(3,2,2), new Vector3Int(4,2,2),
                    new Vector3Int(3,3,2), new Vector3Int(3,3,3),
                    // Şapka (geniş, düzensiz)
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0), new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0), new Vector3Int(5,4,0), new Vector3Int(6,4,0),
                    new Vector3Int(0,4,1), new Vector3Int(1,4,1), new Vector3Int(2,4,1), new Vector3Int(3,4,1), new Vector3Int(4,4,1), new Vector3Int(5,4,1), new Vector3Int(6,4,1),
                    new Vector3Int(1,4,2), new Vector3Int(2,4,2), new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(5,4,2),
                    new Vector3Int(2,4,3), new Vector3Int(3,4,3), new Vector3Int(4,4,3),
                    // Üst tepe
                    new Vector3Int(3,5,1), new Vector3Int(3,5,2), new Vector3Int(4,5,1)
                },
                new Vector3Int[] { new Vector3Int(3,0,2), new Vector3Int(4,0,3), new Vector3Int(0,4,0), new Vector3Int(6,4,0), new Vector3Int(3,5,2) }),

            // Level 30: DNA sarmalı (40 küp)
            CreateLevelWithFixed(30, "DNA", 5, "Double helix DNA",
                new Vector3Int[] {
                    // Sarmal 1
                    new Vector3Int(0,0,2), new Vector3Int(1,0,3), new Vector3Int(2,0,4),
                    new Vector3Int(2,1,4), new Vector3Int(1,1,4),
                    new Vector3Int(0,2,4), new Vector3Int(0,2,3),
                    new Vector3Int(0,3,2), new Vector3Int(1,3,1), new Vector3Int(2,3,0),
                    new Vector3Int(2,4,0), new Vector3Int(1,4,0),
                    new Vector3Int(0,5,0), new Vector3Int(0,5,1),
                    new Vector3Int(0,6,2), new Vector3Int(1,6,3), new Vector3Int(2,6,4),
                    new Vector3Int(2,7,4), new Vector3Int(1,7,3),
                    new Vector3Int(0,8,2),
                    // Sarmal 2
                    new Vector3Int(2,0,2), new Vector3Int(1,0,1), new Vector3Int(0,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(2,2,1),
                    new Vector3Int(2,3,2), new Vector3Int(1,3,3), new Vector3Int(0,3,4),
                    new Vector3Int(0,4,4), new Vector3Int(1,4,4),
                    new Vector3Int(2,5,4), new Vector3Int(2,5,3),
                    new Vector3Int(2,6,2), new Vector3Int(1,6,1), new Vector3Int(0,6,0),
                    new Vector3Int(0,7,0),
                    // Bağ köprüleri
                    new Vector3Int(1,0,2), new Vector3Int(1,3,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,4), new Vector3Int(0,8,2), new Vector3Int(0,7,0) }),

            // ============ BÖLÜM 4: KARMAŞIK ASİMETRİK (31-40) ============

            // Level 31: Rüzgargülü 3D (42 küp)
            CreateLevelWithFixed(31, "Pinwheel", 0, "Spinning blades",
                new Vector3Int[] {
                    // Merkez mili
                    new Vector3Int(4,0,4), new Vector3Int(4,1,4), new Vector3Int(4,2,4), new Vector3Int(4,3,4), new Vector3Int(4,4,4),
                    new Vector3Int(5,2,4), new Vector3Int(3,2,4),
                    // Kanat 1 - kuzeydoğu (z=0'a doğru)
                    new Vector3Int(5,3,3), new Vector3Int(6,4,2), new Vector3Int(7,5,1), new Vector3Int(8,6,0),
                    new Vector3Int(6,3,3), new Vector3Int(7,4,2), new Vector3Int(8,5,1),
                    // Kanat 2 - güneybatı (z=8'e doğru)
                    new Vector3Int(3,1,5), new Vector3Int(2,0,6), new Vector3Int(1,0,7),
                    new Vector3Int(2,1,5), new Vector3Int(1,0,6), new Vector3Int(0,0,7),
                    // Kanat 3 - doğu (düz, farklı yükseklik)
                    new Vector3Int(5,2,5), new Vector3Int(6,2,6), new Vector3Int(7,2,7), new Vector3Int(8,2,8),
                    new Vector3Int(6,3,6), new Vector3Int(7,3,7), new Vector3Int(8,3,8),
                    new Vector3Int(5,3,5),
                    // Kanat 4 - batı (kısa, kalın)
                    new Vector3Int(3,2,3), new Vector3Int(2,2,2), new Vector3Int(1,2,1), new Vector3Int(0,2,0),
                    new Vector3Int(2,1,2), new Vector3Int(1,1,1), new Vector3Int(0,1,0),
                    new Vector3Int(3,1,3),
                    // Ek asimetri
                    new Vector3Int(4,5,4), new Vector3Int(4,2,5), new Vector3Int(4,2,3),
                    new Vector3Int(9,6,0), new Vector3Int(0,0,8)
                },
                new Vector3Int[] { new Vector3Int(8,6,0), new Vector3Int(0,0,8), new Vector3Int(8,3,8), new Vector3Int(0,1,0), new Vector3Int(9,6,0), new Vector3Int(4,5,4) }),

            // Level 32: Kale kalıntısı (45 küp)
            CreateLevelWithFixed(32, "Ruins", 1, "Ruined castle",
                new Vector3Int[] {
                    // Sol kule (yüksek, sağlam)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(0,2,1),
                    new Vector3Int(0,3,0), new Vector3Int(0,3,1),
                    new Vector3Int(0,4,0),
                    // Yıkık duvar (alçalan, boşluklu)
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(3,2,0),
                    // Sağ kule (kısa, yıkık)
                    new Vector3Int(6,0,0), new Vector3Int(7,0,0), new Vector3Int(6,0,1), new Vector3Int(7,0,1),
                    new Vector3Int(6,1,0), new Vector3Int(7,1,1),
                    new Vector3Int(7,2,0),
                    // Arka duvar (kısmen ayakta)
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3),
                    new Vector3Int(0,1,3), new Vector3Int(1,1,3),
                    new Vector3Int(0,2,3),
                    // Yıkıntı enkaz
                    new Vector3Int(5,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,2),
                    new Vector3Int(6,0,2), new Vector3Int(7,0,2),
                    new Vector3Int(2,0,2), new Vector3Int(5,0,2),
                    // Yere düşmüş taşlar
                    new Vector3Int(5,1,0), new Vector3Int(3,0,2), new Vector3Int(7,0,3)
                },
                new Vector3Int[] { new Vector3Int(0,4,0), new Vector3Int(7,0,0), new Vector3Int(0,2,3), new Vector3Int(7,0,3) }),

            // Level 33: Uçak silüeti (44 küp)
            CreateLevelWithFixed(33, "Airplane", 2, "3D airplane",
                new Vector3Int[] {
                    // Gövde (uzun, silindirik)
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2), new Vector3Int(2,2,2), new Vector3Int(3,2,2),
                    new Vector3Int(4,2,2), new Vector3Int(5,2,2), new Vector3Int(6,2,2), new Vector3Int(7,2,2),
                    new Vector3Int(8,2,2), new Vector3Int(9,2,2),
                    new Vector3Int(1,3,2), new Vector3Int(2,3,2), new Vector3Int(3,3,2), new Vector3Int(4,3,2),
                    // Burun
                    new Vector3Int(10,2,2), new Vector3Int(11,2,2),
                    // Sol kanat (geniş, asimetrik sweep)
                    new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(3,2,1), new Vector3Int(4,2,1), new Vector3Int(5,2,1),
                    new Vector3Int(2,2,0), new Vector3Int(2,2,1),
                    // Sağ kanat (biraz farklı)
                    new Vector3Int(3,2,3), new Vector3Int(4,2,3), new Vector3Int(3,2,4), new Vector3Int(4,2,4), new Vector3Int(5,2,3),
                    new Vector3Int(2,2,4), new Vector3Int(2,2,3),
                    // Kuyruk dikey
                    new Vector3Int(0,3,2), new Vector3Int(0,4,2), new Vector3Int(0,5,2), new Vector3Int(1,4,2),
                    // Kuyruk yatay
                    new Vector3Int(0,3,1), new Vector3Int(0,3,3), new Vector3Int(0,2,1), new Vector3Int(0,2,3),
                    new Vector3Int(1,2,1), new Vector3Int(1,2,3),
                    // Motor
                    new Vector3Int(5,1,2), new Vector3Int(6,1,2),
                    new Vector3Int(5,3,2)
                },
                new Vector3Int[] { new Vector3Int(11,2,2), new Vector3Int(0,5,2), new Vector3Int(2,2,0), new Vector3Int(2,2,4), new Vector3Int(0,2,1) }),

            // Level 34: Derviş / Hortum (42 küp)
            CreateLevelWithFixed(34, "Vortex", 3, "Rising spinning vortex",
                new Vector3Int[] {
                    // Taban katman (düzensiz geniş)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    // Dönerek daralan 1
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(4,1,1),
                    // Dönerek daralan 2 (kayarak)
                    new Vector3Int(3,2,1), new Vector3Int(4,2,1), new Vector3Int(4,2,2),
                    new Vector3Int(3,2,2), new Vector3Int(5,2,1),
                    // Dönerek daralan 3
                    new Vector3Int(5,3,2), new Vector3Int(5,3,3), new Vector3Int(4,3,3),
                    new Vector3Int(5,4,3),
                    // Tepe (ince, sallanan)
                    new Vector3Int(5,4,4), new Vector3Int(6,5,4), new Vector3Int(6,5,5),
                    new Vector3Int(6,6,5), new Vector3Int(7,6,5),
                    new Vector3Int(7,7,6), new Vector3Int(7,7,5),
                    // Asimetrik kopuş
                    new Vector3Int(0,0,2), new Vector3Int(5,0,2), new Vector3Int(6,6,6)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,0,0), new Vector3Int(0,0,2), new Vector3Int(7,7,6), new Vector3Int(6,6,6) }),

            // Level 35: İçi boş küre benzeri (25 küp)
            CreateLevelWithFixed(35, "Sphere", 4, "Asymmetric hollow sphere",
                GenerateAsymmetricSphere(3),
                new Vector3Int[] {
                    new Vector3Int(0,1,1), new Vector3Int(6,1,1), new Vector3Int(3,0,1),
                    new Vector3Int(3,5,1), new Vector3Int(3,1,0)
                }),

            // Level 36: Ağaç (50 küp)
            CreateLevelWithFixed(36, "Tree", 5, "Branching tree",
                new Vector3Int[] {
                    // Kök
                    new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(3,0,2), new Vector3Int(4,0,4),
                    new Vector3Int(5,0,3), new Vector3Int(2,0,3),
                    // Gövde
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3),
                    new Vector3Int(3,2,3), new Vector3Int(4,2,3),
                    new Vector3Int(3,3,3), new Vector3Int(4,3,3),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3),
                    new Vector3Int(3,5,3),
                    // Sol alt dal
                    new Vector3Int(2,4,3), new Vector3Int(1,5,3), new Vector3Int(0,6,3), new Vector3Int(0,7,3),
                    new Vector3Int(1,5,2), new Vector3Int(0,6,2),
                    // Sağ alt dal (farklı açı)
                    new Vector3Int(5,4,3), new Vector3Int(6,5,4), new Vector3Int(7,6,4), new Vector3Int(8,7,4),
                    new Vector3Int(6,5,3), new Vector3Int(7,6,3),
                    // Sol üst dal
                    new Vector3Int(2,5,2), new Vector3Int(1,6,1), new Vector3Int(0,7,0), new Vector3Int(0,7,1),
                    new Vector3Int(1,6,2),
                    // Sağ üst dal
                    new Vector3Int(4,5,4), new Vector3Int(5,6,5), new Vector3Int(6,7,6), new Vector3Int(6,7,5),
                    new Vector3Int(5,6,4),
                    // Yaprak kümeleri (düzensiz)
                    new Vector3Int(0,8,3), new Vector3Int(0,8,2),
                    new Vector3Int(8,8,4), new Vector3Int(7,7,4),
                    new Vector3Int(0,8,0), new Vector3Int(0,8,1),
                    new Vector3Int(7,8,6), new Vector3Int(6,8,5), new Vector3Int(7,8,5),
                    // Tepe yaprak
                    new Vector3Int(3,6,3), new Vector3Int(2,6,3), new Vector3Int(4,6,3),
                    new Vector3Int(3,7,3)
                },
                new Vector3Int[] { new Vector3Int(2,0,3), new Vector3Int(5,0,3), new Vector3Int(0,8,0), new Vector3Int(8,8,4), new Vector3Int(3,7,3), new Vector3Int(7,8,6) }),

            // Level 37: Galaksi kolu (52 küp)
            CreateLevelWithFixed(37, "Galaxy", 0, "Spiral galaxy arm",
                new Vector3Int[] {
                    // Merkez (yoğun)
                    new Vector3Int(5,2,5), new Vector3Int(5,3,5), new Vector3Int(6,2,5), new Vector3Int(6,3,5),
                    new Vector3Int(5,2,6), new Vector3Int(6,3,6), new Vector3Int(5,3,6),
                    // Spiral kol 1 (saat yönü, yukarı çıkan)
                    new Vector3Int(7,3,5), new Vector3Int(8,4,5), new Vector3Int(9,4,4), new Vector3Int(10,5,3),
                    new Vector3Int(10,5,2), new Vector3Int(9,5,1), new Vector3Int(8,5,0),
                    new Vector3Int(7,4,5), new Vector3Int(9,5,4), new Vector3Int(10,5,4),
                    // Spiral kol 2 (ters yön, aşağı inen)
                    new Vector3Int(4,2,5), new Vector3Int(3,1,5), new Vector3Int(2,0,6), new Vector3Int(1,0,7),
                    new Vector3Int(1,0,8), new Vector3Int(2,0,9), new Vector3Int(3,1,10),
                    new Vector3Int(3,1,6), new Vector3Int(2,0,7), new Vector3Int(2,1,9),
                    // Küçük kol
                    new Vector3Int(5,4,6), new Vector3Int(5,5,7), new Vector3Int(4,5,8), new Vector3Int(4,6,8),
                    new Vector3Int(3,6,9), new Vector3Int(5,5,8),
                    // Karşı kol
                    new Vector3Int(6,1,4), new Vector3Int(7,0,3), new Vector3Int(7,0,2),
                    new Vector3Int(8,0,1), new Vector3Int(6,0,3),
                    // Yıldız parçacıkları (dağınık)
                    new Vector3Int(0,0,10), new Vector3Int(11,5,0),
                    new Vector3Int(4,7,9), new Vector3Int(9,0,0),
                    new Vector3Int(0,1,8), new Vector3Int(11,6,2),
                    new Vector3Int(4,3,4), new Vector3Int(7,2,6),
                    new Vector3Int(6,2,4), new Vector3Int(5,1,5),
                    new Vector3Int(8,4,4), new Vector3Int(3,2,7)
                },
                new Vector3Int[] { new Vector3Int(0,0,10), new Vector3Int(11,6,2), new Vector3Int(8,5,0), new Vector3Int(3,6,9), new Vector3Int(9,0,0), new Vector3Int(4,7,9) }),

            // Level 38: Şelale (48 küp)
            CreateLevelWithFixed(38, "Waterfall", 1, "3D waterfall",
                new Vector3Int[] {
                    // Üst kayalık (geniş, düzensiz)
                    new Vector3Int(0,5,0), new Vector3Int(1,5,0), new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    new Vector3Int(0,5,1), new Vector3Int(1,5,1), new Vector3Int(2,5,1), new Vector3Int(3,5,1), new Vector3Int(4,5,1),
                    new Vector3Int(1,5,2), new Vector3Int(2,5,2), new Vector3Int(3,5,2),
                    new Vector3Int(0,6,0), new Vector3Int(1,6,0), new Vector3Int(0,6,1),
                    // Su düşüşü (dikey, eğri)
                    new Vector3Int(2,4,1), new Vector3Int(2,3,1), new Vector3Int(3,3,1),
                    new Vector3Int(3,2,2), new Vector3Int(3,1,2), new Vector3Int(3,0,2),
                    new Vector3Int(2,4,2), new Vector3Int(3,2,1),
                    new Vector3Int(4,1,2), new Vector3Int(4,0,3),
                    // Alt havuz (geniş, sığ)
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(2,0,3), new Vector3Int(3,0,3),
                    new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(5,0,3),
                    new Vector3Int(1,0,3), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4),
                    new Vector3Int(6,0,3), new Vector3Int(5,0,4),
                    // Su sıçraması
                    new Vector3Int(1,1,3), new Vector3Int(5,1,3), new Vector3Int(3,1,4),
                    // Yosun kayaları
                    new Vector3Int(0,0,4), new Vector3Int(6,0,4),
                    new Vector3Int(0,0,2), new Vector3Int(0,0,3),
                    new Vector3Int(7,0,3), new Vector3Int(7,0,4)
                },
                new Vector3Int[] { new Vector3Int(0,6,0), new Vector3Int(4,5,1), new Vector3Int(0,0,2), new Vector3Int(7,0,4), new Vector3Int(0,0,4) }),

            // Level 39: Uzay istasyonu (55 küp)
            CreateLevelWithFixed(39, "Station", 2, "Space station",
                new Vector3Int[] {
                    // Ana modül (merkez)
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3),
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3),
                    new Vector3Int(4,2,4), new Vector3Int(5,2,4), new Vector3Int(6,2,4),
                    new Vector3Int(4,3,4), new Vector3Int(5,3,4), new Vector3Int(6,3,4),
                    // Sol güneş paneli (ince, geniş)
                    new Vector3Int(0,2,3), new Vector3Int(1,2,3), new Vector3Int(2,2,3), new Vector3Int(3,2,3),
                    new Vector3Int(0,2,4), new Vector3Int(1,2,4), new Vector3Int(2,2,4), new Vector3Int(3,2,4),
                    new Vector3Int(0,2,5), new Vector3Int(1,2,5),
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2),
                    // Sağ güneş paneli (farklı açı)
                    new Vector3Int(7,3,3), new Vector3Int(8,3,3), new Vector3Int(9,3,3), new Vector3Int(10,3,3),
                    new Vector3Int(7,3,4), new Vector3Int(8,3,4), new Vector3Int(9,3,4), new Vector3Int(10,3,4),
                    new Vector3Int(9,3,5), new Vector3Int(10,3,5),
                    new Vector3Int(9,3,2), new Vector3Int(10,3,2),
                    // Üst kule
                    new Vector3Int(5,4,3), new Vector3Int(5,5,3), new Vector3Int(5,6,3),
                    new Vector3Int(5,4,4), new Vector3Int(5,5,4),
                    new Vector3Int(5,7,3),
                    // Alt laboratuvar
                    new Vector3Int(5,1,3), new Vector3Int(5,0,3), new Vector3Int(4,1,3),
                    new Vector3Int(6,1,4), new Vector3Int(5,0,4),
                    // Dok portu (asimetrik uzantı)
                    new Vector3Int(5,2,5), new Vector3Int(5,2,6), new Vector3Int(5,2,7),
                    new Vector3Int(5,3,6),
                    // Anten
                    new Vector3Int(5,8,3), new Vector3Int(4,7,4), new Vector3Int(6,7,2)
                },
                new Vector3Int[] { new Vector3Int(0,2,2), new Vector3Int(10,3,5), new Vector3Int(5,8,3), new Vector3Int(5,0,3), new Vector3Int(5,2,7), new Vector3Int(0,2,5) }),

            // Level 40: Ejderha kafası (58 küp)
            CreateLevelWithFixed(40, "Dragon", 3, "Dragon head",
                new Vector3Int[] {
                    // Alt çene
                    new Vector3Int(6,0,2), new Vector3Int(7,0,2), new Vector3Int(8,0,2), new Vector3Int(9,0,2), new Vector3Int(10,0,2),
                    new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,0,3),
                    new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    // Kafa (ana kütle)
                    new Vector3Int(4,1,2), new Vector3Int(5,1,2), new Vector3Int(6,1,2), new Vector3Int(7,1,2),
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3), new Vector3Int(7,1,3),
                    new Vector3Int(5,1,4), new Vector3Int(6,1,4),
                    // Üst kafa
                    new Vector3Int(4,2,2), new Vector3Int(5,2,2), new Vector3Int(6,2,2), new Vector3Int(7,2,2),
                    new Vector3Int(5,2,3), new Vector3Int(6,2,3), new Vector3Int(7,2,3),
                    new Vector3Int(5,2,4), new Vector3Int(6,2,4),
                    // Boynuz sol
                    new Vector3Int(4,3,2), new Vector3Int(3,4,1), new Vector3Int(2,5,0), new Vector3Int(2,6,0),
                    new Vector3Int(3,4,2), new Vector3Int(3,5,1),
                    // Boynuz sağ (daha kısa, asimetrik)
                    new Vector3Int(7,3,3), new Vector3Int(8,4,4), new Vector3Int(9,5,4),
                    new Vector3Int(7,3,4), new Vector3Int(8,4,3),
                    // Göz çukuru sol
                    new Vector3Int(5,3,2),
                    // Göz çukuru sağ
                    new Vector3Int(6,3,3),
                    // Burun
                    new Vector3Int(8,1,2), new Vector3Int(9,1,2), new Vector3Int(10,1,2),
                    new Vector3Int(11,0,2),
                    // Boyun
                    new Vector3Int(3,1,2), new Vector3Int(2,1,3), new Vector3Int(1,1,3), new Vector3Int(0,1,3),
                    new Vector3Int(3,1,3), new Vector3Int(2,1,2), new Vector3Int(1,1,2),
                    new Vector3Int(0,0,3), new Vector3Int(0,0,4)
                },
                new Vector3Int[] { new Vector3Int(0,0,3), new Vector3Int(11,0,2), new Vector3Int(2,6,0), new Vector3Int(9,5,4), new Vector3Int(0,0,4), new Vector3Int(10,0,2) }),

            // ============ BÖLÜM 5: EPİK (41-50) ============

            // Level 41: Dev piramit (60 küp)
            CreateLevelWithFixed(41, "Ziggurat", 4, "Mesopotamian ziggurat",
                new Vector3Int[] {
                    // Taban katı (7x7 ama düzensiz kenarlar)
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1), new Vector3Int(6,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4),
                    // 2. kat (5x5, kaydırılmış)
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(4,1,1), new Vector3Int(5,1,1),
                    new Vector3Int(1,1,2), new Vector3Int(2,1,2), new Vector3Int(3,1,2), new Vector3Int(4,1,2), new Vector3Int(5,1,2),
                    new Vector3Int(1,1,3), new Vector3Int(2,1,3), new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3),
                    // 3. kat (3x3, kaydırılmış)
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1),
                    new Vector3Int(2,2,2), new Vector3Int(3,2,2), new Vector3Int(4,2,2),
                    new Vector3Int(2,2,3), new Vector3Int(3,2,3), new Vector3Int(4,2,3),
                    // Tapınak tepesi (asimetrik)
                    new Vector3Int(3,3,2), new Vector3Int(3,3,3),
                    new Vector3Int(3,4,2), new Vector3Int(4,3,2),
                    // Rampa
                    new Vector3Int(6,0,4), new Vector3Int(7,0,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,1), new Vector3Int(6,0,1), new Vector3Int(7,0,3), new Vector3Int(3,4,2), new Vector3Int(0,0,3), new Vector3Int(6,0,3) }),

            // Level 42: Dev yılan (65 küp)
            CreateLevelWithFixed(42, "Cobra", 5, "Giant winding cobra",
                new Vector3Int[] {
                    // Kuyruk (ince)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    // Gövde 1 (kalınlaşıyor, sağa)
                    new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    // Dönüş yukarı
                    new Vector3Int(5,1,0), new Vector3Int(5,1,1), new Vector3Int(5,2,0), new Vector3Int(5,2,1),
                    // Gövde 2 (sola, z=2)
                    new Vector3Int(5,2,2), new Vector3Int(4,2,2), new Vector3Int(3,2,2), new Vector3Int(2,2,2),
                    new Vector3Int(4,3,2), new Vector3Int(3,3,2), new Vector3Int(2,3,2),
                    new Vector3Int(4,2,3), new Vector3Int(3,2,3),
                    // Dönüş tekrar
                    new Vector3Int(2,3,3), new Vector3Int(2,4,3), new Vector3Int(2,4,2),
                    // Gövde 3 (sağa, üst)
                    new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(5,4,2), new Vector3Int(6,4,2),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3),
                    new Vector3Int(3,5,2), new Vector3Int(4,5,2), new Vector3Int(5,5,2),
                    // Boyun (yükseliyor)
                    new Vector3Int(6,5,2), new Vector3Int(6,5,3), new Vector3Int(6,6,2), new Vector3Int(6,6,3),
                    new Vector3Int(7,6,2), new Vector3Int(7,7,2), new Vector3Int(7,7,3),
                    // Kafa (genişliyor, hood)
                    new Vector3Int(7,8,1), new Vector3Int(7,8,2), new Vector3Int(7,8,3), new Vector3Int(7,8,4),
                    new Vector3Int(6,8,1), new Vector3Int(6,8,2), new Vector3Int(6,8,3), new Vector3Int(6,8,4),
                    new Vector3Int(8,8,2), new Vector3Int(8,8,3),
                    new Vector3Int(7,9,2), new Vector3Int(7,9,3),
                    // Dil
                    new Vector3Int(8,7,2), new Vector3Int(9,7,2), new Vector3Int(9,7,3),
                    // Göz (asimetrik)
                    new Vector3Int(7,9,1), new Vector3Int(7,9,4),
                    // Ek kalınlık
                    new Vector3Int(6,7,2), new Vector3Int(6,7,3), new Vector3Int(5,5,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(7,9,1), new Vector3Int(7,9,4), new Vector3Int(9,7,2), new Vector3Int(9,7,3) }),

            // Level 43: Asimetrik kale (70 küp)
            CreateLevelWithFixed(43, "Castle", 0, "Medieval castle",
                new Vector3Int[] {
                    // Sol kule (yüksek)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(0,2,1), new Vector3Int(1,2,1),
                    new Vector3Int(0,3,0), new Vector3Int(0,3,1), new Vector3Int(1,3,0),
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0),
                    // Ön duvar (kısmen yıkık)
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(5,1,0), new Vector3Int(6,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(6,2,0),
                    // Sağ kule (kısa, geniş)
                    new Vector3Int(7,0,0), new Vector3Int(8,0,0), new Vector3Int(7,0,1), new Vector3Int(8,0,1), new Vector3Int(7,0,2), new Vector3Int(8,0,2),
                    new Vector3Int(7,1,0), new Vector3Int(8,1,0), new Vector3Int(7,1,1), new Vector3Int(8,1,1), new Vector3Int(7,1,2), new Vector3Int(8,1,2),
                    new Vector3Int(7,2,0), new Vector3Int(8,2,1), new Vector3Int(7,2,2),
                    // Arka duvar
                    new Vector3Int(0,0,4), new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4),
                    new Vector3Int(0,1,4), new Vector3Int(1,1,4), new Vector3Int(2,1,4),
                    new Vector3Int(0,2,4), new Vector3Int(1,2,4),
                    // İç avlu
                    new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    // Hendek
                    new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    // Bayrak direği
                    new Vector3Int(0,5,0), new Vector3Int(8,2,0),
                    new Vector3Int(8,3,0)
                },
                new Vector3Int[] { new Vector3Int(0,5,0), new Vector3Int(8,3,0), new Vector3Int(0,0,4), new Vector3Int(8,0,4), new Vector3Int(0,2,4), new Vector3Int(8,0,2) }),

            // Level 44: Çift sarmal köprü (72 küp)
            CreateLevelWithFixed(44, "Spiral Bridge", 1, "Double-twisted bridge",
                new Vector3Int[] {
                    // Sol platform
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(0,0,3), new Vector3Int(1,0,3),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(0,1,2), new Vector3Int(1,1,2),
                    // Üst sarmal
                    new Vector3Int(2,1,0), new Vector3Int(3,2,0), new Vector3Int(4,2,1), new Vector3Int(5,3,1),
                    new Vector3Int(6,3,2), new Vector3Int(7,4,2), new Vector3Int(8,4,3), new Vector3Int(9,5,3),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,1), new Vector3Int(4,3,1), new Vector3Int(5,3,2),
                    new Vector3Int(6,4,2), new Vector3Int(7,4,3), new Vector3Int(8,5,3),
                    // Alt sarmal
                    new Vector3Int(2,0,3), new Vector3Int(3,0,4), new Vector3Int(4,1,4), new Vector3Int(5,1,5),
                    new Vector3Int(6,2,5), new Vector3Int(7,2,6), new Vector3Int(8,3,6), new Vector3Int(9,3,7),
                    new Vector3Int(2,0,4), new Vector3Int(3,1,4), new Vector3Int(4,1,5), new Vector3Int(5,2,5),
                    new Vector3Int(6,2,6), new Vector3Int(7,3,6), new Vector3Int(8,3,7),
                    // Sağ platform (farklı yükseklik)
                    new Vector3Int(10,5,3), new Vector3Int(11,5,3), new Vector3Int(10,5,4), new Vector3Int(11,5,4),
                    new Vector3Int(10,5,5), new Vector3Int(11,5,5), new Vector3Int(10,5,6), new Vector3Int(11,5,6), new Vector3Int(10,5,7), new Vector3Int(11,5,7),
                    new Vector3Int(10,4,3), new Vector3Int(11,4,3), new Vector3Int(10,4,7), new Vector3Int(11,4,7),
                    // Bağlantı köprüsü
                    new Vector3Int(9,4,4), new Vector3Int(9,4,5), new Vector3Int(9,4,6),
                    new Vector3Int(10,3,5), new Vector3Int(10,3,6),
                    // Asimetrik detail
                    new Vector3Int(0,2,0), new Vector3Int(11,6,7), new Vector3Int(11,6,3),
                    new Vector3Int(5,2,3), new Vector3Int(5,2,4)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(11,6,7), new Vector3Int(0,0,3), new Vector3Int(11,6,3), new Vector3Int(0,0,0), new Vector3Int(11,4,3) }),

            // Level 45: Volkan (75 küp)
            CreateLevelWithFixed(45, "Volcano", 2, "Erupting volcano",
                new Vector3Int[] {
                    // Taban (geniş, asimetrik)
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3),
                    new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4),
                    new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    // 2. kat (daralan)
                    new Vector3Int(2,1,2), new Vector3Int(3,1,2), new Vector3Int(4,1,2), new Vector3Int(5,1,2),
                    new Vector3Int(2,1,3), new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3),
                    new Vector3Int(3,1,4), new Vector3Int(4,1,4),
                    // Krater halka
                    new Vector3Int(3,2,2), new Vector3Int(4,2,2), new Vector3Int(5,2,2),
                    new Vector3Int(3,2,3), new Vector3Int(5,2,3),
                    new Vector3Int(3,2,4), new Vector3Int(4,2,4), new Vector3Int(5,2,4),
                    // Lav sızıntısı (asimetrik, bir yandan akıyor)
                    new Vector3Int(6,1,2), new Vector3Int(7,0,1), new Vector3Int(8,0,2),
                    new Vector3Int(7,1,3), new Vector3Int(8,1,3),
                    // Lav topu (havada, asimetrik)
                    new Vector3Int(4,3,3), new Vector3Int(4,4,3),
                    new Vector3Int(3,4,2), new Vector3Int(5,4,4),
                    new Vector3Int(4,5,3), new Vector3Int(3,5,3),
                    new Vector3Int(4,6,2), new Vector3Int(5,6,4),
                    new Vector3Int(4,7,3),
                    // Duman
                    new Vector3Int(3,7,2), new Vector3Int(5,7,4), new Vector3Int(4,8,3),
                    new Vector3Int(3,8,2), new Vector3Int(5,8,4), new Vector3Int(2,8,1),
                    new Vector3Int(6,8,5)
                },
                new Vector3Int[] { new Vector3Int(0,0,2), new Vector3Int(8,0,3), new Vector3Int(4,8,3), new Vector3Int(2,8,1), new Vector3Int(6,8,5), new Vector3Int(0,0,3) }),

            // Level 46: Deniz feneri (68 küp)
            CreateLevelWithFixed(46, "Lighthouse", 3, "Coastal lighthouse",
                new Vector3Int[] {
                    // Kayalık kıyı (düzensiz taban)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3),
                    new Vector3Int(3,0,4), new Vector3Int(4,0,4),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,1), new Vector3Int(2,1,2),
                    // Fener tabanı
                    new Vector3Int(2,1,3), new Vector3Int(3,1,3), new Vector3Int(4,1,3),
                    new Vector3Int(2,1,4), new Vector3Int(3,1,4), new Vector3Int(4,1,4),
                    new Vector3Int(3,1,2), new Vector3Int(4,1,2),
                    // Fener gövdesi (daralan)
                    new Vector3Int(3,2,3), new Vector3Int(3,2,4), new Vector3Int(3,3,3), new Vector3Int(3,3,4),
                    new Vector3Int(3,4,3), new Vector3Int(3,4,4), new Vector3Int(3,5,3), new Vector3Int(3,5,4),
                    new Vector3Int(3,6,3), new Vector3Int(3,6,4),
                    new Vector3Int(4,2,3), new Vector3Int(4,3,3),
                    // Balkon
                    new Vector3Int(2,7,3), new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(5,7,3),
                    new Vector3Int(2,7,4), new Vector3Int(3,7,4), new Vector3Int(4,7,4), new Vector3Int(5,7,4),
                    new Vector3Int(2,7,2), new Vector3Int(3,7,2), new Vector3Int(4,7,2),
                    new Vector3Int(3,7,5), new Vector3Int(4,7,5),
                    // Işık odası
                    new Vector3Int(3,8,3), new Vector3Int(3,8,4), new Vector3Int(4,8,3),
                    new Vector3Int(3,9,3), new Vector3Int(3,9,4),
                    // Tepe
                    new Vector3Int(3,10,3),
                    // Işık hüzmesi (asimetrik)
                    new Vector3Int(5,8,3), new Vector3Int(6,8,3), new Vector3Int(7,8,3),
                    new Vector3Int(6,8,4), new Vector3Int(7,8,4),
                    new Vector3Int(8,8,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,0,3), new Vector3Int(3,10,3), new Vector3Int(8,8,3), new Vector3Int(0,1,0) }),

            // Level 47: Asimetrik labirent (80 küp)
            CreateLevelWithFixed(47, "Labyrinth", 4, "3D labyrinth",
                new Vector3Int[] {
                    // Taban kat - labirent duvarları
                    // Dış duvarlar
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0), new Vector3Int(7,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(7,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(7,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(7,0,3),
                    new Vector3Int(0,0,4), new Vector3Int(7,0,4),
                    new Vector3Int(0,0,5), new Vector3Int(1,0,5), new Vector3Int(2,0,5), new Vector3Int(3,0,5), new Vector3Int(4,0,5), new Vector3Int(5,0,5), new Vector3Int(6,0,5), new Vector3Int(7,0,5),
                    // İç duvarlar (asimetrik)
                    new Vector3Int(2,0,1), new Vector3Int(2,0,2), new Vector3Int(2,0,3),
                    new Vector3Int(4,0,2), new Vector3Int(4,0,3), new Vector3Int(4,0,4),
                    new Vector3Int(5,0,1), new Vector3Int(6,0,1),
                    new Vector3Int(3,0,4), new Vector3Int(1,0,3),
                    new Vector3Int(6,0,3),
                    // Üst kat duvarları (kısmi)
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(7,1,0),
                    new Vector3Int(0,1,5), new Vector3Int(7,1,5),
                    new Vector3Int(0,1,1), new Vector3Int(0,1,2), new Vector3Int(0,1,3), new Vector3Int(0,1,4),
                    new Vector3Int(7,1,2), new Vector3Int(7,1,3), new Vector3Int(7,1,4),
                    new Vector3Int(3,1,1), new Vector3Int(3,1,2),
                    new Vector3Int(5,1,3), new Vector3Int(5,1,4), new Vector3Int(5,1,5),
                    // 3. kat (sadece köşe kuleleri)
                    new Vector3Int(0,2,0), new Vector3Int(7,2,0), new Vector3Int(0,2,5), new Vector3Int(7,2,5),
                    // Çıkış rampası (asimetrik)
                    new Vector3Int(7,1,1), new Vector3Int(8,1,1), new Vector3Int(8,0,1), new Vector3Int(8,0,0),
                    new Vector3Int(9,0,0), new Vector3Int(9,0,1),
                    // İç geçit kuleleri
                    new Vector3Int(2,1,3), new Vector3Int(4,1,1), new Vector3Int(6,1,4),
                    // Tuzak odası
                    new Vector3Int(1,0,1), new Vector3Int(1,0,2),
                    new Vector3Int(3,0,1), new Vector3Int(3,0,3),
                    new Vector3Int(5,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(6,0,4), new Vector3Int(5,0,4)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(7,2,0), new Vector3Int(0,2,5), new Vector3Int(7,2,5), new Vector3Int(9,0,0), new Vector3Int(9,0,1) }),

            // Level 48: Uzay gemisi (85 küp)
            CreateLevelWithFixed(48, "Spaceship", 5, "Giant spaceship",
                new Vector3Int[] {
                    // Burun (sivri)
                    new Vector3Int(12,2,3), new Vector3Int(11,2,3), new Vector3Int(11,3,3),
                    // Kokpit
                    new Vector3Int(10,2,2), new Vector3Int(10,2,3), new Vector3Int(10,2,4),
                    new Vector3Int(10,3,2), new Vector3Int(10,3,3), new Vector3Int(10,3,4),
                    // Ön gövde
                    new Vector3Int(9,2,2), new Vector3Int(9,2,3), new Vector3Int(9,2,4), new Vector3Int(9,3,2), new Vector3Int(9,3,3), new Vector3Int(9,3,4),
                    new Vector3Int(9,1,3), new Vector3Int(9,4,3),
                    // Ana gövde (kalın)
                    new Vector3Int(8,1,2), new Vector3Int(8,2,2), new Vector3Int(8,3,2), new Vector3Int(8,4,2),
                    new Vector3Int(8,1,3), new Vector3Int(8,2,3), new Vector3Int(8,3,3), new Vector3Int(8,4,3),
                    new Vector3Int(8,1,4), new Vector3Int(8,2,4), new Vector3Int(8,3,4), new Vector3Int(8,4,4),
                    new Vector3Int(7,1,2), new Vector3Int(7,2,2), new Vector3Int(7,3,2), new Vector3Int(7,4,2),
                    new Vector3Int(7,1,3), new Vector3Int(7,2,3), new Vector3Int(7,3,3), new Vector3Int(7,4,3),
                    new Vector3Int(7,1,4), new Vector3Int(7,2,4), new Vector3Int(7,3,4), new Vector3Int(7,4,4),
                    new Vector3Int(6,2,2), new Vector3Int(6,2,3), new Vector3Int(6,2,4),
                    new Vector3Int(6,3,2), new Vector3Int(6,3,3), new Vector3Int(6,3,4),
                    // Sol kanat (sweep, asimetrik)
                    new Vector3Int(5,2,1), new Vector3Int(4,2,0), new Vector3Int(3,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(5,3,1), new Vector3Int(4,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(6,2,1), new Vector3Int(7,2,1), new Vector3Int(8,2,1),
                    new Vector3Int(1,2,0),
                    // Sağ kanat (farklı sweep açısı)
                    new Vector3Int(5,2,5), new Vector3Int(4,2,6), new Vector3Int(3,2,6), new Vector3Int(2,2,6),
                    new Vector3Int(5,3,5), new Vector3Int(4,3,6), new Vector3Int(3,3,7),
                    new Vector3Int(6,2,5), new Vector3Int(7,2,5), new Vector3Int(8,2,5),
                    new Vector3Int(2,3,7),
                    // Motor bölmesi
                    new Vector3Int(5,2,2), new Vector3Int(5,2,3), new Vector3Int(5,2,4),
                    new Vector3Int(5,3,3),
                    // Motorlar (arka, asimetrik)
                    new Vector3Int(4,2,2), new Vector3Int(4,2,4),
                    new Vector3Int(4,1,3), new Vector3Int(4,4,3),
                    new Vector3Int(3,2,3), new Vector3Int(3,3,3)
                },
                new Vector3Int[] { new Vector3Int(12,2,3), new Vector3Int(1,2,0), new Vector3Int(3,3,7), new Vector3Int(3,2,3), new Vector3Int(2,3,7), new Vector3Int(2,2,0) }),

            // Level 49: Dev ağaç-şehir (90 küp)
            CreateLevelWithFixed(49, "Yggdrasil", 0, "World tree",
                new Vector3Int[] {
                    // Kökler (dağınık, geniş)
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(6,0,4), new Vector3Int(7,0,4),
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(7,0,5), new Vector3Int(8,0,5),
                    new Vector3Int(3,0,1), new Vector3Int(4,0,4),
                    // Gövde (kalın, yükselen)
                    new Vector3Int(3,1,2), new Vector3Int(4,1,2), new Vector3Int(3,1,3), new Vector3Int(4,1,3),
                    new Vector3Int(5,1,2), new Vector3Int(5,1,3),
                    new Vector3Int(3,2,2), new Vector3Int(4,2,2), new Vector3Int(3,2,3), new Vector3Int(4,2,3),
                    new Vector3Int(4,3,2), new Vector3Int(4,3,3), new Vector3Int(3,3,3),
                    new Vector3Int(4,4,2), new Vector3Int(4,4,3), new Vector3Int(3,4,2),
                    new Vector3Int(4,5,3), new Vector3Int(3,5,2), new Vector3Int(4,5,2),
                    // Dal 1 - sol aşağı (elf köyü)
                    new Vector3Int(2,4,2), new Vector3Int(1,5,1), new Vector3Int(0,6,0), new Vector3Int(0,6,1),
                    new Vector3Int(1,5,2), new Vector3Int(0,5,1), new Vector3Int(0,7,0),
                    new Vector3Int(1,6,1), new Vector3Int(1,6,0), new Vector3Int(0,7,1),
                    // Dal 2 - sağ aşağı
                    new Vector3Int(5,4,3), new Vector3Int(6,5,4), new Vector3Int(7,6,5), new Vector3Int(8,7,5),
                    new Vector3Int(6,5,3), new Vector3Int(7,6,4), new Vector3Int(8,7,6),
                    new Vector3Int(7,7,5), new Vector3Int(8,8,5), new Vector3Int(8,8,6),
                    // Dal 3 - yukarı (taç)
                    new Vector3Int(3,6,2), new Vector3Int(4,6,3), new Vector3Int(3,6,3),
                    new Vector3Int(2,7,1), new Vector3Int(3,7,2), new Vector3Int(4,7,3), new Vector3Int(5,7,4),
                    new Vector3Int(2,7,2), new Vector3Int(5,7,3),
                    new Vector3Int(1,8,1), new Vector3Int(2,8,2), new Vector3Int(3,8,2), new Vector3Int(4,8,3), new Vector3Int(5,8,4), new Vector3Int(6,8,4),
                    // Yaprak bulutları
                    new Vector3Int(0,8,0), new Vector3Int(1,8,0), new Vector3Int(0,9,1), new Vector3Int(1,9,1),
                    new Vector3Int(6,8,5), new Vector3Int(7,8,5), new Vector3Int(6,9,4), new Vector3Int(7,9,5),
                    new Vector3Int(3,9,2), new Vector3Int(4,9,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(8,0,5), new Vector3Int(0,9,1), new Vector3Int(7,9,5), new Vector3Int(3,9,2), new Vector3Int(4,9,3) }),

            // Level 50: Son sınav - Kaotik galaksi (100 küp)
            CreateLevelWithFixed(50, "Big Bang", 1, "Chaotic galaxy - Final exam",
                GenerateChaosGalaxy(),
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(12,0,0), new Vector3Int(0,0,12),
                    new Vector3Int(12,0,12), new Vector3Int(6,8,6), new Vector3Int(6,0,6)
                }),

            // ============ BÖLÜM 6: Yeni Ufuklar (51-60) ============

            // Level 51: Kelebek (55 küp) - Asimetrik kanat
            CreateLevelWithFixed(51, "Butterfly", 6, "Autumn butterfly",
                new Vector3Int[] {
                    // Gövde (dikey merkez)
                    new Vector3Int(5,0,3), new Vector3Int(5,1,3), new Vector3Int(5,2,3), new Vector3Int(5,3,3),
                    new Vector3Int(5,4,3), new Vector3Int(5,5,3), new Vector3Int(5,6,3),
                    // Sol üst kanat (büyük)
                    new Vector3Int(4,4,3), new Vector3Int(3,4,3), new Vector3Int(2,4,3),
                    new Vector3Int(4,5,3), new Vector3Int(3,5,3), new Vector3Int(2,5,3), new Vector3Int(1,5,3),
                    new Vector3Int(4,6,3), new Vector3Int(3,6,3), new Vector3Int(2,6,3), new Vector3Int(1,6,3),
                    new Vector3Int(3,7,3), new Vector3Int(2,7,3),
                    // Sağ üst kanat (biraz farklı şekil)
                    new Vector3Int(6,4,3), new Vector3Int(7,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(6,5,3), new Vector3Int(7,5,3), new Vector3Int(8,5,3), new Vector3Int(9,5,3),
                    new Vector3Int(6,6,3), new Vector3Int(7,6,3), new Vector3Int(8,6,3),
                    new Vector3Int(7,7,3), new Vector3Int(8,7,3),
                    // Sol alt kanat
                    new Vector3Int(4,2,3), new Vector3Int(3,2,3), new Vector3Int(2,2,3),
                    new Vector3Int(4,1,3), new Vector3Int(3,1,3), new Vector3Int(2,1,3), new Vector3Int(1,1,3),
                    new Vector3Int(3,0,3), new Vector3Int(2,0,3),
                    // Sağ alt kanat
                    new Vector3Int(6,2,3), new Vector3Int(7,2,3), new Vector3Int(8,2,3),
                    new Vector3Int(6,1,3), new Vector3Int(7,1,3), new Vector3Int(8,1,3),
                    new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,1,3),
                    // Anten
                    new Vector3Int(4,7,3), new Vector3Int(3,8,3),
                    new Vector3Int(6,7,3), new Vector3Int(7,8,3),
                    // Derinlik
                    new Vector3Int(5,3,2), new Vector3Int(5,3,4)
                },
                new Vector3Int[] { new Vector3Int(1,5,3), new Vector3Int(9,5,3), new Vector3Int(3,8,3), new Vector3Int(7,8,3), new Vector3Int(1,1,3), new Vector3Int(9,1,3) }),

            // Level 52: Köprü (50 küp) - Asimetrik kemer köprü
            CreateLevelWithFixed(52, "Bridge", 7, "Tropical arch bridge",
                new Vector3Int[] {
                    // Sol ayak (kalın)
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(0,0,3), new Vector3Int(1,0,3),
                    new Vector3Int(0,1,2), new Vector3Int(1,1,2), new Vector3Int(0,1,3), new Vector3Int(1,1,3),
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2), new Vector3Int(0,2,3),
                    new Vector3Int(0,3,2), new Vector3Int(1,3,2),
                    // Kemer (eğri)
                    new Vector3Int(2,4,2), new Vector3Int(2,4,3),
                    new Vector3Int(3,5,2), new Vector3Int(3,5,3),
                    new Vector3Int(4,5,2), new Vector3Int(4,5,3), new Vector3Int(5,5,2), new Vector3Int(5,5,3),
                    new Vector3Int(6,5,2), new Vector3Int(6,5,3),
                    new Vector3Int(7,4,2), new Vector3Int(7,4,3),
                    // Sağ ayak (ince, farklı)
                    new Vector3Int(8,3,2), new Vector3Int(8,3,3),
                    new Vector3Int(8,2,2), new Vector3Int(8,2,3), new Vector3Int(9,2,2),
                    new Vector3Int(8,1,2), new Vector3Int(8,1,3), new Vector3Int(9,1,2), new Vector3Int(9,1,3),
                    new Vector3Int(8,0,2), new Vector3Int(8,0,3), new Vector3Int(9,0,2), new Vector3Int(9,0,3),
                    // Yol yüzeyi
                    new Vector3Int(1,4,2), new Vector3Int(1,4,3),
                    new Vector3Int(8,4,2), new Vector3Int(8,4,3),
                    // Korkuluk (tek taraf)
                    new Vector3Int(3,6,2), new Vector3Int(4,6,2), new Vector3Int(5,6,2), new Vector3Int(6,6,2),
                    // Temel taşları
                    new Vector3Int(0,0,1), new Vector3Int(9,0,4), new Vector3Int(10,0,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,1), new Vector3Int(10,0,3), new Vector3Int(3,6,2), new Vector3Int(6,6,2), new Vector3Int(4,5,2) }),

            // Level 53: Nota (52 küp) - Müzik notası
            CreateLevelWithFixed(53, "Note", 8, "Lavender melody",
                new Vector3Int[] {
                    // Sol nota başı (daire)
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(0,0,3), new Vector3Int(1,0,3),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(1,0,4), new Vector3Int(2,0,4),
                    new Vector3Int(0,1,3), new Vector3Int(1,1,3),
                    // Sol sap
                    new Vector3Int(3,1,3), new Vector3Int(3,2,3), new Vector3Int(3,3,3), new Vector3Int(3,4,3),
                    new Vector3Int(3,5,3), new Vector3Int(3,6,3), new Vector3Int(3,7,3),
                    // Sağ nota başı
                    new Vector3Int(7,0,2), new Vector3Int(8,0,2), new Vector3Int(6,0,3), new Vector3Int(7,0,3),
                    new Vector3Int(8,0,3), new Vector3Int(9,0,3), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    new Vector3Int(9,1,3), new Vector3Int(8,1,3),
                    // Sağ sap
                    new Vector3Int(9,1,3), new Vector3Int(9,2,3), new Vector3Int(9,3,3), new Vector3Int(9,4,3),
                    new Vector3Int(9,5,3), new Vector3Int(9,6,3), new Vector3Int(9,7,3),
                    // Bağlama çubuğu (üstte)
                    new Vector3Int(4,7,3), new Vector3Int(5,7,3), new Vector3Int(6,7,3),
                    new Vector3Int(7,7,3), new Vector3Int(8,7,3),
                    // Bayraklar (sağ taraf, asimetrik)
                    new Vector3Int(10,7,3), new Vector3Int(10,6,3), new Vector3Int(10,5,3),
                    new Vector3Int(4,7,2), new Vector3Int(4,6,2),
                    // 3D derinlik
                    new Vector3Int(3,4,2), new Vector3Int(9,4,2),
                    new Vector3Int(1,0,3), new Vector3Int(7,0,3),
                    new Vector3Int(5,7,2), new Vector3Int(6,7,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,3), new Vector3Int(6,0,3), new Vector3Int(10,7,3), new Vector3Int(4,6,2), new Vector3Int(5,7,3) }),

            // Level 54: Kale (60 küp) - Ortaçağ kalesi
            CreateLevelWithFixed(54, "Castle", 9, "Night castle",
                new Vector3Int[] {
                    // Taban duvarları
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0),
                    new Vector3Int(0,0,4), new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4),
                    new Vector3Int(0,0,1), new Vector3Int(0,0,2), new Vector3Int(0,0,3),
                    new Vector3Int(6,0,1), new Vector3Int(6,0,2), new Vector3Int(6,0,3),
                    // 2. kat duvarlar
                    new Vector3Int(0,1,0), new Vector3Int(6,1,0), new Vector3Int(0,1,4), new Vector3Int(6,1,4),
                    new Vector3Int(0,1,1), new Vector3Int(0,1,3), new Vector3Int(6,1,1), new Vector3Int(6,1,3),
                    // Kuleler (köşelerde, farklı yüksekler)
                    new Vector3Int(0,2,0), new Vector3Int(0,3,0), new Vector3Int(0,4,0), // Sol-ön kule (yüksek)
                    new Vector3Int(6,2,0), new Vector3Int(6,3,0),                         // Sağ-ön kule (orta)
                    new Vector3Int(0,2,4), new Vector3Int(0,3,4), new Vector3Int(0,4,4), new Vector3Int(0,5,4), // Sol-arka (en yüksek)
                    new Vector3Int(6,2,4), new Vector3Int(6,3,4), new Vector3Int(6,4,4), // Sağ-arka
                    // Mazgallı duvar üstü
                    new Vector3Int(2,2,0), new Vector3Int(4,2,0),
                    new Vector3Int(2,2,4), new Vector3Int(4,2,4),
                    // İç avlu
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    // Kapı kemeri
                    new Vector3Int(3,1,0), new Vector3Int(3,2,0),
                    // Bayrak direkleri
                    new Vector3Int(0,5,0), new Vector3Int(0,6,4),
                    // Hendek (önde, asimetrik)
                    new Vector3Int(1,0,5), new Vector3Int(2,0,5), new Vector3Int(3,0,5), new Vector3Int(4,0,5), new Vector3Int(5,0,5)
                },
                new Vector3Int[] { new Vector3Int(0,5,0), new Vector3Int(0,6,4), new Vector3Int(6,4,4), new Vector3Int(3,2,0), new Vector3Int(1,0,5), new Vector3Int(5,0,5) }),

            // Level 55: Ahtapot (58 küp) - Deniz canlısı
            CreateLevelWithFixed(55, "Octopus", 10, "Coral reef octopus",
                new Vector3Int[] {
                    // Baş (oval)
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3),
                    new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(6,6,3),
                    new Vector3Int(4,7,3), new Vector3Int(5,7,3), new Vector3Int(6,7,3),
                    new Vector3Int(5,8,3), new Vector3Int(5,5,2), new Vector3Int(5,6,2),
                    // Gözler
                    new Vector3Int(4,6,2), new Vector3Int(6,6,2),
                    // Kol 1 - sol ön (kıvrımlı)
                    new Vector3Int(3,4,3), new Vector3Int(2,3,3), new Vector3Int(1,2,3), new Vector3Int(0,1,3), new Vector3Int(0,0,3),
                    // Kol 2 - sol arka
                    new Vector3Int(3,4,4), new Vector3Int(2,3,4), new Vector3Int(1,2,5), new Vector3Int(0,1,5), new Vector3Int(0,0,6),
                    // Kol 3 - sağ ön
                    new Vector3Int(7,4,3), new Vector3Int(8,3,3), new Vector3Int(9,2,3), new Vector3Int(10,1,3), new Vector3Int(10,0,2),
                    // Kol 4 - sağ arka
                    new Vector3Int(7,4,4), new Vector3Int(8,3,4), new Vector3Int(9,2,5), new Vector3Int(10,1,5), new Vector3Int(10,0,5),
                    // Kol 5 - ön orta
                    new Vector3Int(5,4,2), new Vector3Int(5,3,1), new Vector3Int(5,2,0), new Vector3Int(5,1,0), new Vector3Int(4,0,0),
                    // Kol 6 - arka orta
                    new Vector3Int(5,4,4), new Vector3Int(5,3,5), new Vector3Int(5,2,6), new Vector3Int(5,1,6), new Vector3Int(6,0,7),
                    // Kol 7 - sol orta (kısa)
                    new Vector3Int(3,4,2), new Vector3Int(2,3,1), new Vector3Int(1,2,1),
                    // Kol 8 - sağ orta (kısa)
                    new Vector3Int(7,4,2), new Vector3Int(8,3,2), new Vector3Int(9,2,1)
                },
                new Vector3Int[] { new Vector3Int(5,8,3), new Vector3Int(0,0,3), new Vector3Int(0,0,6), new Vector3Int(10,0,2), new Vector3Int(4,0,0), new Vector3Int(6,0,7) }),

            // Level 56: Küp kule (45 küp) - Basit grid + yükseklik
            CreateLevelWithFixed(56, "Tower Blocks", 11, "Golden block towers",
                new Vector3Int[] {
                    // Taban grid (5x5)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3),
                    new Vector3Int(0,0,4), new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4),
                    // Orta yükseklik (3x3)
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(1,1,2), new Vector3Int(2,1,2), new Vector3Int(3,1,2),
                    new Vector3Int(1,1,3), new Vector3Int(2,1,3), new Vector3Int(3,1,3),
                    // Üst (2x2)
                    new Vector3Int(1,2,1), new Vector3Int(2,2,1),
                    new Vector3Int(1,2,2), new Vector3Int(2,2,2),
                    // Zirve
                    new Vector3Int(2,3,2), new Vector3Int(2,3,1),
                    new Vector3Int(2,4,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,0,4), new Vector3Int(4,0,4), new Vector3Int(2,4,2) }),

            // Level 57: Taç (62 küp)
            CreateLevelWithFixed(57, "Crown", 12, "Ice crystal crown",
                new Vector3Int[] {
                    // Taban bant (geniş)
                    new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3),
                    new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,0,3),
                    new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4),
                    new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4), new Vector3Int(9,0,4),
                    // 2. sıra bant
                    new Vector3Int(1,1,3), new Vector3Int(2,1,3), new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3),
                    new Vector3Int(6,1,3), new Vector3Int(7,1,3), new Vector3Int(8,1,3), new Vector3Int(9,1,3),
                    // Sivri uçlar (5 tane, farklı yüksekliklerde)
                    new Vector3Int(1,2,3), new Vector3Int(1,3,3), new Vector3Int(1,4,3),               // Sol uç
                    new Vector3Int(3,2,3), new Vector3Int(3,3,3), new Vector3Int(3,4,3), new Vector3Int(3,5,3), // Sol-orta (uzun)
                    new Vector3Int(5,2,3), new Vector3Int(5,3,3), new Vector3Int(5,4,3), new Vector3Int(5,5,3), new Vector3Int(5,6,3), // Orta (en uzun)
                    new Vector3Int(7,2,3), new Vector3Int(7,3,3), new Vector3Int(7,4,3), new Vector3Int(7,5,3), // Sağ-orta
                    new Vector3Int(9,2,3), new Vector3Int(9,3,3), new Vector3Int(9,4,3),               // Sağ uç
                    // Mücevherler (uçlarda)
                    new Vector3Int(1,4,4), new Vector3Int(3,5,4), new Vector3Int(5,6,4), new Vector3Int(7,5,4), new Vector3Int(9,4,4),
                    // Arka derinlik
                    new Vector3Int(5,1,4), new Vector3Int(3,1,4), new Vector3Int(7,1,4)
                },
                new Vector3Int[] { new Vector3Int(1,4,4), new Vector3Int(5,6,4), new Vector3Int(9,4,4), new Vector3Int(1,0,3), new Vector3Int(9,0,3), new Vector3Int(5,6,3) }),

            // Level 58: Yanardağ (65 küp)
            CreateLevelWithFixed(58, "Volcano", 13, "Erupting volcano",
                new Vector3Int[] {
                    // Geniş taban
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2), new Vector3Int(8,0,2),
                    new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,0,3),
                    new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    new Vector3Int(3,0,5), new Vector3Int(4,0,5), new Vector3Int(5,0,5), new Vector3Int(6,0,5), new Vector3Int(7,0,5),
                    // 2. katman
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3), new Vector3Int(7,1,3),
                    new Vector3Int(3,1,4), new Vector3Int(4,1,4), new Vector3Int(5,1,4), new Vector3Int(6,1,4),
                    new Vector3Int(4,1,2), new Vector3Int(5,1,2), new Vector3Int(6,1,2),
                    // 3. katman
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3),
                    new Vector3Int(5,2,4), new Vector3Int(5,2,2),
                    // Krater (boş orta)
                    new Vector3Int(4,3,3), new Vector3Int(6,3,3), new Vector3Int(5,3,4), new Vector3Int(5,3,2),
                    // Lav akıntısı (asimetrik, bir tarafa akıyor)
                    new Vector3Int(8,1,3), new Vector3Int(9,1,3), new Vector3Int(10,0,3), new Vector3Int(10,0,4),
                    new Vector3Int(11,0,3), new Vector3Int(11,0,4), new Vector3Int(12,0,4),
                    // Duman (yukarı)
                    new Vector3Int(5,4,3), new Vector3Int(5,5,3), new Vector3Int(4,6,3), new Vector3Int(6,6,4)
                },
                new Vector3Int[] { new Vector3Int(1,0,3), new Vector3Int(9,0,3), new Vector3Int(12,0,4), new Vector3Int(5,5,3), new Vector3Int(4,6,3), new Vector3Int(6,6,4) }),

            // Level 59: Saat (55 küp) - Kum saati
            CreateLevelWithFixed(59, "Hourglass", 14, "Flow of time",
                new Vector3Int[] {
                    // Üst geniş kısım
                    new Vector3Int(1,6,2), new Vector3Int(2,6,2), new Vector3Int(3,6,2), new Vector3Int(4,6,2), new Vector3Int(5,6,2),
                    new Vector3Int(1,6,3), new Vector3Int(2,6,3), new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3),
                    new Vector3Int(2,5,2), new Vector3Int(3,5,2), new Vector3Int(4,5,2),
                    new Vector3Int(2,5,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3),
                    // Daralma
                    new Vector3Int(3,4,2), new Vector3Int(3,4,3),
                    // Boğaz
                    new Vector3Int(3,3,2), new Vector3Int(3,3,3),
                    // Genişleme
                    new Vector3Int(3,2,2), new Vector3Int(3,2,3),
                    new Vector3Int(2,1,2), new Vector3Int(3,1,2), new Vector3Int(4,1,2),
                    new Vector3Int(2,1,3), new Vector3Int(3,1,3), new Vector3Int(4,1,3),
                    // Alt geniş kısım
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3),
                    // Kum taneleri (düşen, asimetrik)
                    new Vector3Int(3,2,1), new Vector3Int(4,1,1), new Vector3Int(2,0,1),
                    // Çerçeve derinlik
                    new Vector3Int(1,6,1), new Vector3Int(5,6,1), new Vector3Int(1,0,1), new Vector3Int(5,0,1),
                    // Yükseklik çerçeve
                    new Vector3Int(1,5,2), new Vector3Int(5,5,2), new Vector3Int(1,1,2), new Vector3Int(5,1,2),
                    // Düşen kumlar
                    new Vector3Int(2,4,2), new Vector3Int(4,2,3),
                    new Vector3Int(3,0,1), new Vector3Int(4,0,1)
                },
                new Vector3Int[] { new Vector3Int(1,6,1), new Vector3Int(5,6,1), new Vector3Int(1,0,1), new Vector3Int(5,0,1), new Vector3Int(3,3,2), new Vector3Int(3,3,3) }),

            // Level 60: Deniz kabuğu (68 küp) - Spiral kabuk
            CreateLevelWithFixed(60, "Seashell", 15, "Emerald spiral shell",
                new Vector3Int[] {
                    // Spiral 1. tur (dış)
                    new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0), new Vector3Int(7,0,0), new Vector3Int(8,0,0),
                    new Vector3Int(8,0,1), new Vector3Int(8,0,2), new Vector3Int(8,0,3), new Vector3Int(8,0,4),
                    new Vector3Int(7,0,4), new Vector3Int(6,0,4), new Vector3Int(5,0,4), new Vector3Int(4,0,4),
                    new Vector3Int(4,0,3), new Vector3Int(4,0,2),
                    // Spiral 2. tur (iç, yükselen)
                    new Vector3Int(5,1,1), new Vector3Int(6,1,1), new Vector3Int(7,1,1),
                    new Vector3Int(7,1,2), new Vector3Int(7,1,3),
                    new Vector3Int(6,1,3), new Vector3Int(5,1,3),
                    new Vector3Int(5,1,2),
                    // Spiral 3. tur (merkez, daha yüksek)
                    new Vector3Int(6,2,2), new Vector3Int(6,2,1), new Vector3Int(7,2,2),
                    // Kabuk gövdesi (kalınlık)
                    new Vector3Int(4,1,0), new Vector3Int(5,1,0), new Vector3Int(6,1,0), new Vector3Int(7,1,0), new Vector3Int(8,1,0),
                    new Vector3Int(8,1,1), new Vector3Int(8,1,4), new Vector3Int(7,1,4), new Vector3Int(4,1,4),
                    new Vector3Int(4,1,3), new Vector3Int(4,1,2), new Vector3Int(4,1,1),
                    // Üst kabuk
                    new Vector3Int(5,2,0), new Vector3Int(6,2,0), new Vector3Int(7,2,0), new Vector3Int(8,2,0),
                    new Vector3Int(8,2,1), new Vector3Int(8,2,3), new Vector3Int(8,2,4),
                    new Vector3Int(7,2,4), new Vector3Int(4,2,4), new Vector3Int(4,2,3),
                    // Sivri uç
                    new Vector3Int(3,0,0), new Vector3Int(2,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(3,0,1), new Vector3Int(2,0,1),
                    // Ağız açıklığı
                    new Vector3Int(4,0,1), new Vector3Int(3,1,0), new Vector3Int(3,1,1),
                    // Kabuk deseni
                    new Vector3Int(6,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,1),
                    new Vector3Int(5,0,3), new Vector3Int(7,0,3), new Vector3Int(7,0,1)
                },
                new Vector3Int[] { new Vector3Int(1,0,0), new Vector3Int(8,2,4), new Vector3Int(6,2,2), new Vector3Int(8,0,0), new Vector3Int(4,2,4), new Vector3Int(8,2,0) }),

            // ============ BÖLÜM 7: Derinlik (61-70) ============

            // Level 61: Kuş (60 küp) - Uçan kuş
            CreateLevelWithFixed(61, "Bird", 6, "Flying autumn bird",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3), new Vector3Int(8,3,3),
                    new Vector3Int(5,3,2), new Vector3Int(6,3,2), new Vector3Int(7,3,2),
                    new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    // Baş
                    new Vector3Int(9,3,3), new Vector3Int(9,4,3), new Vector3Int(10,3,3), new Vector3Int(10,4,3),
                    // Gaga
                    new Vector3Int(11,3,3), new Vector3Int(11,4,3),
                    // Göz
                    new Vector3Int(10,4,2),
                    // Sol kanat (yukarı)
                    new Vector3Int(5,4,4), new Vector3Int(5,5,4), new Vector3Int(4,5,5), new Vector3Int(5,5,5),
                    new Vector3Int(3,6,5), new Vector3Int(4,6,6), new Vector3Int(3,6,6), new Vector3Int(2,7,6),
                    new Vector3Int(2,7,7), new Vector3Int(1,7,7),
                    // Sağ kanat (aşağı, asimetrik)
                    new Vector3Int(5,2,4), new Vector3Int(5,1,4), new Vector3Int(4,1,5), new Vector3Int(5,1,5),
                    new Vector3Int(3,0,5), new Vector3Int(4,0,6), new Vector3Int(3,0,6), new Vector3Int(2,0,6),
                    new Vector3Int(2,0,7), new Vector3Int(1,0,7),
                    // Kuyruk
                    new Vector3Int(3,3,3), new Vector3Int(2,3,3), new Vector3Int(1,3,3), new Vector3Int(1,3,4),
                    new Vector3Int(0,3,4), new Vector3Int(0,3,5), new Vector3Int(0,4,4),
                    // Kanat ucu tüyleri
                    new Vector3Int(6,5,4), new Vector3Int(7,5,4),
                    new Vector3Int(6,1,4), new Vector3Int(7,1,4),
                    // Ayaklar
                    new Vector3Int(6,2,3), new Vector3Int(6,1,3), new Vector3Int(6,0,3),
                    new Vector3Int(7,2,3), new Vector3Int(7,1,3), new Vector3Int(8,0,3)
                },
                new Vector3Int[] { new Vector3Int(11,3,3), new Vector3Int(10,4,2), new Vector3Int(1,7,7), new Vector3Int(1,0,7), new Vector3Int(0,3,5), new Vector3Int(0,4,4) }),

            // Level 62: Grid merdiven (48 küp) - Basamaklı grid
            CreateLevelWithFixed(62, "Staircase", 7, "Tropical steps",
                GenerateStaircase(6, 8),
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,0,0), new Vector3Int(0,0,7), new Vector3Int(5,7,7), new Vector3Int(0,7,7) }),

            // Level 63: Balık (58 küp)
            CreateLevelWithFixed(63, "Fish", 10, "Coral fish",
                new Vector3Int[] {
                    // Gövde (oval)
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3), new Vector3Int(7,2,3),
                    new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3), new Vector3Int(8,3,3),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3),
                    // Göz
                    new Vector3Int(7,4,2),
                    // Ağız
                    new Vector3Int(9,3,3), new Vector3Int(9,4,3),
                    // Kuyruk (çatal, asimetrik)
                    new Vector3Int(2,3,3), new Vector3Int(1,2,3), new Vector3Int(0,1,3),
                    new Vector3Int(2,4,3), new Vector3Int(1,5,3), new Vector3Int(0,6,3), new Vector3Int(0,7,3),
                    // Üst yüzgeç
                    new Vector3Int(5,6,3), new Vector3Int(6,6,3), new Vector3Int(5,7,3),
                    // Alt yüzgeç
                    new Vector3Int(5,1,3), new Vector3Int(6,1,3),
                    // Yan yüzgeçler
                    new Vector3Int(6,3,2), new Vector3Int(5,3,2), new Vector3Int(5,2,2),
                    new Vector3Int(6,3,4), new Vector3Int(5,3,4), new Vector3Int(5,4,4),
                    // Pul deseni (derinlik)
                    new Vector3Int(4,3,2), new Vector3Int(4,4,2),
                    new Vector3Int(4,3,4), new Vector3Int(4,4,4),
                    new Vector3Int(7,3,2), new Vector3Int(7,4,4),
                    // Kuyruk derinlik
                    new Vector3Int(1,3,2), new Vector3Int(1,4,4),
                    new Vector3Int(0,1,2), new Vector3Int(0,7,4),
                    // Ek detaylar
                    new Vector3Int(3,2,3), new Vector3Int(3,5,3), new Vector3Int(8,2,3), new Vector3Int(8,5,3)
                },
                new Vector3Int[] { new Vector3Int(9,3,3), new Vector3Int(7,4,2), new Vector3Int(0,1,3), new Vector3Int(0,7,3), new Vector3Int(5,7,3), new Vector3Int(0,1,2) }),

            // Level 64: Mantar (55 küp)
            CreateLevelWithFixed(64, "Mushroom", 8, "Magical lavender mushroom",
                new Vector3Int[] {
                    // Sap (ince, asimetrik)
                    new Vector3Int(4,0,3), new Vector3Int(5,0,3),
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3),
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3),
                    new Vector3Int(5,3,3),
                    // Şapka (geniş, asimetrik)
                    new Vector3Int(2,4,2), new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(5,4,2), new Vector3Int(6,4,2), new Vector3Int(7,4,2),
                    new Vector3Int(2,4,3), new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(2,4,4), new Vector3Int(3,4,4), new Vector3Int(4,4,4), new Vector3Int(5,4,4), new Vector3Int(6,4,4), new Vector3Int(7,4,4),
                    new Vector3Int(3,4,5), new Vector3Int(4,4,5), new Vector3Int(5,4,5), new Vector3Int(6,4,5),
                    // Şapka üstü (kubbemsi)
                    new Vector3Int(3,5,2), new Vector3Int(4,5,2), new Vector3Int(5,5,2), new Vector3Int(6,5,2),
                    new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3),
                    new Vector3Int(3,5,4), new Vector3Int(4,5,4), new Vector3Int(5,5,4), new Vector3Int(6,5,4),
                    new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(6,6,3),
                    new Vector3Int(5,6,2), new Vector3Int(5,6,4),
                    new Vector3Int(5,7,3),
                    // Kök filizleri (taban)
                    new Vector3Int(3,0,3), new Vector3Int(6,0,3), new Vector3Int(4,0,2), new Vector3Int(5,0,4)
                },
                new Vector3Int[] { new Vector3Int(5,7,3), new Vector3Int(2,4,2), new Vector3Int(8,4,3), new Vector3Int(3,0,3), new Vector3Int(6,0,3), new Vector3Int(3,4,5) }),

            // Level 65: At (70 küp) - Satranç atı
            CreateLevelWithFixed(65, "Knight", 11, "Golden chess knight",
                new Vector3Int[] {
                    // Taban (kaide)
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4),
                    // Boyun (dikey, eğik)
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(4,1,4),
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(4,2,4),
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3),
                    new Vector3Int(4,4,3), new Vector3Int(5,4,3),
                    new Vector3Int(5,5,3), new Vector3Int(5,5,4),
                    // Baş (uzun burun)
                    new Vector3Int(5,6,3), new Vector3Int(6,6,3), new Vector3Int(7,6,3),
                    new Vector3Int(5,7,3), new Vector3Int(6,7,3), new Vector3Int(7,7,3), new Vector3Int(8,7,3),
                    new Vector3Int(6,8,3), new Vector3Int(7,8,3),
                    // Burun
                    new Vector3Int(8,6,3), new Vector3Int(9,6,3), new Vector3Int(9,7,3),
                    // Kulaklar
                    new Vector3Int(5,8,3), new Vector3Int(5,9,3),
                    new Vector3Int(7,9,3),
                    // Yele (asimetrik)
                    new Vector3Int(4,5,2), new Vector3Int(4,6,2), new Vector3Int(4,7,2),
                    new Vector3Int(5,5,2), new Vector3Int(5,6,2), new Vector3Int(5,7,2), new Vector3Int(5,8,2),
                    new Vector3Int(4,4,2), new Vector3Int(4,3,2),
                    // Göğüs
                    new Vector3Int(5,1,4), new Vector3Int(5,2,4), new Vector3Int(5,3,4), new Vector3Int(5,4,4),
                    // Kaide kenarları
                    new Vector3Int(3,1,3), new Vector3Int(3,1,2), new Vector3Int(6,1,3),
                    // Kuyruk izi (asimetrik çıkıntı)
                    new Vector3Int(3,1,4), new Vector3Int(3,2,4), new Vector3Int(2,2,4), new Vector3Int(2,3,4),
                    // Göz
                    new Vector3Int(7,7,2)
                },
                new Vector3Int[] { new Vector3Int(2,0,2), new Vector3Int(6,0,2), new Vector3Int(9,6,3), new Vector3Int(5,9,3), new Vector3Int(7,7,2), new Vector3Int(2,3,4) }),

            // Level 66: Ay hilali ve yıldız (60 küp)
            CreateLevelWithFixed(66, "Crescent", 9, "Night crescent moon",
                new Vector3Int[] {
                    // Hilal dış yay
                    new Vector3Int(5,8,3), new Vector3Int(6,8,3),
                    new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(7,7,3),
                    new Vector3Int(2,6,3), new Vector3Int(8,6,3),
                    new Vector3Int(1,5,3), new Vector3Int(8,5,3),
                    new Vector3Int(1,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(1,3,3), new Vector3Int(7,3,3),
                    new Vector3Int(2,2,3), new Vector3Int(7,2,3),
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(6,1,3),
                    new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    // İç yay (boşluk bırakarak)
                    new Vector3Int(5,6,3), new Vector3Int(6,6,3),
                    new Vector3Int(4,5,3), new Vector3Int(6,5,3),
                    new Vector3Int(3,4,3), new Vector3Int(6,4,3),
                    new Vector3Int(3,3,3), new Vector3Int(5,3,3),
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3),
                    // Derinlik
                    new Vector3Int(5,8,2), new Vector3Int(1,4,2), new Vector3Int(1,5,2), new Vector3Int(8,5,2), new Vector3Int(8,4,2),
                    new Vector3Int(5,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(2,6,2), new Vector3Int(8,6,2),
                    // Yıldız (sağ üstte)
                    new Vector3Int(10,7,3),
                    new Vector3Int(9,6,3), new Vector3Int(10,6,3), new Vector3Int(11,6,3),
                    new Vector3Int(10,5,3),
                    new Vector3Int(9,7,3), new Vector3Int(11,7,3),
                    new Vector3Int(10,8,3),
                    // Yıldız derinlik
                    new Vector3Int(10,7,2), new Vector3Int(10,6,2),
                    // Küçük yıldızlar
                    new Vector3Int(11,3,3), new Vector3Int(0,7,3), new Vector3Int(9,1,3),
                    new Vector3Int(11,3,2)
                },
                new Vector3Int[] { new Vector3Int(5,8,3), new Vector3Int(5,0,3), new Vector3Int(10,8,3), new Vector3Int(10,5,3), new Vector3Int(11,3,3), new Vector3Int(0,7,3) }),

            // Level 67: Piramit basamakları (52 küp) - Grid benzeri ama piramit
            CreateLevelWithFixed(67, "Pyramid", 0, "Pyramid in the sun",
                new Vector3Int[] {
                    // Taban (7x7)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1), new Vector3Int(6,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(0,0,4), new Vector3Int(6,0,4),
                    new Vector3Int(0,0,5), new Vector3Int(1,0,5), new Vector3Int(2,0,5), new Vector3Int(3,0,5), new Vector3Int(4,0,5), new Vector3Int(5,0,5), new Vector3Int(6,0,5),
                    new Vector3Int(0,0,6), new Vector3Int(1,0,6), new Vector3Int(2,0,6), new Vector3Int(3,0,6), new Vector3Int(4,0,6), new Vector3Int(5,0,6), new Vector3Int(6,0,6),
                    // 2. kat (5x5 çerçeve)
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(4,1,1), new Vector3Int(5,1,1),
                    new Vector3Int(1,1,5), new Vector3Int(2,1,5), new Vector3Int(3,1,5), new Vector3Int(4,1,5), new Vector3Int(5,1,5),
                    // 3. kat (3x3)
                    new Vector3Int(2,2,2), new Vector3Int(3,2,2), new Vector3Int(4,2,2),
                    new Vector3Int(2,2,4), new Vector3Int(3,2,4), new Vector3Int(4,2,4),
                    // Zirve
                    new Vector3Int(3,3,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(6,0,0), new Vector3Int(0,0,6), new Vector3Int(6,0,6), new Vector3Int(3,3,3) }),

            // Level 68: Çapa (62 küp)
            CreateLevelWithFixed(68, "Anchor", 1, "Ocean anchor",
                new Vector3Int[] {
                    // Halka (üst)
                    new Vector3Int(4,8,3), new Vector3Int(5,8,3), new Vector3Int(6,8,3),
                    new Vector3Int(3,7,3), new Vector3Int(7,7,3),
                    new Vector3Int(3,6,3), new Vector3Int(7,6,3),
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3),
                    // Sap (dikey)
                    new Vector3Int(5,4,3), new Vector3Int(5,3,3), new Vector3Int(5,2,3), new Vector3Int(5,1,3),
                    // Yatay çubuk
                    new Vector3Int(3,5,3), new Vector3Int(2,5,3), new Vector3Int(8,5,3), new Vector3Int(9,5,3),
                    // Sol kanca (eğri)
                    new Vector3Int(2,4,3), new Vector3Int(1,3,3), new Vector3Int(1,2,3), new Vector3Int(1,1,3),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3),
                    // Sağ kanca (farklı eğri)
                    new Vector3Int(9,4,3), new Vector3Int(10,3,3), new Vector3Int(10,2,3),
                    new Vector3Int(10,1,3), new Vector3Int(9,0,3), new Vector3Int(8,0,3),
                    // Derinlik
                    new Vector3Int(5,8,2), new Vector3Int(5,5,2), new Vector3Int(5,3,2), new Vector3Int(5,1,2),
                    new Vector3Int(4,8,2), new Vector3Int(6,8,2),
                    new Vector3Int(1,2,2), new Vector3Int(10,2,2),
                    new Vector3Int(3,0,2), new Vector3Int(8,0,2),
                    // Zincir (asimetrik, yukarı)
                    new Vector3Int(5,9,3), new Vector3Int(5,10,3), new Vector3Int(5,11,3),
                    new Vector3Int(4,10,3), new Vector3Int(6,10,3),
                    new Vector3Int(5,9,2), new Vector3Int(5,11,2),
                    // Yosun (asimetrik)
                    new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(9,0,4),
                    new Vector3Int(1,1,4), new Vector3Int(10,1,4),
                    // Ek detaylar
                    new Vector3Int(4,0,2), new Vector3Int(7,0,3), new Vector3Int(6,0,3)
                },
                new Vector3Int[] { new Vector3Int(5,11,3), new Vector3Int(4,8,3), new Vector3Int(6,8,3), new Vector3Int(2,0,3), new Vector3Int(9,0,3), new Vector3Int(1,1,4) }),

            // Level 69: Gitar (65 küp)
            CreateLevelWithFixed(69, "Guitar", 14, "Cherry guitar",
                new Vector3Int[] {
                    // Gövde (büyük daire)
                    new Vector3Int(2,2,3), new Vector3Int(3,2,3), new Vector3Int(4,2,3),
                    new Vector3Int(1,3,3), new Vector3Int(2,3,3), new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(5,3,3),
                    new Vector3Int(1,4,3), new Vector3Int(2,4,3), new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3),
                    new Vector3Int(1,5,3), new Vector3Int(2,5,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3),
                    new Vector3Int(2,6,3), new Vector3Int(3,6,3), new Vector3Int(4,6,3),
                    // Ses deliği
                    new Vector3Int(3,4,2),
                    // Üst gövde (küçük daire)
                    new Vector3Int(3,7,3), new Vector3Int(4,7,3),
                    new Vector3Int(2,8,3), new Vector3Int(3,8,3), new Vector3Int(4,8,3), new Vector3Int(5,8,3),
                    new Vector3Int(3,9,3), new Vector3Int(4,9,3),
                    // Sap (uzun)
                    new Vector3Int(3,10,3), new Vector3Int(4,10,3),
                    new Vector3Int(3,11,3), new Vector3Int(4,11,3),
                    new Vector3Int(3,12,3), new Vector3Int(4,12,3),
                    new Vector3Int(3,13,3), new Vector3Int(4,13,3),
                    // Başlık
                    new Vector3Int(2,14,3), new Vector3Int(3,14,3), new Vector3Int(4,14,3), new Vector3Int(5,14,3),
                    new Vector3Int(2,15,3), new Vector3Int(5,15,3),
                    // Teller (derinlik)
                    new Vector3Int(3,10,2), new Vector3Int(4,10,2),
                    new Vector3Int(3,12,2), new Vector3Int(4,12,2),
                    // Gövde derinlik
                    new Vector3Int(2,3,2), new Vector3Int(4,3,2), new Vector3Int(2,5,2), new Vector3Int(4,5,2),
                    new Vector3Int(3,3,4), new Vector3Int(3,5,4),
                    // Köprü
                    new Vector3Int(3,2,2), new Vector3Int(4,2,2),
                    new Vector3Int(3,6,2), new Vector3Int(4,6,2),
                    // Kayış
                    new Vector3Int(5,6,3), new Vector3Int(6,7,3), new Vector3Int(6,8,3)
                },
                new Vector3Int[] { new Vector3Int(2,15,3), new Vector3Int(5,15,3), new Vector3Int(1,3,3), new Vector3Int(5,3,3), new Vector3Int(3,4,2), new Vector3Int(6,8,3) }),

            // Level 70: Ejderha kafası (75 küp)
            CreateLevelWithFixed(70, "Dragon", 13, "Volcanic dragon",
                new Vector3Int[] {
                    // Çene (alt)
                    new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,0,3),
                    new Vector3Int(5,1,3), new Vector3Int(6,1,3), new Vector3Int(7,1,3), new Vector3Int(8,1,3), new Vector3Int(9,1,3), new Vector3Int(10,1,3),
                    new Vector3Int(10,0,3), new Vector3Int(11,0,3),
                    // Ağız boşluğu
                    new Vector3Int(7,2,3), new Vector3Int(8,2,3), new Vector3Int(9,2,3),
                    // Dişler (üst)
                    new Vector3Int(6,2,3), new Vector3Int(10,2,3),
                    // Üst çene / Burun
                    new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3), new Vector3Int(8,3,3), new Vector3Int(9,3,3), new Vector3Int(10,3,3), new Vector3Int(11,3,3),
                    new Vector3Int(11,2,3), new Vector3Int(12,2,3), new Vector3Int(12,3,3),
                    // Kafa üstü
                    new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3),
                    new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3),
                    // Göz (çukur)
                    new Vector3Int(8,5,3),
                    // Boynuzlar (asimetrik)
                    new Vector3Int(3,7,3), new Vector3Int(2,8,3), new Vector3Int(1,9,3),
                    new Vector3Int(5,7,3), new Vector3Int(5,8,3), new Vector3Int(6,9,3), new Vector3Int(6,10,3),
                    // Derinlik (kalınlık)
                    new Vector3Int(6,1,2), new Vector3Int(7,1,2), new Vector3Int(8,1,2),
                    new Vector3Int(6,3,2), new Vector3Int(7,3,2), new Vector3Int(8,3,2),
                    new Vector3Int(5,4,2), new Vector3Int(6,4,2), new Vector3Int(7,4,2),
                    new Vector3Int(4,5,2), new Vector3Int(5,5,2),
                    new Vector3Int(11,0,2), new Vector3Int(12,2,2),
                    // Ateş (ağızdan, asimetrik)
                    new Vector3Int(13,2,3), new Vector3Int(14,2,3), new Vector3Int(14,3,3),
                    new Vector3Int(13,1,3), new Vector3Int(14,1,3), new Vector3Int(15,2,3),
                    // Boyun
                    new Vector3Int(2,5,3), new Vector3Int(1,4,3), new Vector3Int(1,3,3), new Vector3Int(0,3,3), new Vector3Int(0,2,3)
                },
                new Vector3Int[] { new Vector3Int(15,2,3), new Vector3Int(1,9,3), new Vector3Int(6,10,3), new Vector3Int(0,2,3), new Vector3Int(11,0,3), new Vector3Int(12,3,3) }),

            // ============ BÖLÜM 8: Ustalar (71-80) ============

            // Level 71: Elmas yüzük (68 küp)
            CreateLevelWithFixed(71, "Ring", 12, "Ice crystal ring",
                new Vector3Int[] {
                    // Halka (yatay daire)
                    new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(2,2,1), new Vector3Int(6,2,1),
                    new Vector3Int(1,2,2), new Vector3Int(7,2,2),
                    new Vector3Int(1,2,3), new Vector3Int(7,2,3),
                    new Vector3Int(1,2,4), new Vector3Int(7,2,4),
                    new Vector3Int(2,2,5), new Vector3Int(6,2,5),
                    new Vector3Int(3,2,6), new Vector3Int(4,2,6), new Vector3Int(5,2,6),
                    // Halka kalınlığı
                    new Vector3Int(3,3,0), new Vector3Int(4,3,0), new Vector3Int(5,3,0),
                    new Vector3Int(2,3,1), new Vector3Int(6,3,1),
                    new Vector3Int(1,3,2), new Vector3Int(7,3,2),
                    new Vector3Int(1,3,3), new Vector3Int(7,3,3),
                    new Vector3Int(1,3,4), new Vector3Int(7,3,4),
                    new Vector3Int(2,3,5), new Vector3Int(6,3,5),
                    new Vector3Int(3,3,6), new Vector3Int(4,3,6), new Vector3Int(5,3,6),
                    // Elmas (üstte)
                    new Vector3Int(3,4,0), new Vector3Int(4,4,0), new Vector3Int(5,4,0),
                    new Vector3Int(3,4,1), new Vector3Int(4,4,1), new Vector3Int(5,4,1),
                    new Vector3Int(4,5,0), new Vector3Int(4,5,1),
                    new Vector3Int(3,5,0), new Vector3Int(5,5,0),
                    new Vector3Int(4,6,0),
                    // Elmas alt
                    new Vector3Int(4,3,1), new Vector3Int(3,3,1), new Vector3Int(5,3,1),
                    // İç halka
                    new Vector3Int(3,2,1), new Vector3Int(5,2,1), new Vector3Int(2,2,2), new Vector3Int(6,2,2),
                    new Vector3Int(2,2,4), new Vector3Int(6,2,4), new Vector3Int(3,2,5), new Vector3Int(5,2,5),
                    // Asimetri
                    new Vector3Int(4,1,6), new Vector3Int(4,1,0),
                    new Vector3Int(1,2,1), new Vector3Int(7,2,5),
                    new Vector3Int(0,2,3), new Vector3Int(8,2,3)
                },
                new Vector3Int[] { new Vector3Int(4,6,0), new Vector3Int(0,2,3), new Vector3Int(8,2,3), new Vector3Int(4,1,6), new Vector3Int(4,1,0), new Vector3Int(1,2,1) }),

            // Level 72: Gemi (72 küp)
            CreateLevelWithFixed(72, "Ship", 1, "Ocean ship",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2), new Vector3Int(8,0,2), new Vector3Int(9,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,0,3),
                    new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    // Pruva
                    new Vector3Int(10,0,2), new Vector3Int(10,0,3), new Vector3Int(11,0,3), new Vector3Int(12,0,3),
                    // Kabin
                    new Vector3Int(3,1,2), new Vector3Int(4,1,2), new Vector3Int(5,1,2), new Vector3Int(6,1,2),
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3),
                    new Vector3Int(3,2,2), new Vector3Int(4,2,2), new Vector3Int(5,2,2), new Vector3Int(6,2,2),
                    new Vector3Int(3,2,3), new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3),
                    // Baca
                    new Vector3Int(5,3,2), new Vector3Int(5,3,3), new Vector3Int(5,4,2), new Vector3Int(5,4,3), new Vector3Int(5,5,3),
                    // Direk
                    new Vector3Int(8,1,3), new Vector3Int(8,2,3), new Vector3Int(8,3,3), new Vector3Int(8,4,3), new Vector3Int(8,5,3), new Vector3Int(8,6,3),
                    // Yelken
                    new Vector3Int(9,3,3), new Vector3Int(9,4,3), new Vector3Int(9,5,3), new Vector3Int(9,6,3),
                    new Vector3Int(10,4,3), new Vector3Int(10,5,3),
                    new Vector3Int(7,5,3), new Vector3Int(7,6,3),
                    // Dümen
                    new Vector3Int(2,1,3), new Vector3Int(1,1,3),
                    new Vector3Int(2,1,2), new Vector3Int(2,2,2),
                    // Dalga
                    new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(11,0,4), new Vector3Int(13,0,3)
                },
                new Vector3Int[] { new Vector3Int(12,0,3), new Vector3Int(13,0,3), new Vector3Int(5,5,3), new Vector3Int(8,6,3), new Vector3Int(1,1,3), new Vector3Int(1,0,4) }),

            // Level 73: Masa lambası (58 küp)
            CreateLevelWithFixed(73, "Lamp", 11, "Golden desk lamp",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4),
                    // Sap
                    new Vector3Int(4,1,3), new Vector3Int(4,2,3), new Vector3Int(4,3,3),
                    new Vector3Int(4,1,2), new Vector3Int(4,2,2),
                    // Abajur
                    new Vector3Int(1,4,2), new Vector3Int(2,4,2), new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(5,4,2), new Vector3Int(6,4,2), new Vector3Int(7,4,2),
                    new Vector3Int(1,4,3), new Vector3Int(2,4,3), new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    new Vector3Int(2,4,4), new Vector3Int(3,4,4), new Vector3Int(4,4,4), new Vector3Int(5,4,4), new Vector3Int(6,4,4),
                    // Abajur üstü
                    new Vector3Int(2,5,2), new Vector3Int(3,5,2), new Vector3Int(4,5,2), new Vector3Int(5,5,2), new Vector3Int(6,5,2),
                    new Vector3Int(2,5,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3),
                    new Vector3Int(3,6,2), new Vector3Int(4,6,2), new Vector3Int(5,6,2),
                    new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3),
                    new Vector3Int(4,7,3),
                    // Işık huzmesi
                    new Vector3Int(0,3,3), new Vector3Int(8,3,3)
                },
                new Vector3Int[] { new Vector3Int(2,0,2), new Vector3Int(6,0,2), new Vector3Int(4,7,3), new Vector3Int(0,3,3), new Vector3Int(8,3,3), new Vector3Int(1,4,2) }),

            // Level 74: Küp dünya (30 küp) - Grid level
            CreateLevelWithFixed(74, "Cube World", 2, "Spring world",
                GenerateHollowBox(4, 4, 4),
                GetCorners(4, 4, 4)),

            // Level 75: Akrep (72 küp)
            CreateLevelWithFixed(75, "Scorpion", 13, "Volcanic scorpion",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3),
                    new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    new Vector3Int(5,0,5), new Vector3Int(6,0,5), new Vector3Int(7,0,5),
                    new Vector3Int(5,1,4), new Vector3Int(6,1,4), new Vector3Int(7,1,4),
                    // Baş
                    new Vector3Int(8,0,3), new Vector3Int(9,0,3), new Vector3Int(9,0,4), new Vector3Int(9,0,5), new Vector3Int(8,0,5),
                    new Vector3Int(10,0,3), new Vector3Int(10,0,5),
                    // Kıskaçlar
                    new Vector3Int(11,0,2), new Vector3Int(11,0,3), new Vector3Int(12,0,2), new Vector3Int(12,0,1),
                    new Vector3Int(11,0,5), new Vector3Int(11,0,6), new Vector3Int(12,0,6), new Vector3Int(12,0,7),
                    new Vector3Int(13,0,1), new Vector3Int(13,0,7),
                    // Kuyruk
                    new Vector3Int(4,0,3), new Vector3Int(3,0,3), new Vector3Int(2,0,3),
                    new Vector3Int(1,0,3), new Vector3Int(1,1,3), new Vector3Int(1,2,3),
                    new Vector3Int(1,3,3), new Vector3Int(2,3,3), new Vector3Int(2,4,3),
                    new Vector3Int(3,4,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3),
                    // İğne
                    new Vector3Int(5,5,3), new Vector3Int(5,5,4), new Vector3Int(6,5,3),
                    // Bacaklar sol
                    new Vector3Int(5,0,2), new Vector3Int(5,0,1),
                    new Vector3Int(6,0,2), new Vector3Int(6,0,1),
                    new Vector3Int(7,0,2), new Vector3Int(7,0,1),
                    new Vector3Int(8,0,2),
                    // Bacaklar sağ
                    new Vector3Int(5,0,6), new Vector3Int(5,0,7),
                    new Vector3Int(6,0,6), new Vector3Int(6,0,7),
                    new Vector3Int(7,0,6), new Vector3Int(7,0,7),
                    new Vector3Int(8,0,6),
                    // Derinlik
                    new Vector3Int(6,1,3), new Vector3Int(6,1,5),
                    new Vector3Int(1,2,2), new Vector3Int(3,4,2), new Vector3Int(5,5,2)
                },
                new Vector3Int[] { new Vector3Int(13,0,1), new Vector3Int(13,0,7), new Vector3Int(6,5,3), new Vector3Int(5,0,1), new Vector3Int(5,0,7), new Vector3Int(1,0,3) }),

            // Level 76: Kafatası (75 küp)
            CreateLevelWithFixed(76, "Skull", 9, "Night skull",
                new Vector3Int[] {
                    // Alt çene
                    new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3),
                    new Vector3Int(3,1,3), new Vector3Int(7,1,3),
                    // Dişler
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3),
                    // Zigomatik
                    new Vector3Int(2,2,3), new Vector3Int(3,2,3), new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3), new Vector3Int(7,2,3), new Vector3Int(8,2,3),
                    // Göz çevresi
                    new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    // Burun
                    new Vector3Int(5,3,3),
                    // Alın
                    new Vector3Int(2,5,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3), new Vector3Int(8,5,3),
                    new Vector3Int(2,6,3), new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(6,6,3), new Vector3Int(7,6,3), new Vector3Int(8,6,3),
                    new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(5,7,3), new Vector3Int(6,7,3), new Vector3Int(7,7,3),
                    new Vector3Int(4,8,3), new Vector3Int(5,8,3), new Vector3Int(6,8,3),
                    // Derinlik
                    new Vector3Int(3,3,2), new Vector3Int(7,3,2),
                    new Vector3Int(3,5,2), new Vector3Int(7,5,2),
                    new Vector3Int(4,6,2), new Vector3Int(5,6,2), new Vector3Int(6,6,2),
                    new Vector3Int(5,7,2), new Vector3Int(2,2,2), new Vector3Int(8,2,2),
                    // Yan kemikler
                    new Vector3Int(1,3,3), new Vector3Int(1,4,3), new Vector3Int(1,5,3),
                    new Vector3Int(9,3,3), new Vector3Int(9,4,3),
                    // Çapraz kemikler
                    new Vector3Int(2,0,2), new Vector3Int(1,0,1), new Vector3Int(8,0,4), new Vector3Int(9,0,5),
                    new Vector3Int(2,0,4), new Vector3Int(1,0,5), new Vector3Int(8,0,2), new Vector3Int(9,0,1)
                },
                new Vector3Int[] { new Vector3Int(5,8,3), new Vector3Int(1,0,1), new Vector3Int(9,0,1), new Vector3Int(1,0,5), new Vector3Int(9,0,5), new Vector3Int(5,3,3) }),

            // Level 77: Robot (78 küp)
            CreateLevelWithFixed(77, "Robot", 12, "Ice robot",
                new Vector3Int[] {
                    // Ayaklar
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(2,0,4), new Vector3Int(3,0,4),
                    new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(6,0,4), new Vector3Int(7,0,4),
                    // Bacaklar
                    new Vector3Int(2,1,3), new Vector3Int(3,1,3), new Vector3Int(6,1,3), new Vector3Int(7,1,3),
                    new Vector3Int(2,2,3), new Vector3Int(3,2,3), new Vector3Int(6,2,3), new Vector3Int(7,2,3),
                    // Gövde
                    new Vector3Int(1,3,2), new Vector3Int(2,3,2), new Vector3Int(3,3,2), new Vector3Int(4,3,2), new Vector3Int(5,3,2), new Vector3Int(6,3,2), new Vector3Int(7,3,2), new Vector3Int(8,3,2),
                    new Vector3Int(1,3,3), new Vector3Int(2,3,3), new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3), new Vector3Int(8,3,3),
                    new Vector3Int(1,3,4), new Vector3Int(2,3,4), new Vector3Int(3,3,4), new Vector3Int(4,3,4), new Vector3Int(5,3,4), new Vector3Int(6,3,4), new Vector3Int(7,3,4), new Vector3Int(8,3,4),
                    new Vector3Int(2,4,3), new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    new Vector3Int(2,4,2), new Vector3Int(7,4,2), new Vector3Int(2,4,4), new Vector3Int(7,4,4),
                    // Boyun
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3),
                    // Baş
                    new Vector3Int(3,6,2), new Vector3Int(4,6,2), new Vector3Int(5,6,2), new Vector3Int(6,6,2),
                    new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(6,6,3),
                    new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(5,7,3), new Vector3Int(6,7,3),
                    // Anten
                    new Vector3Int(4,8,3), new Vector3Int(5,8,3), new Vector3Int(4,9,3),
                    // Kollar (asimetrik)
                    new Vector3Int(0,3,3), new Vector3Int(0,2,3), new Vector3Int(0,1,3),
                    new Vector3Int(9,3,3), new Vector3Int(9,4,3), new Vector3Int(10,4,3)
                },
                new Vector3Int[] { new Vector3Int(2,0,3), new Vector3Int(7,0,4), new Vector3Int(4,9,3), new Vector3Int(0,1,3), new Vector3Int(10,4,3), new Vector3Int(1,3,2) }),

            // Level 78: Kedi (70 küp)
            CreateLevelWithFixed(78, "Cat", 14, "Cherry blossom cat",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3), new Vector3Int(7,1,3),
                    new Vector3Int(3,1,4), new Vector3Int(4,1,4), new Vector3Int(5,1,4), new Vector3Int(6,1,4), new Vector3Int(7,1,4),
                    new Vector3Int(3,2,3), new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3), new Vector3Int(7,2,3),
                    new Vector3Int(3,2,4), new Vector3Int(4,2,4), new Vector3Int(5,2,4), new Vector3Int(6,2,4), new Vector3Int(7,2,4),
                    // Ön bacaklar
                    new Vector3Int(7,0,3), new Vector3Int(7,0,4), new Vector3Int(8,0,3), new Vector3Int(8,0,4),
                    // Arka bacaklar
                    new Vector3Int(3,0,3), new Vector3Int(3,0,4), new Vector3Int(3,0,5),
                    new Vector3Int(2,0,3), new Vector3Int(2,0,4),
                    // Baş
                    new Vector3Int(8,2,3), new Vector3Int(9,2,3), new Vector3Int(8,2,4), new Vector3Int(9,2,4),
                    new Vector3Int(8,3,3), new Vector3Int(9,3,3), new Vector3Int(8,3,4), new Vector3Int(9,3,4),
                    new Vector3Int(10,3,3), new Vector3Int(10,3,4),
                    // Kulaklar
                    new Vector3Int(8,4,3), new Vector3Int(8,5,3),
                    new Vector3Int(9,4,4), new Vector3Int(9,4,3),
                    // Burun / Bıyıklar
                    new Vector3Int(10,2,3), new Vector3Int(10,2,4),
                    new Vector3Int(11,3,3), new Vector3Int(11,2,4),
                    // Kuyruk
                    new Vector3Int(2,1,3), new Vector3Int(1,1,3), new Vector3Int(1,2,3), new Vector3Int(1,3,3),
                    new Vector3Int(1,4,3), new Vector3Int(2,4,3), new Vector3Int(2,5,3),
                    // Göz / Detaylar
                    new Vector3Int(9,3,2), new Vector3Int(8,0,2), new Vector3Int(2,0,5),
                    new Vector3Int(5,1,5), new Vector3Int(6,1,5),
                    new Vector3Int(7,3,3), new Vector3Int(7,3,4)
                },
                new Vector3Int[] { new Vector3Int(8,5,3), new Vector3Int(9,3,2), new Vector3Int(11,3,3), new Vector3Int(2,5,3), new Vector3Int(8,0,2), new Vector3Int(2,0,5) }),

            // Level 79: Yeldeğirmeni (68 küp)
            CreateLevelWithFixed(79, "Windmill", 2, "Spring windmill",
                new Vector3Int[] {
                    // Gövde (kule)
                    new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(4,0,4), new Vector3Int(5,0,4),
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(4,1,4), new Vector3Int(5,1,4),
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(4,2,4), new Vector3Int(5,2,4),
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(4,3,4), new Vector3Int(5,3,4),
                    new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(4,4,4),
                    // Çatı
                    new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3),
                    new Vector3Int(3,5,4), new Vector3Int(4,5,4), new Vector3Int(5,5,4), new Vector3Int(6,5,4),
                    new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(4,6,4), new Vector3Int(5,6,4),
                    new Vector3Int(5,7,3),
                    // Kanatlar
                    new Vector3Int(3,4,3), new Vector3Int(2,5,3), new Vector3Int(1,6,3), new Vector3Int(0,7,3),
                    new Vector3Int(3,4,4), new Vector3Int(2,5,4),
                    new Vector3Int(6,4,3), new Vector3Int(7,5,3), new Vector3Int(8,6,3), new Vector3Int(9,7,3),
                    new Vector3Int(6,4,4), new Vector3Int(7,5,4),
                    new Vector3Int(3,2,3), new Vector3Int(2,1,3), new Vector3Int(1,0,3),
                    new Vector3Int(6,2,3), new Vector3Int(7,1,3),
                    // Kapı / Pencere
                    new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(5,3,2),
                    // Bayrak
                    new Vector3Int(5,8,3), new Vector3Int(5,8,4),
                    // Taban
                    new Vector3Int(3,0,3), new Vector3Int(6,0,3), new Vector3Int(3,0,4), new Vector3Int(6,0,4)
                },
                new Vector3Int[] { new Vector3Int(0,7,3), new Vector3Int(9,7,3), new Vector3Int(5,8,3), new Vector3Int(1,0,3), new Vector3Int(5,0,2), new Vector3Int(5,3,2) }),

            // Level 80: Kristal mağara (85 küp)
            CreateLevelWithFixed(80, "Crystal Cave", 15, "Emerald crystal cave",
                new Vector3Int[] {
                    // Mağara tabanı
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2), new Vector3Int(8,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3), new Vector3Int(9,0,3),
                    new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    new Vector3Int(2,0,5), new Vector3Int(3,0,5), new Vector3Int(4,0,5), new Vector3Int(5,0,5), new Vector3Int(6,0,5),
                    // Sol duvar
                    new Vector3Int(0,1,2), new Vector3Int(0,2,2), new Vector3Int(0,3,2), new Vector3Int(0,4,2),
                    new Vector3Int(0,1,3), new Vector3Int(0,2,3), new Vector3Int(0,3,3),
                    // Sağ duvar
                    new Vector3Int(9,1,3), new Vector3Int(9,2,3), new Vector3Int(9,3,3),
                    new Vector3Int(8,1,2), new Vector3Int(8,2,2),
                    // Tavan
                    new Vector3Int(0,5,2), new Vector3Int(1,5,2), new Vector3Int(2,5,2), new Vector3Int(3,5,2),
                    new Vector3Int(0,5,3), new Vector3Int(1,5,3), new Vector3Int(7,5,3), new Vector3Int(8,5,3),
                    new Vector3Int(8,4,2), new Vector3Int(8,4,3),
                    // Sarkıtlar
                    new Vector3Int(2,4,2), new Vector3Int(2,4,3),
                    new Vector3Int(6,4,3), new Vector3Int(6,3,3),
                    new Vector3Int(4,4,2),
                    // Dikitler
                    new Vector3Int(3,1,3), new Vector3Int(3,2,3),
                    new Vector3Int(5,1,4), new Vector3Int(5,2,4), new Vector3Int(5,3,4),
                    new Vector3Int(7,1,3), new Vector3Int(7,2,3), new Vector3Int(7,3,3),
                    // Büyük kristal
                    new Vector3Int(4,1,3), new Vector3Int(4,2,3), new Vector3Int(4,3,3), new Vector3Int(4,4,3),
                    // Hazine
                    new Vector3Int(1,1,4), new Vector3Int(2,1,4), new Vector3Int(3,1,4),
                    // Giriş
                    new Vector3Int(9,0,2), new Vector3Int(10,0,3), new Vector3Int(10,0,2)
                },
                new Vector3Int[] { new Vector3Int(0,5,2), new Vector3Int(8,5,3), new Vector3Int(5,3,4), new Vector3Int(4,4,3), new Vector3Int(10,0,3), new Vector3Int(1,1,4) }),

            // ============ BÖLÜM 9: Efsane (81-90) ============

            // Level 81: Tavus kuşu (80 küp)
            CreateLevelWithFixed(81, "Peacock", 10, "Coral peacock",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(5,2,5), new Vector3Int(6,2,5), new Vector3Int(5,3,5), new Vector3Int(6,3,5),
                    new Vector3Int(5,2,6), new Vector3Int(6,2,6),
                    // Boyun
                    new Vector3Int(6,4,5), new Vector3Int(6,5,5), new Vector3Int(6,6,5),
                    // Baş
                    new Vector3Int(6,7,5), new Vector3Int(7,7,5), new Vector3Int(6,7,4),
                    // Gaga
                    new Vector3Int(8,7,5),
                    // Tepelik
                    new Vector3Int(6,8,5), new Vector3Int(6,9,5),
                    // Bacaklar
                    new Vector3Int(5,1,5), new Vector3Int(5,0,5), new Vector3Int(6,1,5), new Vector3Int(6,0,5),
                    // Kuyruk yelpazesi (düz, geniş, asimetrik)
                    new Vector3Int(4,2,5), new Vector3Int(3,3,5), new Vector3Int(2,4,5), new Vector3Int(1,5,5),
                    new Vector3Int(4,3,5), new Vector3Int(3,4,5), new Vector3Int(2,5,5),
                    new Vector3Int(4,4,5), new Vector3Int(3,5,5),
                    new Vector3Int(0,6,5), new Vector3Int(1,6,5), new Vector3Int(2,6,5), new Vector3Int(3,6,5), new Vector3Int(4,5,5),
                    new Vector3Int(0,7,5), new Vector3Int(1,7,5), new Vector3Int(2,7,5), new Vector3Int(3,7,5), new Vector3Int(4,6,5),
                    new Vector3Int(1,8,5), new Vector3Int(2,8,5), new Vector3Int(3,8,5),
                    new Vector3Int(2,9,5), new Vector3Int(3,9,5),
                    // Kuyruk gözleri (3D)
                    new Vector3Int(0,6,4), new Vector3Int(2,7,4), new Vector3Int(4,5,4),
                    new Vector3Int(1,8,4), new Vector3Int(3,9,4),
                    new Vector3Int(0,7,6), new Vector3Int(2,8,6), new Vector3Int(4,6,6),
                    // Kanat
                    new Vector3Int(5,3,4), new Vector3Int(5,3,6), new Vector3Int(5,4,4), new Vector3Int(5,4,6),
                    new Vector3Int(4,3,4), new Vector3Int(4,3,6),
                    // İç gövde
                    new Vector3Int(5,2,4), new Vector3Int(6,2,4),
                    // Kuyruk uçları
                    new Vector3Int(0,5,5), new Vector3Int(0,8,5), new Vector3Int(1,9,5),
                    new Vector3Int(4,7,5), new Vector3Int(5,5,5)
                },
                new Vector3Int[] { new Vector3Int(8,7,5), new Vector3Int(6,9,5), new Vector3Int(0,5,5), new Vector3Int(0,8,5), new Vector3Int(3,9,4), new Vector3Int(4,6,6) }),

            // Level 82: Basamaklı grid (54 küp) - Grid level
            CreateLevelWithFixed(82, "Pyramid Grid", 4, "Desert steps",
                GenerateSteppedPyramid(),
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(6,0,0), new Vector3Int(0,0,6), new Vector3Int(6,0,6), new Vector3Int(3,3,3) }),

            // Level 83: Geyik (78 küp)
            CreateLevelWithFixed(83, "Deer", 6, "Autumn deer",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3),
                    new Vector3Int(3,3,4), new Vector3Int(4,3,4), new Vector3Int(5,3,4), new Vector3Int(6,3,4), new Vector3Int(7,3,4),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    new Vector3Int(4,4,4), new Vector3Int(5,4,4), new Vector3Int(6,4,4),
                    // Ön bacaklar
                    new Vector3Int(7,2,3), new Vector3Int(7,1,3), new Vector3Int(7,0,3),
                    new Vector3Int(7,2,4), new Vector3Int(7,1,4), new Vector3Int(7,0,4),
                    // Arka bacaklar (daha kalın)
                    new Vector3Int(3,2,3), new Vector3Int(3,1,3), new Vector3Int(3,0,3),
                    new Vector3Int(3,2,4), new Vector3Int(3,1,4), new Vector3Int(3,0,4),
                    new Vector3Int(2,2,3), new Vector3Int(2,1,3), new Vector3Int(2,0,3),
                    // Boyun
                    new Vector3Int(8,4,3), new Vector3Int(8,5,3), new Vector3Int(8,4,4),
                    // Baş
                    new Vector3Int(8,6,3), new Vector3Int(9,6,3), new Vector3Int(8,6,4), new Vector3Int(9,6,4),
                    new Vector3Int(9,7,3), new Vector3Int(9,7,4),
                    // Burun
                    new Vector3Int(10,6,3), new Vector3Int(10,6,4),
                    // Boynuz sol (yükselen, dallanan)
                    new Vector3Int(8,7,3), new Vector3Int(8,8,3), new Vector3Int(7,9,3), new Vector3Int(7,10,3),
                    new Vector3Int(9,8,3), new Vector3Int(9,9,3),
                    new Vector3Int(6,10,3), new Vector3Int(10,9,3),
                    // Boynuz sağ (farklı)
                    new Vector3Int(8,7,4), new Vector3Int(8,8,4), new Vector3Int(7,9,4),
                    new Vector3Int(9,8,4), new Vector3Int(9,9,4), new Vector3Int(10,9,4),
                    new Vector3Int(6,9,4),
                    // Kuyruk
                    new Vector3Int(2,4,3), new Vector3Int(1,4,3), new Vector3Int(1,5,3),
                    // Karın
                    new Vector3Int(4,3,2), new Vector3Int(5,3,2), new Vector3Int(6,3,2),
                    // Göz
                    new Vector3Int(9,7,2),
                    // Toynak
                    new Vector3Int(8,0,3), new Vector3Int(2,0,4)
                },
                new Vector3Int[] { new Vector3Int(7,10,3), new Vector3Int(6,10,3), new Vector3Int(10,9,4), new Vector3Int(10,6,3), new Vector3Int(1,5,3), new Vector3Int(9,7,2) }),

            // Level 84: Uçurtma (55 küp)
            CreateLevelWithFixed(84, "Kite", 7, "Tropical kite",
                new Vector3Int[] {
                    // Baklava şekli (merkez)
                    new Vector3Int(5,4,3),
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3),
                    new Vector3Int(3,2,3), new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3), new Vector3Int(7,2,3),
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(6,1,3),
                    new Vector3Int(5,0,3),
                    // Üst genişleme
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3),
                    new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(6,6,3), new Vector3Int(7,6,3),
                    new Vector3Int(4,7,3), new Vector3Int(5,7,3), new Vector3Int(6,7,3),
                    new Vector3Int(5,8,3),
                    // Derinlik (çerçeve)
                    new Vector3Int(5,4,2), new Vector3Int(5,0,2), new Vector3Int(5,8,2),
                    new Vector3Int(3,2,2), new Vector3Int(7,2,2), new Vector3Int(3,6,2), new Vector3Int(7,6,2),
                    // Kuyruk ip (aşağı sarkan)
                    new Vector3Int(5,0,4), new Vector3Int(5,0,5), new Vector3Int(4,0,6), new Vector3Int(4,0,7),
                    new Vector3Int(5,0,7), new Vector3Int(5,0,8),
                    // Kuyruk fiyonkları
                    new Vector3Int(3,0,6), new Vector3Int(6,0,5), new Vector3Int(3,0,8), new Vector3Int(6,0,7),
                    // Çerçeve çubuğu
                    new Vector3Int(5,4,4), new Vector3Int(5,0,4),
                    new Vector3Int(2,2,3), new Vector3Int(8,2,3), new Vector3Int(2,6,3), new Vector3Int(8,6,3),
                    // İç süsleme
                    new Vector3Int(4,3,2), new Vector3Int(6,3,2), new Vector3Int(4,5,2), new Vector3Int(6,5,2),
                    new Vector3Int(5,2,4), new Vector3Int(5,6,4)
                },
                new Vector3Int[] { new Vector3Int(5,8,3), new Vector3Int(5,0,8), new Vector3Int(2,2,3), new Vector3Int(8,2,3), new Vector3Int(3,0,8), new Vector3Int(6,0,7) }),

            // Level 85: Kadeh (60 küp)
            CreateLevelWithFixed(85, "Goblet", 11, "Golden goblet",
                new Vector3Int[] {
                    // Taban (geniş)
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4),
                    // Sap
                    new Vector3Int(4,1,3), new Vector3Int(4,2,3), new Vector3Int(4,3,3),
                    // Kase (genişleyen)
                    new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(5,4,2),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3),
                    new Vector3Int(3,4,4), new Vector3Int(4,4,4), new Vector3Int(5,4,4),
                    new Vector3Int(2,5,1), new Vector3Int(3,5,1), new Vector3Int(4,5,1), new Vector3Int(5,5,1), new Vector3Int(6,5,1),
                    new Vector3Int(2,5,2), new Vector3Int(6,5,2),
                    new Vector3Int(2,5,3), new Vector3Int(6,5,3),
                    new Vector3Int(2,5,4), new Vector3Int(6,5,4),
                    new Vector3Int(2,5,5), new Vector3Int(3,5,5), new Vector3Int(4,5,5), new Vector3Int(5,5,5), new Vector3Int(6,5,5),
                    // Ağız (en geniş)
                    new Vector3Int(1,6,1), new Vector3Int(2,6,1), new Vector3Int(3,6,1), new Vector3Int(4,6,1), new Vector3Int(5,6,1), new Vector3Int(6,6,1), new Vector3Int(7,6,1),
                    new Vector3Int(1,6,5), new Vector3Int(2,6,5), new Vector3Int(3,6,5), new Vector3Int(4,6,5), new Vector3Int(5,6,5), new Vector3Int(6,6,5), new Vector3Int(7,6,5)
                },
                new Vector3Int[] { new Vector3Int(2,0,2), new Vector3Int(6,0,4), new Vector3Int(1,6,1), new Vector3Int(7,6,1), new Vector3Int(1,6,5), new Vector3Int(7,6,5) }),

            // Level 86: Çift sarmal DNA (82 küp)
            CreateLevelWithFixed(86, "DNA", 8, "Lavender DNA helix",
                new Vector3Int[] {
                    // Sol sarmal
                    new Vector3Int(2,0,3), new Vector3Int(1,1,3), new Vector3Int(1,2,3), new Vector3Int(2,3,3),
                    new Vector3Int(3,4,3), new Vector3Int(3,5,3), new Vector3Int(2,6,3), new Vector3Int(1,7,3),
                    new Vector3Int(1,8,3), new Vector3Int(2,9,3), new Vector3Int(3,10,3), new Vector3Int(3,11,3),
                    new Vector3Int(2,12,3), new Vector3Int(1,13,3),
                    // Sağ sarmal
                    new Vector3Int(6,0,3), new Vector3Int(7,1,3), new Vector3Int(7,2,3), new Vector3Int(6,3,3),
                    new Vector3Int(5,4,3), new Vector3Int(5,5,3), new Vector3Int(6,6,3), new Vector3Int(7,7,3),
                    new Vector3Int(7,8,3), new Vector3Int(6,9,3), new Vector3Int(5,10,3), new Vector3Int(5,11,3),
                    new Vector3Int(6,12,3), new Vector3Int(7,13,3),
                    // Bağlantı çubukları (nükleotidler)
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(5,1,3),
                    new Vector3Int(3,3,3), new Vector3Int(4,3,3), new Vector3Int(5,3,3),
                    new Vector3Int(4,5,3),
                    new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(5,7,3),
                    new Vector3Int(3,9,3), new Vector3Int(4,9,3), new Vector3Int(5,9,3),
                    new Vector3Int(4,11,3),
                    new Vector3Int(3,13,3), new Vector3Int(4,13,3), new Vector3Int(5,13,3),
                    // Derinlik (3D efekt)
                    new Vector3Int(2,0,2), new Vector3Int(6,0,4),
                    new Vector3Int(1,2,2), new Vector3Int(7,2,4),
                    new Vector3Int(3,4,2), new Vector3Int(5,4,4),
                    new Vector3Int(1,7,2), new Vector3Int(7,7,4),
                    new Vector3Int(2,9,2), new Vector3Int(6,9,4),
                    new Vector3Int(3,10,2), new Vector3Int(5,10,4),
                    new Vector3Int(1,13,2), new Vector3Int(7,13,4),
                    // Fosfat grubu (uçlarda)
                    new Vector3Int(2,0,4), new Vector3Int(6,0,2),
                    new Vector3Int(1,13,4), new Vector3Int(7,13,2),
                    // Ek bağlar
                    new Vector3Int(4,1,2), new Vector3Int(4,7,2), new Vector3Int(4,13,2),
                    new Vector3Int(4,3,4), new Vector3Int(4,9,4)
                },
                new Vector3Int[] { new Vector3Int(2,0,3), new Vector3Int(6,0,3), new Vector3Int(1,13,3), new Vector3Int(7,13,3), new Vector3Int(4,5,3), new Vector3Int(4,11,3) }),

            // Level 87: Ev (65 küp)
            CreateLevelWithFixed(87, "House", 0, "Sunny house",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(5,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3),
                    // 1. kat duvarlar
                    new Vector3Int(0,1,0), new Vector3Int(5,1,0),
                    new Vector3Int(0,1,3), new Vector3Int(5,1,3),
                    new Vector3Int(0,1,1), new Vector3Int(0,1,2),
                    new Vector3Int(5,1,1), new Vector3Int(5,1,2),
                    // Kapı
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    // Pencereler
                    new Vector3Int(1,1,0), new Vector3Int(4,1,0),
                    // 2. kat
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(0,2,3), new Vector3Int(1,2,3), new Vector3Int(2,2,3), new Vector3Int(3,2,3), new Vector3Int(4,2,3), new Vector3Int(5,2,3),
                    new Vector3Int(0,2,1), new Vector3Int(5,2,1),
                    // Çatı (üçgen)
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(1,3,3), new Vector3Int(2,3,3), new Vector3Int(3,3,3), new Vector3Int(4,3,3),
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0),
                    new Vector3Int(2,4,3), new Vector3Int(3,4,3),
                    // Baca (asimetrik)
                    new Vector3Int(4,4,0), new Vector3Int(4,5,0),
                    // Bahçe (asimetrik)
                    new Vector3Int(6,0,0), new Vector3Int(7,0,0), new Vector3Int(6,0,1), new Vector3Int(7,0,1),
                    // Ağaç
                    new Vector3Int(7,1,0), new Vector3Int(7,2,0), new Vector3Int(6,3,0), new Vector3Int(7,3,0), new Vector3Int(8,3,0)
                },
                new Vector3Int[] { new Vector3Int(4,5,0), new Vector3Int(8,3,0), new Vector3Int(0,0,0), new Vector3Int(5,0,3), new Vector3Int(2,4,0), new Vector3Int(3,4,3) }),

            // Level 88: Kelebek balığı (72 küp)
            CreateLevelWithFixed(88, "Butterflyfish", 7, "Tropical butterflyfish",
                new Vector3Int[] {
                    // Disk gövde (düz, geniş)
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(6,3,3),
                    new Vector3Int(3,4,3), new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    new Vector3Int(2,5,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3), new Vector3Int(8,5,3),
                    new Vector3Int(2,6,3), new Vector3Int(3,6,3), new Vector3Int(4,6,3), new Vector3Int(5,6,3), new Vector3Int(6,6,3), new Vector3Int(7,6,3), new Vector3Int(8,6,3),
                    new Vector3Int(2,7,3), new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(5,7,3), new Vector3Int(6,7,3), new Vector3Int(7,7,3), new Vector3Int(8,7,3),
                    new Vector3Int(3,8,3), new Vector3Int(4,8,3), new Vector3Int(5,8,3), new Vector3Int(6,8,3), new Vector3Int(7,8,3),
                    new Vector3Int(4,9,3), new Vector3Int(5,9,3), new Vector3Int(6,9,3),
                    // Ağız (uzun burun)
                    new Vector3Int(8,4,3), new Vector3Int(9,4,3), new Vector3Int(9,5,3),
                    // Göz
                    new Vector3Int(7,7,2),
                    // Kuyruk
                    new Vector3Int(1,5,3), new Vector3Int(0,4,3), new Vector3Int(0,6,3),
                    // Üst yüzgeç (asimetrik)
                    new Vector3Int(5,10,3), new Vector3Int(4,10,3), new Vector3Int(4,11,3),
                    // Alt yüzgeç
                    new Vector3Int(5,2,3), new Vector3Int(4,2,3),
                    // Derinlik
                    new Vector3Int(4,5,2), new Vector3Int(6,5,2), new Vector3Int(5,7,2), new Vector3Int(5,5,4),
                    new Vector3Int(4,7,2), new Vector3Int(6,7,2), new Vector3Int(4,7,4), new Vector3Int(6,7,4),
                    // Bant deseni
                    new Vector3Int(3,5,2), new Vector3Int(7,5,2), new Vector3Int(3,7,4), new Vector3Int(7,7,4),
                    // Çizgi
                    new Vector3Int(6,4,2), new Vector3Int(6,6,2), new Vector3Int(6,8,2)
                },
                new Vector3Int[] { new Vector3Int(9,4,3), new Vector3Int(7,7,2), new Vector3Int(0,4,3), new Vector3Int(0,6,3), new Vector3Int(4,11,3), new Vector3Int(4,2,3) }),

            // Level 89: Şato (88 küp)
            CreateLevelWithFixed(89, "Château", 3, "Purple château",
                new Vector3Int[] {
                    // Ana bina
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2), new Vector3Int(8,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3),
                    new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4), new Vector3Int(6,0,4), new Vector3Int(7,0,4), new Vector3Int(8,0,4),
                    // 1. kat duvarlar
                    new Vector3Int(2,1,2), new Vector3Int(8,1,2), new Vector3Int(2,1,4), new Vector3Int(8,1,4),
                    new Vector3Int(2,1,3), new Vector3Int(8,1,3),
                    new Vector3Int(4,1,2), new Vector3Int(6,1,2), // Pencereler
                    // 2. kat
                    new Vector3Int(2,2,2), new Vector3Int(3,2,2), new Vector3Int(4,2,2), new Vector3Int(5,2,2), new Vector3Int(6,2,2), new Vector3Int(7,2,2), new Vector3Int(8,2,2),
                    new Vector3Int(2,2,4), new Vector3Int(8,2,4),
                    // Kuleler (4 köşe, farklı yüksek)
                    new Vector3Int(2,3,2), new Vector3Int(2,4,2), new Vector3Int(2,5,2), new Vector3Int(2,6,2), // Sol ön (en yüksek)
                    new Vector3Int(8,3,2), new Vector3Int(8,4,2), new Vector3Int(8,5,2),                       // Sağ ön
                    new Vector3Int(2,3,4), new Vector3Int(2,4,4), new Vector3Int(2,5,4), new Vector3Int(2,6,4), new Vector3Int(2,7,4), // Sol arka (en yüksek)
                    new Vector3Int(8,3,4), new Vector3Int(8,4,4), new Vector3Int(8,5,4), new Vector3Int(8,6,4), // Sağ arka
                    // Burçlar
                    new Vector3Int(1,6,2), new Vector3Int(3,6,2),
                    new Vector3Int(1,7,4), new Vector3Int(3,7,4),
                    new Vector3Int(7,5,2), new Vector3Int(9,5,2),
                    new Vector3Int(7,6,4), new Vector3Int(9,6,4),
                    // Ana kapı
                    new Vector3Int(5,1,2), new Vector3Int(5,2,2), new Vector3Int(5,3,2),
                    // İç avlu
                    new Vector3Int(5,1,3), new Vector3Int(5,1,4),
                    // Sur (ön)
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(9,0,2), new Vector3Int(10,0,2),
                    new Vector3Int(0,1,2), new Vector3Int(10,1,2),
                    // Bayrak
                    new Vector3Int(2,8,4), new Vector3Int(2,8,3)
                },
                new Vector3Int[] { new Vector3Int(2,8,4), new Vector3Int(2,8,3), new Vector3Int(1,6,2), new Vector3Int(9,5,2), new Vector3Int(0,0,2), new Vector3Int(10,0,2) }),

            // Level 90: Kozmos (100 küp) - Boss
            CreateLevelWithFixed(90, "Cosmos", 5, "Cosmic nebula",
                GenerateCosmicNebula(),
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(12,0,0), new Vector3Int(0,0,10),
                    new Vector3Int(12,0,10), new Vector3Int(6,6,5), new Vector3Int(6,0,5)
                }),

            // ============ BÖLÜM 10: Efsanevi (91-100) ============

            // Level 91: Phoenix (85 küp) - Anka kuşu
            CreateLevelWithFixed(91, "Phoenix", 7, "Phoenix reborn from fire",
                new Vector3Int[] {
                    // Gövde (merkez)
                    new Vector3Int(6,4,3), new Vector3Int(7,4,3),
                    new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3), new Vector3Int(8,5,3),
                    new Vector3Int(6,6,3), new Vector3Int(7,6,3),
                    // Baş
                    new Vector3Int(7,7,3), new Vector3Int(8,7,3), new Vector3Int(8,8,3),
                    // Gaga
                    new Vector3Int(9,8,3),
                    // Tepelik
                    new Vector3Int(7,8,3), new Vector3Int(7,9,3), new Vector3Int(6,9,3),
                    // Sol kanat (geniş, yukarı)
                    new Vector3Int(5,5,4), new Vector3Int(4,5,4), new Vector3Int(3,5,5), new Vector3Int(4,5,5),
                    new Vector3Int(2,5,5), new Vector3Int(2,5,6), new Vector3Int(1,5,6), new Vector3Int(1,5,7),
                    new Vector3Int(0,5,7), new Vector3Int(0,5,8),
                    new Vector3Int(4,6,4), new Vector3Int(3,6,5), new Vector3Int(2,6,6),
                    // Sağ kanat
                    new Vector3Int(5,5,2), new Vector3Int(4,5,2), new Vector3Int(3,5,1), new Vector3Int(4,5,1),
                    new Vector3Int(2,5,1), new Vector3Int(2,5,0), new Vector3Int(1,5,0), new Vector3Int(1,4,0),
                    new Vector3Int(0,4,0),
                    new Vector3Int(4,4,2), new Vector3Int(3,4,1), new Vector3Int(2,4,0),
                    // Kuyruk (uzun, aşağı sarkan, ateş)
                    new Vector3Int(6,3,3), new Vector3Int(5,2,3), new Vector3Int(4,1,3), new Vector3Int(3,0,3),
                    new Vector3Int(5,2,4), new Vector3Int(4,1,4), new Vector3Int(3,0,4), new Vector3Int(2,0,3),
                    new Vector3Int(5,2,2), new Vector3Int(4,1,2), new Vector3Int(3,0,2),
                    new Vector3Int(2,0,4), new Vector3Int(1,0,3),
                    // Kuyruk uçları (ateş)
                    new Vector3Int(2,0,2), new Vector3Int(1,0,4), new Vector3Int(0,0,3),
                    // Derinlik
                    new Vector3Int(6,5,2), new Vector3Int(6,5,4), new Vector3Int(7,5,2), new Vector3Int(7,5,4),
                    new Vector3Int(8,5,2), new Vector3Int(8,5,4),
                    // Ateş aura
                    new Vector3Int(8,4,2), new Vector3Int(8,4,4), new Vector3Int(9,5,3),
                    new Vector3Int(5,6,3), new Vector3Int(5,4,3),
                    // Kanat uçları
                    new Vector3Int(0,6,8), new Vector3Int(0,3,0),
                    new Vector3Int(1,6,7), new Vector3Int(1,3,0)
                },
                new Vector3Int[] { new Vector3Int(9,8,3), new Vector3Int(7,9,3), new Vector3Int(0,5,8), new Vector3Int(0,4,0), new Vector3Int(0,0,3), new Vector3Int(0,6,8) }),

            // Level 92: Kaktüs (60 küp)
            CreateLevelWithFixed(92, "Cactus", 15, "Desert cactus",
                new Vector3Int[] {
                    // Ana gövde (dikey)
                    new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(4,0,4), new Vector3Int(5,0,4),
                    new Vector3Int(4,1,3), new Vector3Int(5,1,3), new Vector3Int(4,1,4), new Vector3Int(5,1,4),
                    new Vector3Int(4,2,3), new Vector3Int(5,2,3), new Vector3Int(4,2,4), new Vector3Int(5,2,4),
                    new Vector3Int(4,3,3), new Vector3Int(5,3,3), new Vector3Int(4,3,4), new Vector3Int(5,3,4),
                    new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(4,4,4),
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3),
                    new Vector3Int(4,6,3), new Vector3Int(5,6,3),
                    new Vector3Int(5,7,3),
                    // Sol kol (yatay çıkıp yukarı)
                    new Vector3Int(3,2,3), new Vector3Int(2,2,3), new Vector3Int(1,2,3),
                    new Vector3Int(1,3,3), new Vector3Int(1,4,3), new Vector3Int(1,5,3),
                    new Vector3Int(1,2,4), new Vector3Int(1,3,4),
                    // Sağ kol (daha yukarıda, asimetrik)
                    new Vector3Int(6,4,3), new Vector3Int(7,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(8,5,3), new Vector3Int(8,6,3),
                    new Vector3Int(8,4,4), new Vector3Int(8,5,4),
                    // Çiçek (tepede)
                    new Vector3Int(4,7,3), new Vector3Int(5,8,3), new Vector3Int(6,7,3),
                    new Vector3Int(5,7,2), new Vector3Int(5,7,4),
                    // Dikenler (asimetrik çıkıntılar)
                    new Vector3Int(3,1,3), new Vector3Int(6,3,3), new Vector3Int(3,5,3),
                    new Vector3Int(5,1,2), new Vector3Int(4,3,2), new Vector3Int(5,5,4),
                    // Toprak
                    new Vector3Int(3,0,3), new Vector3Int(6,0,3), new Vector3Int(3,0,4), new Vector3Int(6,0,4)
                },
                new Vector3Int[] { new Vector3Int(5,8,3), new Vector3Int(1,5,3), new Vector3Int(8,6,3), new Vector3Int(3,0,3), new Vector3Int(6,0,4), new Vector3Int(6,7,3) }),

            // Level 93: Dürbün (55 küp) - Grid benzeri
            CreateLevelWithFixed(93, "Binoculars", 12, "Ice binoculars",
                new Vector3Int[] {
                    // Sol tüp (silindir)
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2), new Vector3Int(2,2,2), new Vector3Int(3,2,2),
                    new Vector3Int(0,3,2), new Vector3Int(1,3,2), new Vector3Int(2,3,2), new Vector3Int(3,3,2),
                    new Vector3Int(0,2,3), new Vector3Int(1,2,3), new Vector3Int(2,2,3), new Vector3Int(3,2,3),
                    new Vector3Int(0,3,3), new Vector3Int(1,3,3), new Vector3Int(2,3,3), new Vector3Int(3,3,3),
                    // Sağ tüp
                    new Vector3Int(0,2,5), new Vector3Int(1,2,5), new Vector3Int(2,2,5), new Vector3Int(3,2,5),
                    new Vector3Int(0,3,5), new Vector3Int(1,3,5), new Vector3Int(2,3,5), new Vector3Int(3,3,5),
                    new Vector3Int(0,2,6), new Vector3Int(1,2,6), new Vector3Int(2,2,6), new Vector3Int(3,2,6),
                    new Vector3Int(0,3,6), new Vector3Int(1,3,6), new Vector3Int(2,3,6), new Vector3Int(3,3,6),
                    // Köprü (bağlantı)
                    new Vector3Int(1,2,4), new Vector3Int(2,2,4), new Vector3Int(1,3,4), new Vector3Int(2,3,4),
                    // Mercekler (ön uçlarda)
                    new Vector3Int(4,2,2), new Vector3Int(4,3,2), new Vector3Int(4,2,3), new Vector3Int(4,3,3),
                    new Vector3Int(4,2,5), new Vector3Int(4,3,5), new Vector3Int(4,2,6), new Vector3Int(4,3,6),
                    // Göz parçaları (arka)
                    new Vector3Int(0,1,2), new Vector3Int(0,4,2), new Vector3Int(0,1,6), new Vector3Int(0,4,6),
                    // Odaklama halkası
                    new Vector3Int(2,1,2), new Vector3Int(2,4,2), new Vector3Int(2,4,6), new Vector3Int(2,1,6)
                },
                new Vector3Int[] { new Vector3Int(4,2,2), new Vector3Int(4,3,6), new Vector3Int(0,1,2), new Vector3Int(0,4,6), new Vector3Int(0,1,6), new Vector3Int(0,4,2) }),

            // Level 94: Deniz atı (68 küp)
            CreateLevelWithFixed(94, "Seahorse", 10, "Coral seahorse",
                new Vector3Int[] {
                    // Baş (profil)
                    new Vector3Int(4,9,3), new Vector3Int(5,9,3), new Vector3Int(3,8,3), new Vector3Int(4,8,3), new Vector3Int(5,8,3),
                    new Vector3Int(3,7,3), new Vector3Int(4,7,3), new Vector3Int(5,7,3),
                    // Burun (uzun)
                    new Vector3Int(6,8,3), new Vector3Int(7,8,3), new Vector3Int(8,8,3),
                    // Taç
                    new Vector3Int(4,10,3), new Vector3Int(3,10,3), new Vector3Int(4,11,3),
                    // Boyun (S eğrisi)
                    new Vector3Int(3,6,3), new Vector3Int(3,5,3), new Vector3Int(4,5,3),
                    new Vector3Int(4,4,3), new Vector3Int(5,4,3),
                    // Gövde
                    new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(5,2,3), new Vector3Int(6,2,3),
                    new Vector3Int(5,3,4), new Vector3Int(6,3,4), new Vector3Int(5,2,4), new Vector3Int(6,2,4),
                    // Karın
                    new Vector3Int(7,3,3), new Vector3Int(7,2,3), new Vector3Int(7,3,4),
                    // Kuyruk (kıvrımlı)
                    new Vector3Int(4,1,3), new Vector3Int(3,1,3), new Vector3Int(3,0,3),
                    new Vector3Int(2,0,3), new Vector3Int(2,0,4), new Vector3Int(3,0,4),
                    new Vector3Int(4,0,4), new Vector3Int(4,0,5),
                    // Sırt yüzgeci
                    new Vector3Int(4,5,2), new Vector3Int(4,4,2), new Vector3Int(5,3,2), new Vector3Int(5,2,2),
                    new Vector3Int(3,6,2), new Vector3Int(3,5,2),
                    // Derinlik
                    new Vector3Int(4,8,2), new Vector3Int(5,8,2),
                    new Vector3Int(3,7,2), new Vector3Int(5,7,2),
                    new Vector3Int(6,2,2), new Vector3Int(6,3,2),
                    // Göğüs yüzgeci
                    new Vector3Int(7,2,4), new Vector3Int(8,2,4),
                    // Göz
                    new Vector3Int(4,8,4),
                    // Desen
                    new Vector3Int(5,4,4), new Vector3Int(4,1,4),
                    new Vector3Int(3,8,4), new Vector3Int(5,9,2)
                },
                new Vector3Int[] { new Vector3Int(8,8,3), new Vector3Int(4,11,3), new Vector3Int(4,0,5), new Vector3Int(2,0,3), new Vector3Int(8,2,4), new Vector3Int(4,8,4) }),

            // Level 95: Zeplin (75 küp)
            CreateLevelWithFixed(95, "Zeppelin", 4, "Desert zeppelin",
                new Vector3Int[] {
                    // Balon gövdesi (oval)
                    new Vector3Int(4,4,3), new Vector3Int(5,4,3), new Vector3Int(6,4,3), new Vector3Int(7,4,3), new Vector3Int(8,4,3),
                    new Vector3Int(3,4,2), new Vector3Int(4,4,2), new Vector3Int(5,4,2), new Vector3Int(6,4,2), new Vector3Int(7,4,2), new Vector3Int(8,4,2), new Vector3Int(9,4,2),
                    new Vector3Int(2,4,2), new Vector3Int(10,4,2),
                    new Vector3Int(3,4,4), new Vector3Int(4,4,4), new Vector3Int(5,4,4), new Vector3Int(6,4,4), new Vector3Int(7,4,4), new Vector3Int(8,4,4), new Vector3Int(9,4,4),
                    // Üst (hafif yüksek)
                    new Vector3Int(4,5,2), new Vector3Int(5,5,2), new Vector3Int(6,5,2), new Vector3Int(7,5,2), new Vector3Int(8,5,2),
                    new Vector3Int(4,5,3), new Vector3Int(5,5,3), new Vector3Int(6,5,3), new Vector3Int(7,5,3), new Vector3Int(8,5,3),
                    new Vector3Int(5,6,2), new Vector3Int(6,6,2), new Vector3Int(7,6,2),
                    new Vector3Int(5,6,3), new Vector3Int(6,6,3), new Vector3Int(7,6,3),
                    // Alt
                    new Vector3Int(5,3,2), new Vector3Int(6,3,2), new Vector3Int(7,3,2),
                    new Vector3Int(5,3,3), new Vector3Int(6,3,3), new Vector3Int(7,3,3),
                    // Gondol (altta asılı)
                    new Vector3Int(5,1,2), new Vector3Int(6,1,2), new Vector3Int(7,1,2),
                    new Vector3Int(5,1,3), new Vector3Int(6,1,3), new Vector3Int(7,1,3),
                    new Vector3Int(5,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2),
                    // Bağlantı ipleri
                    new Vector3Int(5,2,2), new Vector3Int(7,2,2), new Vector3Int(6,2,3),
                    // Kuyruk yüzgeci (asimetrik)
                    new Vector3Int(2,5,2), new Vector3Int(1,5,2), new Vector3Int(1,6,2),
                    new Vector3Int(2,3,2), new Vector3Int(1,3,2),
                    new Vector3Int(2,5,4), new Vector3Int(1,4,4),
                    // Burun
                    new Vector3Int(11,4,2), new Vector3Int(11,4,3)
                },
                new Vector3Int[] { new Vector3Int(2,4,2), new Vector3Int(11,4,2), new Vector3Int(1,6,2), new Vector3Int(5,0,2), new Vector3Int(7,0,2), new Vector3Int(1,4,4) }),

            // Level 96: Örümcek ağı (78 küp)
            CreateLevelWithFixed(96, "Spiderweb", 9, "Night web",
                new Vector3Int[] {
                    // Merkez
                    new Vector3Int(5,0,5),
                    // İç halka
                    new Vector3Int(4,0,5), new Vector3Int(6,0,5), new Vector3Int(5,0,4), new Vector3Int(5,0,6),
                    new Vector3Int(4,0,4), new Vector3Int(6,0,4), new Vector3Int(4,0,6), new Vector3Int(6,0,6),
                    // Kol 1 (sağ yukarı)
                    new Vector3Int(7,0,5), new Vector3Int(8,0,5), new Vector3Int(9,0,5), new Vector3Int(10,0,5),
                    // Kol 2 (sol yukarı)
                    new Vector3Int(3,0,5), new Vector3Int(2,0,5), new Vector3Int(1,0,5), new Vector3Int(0,0,5),
                    // Kol 3 (yukarı)
                    new Vector3Int(5,0,3), new Vector3Int(5,0,2), new Vector3Int(5,0,1), new Vector3Int(5,0,0),
                    // Kol 4 (aşağı)
                    new Vector3Int(5,0,7), new Vector3Int(5,0,8), new Vector3Int(5,0,9), new Vector3Int(5,0,10),
                    // Çapraz kollar
                    new Vector3Int(7,0,3), new Vector3Int(8,0,2), new Vector3Int(9,0,1), new Vector3Int(10,0,0),
                    new Vector3Int(3,0,7), new Vector3Int(2,0,8), new Vector3Int(1,0,9), new Vector3Int(0,0,10),
                    new Vector3Int(7,0,7), new Vector3Int(8,0,8), new Vector3Int(9,0,9), new Vector3Int(10,0,10),
                    new Vector3Int(3,0,3), new Vector3Int(2,0,2), new Vector3Int(1,0,1), new Vector3Int(0,0,0),
                    // Dış halka bağlantıları
                    new Vector3Int(7,0,4), new Vector3Int(8,0,3),
                    new Vector3Int(7,0,6), new Vector3Int(8,0,7),
                    new Vector3Int(3,0,4), new Vector3Int(2,0,3),
                    new Vector3Int(3,0,6), new Vector3Int(2,0,7),
                    // Orta halka
                    new Vector3Int(7,0,3), new Vector3Int(3,0,7), new Vector3Int(7,0,7), new Vector3Int(3,0,3),
                    // Yükseklik (3D)
                    new Vector3Int(5,1,5), new Vector3Int(10,1,5), new Vector3Int(0,1,5),
                    new Vector3Int(5,1,0), new Vector3Int(5,1,10),
                    new Vector3Int(10,1,0), new Vector3Int(0,1,10), new Vector3Int(10,1,10), new Vector3Int(0,1,0),
                    // Örümcek (merkezde)
                    new Vector3Int(5,2,5), new Vector3Int(4,2,4), new Vector3Int(6,2,6),
                    new Vector3Int(4,1,5), new Vector3Int(6,1,5)
                },
                new Vector3Int[] { new Vector3Int(10,0,0), new Vector3Int(0,0,0), new Vector3Int(10,0,10), new Vector3Int(0,0,10), new Vector3Int(5,2,5), new Vector3Int(10,0,5) }),

            // Level 97: Kum kalesi (80 küp) - Grid/Kale hybrid
            CreateLevelWithFixed(97, "Sandcastle", 0, "Golden sandcastle",
                new Vector3Int[] {
                    // Geniş taban (grid)
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0), new Vector3Int(7,0,0), new Vector3Int(8,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1), new Vector3Int(6,0,1), new Vector3Int(7,0,1), new Vector3Int(8,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(6,0,2), new Vector3Int(7,0,2), new Vector3Int(8,0,2),
                    // Duvarlar
                    new Vector3Int(0,1,0), new Vector3Int(4,1,0), new Vector3Int(8,1,0),
                    new Vector3Int(0,1,2), new Vector3Int(8,1,2),
                    new Vector3Int(0,1,1), new Vector3Int(8,1,1),
                    // Kuleler (köşelerde)
                    new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    new Vector3Int(8,2,0), new Vector3Int(8,3,0),
                    new Vector3Int(0,2,2), new Vector3Int(0,3,2), new Vector3Int(0,4,2),
                    new Vector3Int(8,2,2), new Vector3Int(8,3,2),
                    // Merkez kule (yüksek)
                    new Vector3Int(4,0,2), new Vector3Int(4,1,2), new Vector3Int(4,2,2),
                    new Vector3Int(4,1,1), new Vector3Int(4,2,1), new Vector3Int(4,3,1), new Vector3Int(4,4,1), new Vector3Int(4,5,1),
                    // Süsler
                    new Vector3Int(3,5,1), new Vector3Int(5,5,1), new Vector3Int(4,6,1),
                    new Vector3Int(2,1,0), new Vector3Int(6,1,0),
                    // Hendek
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3), new Vector3Int(6,0,3), new Vector3Int(7,0,3), new Vector3Int(8,0,3),
                    // Asimetrik çıkıntılar
                    new Vector3Int(1,1,0), new Vector3Int(7,1,0),
                    new Vector3Int(3,0,2), new Vector3Int(5,0,2)
                },
                new Vector3Int[] { new Vector3Int(4,6,1), new Vector3Int(0,4,2), new Vector3Int(0,3,0), new Vector3Int(8,3,0), new Vector3Int(0,0,3), new Vector3Int(8,0,3) }),

            // Level 98: Deniz feneri (90 küp)
            CreateLevelWithFixed(98, "Radiance", 6, "Autumn lighthouse",
                new Vector3Int[] {
                    // Kayalık ada (düzensiz)
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3),
                    new Vector3Int(0,0,4), new Vector3Int(1,0,4), new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4),
                    new Vector3Int(1,0,5), new Vector3Int(2,0,5), new Vector3Int(3,0,5), new Vector3Int(4,0,5), new Vector3Int(5,0,5),
                    new Vector3Int(2,0,6), new Vector3Int(3,0,6), new Vector3Int(4,0,6),
                    new Vector3Int(3,0,7), new Vector3Int(4,0,7),
                    // Kaya yüksekliği
                    new Vector3Int(1,1,4), new Vector3Int(2,1,4), new Vector3Int(2,1,5), new Vector3Int(3,1,5),
                    // Fener tabanı (kare)
                    new Vector3Int(2,2,4), new Vector3Int(3,2,4), new Vector3Int(2,2,5), new Vector3Int(3,2,5),
                    // Fener gövdesi
                    new Vector3Int(2,3,4), new Vector3Int(3,3,4), new Vector3Int(2,3,5), new Vector3Int(3,3,5),
                    new Vector3Int(2,4,4), new Vector3Int(3,4,4), new Vector3Int(2,4,5), new Vector3Int(3,4,5),
                    new Vector3Int(2,5,4), new Vector3Int(3,5,4), new Vector3Int(2,5,5), new Vector3Int(3,5,5),
                    new Vector3Int(2,6,4), new Vector3Int(3,6,4), new Vector3Int(3,6,5),
                    new Vector3Int(2,7,4), new Vector3Int(3,7,4),
                    new Vector3Int(3,8,4), new Vector3Int(3,8,5),
                    // Balkon
                    new Vector3Int(1,7,3), new Vector3Int(1,7,4), new Vector3Int(1,7,5), new Vector3Int(1,7,6),
                    new Vector3Int(2,7,3), new Vector3Int(2,7,5), new Vector3Int(2,7,6),
                    new Vector3Int(3,7,3), new Vector3Int(3,7,5), new Vector3Int(3,7,6),
                    new Vector3Int(4,7,3), new Vector3Int(4,7,4), new Vector3Int(4,7,5), new Vector3Int(4,7,6),
                    // Işık odası
                    new Vector3Int(2,8,4), new Vector3Int(2,8,5),
                    new Vector3Int(3,9,4),
                    // Işık huzmesi (asimetrik - sağa doğru)
                    new Vector3Int(4,8,4), new Vector3Int(5,8,4), new Vector3Int(6,8,4), new Vector3Int(7,8,3),
                    new Vector3Int(5,8,5), new Vector3Int(6,8,5), new Vector3Int(7,8,5),
                    new Vector3Int(8,8,4), new Vector3Int(9,8,4),
                    // Dalga (asimetrik)
                    new Vector3Int(5,0,3), new Vector3Int(6,0,4), new Vector3Int(6,0,5),
                    new Vector3Int(0,0,5), new Vector3Int(0,0,6),
                    // İskele
                    new Vector3Int(4,0,3), new Vector3Int(5,0,4), new Vector3Int(5,1,4)
                },
                new Vector3Int[] { new Vector3Int(3,9,4), new Vector3Int(9,8,4), new Vector3Int(7,8,3), new Vector3Int(0,0,3), new Vector3Int(3,0,7), new Vector3Int(0,0,6) }),

            // Level 99: Kaos düzeni (110 küp)
            CreateLevelWithFixed(99, "Chaos Order", 3, "Purple chaos order",
                GenerateChaosOrder(),
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(14,0,0), new Vector3Int(0,0,14),
                    new Vector3Int(14,0,14), new Vector3Int(7,5,7), new Vector3Int(7,0,7)
                }),

            // Level 100: Son efsane (120 küp) - Ultimate level
            CreateLevelWithFixed(100, "Eternity", 8, "Lavender infinity - Ultimate exam",
                GenerateEternity(),
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(14,0,0), new Vector3Int(0,0,14),
                    new Vector3Int(14,0,14), new Vector3Int(7,10,7), new Vector3Int(7,0,7),
                    new Vector3Int(0,5,7), new Vector3Int(14,5,7)
                })
        };
    }

    // ============ YARDIMCI ŞEKİL OLUŞTURMA FONKSİYONLARI ============

    /// <summary>
    /// width x height x depth boyutlarında dolu dikdörtgen grid oluşturur
    /// </summary>
    private static Vector3Int[] GenerateGrid(int width, int height, int depth)
    {
        var positions = new List<Vector3Int>();
        for (int z = 0; z < depth; z++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    positions.Add(new Vector3Int(x, y, z));
        return positions.ToArray();
    }

    /// <summary>
    /// Dikdörtgen grid'in köşelerini döndürür
    /// </summary>
    private static Vector3Int[] GetCorners(int width, int height, int depth)
    {
        var corners = new HashSet<Vector3Int>();
        int maxX = width - 1;
        int maxY = height - 1;
        int maxZ = depth - 1;

        corners.Add(new Vector3Int(0, 0, 0));
        corners.Add(new Vector3Int(maxX, 0, 0));
        corners.Add(new Vector3Int(0, maxY, 0));
        corners.Add(new Vector3Int(maxX, maxY, 0));

        if (depth > 1)
        {
            corners.Add(new Vector3Int(0, 0, maxZ));
            corners.Add(new Vector3Int(maxX, 0, maxZ));
            corners.Add(new Vector3Int(0, maxY, maxZ));
            corners.Add(new Vector3Int(maxX, maxY, maxZ));
        }

        return new List<Vector3Int>(corners).ToArray();
    }

    /// <summary>
    /// İçi boş kutu oluşturur
    /// </summary>
    private static Vector3Int[] GenerateHollowBox(int width, int height, int depth)
    {
        var positions = new List<Vector3Int>();
        for (int z = 0; z < depth; z++)
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1 || z == 0 || z == depth - 1)
                        positions.Add(new Vector3Int(x, y, z));
                }
        return positions.ToArray();
    }

    /// <summary>
    /// Asimetrik küre benzeri şekil oluşturur (radius tabanlı, düzensiz)
    /// </summary>
    private static Vector3Int[] GenerateAsymmetricSphere(int radius)
    {
        var positions = new List<Vector3Int>();
        int center = radius;
        float rSq = radius * radius;

        for (int z = 0; z <= radius * 2; z++)
            for (int y = 0; y <= radius * 2; y++)
                for (int x = 0; x <= radius * 2; x++)
                {
                    float dx = x - center + 0.3f;
                    float dy = y - center;
                    float dz = z - center - 0.2f;
                    float distSq = dx * dx + dy * dy + dz * dz;

                    // Kabuk: daha kalın iç yarıçap (daha az küp üretir)
                    float innerR = radius - 0.6f;
                    if (distSq <= rSq && distSq >= innerR * innerR)
                        positions.Add(new Vector3Int(x, y, z));
                }

        // Asimetri çıkıntısı
        positions.Add(new Vector3Int(radius * 2 + 1, center, center));

        return positions.ToArray();
    }

    /// <summary>
    /// Level 50 için kaotik galaksi şekli oluşturur
    /// </summary>
    private static Vector3Int[] GenerateChaosGalaxy()
    {
        var positions = new HashSet<Vector3Int>();

        // Kompakt merkez küme (3x3)
        for (int x = 2; x <= 4; x++)
            for (int z = 2; z <= 4; z++)
            {
                positions.Add(new Vector3Int(x, 0, z));
                if (x == 3 && z == 3)
                    positions.Add(new Vector3Int(x, 1, z));
            }

        // Spiral kol 1 (kısa)
        positions.Add(new Vector3Int(5, 0, 3));
        positions.Add(new Vector3Int(6, 0, 4));
        positions.Add(new Vector3Int(6, 1, 5));
        positions.Add(new Vector3Int(5, 1, 6));

        // Spiral kol 2 (karşı yön)
        positions.Add(new Vector3Int(1, 0, 3));
        positions.Add(new Vector3Int(0, 0, 2));
        positions.Add(new Vector3Int(0, 1, 1));
        positions.Add(new Vector3Int(1, 1, 0));

        // Yukarı çıkan nokta
        positions.Add(new Vector3Int(3, 2, 3));
        positions.Add(new Vector3Int(3, 3, 4));
        positions.Add(new Vector3Int(4, 4, 4));

        // Birkaç dağınık yıldız
        positions.Add(new Vector3Int(0, 0, 5));
        positions.Add(new Vector3Int(6, 0, 0));
        positions.Add(new Vector3Int(5, 2, 2));

        return new List<Vector3Int>(positions).ToArray();
    }

    /// <summary>
    /// Merdiven (basamak) şekli oluşturur. Her satır bir adım yukarı çıkar.
    /// </summary>
    private static Vector3Int[] GenerateStaircase(int width, int depth)
    {
        var positions = new List<Vector3Int>();
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                positions.Add(new Vector3Int(x, z, z));
            }
        }
        return positions.ToArray();
    }

    /// <summary>
    /// Basamaklı piramit oluşturur - Her kat bir küçük kare
    /// </summary>
    private static Vector3Int[] GenerateSteppedPyramid()
    {
        var positions = new List<Vector3Int>();
        // 3 basamak: 5x5, 3x3, 1x1
        int[] sizes = { 5, 3, 1 };
        for (int layer = 0; layer < sizes.Length; layer++)
        {
            int s = sizes[layer];
            int offset = (5 - s) / 2;
            for (int x = 0; x < s; x++)
                for (int z = 0; z < s; z++)
                    positions.Add(new Vector3Int(x + offset, layer, z + offset));
        }
        return positions.ToArray();
    }

    /// <summary>
    /// Kozmik nebula şekli - Dağınık ama bağlı galaktik bulut
    /// </summary>
    private static Vector3Int[] GenerateCosmicNebula()
    {
        var positions = new HashSet<Vector3Int>();
        
        // Kompakt merkez (3x3)
        for (int x = 1; x <= 3; x++)
            for (int z = 1; z <= 3; z++)
            {
                positions.Add(new Vector3Int(x, 0, z));
                if (x == 2 && z == 2)
                    positions.Add(new Vector3Int(x, 1, z));
            }
        
        // Kol 1 - sağ (kısa)
        positions.Add(new Vector3Int(4, 0, 2));
        positions.Add(new Vector3Int(5, 0, 3));
        positions.Add(new Vector3Int(5, 1, 4));
        
        // Kol 2 - sol
        positions.Add(new Vector3Int(0, 0, 2));
        positions.Add(new Vector3Int(0, 0, 3));
        
        // Yukarı spiral
        positions.Add(new Vector3Int(2, 2, 2));
        positions.Add(new Vector3Int(3, 3, 2));
        positions.Add(new Vector3Int(3, 4, 3));
        
        // Birkaç toz noktası
        positions.Add(new Vector3Int(4, 0, 0));
        positions.Add(new Vector3Int(0, 0, 5));
        positions.Add(new Vector3Int(1, 1, 4));
        
        return new List<Vector3Int>(positions).ToArray();
    }

    /// <summary>
    /// Kaos düzeni - Büyük dağınık ama bağlı yapı
    /// </summary>
    private static Vector3Int[] GenerateChaosOrder()
    {
        var positions = new HashSet<Vector3Int>();
        
        // Kompakt merkez platform (4x4)
        for (int x = 1; x <= 4; x++)
            for (int z = 1; z <= 4; z++)
                positions.Add(new Vector3Int(x, 0, z));
        
        // Kısa çapraz kollar
        positions.Add(new Vector3Int(0, 0, 0));
        positions.Add(new Vector3Int(5, 0, 0));
        positions.Add(new Vector3Int(0, 0, 5));
        positions.Add(new Vector3Int(5, 0, 5));
        
        // Yükselen merkez
        for (int y = 1; y <= 3; y++)
        {
            positions.Add(new Vector3Int(2, y, 2));
            positions.Add(new Vector3Int(3, y, 3));
            if (y <= 2)
            {
                positions.Add(new Vector3Int(3, y, 2));
                positions.Add(new Vector3Int(2, y, 3));
            }
        }
        
        // Köşe yükseklikleri
        positions.Add(new Vector3Int(1, 1, 1));
        positions.Add(new Vector3Int(4, 1, 4));
        
        return new List<Vector3Int>(positions).ToArray();
    }

    /// <summary>
    /// Ebediyet - Son level için büyük, karmaşık yapı
    /// </summary>
    private static Vector3Int[] GenerateEternity()
    {
        var positions = new HashSet<Vector3Int>();
        
        // Alt platform (kompakt diamond)
        for (int x = 1; x <= 5; x++)
            for (int z = 1; z <= 5; z++)
            {
                if (Mathf.Abs(x - 3) + Mathf.Abs(z - 3) <= 3)
                    positions.Add(new Vector3Int(x, 0, z));
            }
        
        // 4 kısa sütun (köşelerde)
        for (int y = 1; y <= 4; y++)
        {
            positions.Add(new Vector3Int(1, y, 1));
            positions.Add(new Vector3Int(5, y, 1));
            positions.Add(new Vector3Int(1, y, 5));
            positions.Add(new Vector3Int(5, y, 5));
        }
        
        // Üst platform (küçük)
        for (int x = 2; x <= 4; x++)
            for (int z = 2; z <= 4; z++)
            {
                if (Mathf.Abs(x - 3) + Mathf.Abs(z - 3) <= 1)
                    positions.Add(new Vector3Int(x, 5, z));
            }
        
        // Merkez sütun
        for (int y = 1; y <= 6; y++)
            positions.Add(new Vector3Int(3, y, 3));
        
        // Kemerler (sütunları bağlayan)
        for (int i = 2; i <= 4; i++)
        {
            positions.Add(new Vector3Int(i, 4, 1));
            positions.Add(new Vector3Int(i, 4, 5));
            positions.Add(new Vector3Int(1, 4, i));
            positions.Add(new Vector3Int(5, 4, i));
        }
        
        return new List<Vector3Int>(positions).ToArray();
    }
    
    /// <summary>
    /// Level oluşturma yardımcı metodu
    /// </summary>
    private static LevelData CreateLevel(int num, string name, int palette, float fixedRatio, string desc, Vector3Int[] positions)
    {
        return new LevelData
        {
            levelNumber = num,
            levelName = name,
            colorPaletteIndex = palette,
            fixedCubeRatio = fixedRatio,
            description = desc,
            customPositions = new List<Vector3Int>(positions),
            fixedPositions = null // NULL - ratio ile belirlenecek
        };
    }
    
    /// <summary>
    /// Sabit küp pozisyonları belirlenmiş level oluşturma metodu
    /// </summary>
    private static LevelData CreateLevelWithFixed(int num, string name, int palette, string desc, Vector3Int[] allPositions, Vector3Int[] fixedPositions)
    {
        return new LevelData
        {
            levelNumber = num,
            levelName = name,
            colorPaletteIndex = palette,
            fixedCubeRatio = fixedPositions.Length / (float)allPositions.Length, // Otomatik hesapla
            description = desc,
            customPositions = new List<Vector3Int>(allPositions),
            fixedPositions = new List<Vector3Int>(fixedPositions)
        };
    }
    
    /// <summary>
    /// Level verisini döndürür - Önce JSON dosyasından, yoksa hardcoded'dan
    /// </summary>
    public static LevelData GetLevel(int levelNumber)
    {
        // Hardcoded level'lardan al, küp sayısına göre sıralanmış
        if (levels == null || levels.Length == 0)
        {
            CreateLevels();
        }
        
        // Sıralama henüz yapılmadıysa, küp sayısına göre sırala (kolaydan zora)
        if (sortedLevels == null)
        {
            sortedLevels = levels
                .OrderBy(l => l.customPositions != null ? l.customPositions.Count : 0)
                .ToArray();
            
            // Level numaralarını yeni sıraya göre güncelle
            for (int i = 0; i < sortedLevels.Length; i++)
            {
                sortedLevels[i].levelNumber = i + 1;
            }
            Debug.Log($"[LevelManager] {sortedLevels.Length} levels sorted by cube count (easy to hard)");
        }
        
        int index = Mathf.Clamp(levelNumber - 1, 0, sortedLevels.Length - 1);
        return sortedLevels[index];
    }
    
    /// <summary>
    /// Level şekil pozisyonlarını döndürür
    /// </summary>
    public static List<Vector3Int> GenerateShapePositions(LevelData level)
    {
        if (level.customPositions != null && level.customPositions.Count > 0)
        {
            return new List<Vector3Int>(level.customPositions);
        }
        
        // Fallback - basit çizgi
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int i = 0; i < 5; i++)
        {
            positions.Add(new Vector3Int(i, 0, 0));
        }
        return positions;
    }
}
