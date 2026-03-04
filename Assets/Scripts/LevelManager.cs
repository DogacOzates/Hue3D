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
    /// I Love Hue 3D - 100 level, max 30 küp
    /// Bölüm 1 (1-10): 4-8 küp, basit 2D
    /// Bölüm 2 (11-20): 8-12 küp, 2D şekiller
    /// Bölüm 3 (21-30): 10-14 küp, 3D giriş
    /// Bölüm 4 (31-40): 12-18 küp, 3D şekiller
    /// Bölüm 5 (41-50): 14-20 küp, karmaşık 3D
    /// Bölüm 6 (51-60): 16-22 küp, yaratıcı
    /// Bölüm 7 (61-70): 18-24 küp, detaylı
    /// Bölüm 8 (71-80): 20-26 küp, usta
    /// Bölüm 9 (81-90): 22-28 küp, efsane
    /// Bölüm 10 (91-100): 24-30 küp, efsanevi
    /// </summary>
    private static void CreateLevels()
    {
        levels = new LevelData[]
        {
            // ============ TUTORIAL ============
            
            // Level 0: Tutorial - 4 küp yan yana
            CreateLevelWithFixed(0, "Tutorial", 3, "Learn to play",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), 
                    new Vector3Int(3,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0) }),

            // ============ BÖLÜM 1: GİRİŞ (1-10) — 4-8 küp ============

            // Level 1: 2x2 grid (4 küp)
            CreateLevelWithFixed(1, "First Touch", 1, "2x2 intro",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(1,1,0) }),

            // Level 2: L şekli (5 küp)
            CreateLevelWithFixed(2, "Canvas", 4, "L shape",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0)
                },
                new Vector3Int[] { new Vector3Int(2,0,0), new Vector3Int(0,2,0) }),

            // Level 3: T şekli (5 küp)
            CreateLevelWithFixed(3, "Board", 7, "T shape",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(1,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(1,2,0) }),

            // Level 4: Çapraz (6 küp)
            CreateLevelWithFixed(4, "Zigzag", 10, "Zigzag steps",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,2,0) }),

            // Level 5: Plus (5 küp)
            CreateLevelWithFixed(5, "Plus", 13, "Plus shape",
                new Vector3Int[] {
                    new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(2,1,0), new Vector3Int(1,2,0)
                },
                new Vector3Int[] { new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(2,1,0) }),

            // Level 6: 2x3 grid (6 küp)
            CreateLevelWithFixed(6, "Brick", 0, "2x3 grid",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(0,1,0) }),

            // Level 7: U şekli (7 küp)
            CreateLevelWithFixed(7, "Valley", 5, "U shape",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(0,1,0), new Vector3Int(0,2,0),
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(2,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(2,2,0) }),

            // Level 8: Z şekli (7 küp)
            CreateLevelWithFixed(8, "Flow", 8, "Z shape",
                new Vector3Int[] {
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(1,1,0),
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(2,2,0), new Vector3Int(0,0,0) }),

            // Level 9: 3x3 grid eksik köşe (8 küp)
            CreateLevelWithFixed(9, "Corner", 11, "Broken grid",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0)
                },
                new Vector3Int[] { new Vector3Int(2,0,0), new Vector3Int(0,2,0), new Vector3Int(1,2,0) }),

            // Level 10: Yılan (8 küp)
            CreateLevelWithFixed(10, "Snake", 14, "Winding snake",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(3,3,0), new Vector3Int(4,3,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,3,0) }),

            // ============ BÖLÜM 2: ŞEKİLLER (11-20) — 8-12 küp ============

            // Level 11: Ok (8 küp)
            CreateLevelWithFixed(11, "Arrow", 2, "Arrow pointing right",
                new Vector3Int[] {
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(3,1,0), new Vector3Int(4,0,0), new Vector3Int(4,2,0),
                    new Vector3Int(3,0,0), new Vector3Int(3,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(4,0,0), new Vector3Int(4,2,0) }),

            // Level 12: Kalp (9 küp)
            CreateLevelWithFixed(12, "Heart", 6, "Pixel heart",
                new Vector3Int[] {
                    new Vector3Int(0,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(1,3,0),
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(2,1,0), new Vector3Int(1,3,0) }),

            // Level 13: Çerçeve (8 küp)
            CreateLevelWithFixed(13, "Frame", 9, "Hollow frame",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(0,2,0), new Vector3Int(2,2,0) }),

            // Level 14: H şekli (9 küp)
            CreateLevelWithFixed(14, "Bridge", 12, "H bridge",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(0,1,0), new Vector3Int(0,2,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(3,0,0), new Vector3Int(3,1,0), new Vector3Int(3,2,0),
                    new Vector3Int(2,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(0,2,0), new Vector3Int(3,2,0) }),

            // Level 15: Büyük L (10 küp)
            CreateLevelWithFixed(15, "Corner", 15, "Big L",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(1,2,0)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(0,3,0), new Vector3Int(2,1,0) }),

            // Level 16: Yıldız (9 küp)
            CreateLevelWithFixed(16, "Star", 3, "Small star",
                new Vector3Int[] {
                    new Vector3Int(2,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(1,2,0), new Vector3Int(3,2,0)
                },
                new Vector3Int[] { new Vector3Int(2,0,0), new Vector3Int(0,1,0), new Vector3Int(4,1,0) }),

            // Level 17: Merdiven (10 küp)
            CreateLevelWithFixed(17, "Steps", 7, "Rising steps",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(4,4,0), new Vector3Int(5,4,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,4,0) }),

            // Level 18: 3x4 grid (12 küp)
            CreateLevelWithFixed(18, "Field", 10, "3x4 grid",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(0,3,0), new Vector3Int(2,3,0) }),

            // Level 19: Tetris T (11 küp)
            CreateLevelWithFixed(19, "Tetris", 13, "Double T",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(1,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0),
                    new Vector3Int(2,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,2,0), new Vector3Int(2,3,0) }),

            // Level 20: Çapraz bloklar (12 küp)
            CreateLevelWithFixed(20, "Diamonds", 0, "Diagonal blocks",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0),
                    new Vector3Int(3,0,0), new Vector3Int(3,1,0),
                    new Vector3Int(2,0,0), new Vector3Int(1,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0), new Vector3Int(0,2,0), new Vector3Int(3,2,0) }),

            // ============ BÖLÜM 3: 3D GİRİŞ (21-30) — 10-14 küp ============

            // Level 21: İlk 3D - 2x2x2 küp (8 küp) + çıkıntı (10 küp)
            CreateLevelWithFixed(21, "First Cube", 5, "First 3D shape",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(2,0,0), new Vector3Int(2,0,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(1,1,1), new Vector3Int(2,0,1) }),

            // Level 22: 3D L şekli (10 küp)
            CreateLevelWithFixed(22, "3D Corner", 8, "L in 3D",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(0,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(0,2,1)
                },
                new Vector3Int[] { new Vector3Int(2,0,0), new Vector3Int(2,0,1), new Vector3Int(0,2,0) }),

            // Level 23: Basamak 3D (11 küp)
            CreateLevelWithFixed(23, "Stairs", 11, "3D staircase",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,2,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,2,0), new Vector3Int(0,0,1) }),

            // Level 24: T 3D (12 küp)
            CreateLevelWithFixed(24, "3D Tee", 14, "T with depth",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0), new Vector3Int(0,0,1), new Vector3Int(3,0,1) }),

            // Level 25: Küçük kule (11 küp)
            CreateLevelWithFixed(25, "Tower", 2, "Small tower",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0),
                    new Vector3Int(0,3,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(0,3,0) }),

            // Level 26: Köprü 3D (12 küp)
            CreateLevelWithFixed(26, "Arch", 6, "Simple arch",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(0,1,0), new Vector3Int(0,2,0),
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(2,1,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(0,1,1),
                    new Vector3Int(2,0,1), new Vector3Int(2,1,1),
                    new Vector3Int(1,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(0,0,1), new Vector3Int(2,0,1) }),

            // Level 27: Asimetrik blok (13 küp)
            CreateLevelWithFixed(27, "Cluster", 9, "Asymmetric cluster",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0),
                    new Vector3Int(3,0,0)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(0,2,0), new Vector3Int(2,1,1) }),

            // Level 28: Düz artı derinlik (13 küp)
            CreateLevelWithFixed(28, "Cross 3D", 12, "3D cross",
                new Vector3Int[] {
                    new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(2,1,0), new Vector3Int(1,2,0),
                    new Vector3Int(1,0,1), new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(2,1,1), new Vector3Int(1,2,1),
                    new Vector3Int(1,1,2), new Vector3Int(0,1,2), new Vector3Int(2,1,2)
                },
                new Vector3Int[] { new Vector3Int(1,0,0), new Vector3Int(1,2,0), new Vector3Int(0,1,2), new Vector3Int(2,1,2) }),

            // Level 29: İnce uzun yapı (14 küp)
            CreateLevelWithFixed(29, "Beam", 15, "Long beam",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(5,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,0,0), new Vector3Int(0,1,0), new Vector3Int(5,1,0) }),

            // Level 30: Mini piramit (14 küp)
            CreateLevelWithFixed(30, "Pyramid", 4, "Small pyramid",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1),
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,0), new Vector3Int(0,0,1), new Vector3Int(1,2,0) }),

            // ============ BÖLÜM 4: 3D ŞEKİLLER (31-40) — 12-18 küp ============

            // Level 31: Spiral merdiven (14 küp)
            CreateLevelWithFixed(31, "Spiral", 1, "Spiral staircase",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0),
                    new Vector3Int(2,0,0), new Vector3Int(2,0,1),
                    new Vector3Int(2,1,1), new Vector3Int(2,1,2),
                    new Vector3Int(1,1,2), new Vector3Int(0,1,2),
                    new Vector3Int(0,2,2), new Vector3Int(0,2,1),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0),
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,3,0) }),

            // Level 32: Küçük ev (15 küp)
            CreateLevelWithFixed(32, "House", 7, "Tiny house",
                new Vector3Int[] {
                    // Duvarlar
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(2,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2),
                    // Üst kat
                    new Vector3Int(0,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,1,2), new Vector3Int(2,1,2),
                    // Çatı
                    new Vector3Int(0,2,1), new Vector3Int(1,2,1), new Vector3Int(2,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(1,2,1) }),

            // Level 33: Mini ağaç (14 küp)
            CreateLevelWithFixed(33, "Tree", 10, "Tiny tree",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(1,0,1), new Vector3Int(1,1,1), new Vector3Int(1,2,1),
                    // Yapraklar
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0),
                    new Vector3Int(0,3,1), new Vector3Int(1,3,1), new Vector3Int(2,3,1),
                    new Vector3Int(0,3,2), new Vector3Int(1,3,2), new Vector3Int(2,3,2),
                    // Tepe
                    new Vector3Int(1,4,1), new Vector3Int(1,4,0)
                },
                new Vector3Int[] { new Vector3Int(1,0,1), new Vector3Int(0,3,0), new Vector3Int(2,3,2), new Vector3Int(1,4,1) }),

            // Level 34: Yılan 3D (15 küp)
            CreateLevelWithFixed(34, "Serpent", 13, "3D serpent",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(2,0,1), new Vector3Int(2,0,2),
                    new Vector3Int(2,1,2), new Vector3Int(1,1,2), new Vector3Int(0,1,2),
                    new Vector3Int(0,1,1), new Vector3Int(0,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(2,2,1), new Vector3Int(2,2,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,2,2) }),

            // Level 35: Mini kale (16 küp)
            CreateLevelWithFixed(35, "Castle", 0, "Tiny castle",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    // Kuleler
                    new Vector3Int(0,1,0), new Vector3Int(3,1,0), new Vector3Int(0,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(0,2,0), new Vector3Int(3,2,0),
                    // Kapı kemeri
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(3,2,0), new Vector3Int(0,0,1), new Vector3Int(3,0,1) }),

            // Level 36: Asimetrik platform (16 küp)
            CreateLevelWithFixed(36, "Platform", 5, "Floating platform",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    new Vector3Int(3,0,2), new Vector3Int(3,1,1),
                    new Vector3Int(2,1,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,2), new Vector3Int(3,0,2), new Vector3Int(0,1,0), new Vector3Int(3,1,1) }),

            // Level 37: Mini köprü (17 küp)
            CreateLevelWithFixed(37, "Bridge", 8, "Small bridge",
                new Vector3Int[] {
                    // Sol ayak
                    new Vector3Int(0,0,0), new Vector3Int(0,0,1), new Vector3Int(0,1,0), new Vector3Int(0,1,1),
                    // Köprü yolu
                    new Vector3Int(1,2,0), new Vector3Int(1,2,1), new Vector3Int(2,2,0), new Vector3Int(2,2,1),
                    new Vector3Int(3,2,0), new Vector3Int(3,2,1),
                    // Sağ ayak
                    new Vector3Int(4,0,0), new Vector3Int(4,0,1), new Vector3Int(4,1,0), new Vector3Int(4,1,1),
                    // Korkuluk
                    new Vector3Int(2,3,0), new Vector3Int(0,2,0), new Vector3Int(4,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(2,3,0), new Vector3Int(0,0,1) }),

            // Level 38: Mini gemi (17 küp)
            CreateLevelWithFixed(38, "Boat", 11, "Tiny boat",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    // Burun
                    new Vector3Int(5,0,1), new Vector3Int(5,0,2),
                    // Kabin
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(2,1,2), new Vector3Int(3,1,2),
                    // Direk + yelken
                    new Vector3Int(4,1,1), new Vector3Int(4,2,1), new Vector3Int(4,2,2)
                },
                new Vector3Int[] { new Vector3Int(1,0,1), new Vector3Int(5,0,2), new Vector3Int(4,2,1) }),

            // Level 39: Çift kule (18 küp)
            CreateLevelWithFixed(39, "Twin Towers", 14, "Two towers",
                new Vector3Int[] {
                    // Sol kule
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    // Sağ kule
                    new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(4,2,0), new Vector3Int(4,3,0), new Vector3Int(4,4,0),
                    // Bağlantı
                    new Vector3Int(2,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(4,4,0), new Vector3Int(0,0,1), new Vector3Int(4,0,1) }),

            // Level 40: Taht (18 küp)
            CreateLevelWithFixed(40, "Throne", 3, "Mini throne",
                new Vector3Int[] {
                    // Oturma
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    // Sırtlık
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(3,3,0),
                    // Kolçak
                    new Vector3Int(0,1,1), new Vector3Int(3,1,1)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(3,3,0), new Vector3Int(0,1,1), new Vector3Int(3,1,1) }),

            // ============ BÖLÜM 5: KARMAŞIK 3D (41-50) — 14-20 küp ============

            // Level 41: Mini piramit katmanlı (15 küp)
            CreateLevelWithFixed(41, "Ziggurat", 4, "Stepped pyramid",
                new Vector3Int[] {
                    // Taban 3x3
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2),
                    // 2. kat
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,1,1), new Vector3Int(2,1,1),
                    // Zirve
                    new Vector3Int(1,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,2), new Vector3Int(1,2,0) }),

            // Level 42: Mini yılan 3D (16 küp)
            CreateLevelWithFixed(42, "Cobra", 6, "Winding cobra",
                new Vector3Int[] {
                    // Kuyruk
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    // Gövde dönüş
                    new Vector3Int(2,0,1), new Vector3Int(2,1,1), new Vector3Int(2,1,0),
                    // Gövde 2
                    new Vector3Int(1,1,0), new Vector3Int(0,1,0), new Vector3Int(0,2,0),
                    // Boyun
                    new Vector3Int(0,2,1), new Vector3Int(1,2,1), new Vector3Int(1,3,1),
                    // Kafa
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(2,3,1),
                    new Vector3Int(2,4,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,4,0), new Vector3Int(2,3,1) }),

            // Level 43: Mini kale 3D (18 küp)
            CreateLevelWithFixed(43, "Fortress", 9, "Mini fortress",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2),
                    // Kuleler
                    new Vector3Int(0,1,0), new Vector3Int(3,1,0), new Vector3Int(0,1,2), new Vector3Int(3,1,2),
                    new Vector3Int(0,2,0), new Vector3Int(3,2,2),
                    // Kapı
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(3,2,2), new Vector3Int(0,0,2), new Vector3Int(3,0,0) }),

            // Level 44: Spiral köprü (18 küp)
            CreateLevelWithFixed(44, "Helix", 12, "Helical bridge",
                new Vector3Int[] {
                    // Sol platform
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    // Sarmal
                    new Vector3Int(2,1,0), new Vector3Int(2,1,1),
                    new Vector3Int(3,2,1), new Vector3Int(3,2,2),
                    new Vector3Int(4,3,2), new Vector3Int(4,3,3),
                    // Sağ platform
                    new Vector3Int(5,4,2), new Vector3Int(6,4,2), new Vector3Int(5,4,3), new Vector3Int(6,4,3),
                    // Alt sarmal
                    new Vector3Int(2,0,1), new Vector3Int(3,1,2),
                    new Vector3Int(4,2,3), new Vector3Int(5,3,3)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(6,4,3), new Vector3Int(0,0,1), new Vector3Int(6,4,2) }),

            // Level 45: Mini volkan (18 küp)
            CreateLevelWithFixed(45, "Volcano", 15, "Small volcano",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2),
                    // 2. kat
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1),
                    new Vector3Int(1,1,2), new Vector3Int(2,1,2),
                    // Krater
                    new Vector3Int(1,2,1), new Vector3Int(2,2,1), new Vector3Int(1,2,2), new Vector3Int(2,2,2),
                    // Duman
                    new Vector3Int(1,3,1), new Vector3Int(2,3,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,1), new Vector3Int(3,0,2), new Vector3Int(1,3,1), new Vector3Int(2,3,2) }),

            // Level 46: Mini fener (18 küp)
            CreateLevelWithFixed(46, "Lighthouse", 2, "Tiny lighthouse",
                new Vector3Int[] {
                    // Kayalık
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    // Fener gövdesi
                    new Vector3Int(1,1,0), new Vector3Int(1,1,1),
                    new Vector3Int(1,2,0), new Vector3Int(1,2,1),
                    new Vector3Int(1,3,0), new Vector3Int(1,3,1),
                    // Balkon
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0), new Vector3Int(2,4,0),
                    new Vector3Int(0,4,1), new Vector3Int(1,4,1), new Vector3Int(2,4,1),
                    // Işık
                    new Vector3Int(1,5,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,0,0), new Vector3Int(1,5,0), new Vector3Int(2,4,1) }),

            // Level 47: Mini labirent (19 küp)
            CreateLevelWithFixed(47, "Maze", 7, "Small maze",
                new Vector3Int[] {
                    // Dış duvarlar
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(3,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3),
                    // İç duvarlar
                    new Vector3Int(2,0,1), new Vector3Int(1,0,2),
                    // Üst kat (kısmi)
                    new Vector3Int(0,1,0), new Vector3Int(3,1,0), new Vector3Int(0,1,3), new Vector3Int(3,1,3),
                    // Merkez kule
                    new Vector3Int(1,1,1)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(3,1,0), new Vector3Int(0,1,3), new Vector3Int(3,1,3) }),

            // Level 48: Mini uzay gemisi (19 küp)
            CreateLevelWithFixed(48, "Spaceship", 10, "Tiny spaceship",
                new Vector3Int[] {
                    // Burun
                    new Vector3Int(5,1,1),
                    // Kokpit
                    new Vector3Int(4,1,0), new Vector3Int(4,1,1), new Vector3Int(4,1,2),
                    // Gövde
                    new Vector3Int(3,1,0), new Vector3Int(3,1,1), new Vector3Int(3,1,2),
                    new Vector3Int(2,1,1), new Vector3Int(2,0,1), new Vector3Int(2,2,1),
                    // Sol kanat
                    new Vector3Int(3,0,0), new Vector3Int(2,0,0), new Vector3Int(1,0,0),
                    // Sağ kanat
                    new Vector3Int(3,2,0), new Vector3Int(2,2,0), new Vector3Int(1,2,0),
                    // Motorlar
                    new Vector3Int(1,1,0), new Vector3Int(1,1,2),
                    new Vector3Int(0,1,1)
                },
                new Vector3Int[] { new Vector3Int(5,1,1), new Vector3Int(1,0,0), new Vector3Int(1,2,0), new Vector3Int(0,1,1) }),

            // Level 49: Dev ağaç mini (20 küp)
            CreateLevelWithFixed(49, "Yggdrasil", 13, "World tree",
                new Vector3Int[] {
                    // Kökler
                    new Vector3Int(0,0,1), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(3,0,1), new Vector3Int(1,0,2), new Vector3Int(2,0,2),
                    // Gövde
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1),
                    new Vector3Int(1,2,1), new Vector3Int(2,2,1),
                    new Vector3Int(2,3,1),
                    // Dallar
                    new Vector3Int(0,3,0), new Vector3Int(0,4,0),
                    new Vector3Int(3,3,2), new Vector3Int(3,4,2),
                    // Yaprak taçları
                    new Vector3Int(0,4,1), new Vector3Int(1,4,1),
                    new Vector3Int(2,4,1), new Vector3Int(3,4,1),
                    new Vector3Int(2,5,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,1), new Vector3Int(3,0,1), new Vector3Int(0,4,0), new Vector3Int(3,4,2), new Vector3Int(2,5,1) }),

            // Level 50: Galaksi mini (20 küp)
            CreateLevelWithFixed(50, "Big Bang", 1, "Mini galaxy",
                new Vector3Int[] {
                    // Merkez
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(2,0,3), new Vector3Int(3,0,3),
                    new Vector3Int(2,1,2), new Vector3Int(3,1,3),
                    // Spiral kol 1
                    new Vector3Int(4,0,3), new Vector3Int(5,0,4), new Vector3Int(5,1,5),
                    // Spiral kol 2
                    new Vector3Int(1,0,2), new Vector3Int(0,0,1), new Vector3Int(0,1,0),
                    // Yıldız parçaları
                    new Vector3Int(4,0,1), new Vector3Int(1,0,4),
                    new Vector3Int(0,0,3), new Vector3Int(5,0,2),
                    // Dikey
                    new Vector3Int(3,2,2), new Vector3Int(2,2,3),
                    new Vector3Int(3,3,3), new Vector3Int(2,3,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,1), new Vector3Int(5,0,4), new Vector3Int(0,1,0), new Vector3Int(5,1,5), new Vector3Int(3,3,3) }),

            // ============ BÖLÜM 6: YARATICI (51-60) — 16-22 küp ============

            // Level 51: Kelebek (18 küp)
            CreateLevelWithFixed(51, "Butterfly", 6, "Small butterfly",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(2,0,0), new Vector3Int(2,1,0), new Vector3Int(2,2,0), new Vector3Int(2,3,0),
                    // Sol kanat
                    new Vector3Int(1,1,0), new Vector3Int(0,1,0), new Vector3Int(1,2,0), new Vector3Int(0,2,0),
                    new Vector3Int(0,3,0),
                    // Sağ kanat
                    new Vector3Int(3,1,0), new Vector3Int(4,1,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0),
                    new Vector3Int(4,3,0),
                    // Derinlik
                    new Vector3Int(2,1,1), new Vector3Int(2,2,1),
                    // Anten
                    new Vector3Int(1,4,0), new Vector3Int(3,4,0)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(4,1,0), new Vector3Int(1,4,0), new Vector3Int(3,4,0) }),

            // Level 52: Köprü kemer (18 küp)
            CreateLevelWithFixed(52, "Arch Bridge", 9, "Arch bridge",
                new Vector3Int[] {
                    // Sol ayak
                    new Vector3Int(0,0,0), new Vector3Int(0,0,1), new Vector3Int(0,1,0), new Vector3Int(0,1,1),
                    // Kemer
                    new Vector3Int(1,2,0), new Vector3Int(1,2,1), new Vector3Int(2,3,0), new Vector3Int(2,3,1),
                    new Vector3Int(3,3,0), new Vector3Int(3,3,1), new Vector3Int(4,2,0), new Vector3Int(4,2,1),
                    // Sağ ayak
                    new Vector3Int(5,0,0), new Vector3Int(5,0,1), new Vector3Int(5,1,0), new Vector3Int(5,1,1),
                    // Korkuluk
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,0,0), new Vector3Int(2,4,0), new Vector3Int(3,4,0) }),

            // Level 53: Nota (18 küp)
            CreateLevelWithFixed(53, "Note", 12, "Music note",
                new Vector3Int[] {
                    // Sol nota
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1), new Vector3Int(1,0,1),
                    // Sol sap
                    new Vector3Int(1,1,0), new Vector3Int(1,2,0), new Vector3Int(1,3,0), new Vector3Int(1,4,0),
                    // Bağlantı
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    // Sağ sap
                    new Vector3Int(4,3,0), new Vector3Int(4,2,0), new Vector3Int(4,1,0),
                    // Sağ nota
                    new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(3,0,1), new Vector3Int(4,0,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,1), new Vector3Int(2,4,0) }),

            // Level 54: Mini kale gece (20 küp)
            CreateLevelWithFixed(54, "Night Castle", 15, "Castle at night",
                new Vector3Int[] {
                    // Taban duvar
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(4,0,1),
                    // Duvarlar
                    new Vector3Int(0,1,0), new Vector3Int(4,1,0), new Vector3Int(0,1,1), new Vector3Int(4,1,1),
                    // Kuleler
                    new Vector3Int(0,2,0), new Vector3Int(4,2,0), new Vector3Int(0,3,0),
                    // Kapı
                    new Vector3Int(2,1,0),
                    // İç avlu
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    // Mazgal
                    new Vector3Int(1,2,0), new Vector3Int(3,2,0)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(4,2,0), new Vector3Int(0,0,1), new Vector3Int(4,0,1) }),

            // Level 55: Ahtapot (20 küp)
            CreateLevelWithFixed(55, "Octopus", 3, "Tiny octopus",
                new Vector3Int[] {
                    // Baş
                    new Vector3Int(2,3,1), new Vector3Int(3,3,1), new Vector3Int(2,3,2), new Vector3Int(3,3,2),
                    new Vector3Int(2,4,1), new Vector3Int(3,4,1),
                    // Kollar (4 çift)
                    new Vector3Int(1,2,1), new Vector3Int(0,1,0),
                    new Vector3Int(4,2,1), new Vector3Int(5,1,0),
                    new Vector3Int(1,2,2), new Vector3Int(0,1,3),
                    new Vector3Int(4,2,2), new Vector3Int(5,1,3),
                    // Orta kollar
                    new Vector3Int(2,2,0), new Vector3Int(2,1,0),
                    new Vector3Int(3,2,3), new Vector3Int(3,1,3),
                    // Göz
                    new Vector3Int(2,4,0), new Vector3Int(3,4,2)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(5,1,0), new Vector3Int(0,1,3), new Vector3Int(5,1,3), new Vector3Int(2,4,1) }),

            // Level 56: Kule blokları (20 küp)
            CreateLevelWithFixed(56, "Tower Blocks", 7, "Block tower",
                new Vector3Int[] {
                    // Taban 3x3
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2),
                    // 2. kat 2x2
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0),
                    new Vector3Int(0,1,1), new Vector3Int(1,1,1),
                    // 3. kat
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0),
                    new Vector3Int(0,2,1), new Vector3Int(1,2,1),
                    // Zirve
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0),
                    new Vector3Int(0,4,0)
                },
                new Vector3Int[] { new Vector3Int(2,0,2), new Vector3Int(0,4,0), new Vector3Int(2,0,0) }),

            // Level 57: Taç (20 küp)
            CreateLevelWithFixed(57, "Crown", 10, "Royal crown",
                new Vector3Int[] {
                    // Bant
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    // 2. sıra
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    // Sivri uçlar
                    new Vector3Int(0,2,0), new Vector3Int(2,2,0), new Vector3Int(4,2,0),
                    // Mücevherler
                    new Vector3Int(0,2,1), new Vector3Int(4,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(4,2,0), new Vector3Int(2,2,0), new Vector3Int(0,2,1) }),

            // Level 58: Yanardağ (22 küp)
            CreateLevelWithFixed(58, "Eruption", 13, "Volcanic eruption",
                new Vector3Int[] {
                    // Geniş taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    // 2. katman
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    // Krater
                    new Vector3Int(2,2,0), new Vector3Int(2,2,1),
                    // Duman
                    new Vector3Int(2,3,0), new Vector3Int(2,4,0),
                    // Lav
                    new Vector3Int(4,1,0), new Vector3Int(5,0,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(5,0,0), new Vector3Int(2,4,0), new Vector3Int(0,0,1) }),

            // Level 59: Kum saati (20 küp)
            CreateLevelWithFixed(59, "Hourglass", 0, "Time flows",
                new Vector3Int[] {
                    // Üst geniş
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0), new Vector3Int(2,4,0), new Vector3Int(3,4,0),
                    new Vector3Int(0,4,1), new Vector3Int(1,4,1), new Vector3Int(2,4,1), new Vector3Int(3,4,1),
                    // Daralma
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0),
                    // Boğaz
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    // Alt geniş
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1)
                },
                new Vector3Int[] { new Vector3Int(0,4,0), new Vector3Int(3,4,0), new Vector3Int(0,0,0), new Vector3Int(3,0,1) }),

            // Level 60: Deniz kabuğu (22 küp)
            CreateLevelWithFixed(60, "Seashell", 5, "Spiral shell",
                new Vector3Int[] {
                    // Spiral dış
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(3,0,1), new Vector3Int(3,0,2),
                    new Vector3Int(2,0,2), new Vector3Int(1,0,2), new Vector3Int(0,0,2),
                    new Vector3Int(0,0,1),
                    // Spiral iç (yükselen)
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,1,1), new Vector3Int(1,1,1),
                    // Spiral merkez
                    new Vector3Int(1,2,1), new Vector3Int(2,2,1),
                    // Kabuk kalınlık
                    new Vector3Int(0,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(3,1,2), new Vector3Int(0,1,2),
                    // Sivri uç
                    new Vector3Int(4,0,0), new Vector3Int(4,0,1)
                },
                new Vector3Int[] { new Vector3Int(4,0,0), new Vector3Int(0,0,2), new Vector3Int(1,2,1), new Vector3Int(2,2,1) }),

            // ============ BÖLÜM 7: DERİNLİK (61-70) — 18-24 küp ============

            // Level 61: Kuş (20 küp)
            CreateLevelWithFixed(61, "Bird", 8, "Flying bird",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1),
                    new Vector3Int(3,2,0), new Vector3Int(3,2,2),
                    // Baş
                    new Vector3Int(5,2,1), new Vector3Int(5,3,1),
                    // Gaga
                    new Vector3Int(6,3,1),
                    // Sol kanat yukarı
                    new Vector3Int(2,3,2), new Vector3Int(1,4,2), new Vector3Int(0,5,2),
                    new Vector3Int(2,3,0), new Vector3Int(1,4,0), new Vector3Int(0,5,0),
                    // Kuyruk
                    new Vector3Int(1,2,1), new Vector3Int(0,2,1), new Vector3Int(0,3,1),
                    // Ayaklar
                    new Vector3Int(3,1,1), new Vector3Int(3,0,1),
                    new Vector3Int(4,1,1)
                },
                new Vector3Int[] { new Vector3Int(6,3,1), new Vector3Int(0,5,2), new Vector3Int(0,5,0), new Vector3Int(0,3,1) }),

            // Level 62: Grid merdiven (20 küp)
            CreateLevelWithFixed(62, "Staircase", 11, "Step grid",
                new Vector3Int[] {
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,1,1), new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2), new Vector3Int(2,2,2), new Vector3Int(3,2,2),
                    new Vector3Int(0,3,3), new Vector3Int(1,3,3), new Vector3Int(2,3,3), new Vector3Int(3,3,3),
                    // Korkuluk
                    new Vector3Int(0,1,0), new Vector3Int(0,2,1), new Vector3Int(0,3,2), new Vector3Int(0,4,3)
                },
                new Vector3Int[] { new Vector3Int(3,0,0), new Vector3Int(3,3,3), new Vector3Int(0,4,3), new Vector3Int(0,1,0) }),

            // Level 63: Balık (22 küp)
            CreateLevelWithFixed(63, "Fish", 14, "Swimming fish",
                new Vector3Int[] {
                    // Gövde (oval)
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    // Göz
                    new Vector3Int(4,3,1),
                    // Kuyruk
                    new Vector3Int(0,1,0), new Vector3Int(0,3,0),
                    // Yüzgeçler
                    new Vector3Int(3,4,0), new Vector3Int(3,0,0),
                    // Derinlik
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1),
                    new Vector3Int(1,2,1),
                    // Ağız
                    new Vector3Int(5,2,1), new Vector3Int(6,2,0)
                },
                new Vector3Int[] { new Vector3Int(6,2,0), new Vector3Int(0,1,0), new Vector3Int(0,3,0), new Vector3Int(4,3,1) }),

            // Level 64: Mantar (22 küp)
            CreateLevelWithFixed(64, "Mushroom", 2, "Magic mushroom",
                new Vector3Int[] {
                    // Sap
                    new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    // Şapka
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0),
                    new Vector3Int(0,2,1), new Vector3Int(1,2,1), new Vector3Int(2,2,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1),
                    new Vector3Int(1,2,2), new Vector3Int(2,2,2), new Vector3Int(3,2,2),
                    // Üst kubba
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(2,3,1), new Vector3Int(2,4,0)
                },
                new Vector3Int[] { new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(0,2,0), new Vector3Int(4,2,1), new Vector3Int(2,4,0) }),

            // Level 65: At (22 küp)
            CreateLevelWithFixed(65, "Knight", 6, "Chess knight",
                new Vector3Int[] {
                    // Kaide
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    // Boyun
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(2,2,0), new Vector3Int(2,3,0),
                    // Baş
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(3,5,0), new Vector3Int(4,3,0),
                    // Yele
                    new Vector3Int(1,2,0), new Vector3Int(1,3,0), new Vector3Int(1,4,0),
                    // Derinlik
                    new Vector3Int(2,4,1), new Vector3Int(3,4,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,1), new Vector3Int(4,4,0), new Vector3Int(3,5,0) }),

            // Level 66: Hilal ay (22 küp)
            CreateLevelWithFixed(66, "Crescent", 9, "Crescent moon",
                new Vector3Int[] {
                    // Dış yay
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(1,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(4,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(1,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    // İç boşluk (dolu olmayan iç yay)
                    new Vector3Int(2,1,0), new Vector3Int(3,2,0),
                    new Vector3Int(3,3,0), new Vector3Int(2,4,0),
                    // Yıldız
                    new Vector3Int(6,4,0), new Vector3Int(5,3,0), new Vector3Int(6,3,0), new Vector3Int(7,3,0),
                    // Derinlik
                    new Vector3Int(0,2,1), new Vector3Int(0,3,1)
                },
                new Vector3Int[] { new Vector3Int(2,0,0), new Vector3Int(2,5,0), new Vector3Int(6,4,0), new Vector3Int(5,3,0) }),

            // Level 67: Piramit çerçeve (23 küp)
            CreateLevelWithFixed(67, "Pyramid", 12, "Frame pyramid",
                new Vector3Int[] {
                    // Taban çerçeve
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(4,0,2),
                    new Vector3Int(0,0,3), new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3),
                    // 2. kat
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(1,1,2), new Vector3Int(3,1,2),
                    // 3. kat
                    new Vector3Int(2,2,1), new Vector3Int(2,2,2),
                    // Zirve
                    new Vector3Int(2,3,1),
                    new Vector3Int(2,1,2)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,0,3), new Vector3Int(4,0,3), new Vector3Int(2,3,1) }),

            // Level 68: Çapa (24 küp)
            CreateLevelWithFixed(68, "Anchor", 15, "Ship anchor",
                new Vector3Int[] {
                    // Halka üst
                    new Vector3Int(2,6,0), new Vector3Int(3,6,0), new Vector3Int(1,5,0), new Vector3Int(4,5,0),
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0),
                    // Sap
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    // Yatay çubuk
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(4,3,0), new Vector3Int(5,3,0),
                    // Kancalar
                    new Vector3Int(0,2,0), new Vector3Int(0,1,0), new Vector3Int(1,0,0),
                    new Vector3Int(5,2,0), new Vector3Int(5,1,0), new Vector3Int(4,0,0),
                    // Derinlik
                    new Vector3Int(2,6,1), new Vector3Int(3,1,1)
                },
                new Vector3Int[] { new Vector3Int(2,6,0), new Vector3Int(3,6,0), new Vector3Int(1,0,0), new Vector3Int(4,0,0), new Vector3Int(2,6,1) }),

            // Level 69: Gitar (24 küp)
            CreateLevelWithFixed(69, "Guitar", 4, "Acoustic guitar",
                new Vector3Int[] {
                    // Gövde (alt daire)
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(1,4,0), new Vector3Int(2,4,0),
                    // Ses deliği
                    new Vector3Int(1,2,1),
                    // Sap
                    new Vector3Int(1,5,0), new Vector3Int(2,5,0),
                    new Vector3Int(1,6,0), new Vector3Int(2,6,0),
                    new Vector3Int(1,7,0), new Vector3Int(2,7,0),
                    // Başlık
                    new Vector3Int(0,8,0), new Vector3Int(1,8,0), new Vector3Int(2,8,0), new Vector3Int(3,8,0)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(3,3,0), new Vector3Int(0,8,0), new Vector3Int(3,8,0), new Vector3Int(1,2,1) }),

            // Level 70: Ejderha kafası (24 küp)
            CreateLevelWithFixed(70, "Dragon", 7, "Dragon head",
                new Vector3Int[] {
                    // Alt çene
                    new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0), new Vector3Int(5,1,0),
                    // Üst çene
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0), new Vector3Int(6,2,0),
                    // Kafa
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(1,4,0), new Vector3Int(2,4,0),
                    // Boynuz
                    new Vector3Int(0,5,0), new Vector3Int(2,5,0),
                    // Ateş
                    new Vector3Int(6,1,0), new Vector3Int(7,1,0),
                    // Derinlik
                    new Vector3Int(3,2,1), new Vector3Int(2,3,1),
                    // Boyun
                    new Vector3Int(0,3,0)
                },
                new Vector3Int[] { new Vector3Int(7,1,0), new Vector3Int(0,5,0), new Vector3Int(2,5,0), new Vector3Int(0,3,0) }),

            // ============ BÖLÜM 8: USTALAR (71-80) — 20-26 küp ============

            // Level 71: Yüzük (22 küp)
            CreateLevelWithFixed(71, "Ring", 10, "Diamond ring",
                new Vector3Int[] {
                    // Halka
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(4,0,2),
                    new Vector3Int(1,0,3), new Vector3Int(2,0,3), new Vector3Int(3,0,3),
                    // Halka kalınlık
                    new Vector3Int(1,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(0,1,1), new Vector3Int(4,1,1),
                    new Vector3Int(0,1,2), new Vector3Int(4,1,2),
                    new Vector3Int(1,1,3), new Vector3Int(3,1,3),
                    // Elmas
                    new Vector3Int(2,1,0), new Vector3Int(2,2,0),
                    new Vector3Int(1,2,0), new Vector3Int(3,2,0)
                },
                new Vector3Int[] { new Vector3Int(2,2,0), new Vector3Int(0,0,2), new Vector3Int(4,0,2), new Vector3Int(2,0,3) }),

            // Level 72: Gemi (24 küp)
            CreateLevelWithFixed(72, "Ship", 13, "Small ship",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    // Pruva
                    new Vector3Int(6,0,1), new Vector3Int(6,0,2),
                    // Kabin
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(2,1,2), new Vector3Int(3,1,2),
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1),
                    // Direk + yelken
                    new Vector3Int(5,1,1), new Vector3Int(5,2,1), new Vector3Int(5,3,1),
                    new Vector3Int(5,2,2), new Vector3Int(5,3,2),
                    // Dümen
                    new Vector3Int(0,0,1)
                },
                new Vector3Int[] { new Vector3Int(6,0,1), new Vector3Int(0,0,1), new Vector3Int(5,3,1), new Vector3Int(5,3,2) }),

            // Level 73: Masa lambası (24 küp)
            CreateLevelWithFixed(73, "Lamp", 1, "Desk lamp",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2),
                    // Sap
                    new Vector3Int(2,1,1), new Vector3Int(2,2,1), new Vector3Int(2,3,1),
                    // Abajur
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0), new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(0,4,1), new Vector3Int(1,4,1), new Vector3Int(2,4,1), new Vector3Int(3,4,1), new Vector3Int(4,4,1),
                    // Abajur üstü
                    new Vector3Int(1,5,0), new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    new Vector3Int(2,5,1), new Vector3Int(2,6,0)
                },
                new Vector3Int[] { new Vector3Int(1,0,1), new Vector3Int(3,0,2), new Vector3Int(0,4,0), new Vector3Int(4,4,0), new Vector3Int(2,6,0) }),

            // Level 74: İçi boş kutu (24 küp)
            CreateLevelWithFixed(74, "Cube World", 5, "Hollow cube",
                new Vector3Int[] {
                    // Alt yüz
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(2,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2),
                    // Orta kenarlar
                    new Vector3Int(0,1,0), new Vector3Int(2,1,0), new Vector3Int(0,1,2), new Vector3Int(2,1,2),
                    // Üst yüz
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0),
                    new Vector3Int(0,2,1), new Vector3Int(2,2,1),
                    new Vector3Int(0,2,2), new Vector3Int(1,2,2), new Vector3Int(2,2,2),
                    // Çıkıntılar
                    new Vector3Int(3,0,0), new Vector3Int(3,0,2),
                    new Vector3Int(1,0,1), new Vector3Int(1,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(2,2,2), new Vector3Int(3,0,0), new Vector3Int(3,0,2) }),

            // Level 75: Akrep (24 küp)
            CreateLevelWithFixed(75, "Scorpion", 8, "Tiny scorpion",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(5,0,3),
                    // Baş + kıskaçlar
                    new Vector3Int(6,0,2), new Vector3Int(6,0,3),
                    new Vector3Int(7,0,1), new Vector3Int(7,0,4),
                    // Kuyruk
                    new Vector3Int(2,0,2), new Vector3Int(1,0,2), new Vector3Int(1,1,2),
                    new Vector3Int(1,2,2), new Vector3Int(2,2,2), new Vector3Int(2,3,2),
                    new Vector3Int(3,3,2), new Vector3Int(3,3,3),
                    // Bacaklar
                    new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(3,0,4), new Vector3Int(4,0,4), new Vector3Int(5,0,4)
                },
                new Vector3Int[] { new Vector3Int(7,0,1), new Vector3Int(7,0,4), new Vector3Int(3,3,3), new Vector3Int(1,0,2) }),

            // Level 76: Kafatası (24 küp)
            CreateLevelWithFixed(76, "Skull", 11, "Pixel skull",
                new Vector3Int[] {
                    // Çene
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    // Zigomatik
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    // Göz çevresi
                    new Vector3Int(1,2,0), new Vector3Int(3,2,0),
                    // Burun
                    new Vector3Int(2,2,0),
                    // Alın
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(0,4,0), new Vector3Int(1,4,0), new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(1,5,0), new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    // Derinlik
                    new Vector3Int(2,3,1)
                },
                new Vector3Int[] { new Vector3Int(2,5,0), new Vector3Int(0,1,0), new Vector3Int(4,1,0), new Vector3Int(2,2,0) }),

            // Level 77: Robot (26 küp)
            CreateLevelWithFixed(77, "Robot", 14, "Mini robot",
                new Vector3Int[] {
                    // Ayaklar
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    // Bacaklar
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(4,1,1), new Vector3Int(5,1,1),
                    // Gövde
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0), new Vector3Int(6,2,0),
                    new Vector3Int(1,2,1), new Vector3Int(5,2,1),
                    // Baş
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    // Anten
                    new Vector3Int(3,5,0),
                    // Kollar
                    new Vector3Int(0,1,0), new Vector3Int(6,1,0)
                },
                new Vector3Int[] { new Vector3Int(1,0,1), new Vector3Int(5,0,1), new Vector3Int(3,5,0), new Vector3Int(0,1,0), new Vector3Int(6,1,0) }),

            // Level 78: Kedi (26 küp)
            CreateLevelWithFixed(78, "Cat", 3, "Sitting cat",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(4,1,1),
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0),
                    // Bacaklar
                    new Vector3Int(2,0,0), new Vector3Int(2,0,1), new Vector3Int(4,0,0), new Vector3Int(4,0,1),
                    // Baş
                    new Vector3Int(5,2,0), new Vector3Int(5,2,1), new Vector3Int(5,3,0), new Vector3Int(5,3,1),
                    new Vector3Int(6,3,0),
                    // Kulaklar
                    new Vector3Int(5,4,0), new Vector3Int(5,4,1),
                    // Burun
                    new Vector3Int(6,2,0),
                    // Kuyruk
                    new Vector3Int(1,1,0), new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    // Bıyık detay
                    new Vector3Int(6,3,1), new Vector3Int(3,2,1)
                },
                new Vector3Int[] { new Vector3Int(5,4,0), new Vector3Int(5,4,1), new Vector3Int(0,3,0), new Vector3Int(6,3,0) }),

            // Level 79: Yel değirmeni (26 küp)
            CreateLevelWithFixed(79, "Windmill", 6, "Small windmill",
                new Vector3Int[] {
                    // Kule
                    new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(2,0,2), new Vector3Int(3,0,2),
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1), new Vector3Int(2,1,2), new Vector3Int(3,1,2),
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1),
                    // Çatı
                    new Vector3Int(2,3,1), new Vector3Int(3,3,1),
                    new Vector3Int(2,3,2), new Vector3Int(3,3,2),
                    // Kanatlar
                    new Vector3Int(1,2,1), new Vector3Int(0,3,1),
                    new Vector3Int(4,2,1), new Vector3Int(5,3,1),
                    new Vector3Int(1,1,1), new Vector3Int(0,0,1),
                    new Vector3Int(4,1,1), new Vector3Int(5,0,1),
                    // Kapı
                    new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    // Bayrak
                    new Vector3Int(3,4,1), new Vector3Int(3,4,2)
                },
                new Vector3Int[] { new Vector3Int(0,3,1), new Vector3Int(5,3,1), new Vector3Int(0,0,1), new Vector3Int(5,0,1), new Vector3Int(3,4,2) }),

            // Level 80: Kristal mağara (26 küp)
            CreateLevelWithFixed(80, "Crystal Cave", 9, "Gem cave",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    // Sol duvar
                    new Vector3Int(0,1,0), new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    // Sağ duvar
                    new Vector3Int(4,1,0), new Vector3Int(4,2,0),
                    // Tavan
                    new Vector3Int(0,3,1), new Vector3Int(1,3,0), new Vector3Int(2,3,0),
                    // Dikitler
                    new Vector3Int(2,1,0), new Vector3Int(2,2,0),
                    // Kristal
                    new Vector3Int(3,1,1), new Vector3Int(3,2,1),
                    // Sarkıt
                    new Vector3Int(1,2,0), new Vector3Int(3,2,0),
                    // Giriş
                    new Vector3Int(5,0,0), new Vector3Int(5,0,1)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(5,0,0), new Vector3Int(5,0,1), new Vector3Int(3,2,1), new Vector3Int(0,3,1) }),

            // ============ BÖLÜM 9: EFSANE (81-90) — 22-28 küp ============

            // Level 81: Tavus kuşu (24 küp)
            CreateLevelWithFixed(81, "Peacock", 12, "Proud peacock",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(3,1,1), new Vector3Int(3,1,2), new Vector3Int(3,2,1),
                    // Boyun + baş
                    new Vector3Int(4,2,1), new Vector3Int(4,3,1), new Vector3Int(5,3,1),
                    // Gaga
                    new Vector3Int(6,3,1),
                    // Bacaklar
                    new Vector3Int(3,0,1), new Vector3Int(3,0,2),
                    // Kuyruk yelpaze
                    new Vector3Int(2,1,1), new Vector3Int(1,2,0), new Vector3Int(1,2,1), new Vector3Int(1,2,2),
                    new Vector3Int(0,3,0), new Vector3Int(0,3,1), new Vector3Int(0,3,2), new Vector3Int(0,3,3),
                    new Vector3Int(1,4,0), new Vector3Int(1,4,1), new Vector3Int(1,4,2), new Vector3Int(1,4,3),
                    // Kuyruk göz deseni
                    new Vector3Int(0,4,0), new Vector3Int(0,4,3),
                    // Tepelik
                    new Vector3Int(4,4,1)
                },
                new Vector3Int[] { new Vector3Int(6,3,1), new Vector3Int(0,4,0), new Vector3Int(0,4,3), new Vector3Int(4,4,1), new Vector3Int(3,0,2) }),

            // Level 82: Piramit grid (26 küp)
            CreateLevelWithFixed(82, "Pyramid Grid", 15, "Desert steps",
                new Vector3Int[] {
                    // Taban 5x5
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    // 2. kat 3x3
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(1,1,2), new Vector3Int(2,1,2), new Vector3Int(3,1,2),
                    // Zirve
                    new Vector3Int(2,2,1), new Vector3Int(2,3,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(0,0,2), new Vector3Int(4,0,2), new Vector3Int(2,3,1) }),

            // Level 83: Geyik (26 küp)
            CreateLevelWithFixed(83, "Deer", 2, "Majestic deer",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1), new Vector3Int(5,2,1),
                    // Bacaklar
                    new Vector3Int(2,1,0), new Vector3Int(2,0,0), new Vector3Int(5,1,0), new Vector3Int(5,0,0),
                    // Boyun + Baş
                    new Vector3Int(6,3,0), new Vector3Int(6,3,1), new Vector3Int(6,4,0), new Vector3Int(7,4,0),
                    // Boynuzlar
                    new Vector3Int(6,5,0), new Vector3Int(5,6,0), new Vector3Int(7,5,0),
                    new Vector3Int(6,5,1), new Vector3Int(5,6,1),
                    // Kuyruk
                    new Vector3Int(1,3,0), new Vector3Int(0,3,0),
                    // Burun
                    new Vector3Int(7,3,0),
                    // Detay
                    new Vector3Int(3,3,0), new Vector3Int(4,3,0)
                },
                new Vector3Int[] { new Vector3Int(5,6,0), new Vector3Int(5,6,1), new Vector3Int(7,4,0), new Vector3Int(0,3,0), new Vector3Int(7,3,0) }),

            // Level 84: Uçurtma (26 küp)
            CreateLevelWithFixed(84, "Kite", 7, "Flying kite",
                new Vector3Int[] {
                    // Baklava gövde
                    new Vector3Int(2,2,0),
                    new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(2,4,0),
                    // Derinlik
                    new Vector3Int(2,2,1), new Vector3Int(2,0,1), new Vector3Int(2,4,1),
                    new Vector3Int(0,0,1), new Vector3Int(4,0,1),
                    // Kuyruk
                    new Vector3Int(2,5,0), new Vector3Int(2,6,0), new Vector3Int(1,7,0), new Vector3Int(3,7,0),
                    // İp
                    new Vector3Int(2,5,1), new Vector3Int(2,6,1),
                    // Süsleme
                    new Vector3Int(1,0,1), new Vector3Int(3,0,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,0), new Vector3Int(1,7,0), new Vector3Int(3,7,0), new Vector3Int(2,4,1) }),

            // Level 85: Kadeh (26 küp)
            CreateLevelWithFixed(85, "Goblet", 10, "Golden goblet",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0),
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    // Sap
                    new Vector3Int(2,1,0), new Vector3Int(2,1,1),
                    new Vector3Int(2,2,0), new Vector3Int(2,2,1),
                    // Kase
                    new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    new Vector3Int(0,3,1), new Vector3Int(4,3,1),
                    new Vector3Int(0,3,2), new Vector3Int(1,3,2), new Vector3Int(2,3,2), new Vector3Int(3,3,2), new Vector3Int(4,3,2),
                    // Ağız
                    new Vector3Int(0,4,0), new Vector3Int(4,4,0),
                    new Vector3Int(0,4,2), new Vector3Int(4,4,2)
                },
                new Vector3Int[] { new Vector3Int(1,0,0), new Vector3Int(3,0,1), new Vector3Int(0,4,0), new Vector3Int(4,4,0), new Vector3Int(0,4,2) }),

            // Level 86: DNA sarmalı (28 küp)
            CreateLevelWithFixed(86, "DNA", 13, "DNA helix",
                new Vector3Int[] {
                    // Sol sarmal
                    new Vector3Int(1,0,0), new Vector3Int(0,1,0), new Vector3Int(0,2,0), new Vector3Int(1,3,0),
                    new Vector3Int(2,4,0), new Vector3Int(2,5,0), new Vector3Int(1,6,0), new Vector3Int(0,7,0),
                    // Sağ sarmal
                    new Vector3Int(4,0,0), new Vector3Int(5,1,0), new Vector3Int(5,2,0), new Vector3Int(4,3,0),
                    new Vector3Int(3,4,0), new Vector3Int(3,5,0), new Vector3Int(4,6,0), new Vector3Int(5,7,0),
                    // Bağlantılar
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    new Vector3Int(2,7,0), new Vector3Int(3,7,0),
                    // Derinlik
                    new Vector3Int(1,0,1), new Vector3Int(4,0,1),
                    new Vector3Int(0,7,1), new Vector3Int(5,7,1)
                },
                new Vector3Int[] { new Vector3Int(1,0,0), new Vector3Int(4,0,0), new Vector3Int(0,7,0), new Vector3Int(5,7,0), new Vector3Int(1,0,1) }),

            // Level 87: Ev (28 küp)
            CreateLevelWithFixed(87, "House", 0, "Cozy house",
                new Vector3Int[] {
                    // Taban duvarlar
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    new Vector3Int(0,0,1), new Vector3Int(4,0,1),
                    // 1. kat
                    new Vector3Int(0,1,0), new Vector3Int(4,1,0), new Vector3Int(0,1,2), new Vector3Int(4,1,2),
                    // Çatı kat
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0),
                    // Çatı sivri
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0),
                    new Vector3Int(2,4,0),
                    // Baca
                    new Vector3Int(4,3,0), new Vector3Int(4,4,0),
                    // Kapı
                    new Vector3Int(2,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(4,0,2), new Vector3Int(2,4,0), new Vector3Int(4,4,0) }),

            // Level 88: Kelebek balığı (28 küp)
            CreateLevelWithFixed(88, "Butterflyfish", 5, "Tropical fish",
                new Vector3Int[] {
                    // Disk gövde
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0), new Vector3Int(4,1,0),
                    new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0),
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0), new Vector3Int(5,3,0),
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0), new Vector3Int(4,4,0),
                    // Burun
                    new Vector3Int(6,2,0), new Vector3Int(6,3,0),
                    // Kuyruk
                    new Vector3Int(0,1,0), new Vector3Int(0,4,0),
                    // Yüzgeçler
                    new Vector3Int(3,5,0), new Vector3Int(3,0,0),
                    // Göz
                    new Vector3Int(4,3,1),
                    // Derinlik bant
                    new Vector3Int(2,2,1), new Vector3Int(2,3,1), new Vector3Int(3,2,1), new Vector3Int(3,3,1),
                    new Vector3Int(4,2,1)
                },
                new Vector3Int[] { new Vector3Int(6,2,0), new Vector3Int(0,1,0), new Vector3Int(0,4,0), new Vector3Int(4,3,1), new Vector3Int(3,5,0) }),

            // Level 89: Şato (28 küp)
            CreateLevelWithFixed(89, "Chateau", 8, "Grand chateau",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(0,0,2), new Vector3Int(1,0,2), new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2), new Vector3Int(5,0,2),
                    // 1. kat köşeler
                    new Vector3Int(0,1,0), new Vector3Int(5,1,0), new Vector3Int(0,1,2), new Vector3Int(5,1,2),
                    // Kuleler
                    new Vector3Int(0,2,0), new Vector3Int(0,3,0), new Vector3Int(5,2,0),
                    new Vector3Int(0,2,2), new Vector3Int(0,3,2), new Vector3Int(0,4,2),
                    new Vector3Int(5,2,2), new Vector3Int(5,3,2),
                    // Kapı
                    new Vector3Int(2,1,0), new Vector3Int(3,1,0)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(0,4,2), new Vector3Int(5,3,2), new Vector3Int(5,2,0), new Vector3Int(3,1,0) }),

            // Level 90: Kozmos (28 küp)
            CreateLevelWithFixed(90, "Cosmos", 11, "Cosmic nebula",
                new Vector3Int[] {
                    // Merkez kütle
                    new Vector3Int(3,0,3), new Vector3Int(4,0,3), new Vector3Int(3,0,4), new Vector3Int(4,0,4),
                    new Vector3Int(3,1,3), new Vector3Int(4,1,3), new Vector3Int(3,1,4), new Vector3Int(4,1,4),
                    // Spiral kol 1
                    new Vector3Int(5,0,4), new Vector3Int(6,0,5), new Vector3Int(6,1,6),
                    new Vector3Int(5,1,5),
                    // Spiral kol 2
                    new Vector3Int(2,0,3), new Vector3Int(1,0,2), new Vector3Int(1,1,1),
                    new Vector3Int(2,1,2),
                    // Yıldızlar
                    new Vector3Int(7,0,6), new Vector3Int(0,0,1), new Vector3Int(0,0,0),
                    new Vector3Int(7,0,3), new Vector3Int(0,0,4),
                    // Dikey
                    new Vector3Int(3,2,3), new Vector3Int(4,2,4),
                    new Vector3Int(3,3,3), new Vector3Int(4,3,4),
                    // Toz
                    new Vector3Int(5,0,2), new Vector3Int(2,0,5),
                    new Vector3Int(6,0,4)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(7,0,6), new Vector3Int(3,3,3), new Vector3Int(4,3,4), new Vector3Int(7,0,3) }),

            // ============ BÖLÜM 10: EFSANEVİ (91-100) — 24-30 küp ============

            // Level 91: Anka kuşu (26 küp)
            CreateLevelWithFixed(91, "Phoenix", 4, "Rising phoenix",
                new Vector3Int[] {
                    // Gövde
                    new Vector3Int(3,3,1), new Vector3Int(4,3,1), new Vector3Int(3,4,1), new Vector3Int(4,4,1),
                    // Baş
                    new Vector3Int(4,5,1), new Vector3Int(5,5,1), new Vector3Int(5,6,1),
                    // Gaga
                    new Vector3Int(6,6,1),
                    // Sol kanat
                    new Vector3Int(2,4,2), new Vector3Int(1,5,2), new Vector3Int(0,6,2),
                    new Vector3Int(2,4,0), new Vector3Int(1,5,0), new Vector3Int(0,6,0),
                    // Kuyruk (ateş)
                    new Vector3Int(3,2,1), new Vector3Int(2,1,1), new Vector3Int(1,0,1),
                    new Vector3Int(2,1,0), new Vector3Int(1,0,0), new Vector3Int(0,0,1),
                    new Vector3Int(2,1,2), new Vector3Int(1,0,2),
                    // Tepelik
                    new Vector3Int(4,6,1), new Vector3Int(4,7,1),
                    // Aura
                    new Vector3Int(5,4,1), new Vector3Int(3,5,1)
                },
                new Vector3Int[] { new Vector3Int(6,6,1), new Vector3Int(0,6,2), new Vector3Int(0,6,0), new Vector3Int(0,0,1), new Vector3Int(4,7,1) }),

            // Level 92: Kaktüs (26 küp)
            CreateLevelWithFixed(92, "Cactus", 14, "Desert cactus",
                new Vector3Int[] {
                    // Ana gövde
                    new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(2,0,2), new Vector3Int(3,0,2),
                    new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    new Vector3Int(2,2,1), new Vector3Int(3,2,1),
                    new Vector3Int(2,3,1), new Vector3Int(3,3,1),
                    new Vector3Int(3,4,1), new Vector3Int(3,5,1),
                    // Sol kol
                    new Vector3Int(1,2,1), new Vector3Int(0,2,1), new Vector3Int(0,3,1), new Vector3Int(0,4,1),
                    // Sağ kol (daha yukarıda)
                    new Vector3Int(4,3,1), new Vector3Int(5,3,1), new Vector3Int(5,4,1), new Vector3Int(5,5,1),
                    // Çiçek
                    new Vector3Int(2,5,1), new Vector3Int(3,6,1), new Vector3Int(4,5,1),
                    // Dikenler
                    new Vector3Int(1,1,1), new Vector3Int(4,2,1),
                    // Toprak
                    new Vector3Int(1,0,1)
                },
                new Vector3Int[] { new Vector3Int(3,6,1), new Vector3Int(0,4,1), new Vector3Int(5,5,1), new Vector3Int(1,0,1), new Vector3Int(3,0,2) }),

            // Level 93: Dürbün (28 küp)
            CreateLevelWithFixed(93, "Binoculars", 1, "Looking glass",
                new Vector3Int[] {
                    // Sol tüp
                    new Vector3Int(0,1,0), new Vector3Int(1,1,0), new Vector3Int(2,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(1,2,0), new Vector3Int(2,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(0,1,1), new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(3,1,1),
                    // Sağ tüp
                    new Vector3Int(0,1,3), new Vector3Int(1,1,3), new Vector3Int(2,1,3), new Vector3Int(3,1,3),
                    new Vector3Int(0,2,3), new Vector3Int(1,2,3), new Vector3Int(2,2,3), new Vector3Int(3,2,3),
                    new Vector3Int(0,1,4), new Vector3Int(1,1,4), new Vector3Int(2,1,4), new Vector3Int(3,1,4),
                    // Köprü
                    new Vector3Int(1,1,2), new Vector3Int(2,1,2),
                    // Mercek
                    new Vector3Int(0,0,0), new Vector3Int(0,0,3)
                },
                new Vector3Int[] { new Vector3Int(3,1,0), new Vector3Int(3,1,3), new Vector3Int(3,2,0), new Vector3Int(3,2,3), new Vector3Int(0,0,0) }),

            // Level 94: Deniz atı (28 küp)
            CreateLevelWithFixed(94, "Seahorse", 6, "Coral seahorse",
                new Vector3Int[] {
                    // Baş
                    new Vector3Int(3,6,0), new Vector3Int(3,6,1), new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    // Burun
                    new Vector3Int(4,5,0), new Vector3Int(5,5,0),
                    // Taç
                    new Vector3Int(2,7,0), new Vector3Int(3,7,0),
                    // Boyun S
                    new Vector3Int(2,4,0), new Vector3Int(3,4,0),
                    new Vector3Int(3,3,0), new Vector3Int(4,3,0),
                    // Gövde
                    new Vector3Int(4,2,0), new Vector3Int(4,2,1), new Vector3Int(3,2,1),
                    new Vector3Int(4,1,0), new Vector3Int(4,1,1),
                    // Kuyruk (kıvrık)
                    new Vector3Int(3,0,0), new Vector3Int(2,0,0), new Vector3Int(2,0,1),
                    new Vector3Int(3,0,1), new Vector3Int(3,0,2),
                    // Sırt yüzgeci
                    new Vector3Int(2,4,1), new Vector3Int(2,3,1),
                    new Vector3Int(3,3,1), new Vector3Int(4,3,1),
                    // Göz
                    new Vector3Int(3,6,2), new Vector3Int(2,5,1)
                },
                new Vector3Int[] { new Vector3Int(5,5,0), new Vector3Int(2,7,0), new Vector3Int(3,0,2), new Vector3Int(2,0,0), new Vector3Int(3,6,2) }),

            // Level 95: Zeplin (28 küp)
            CreateLevelWithFixed(95, "Zeppelin", 9, "Airship",
                new Vector3Int[] {
                    // Balon (oval)
                    new Vector3Int(2,2,0), new Vector3Int(3,2,0), new Vector3Int(4,2,0), new Vector3Int(5,2,0), new Vector3Int(6,2,0),
                    new Vector3Int(1,2,1), new Vector3Int(2,2,1), new Vector3Int(3,2,1), new Vector3Int(4,2,1), new Vector3Int(5,2,1), new Vector3Int(6,2,1), new Vector3Int(7,2,1),
                    new Vector3Int(2,3,0), new Vector3Int(3,3,0), new Vector3Int(4,3,0), new Vector3Int(5,3,0), new Vector3Int(6,3,0),
                    // Gondol
                    new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    // Bağlantı
                    new Vector3Int(4,1,0),
                    // Kuyruk yüzgeci
                    new Vector3Int(1,3,0), new Vector3Int(0,3,0),
                    new Vector3Int(1,1,0),
                    // Burun
                    new Vector3Int(8,2,0), new Vector3Int(8,2,1),
                    // Pervane
                    new Vector3Int(3,0,1), new Vector3Int(5,0,1)
                },
                new Vector3Int[] { new Vector3Int(8,2,0), new Vector3Int(8,2,1), new Vector3Int(0,3,0), new Vector3Int(3,0,0), new Vector3Int(5,0,1) }),

            // Level 96: Örümcek ağı (28 küp)
            CreateLevelWithFixed(96, "Spiderweb", 12, "Spider's web",
                new Vector3Int[] {
                    // Merkez
                    new Vector3Int(3,0,3),
                    // İç halka
                    new Vector3Int(2,0,3), new Vector3Int(4,0,3), new Vector3Int(3,0,2), new Vector3Int(3,0,4),
                    // Kollar
                    new Vector3Int(5,0,3), new Vector3Int(6,0,3),
                    new Vector3Int(1,0,3), new Vector3Int(0,0,3),
                    new Vector3Int(3,0,1), new Vector3Int(3,0,0),
                    new Vector3Int(3,0,5), new Vector3Int(3,0,6),
                    // Çapraz kollar
                    new Vector3Int(4,0,4), new Vector3Int(5,0,5), new Vector3Int(6,0,6),
                    new Vector3Int(2,0,2), new Vector3Int(1,0,1), new Vector3Int(0,0,0),
                    new Vector3Int(4,0,2), new Vector3Int(5,0,1),
                    new Vector3Int(2,0,4), new Vector3Int(1,0,5),
                    // 3D
                    new Vector3Int(3,1,3), new Vector3Int(3,2,3),
                    // Örümcek
                    new Vector3Int(2,1,2), new Vector3Int(4,1,4)
                },
                new Vector3Int[] { new Vector3Int(6,0,6), new Vector3Int(0,0,0), new Vector3Int(6,0,3), new Vector3Int(0,0,3), new Vector3Int(3,2,3) }),

            // Level 97: Kum kalesi (30 küp)
            CreateLevelWithFixed(97, "Sandcastle", 3, "Grand sandcastle",
                new Vector3Int[] {
                    // Taban
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0), new Vector3Int(3,0,0), new Vector3Int(4,0,0), new Vector3Int(5,0,0), new Vector3Int(6,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1), new Vector3Int(6,0,1),
                    // Duvarlar
                    new Vector3Int(0,1,0), new Vector3Int(6,1,0), new Vector3Int(0,1,1), new Vector3Int(6,1,1),
                    new Vector3Int(3,1,0),
                    // Kuleler
                    new Vector3Int(0,2,0), new Vector3Int(0,3,0),
                    new Vector3Int(6,2,0), new Vector3Int(6,3,0),
                    new Vector3Int(0,2,1), new Vector3Int(6,2,1),
                    // Merkez kule
                    new Vector3Int(3,2,0), new Vector3Int(3,3,0), new Vector3Int(3,4,0),
                    // Bayrak
                    new Vector3Int(3,5,0), new Vector3Int(3,5,1)
                },
                new Vector3Int[] { new Vector3Int(0,3,0), new Vector3Int(6,3,0), new Vector3Int(3,5,0), new Vector3Int(3,5,1), new Vector3Int(0,0,1), new Vector3Int(6,0,1) }),

            // Level 98: Fener detaylı (30 küp)
            CreateLevelWithFixed(98, "Radiance", 7, "Grand lighthouse",
                new Vector3Int[] {
                    // Kayalık
                    new Vector3Int(0,0,0), new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,0,1), new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(3,0,1),
                    new Vector3Int(1,0,2), new Vector3Int(2,0,2),
                    // Fener taban
                    new Vector3Int(1,1,1), new Vector3Int(2,1,1), new Vector3Int(1,1,2), new Vector3Int(2,1,2),
                    // Fener gövde
                    new Vector3Int(1,2,1), new Vector3Int(2,2,1),
                    new Vector3Int(1,3,1), new Vector3Int(2,3,1),
                    new Vector3Int(2,4,1),
                    // Balkon
                    new Vector3Int(0,5,0), new Vector3Int(1,5,0), new Vector3Int(2,5,0), new Vector3Int(3,5,0),
                    new Vector3Int(0,5,1), new Vector3Int(3,5,1),
                    new Vector3Int(0,5,2), new Vector3Int(1,5,2), new Vector3Int(2,5,2), new Vector3Int(3,5,2),
                    // Tepe
                    new Vector3Int(1,6,1), new Vector3Int(2,7,1)
                },
                new Vector3Int[] { new Vector3Int(0,0,0), new Vector3Int(3,0,1), new Vector3Int(2,7,1), new Vector3Int(0,5,0), new Vector3Int(3,5,2) }),

            // Level 99: Kaos düzeni (30 küp)
            CreateLevelWithFixed(99, "Chaos Order", 15, "Ordered chaos",
                new Vector3Int[] {
                    // Merkez platform
                    new Vector3Int(2,0,2), new Vector3Int(3,0,2), new Vector3Int(4,0,2),
                    new Vector3Int(2,0,3), new Vector3Int(3,0,3), new Vector3Int(4,0,3),
                    new Vector3Int(2,0,4), new Vector3Int(3,0,4), new Vector3Int(4,0,4),
                    // Köşe sütunlar
                    new Vector3Int(0,0,0), new Vector3Int(0,1,0), new Vector3Int(0,2,0),
                    new Vector3Int(6,0,0), new Vector3Int(6,1,0), new Vector3Int(6,2,0),
                    new Vector3Int(0,0,6), new Vector3Int(0,1,6),
                    new Vector3Int(6,0,6), new Vector3Int(6,1,6), new Vector3Int(6,2,6),
                    // Merkez kule
                    new Vector3Int(3,1,3), new Vector3Int(3,2,3), new Vector3Int(3,3,3), new Vector3Int(3,4,3),
                    // Bağlantılar
                    new Vector3Int(1,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(1,0,5), new Vector3Int(5,0,5),
                    // Tavan köşe
                    new Vector3Int(0,3,0), new Vector3Int(6,3,6)
                },
                new Vector3Int[] { new Vector3Int(0,2,0), new Vector3Int(6,2,0), new Vector3Int(0,1,6), new Vector3Int(6,2,6), new Vector3Int(3,4,3), new Vector3Int(0,3,0) }),

            // Level 100: Sonsuzluk (30 küp)
            CreateLevelWithFixed(100, "Eternity", 0, "Infinite challenge",
                new Vector3Int[] {
                    // Sonsuzluk işareti sol döngü
                    new Vector3Int(1,0,0), new Vector3Int(2,0,0),
                    new Vector3Int(0,1,0), new Vector3Int(3,1,0),
                    new Vector3Int(0,2,0), new Vector3Int(3,2,0),
                    new Vector3Int(1,3,0), new Vector3Int(2,3,0),
                    // Sonsuzluk işareti sağ döngü (3,1 ve 3,2 ortaklaştı)
                    new Vector3Int(4,0,0), new Vector3Int(5,0,0),
                    new Vector3Int(6,1,0), new Vector3Int(6,2,0),
                    new Vector3Int(4,3,0), new Vector3Int(5,3,0),
                    // Derinlik katmanı
                    new Vector3Int(1,0,1), new Vector3Int(2,0,1), new Vector3Int(4,0,1), new Vector3Int(5,0,1),
                    new Vector3Int(0,1,1), new Vector3Int(6,1,1),
                    new Vector3Int(0,2,1), new Vector3Int(6,2,1),
                    new Vector3Int(1,3,1), new Vector3Int(2,3,1), new Vector3Int(4,3,1), new Vector3Int(5,3,1),
                    // Merkez bağ (3D)
                    new Vector3Int(3,1,1), new Vector3Int(3,2,1)
                },
                new Vector3Int[] { new Vector3Int(0,1,0), new Vector3Int(6,1,0), new Vector3Int(0,2,1), new Vector3Int(6,2,1), new Vector3Int(1,0,0), new Vector3Int(5,0,0) })
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
    /// İçi boş kutu oluşturur (yardımcı)
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
