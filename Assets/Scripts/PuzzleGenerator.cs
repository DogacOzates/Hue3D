using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 3D puzzle şekillerini ve renk gradientlerini oluşturur
/// </summary>
public class PuzzleGenerator : MonoBehaviour
{
    public static PuzzleGenerator Instance { get; private set; }
    
    [Header("Puzzle Settings")]
    public int puzzleWidth = 5;
    public int puzzleHeight = 5;
    public int puzzleDepth = 3;
    
    [Header("Color Settings")]
    public Color startColor = new Color(0.4f, 0.8f, 1f);   // Açık mavi
    public Color endColor = new Color(0.2f, 0.3f, 0.8f);   // Koyu mavi
    public Color accentColor = new Color(1f, 0.6f, 0.8f);  // Pembe accent
    public bool useColorPalette = true;
    public int currentPaletteIndex = 0;
    
    [Header("Prefab")]
    public GameObject cubePrefab;
    
    [Header("Generated Data")]
    public List<Cube> allCubes = new List<Cube>();
    public List<Vector3Int> puzzleShape = new List<Vector3Int>();
    
    // Duplicate oluşturmayı engelle
    private int lastGeneratedLevel = -1;
    private static int generationCount = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Yeni bir puzzle oluşturur (eski metod - geriye uyumluluk için)
    /// </summary>
    public void GeneratePuzzle(int difficulty = 1)
    {
        // Level verisi kullan
        LevelData level = LevelManager.GetLevel(difficulty);
        GeneratePuzzleFromLevel(level);
    }
    
    /// <summary>
    /// Level verisinden puzzle oluşturur
    /// </summary>
    public void GeneratePuzzleFromLevel(LevelData level, bool shuffle = true)
    {
        generationCount++;
        Debug.Log($"[PuzzleGenerator] GeneratePuzzleFromLevel #{generationCount}: Level {level.levelNumber} (shuffle={shuffle})");
        
        // ÖNCELİKLE sahnedeki TÜM küpleri temizle
        ClearAllCubesInScene();
        
        ClearPuzzle();
        
        // Level'a göre renk paletini ayarla - tüm 16 paleti eşit dağıt
        currentPaletteIndex = level.levelNumber % ColorPalettes.AllPalettes.Length;
        
        // Level şeklini al
        puzzleShape = LevelManager.GenerateShapePositions(level);
        
        // Küpleri oluştur ve doğru renkleri ata
        CreateCubesWithColorsForLevel(level);
        
        // Renkleri karıştır (istenirse)
        if (shuffle)
        {
            ShuffleColors();
        }
        
        lastGeneratedLevel = level.levelNumber;
        Debug.Log($"[PuzzleGenerator] Puzzle #{generationCount} created: {allCubes.Count} cubes");
    }
    
    /// <summary>
    /// Sahnedeki TÜM Cube objelerini temizler
    /// </summary>
    private void ClearAllCubesInScene()
    {
        // Sahnedeki tüm Cube component'larını bul ve sil
        Cube[] sceneCubes = FindObjectsByType<Cube>(FindObjectsSortMode.None);
        
        if (sceneCubes.Length > 0)
        {
            Debug.Log($"[PuzzleGenerator] Found {sceneCubes.Length} old cubes in scene, deleting...");
            foreach (var cube in sceneCubes)
            {
                if (cube != null)
                {
                    DestroyImmediate(cube.gameObject);
                }
            }
        }
        
        // Preview objelerini de temizle
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.scene.isLoaded && 
                (obj.name.StartsWith("_LevelDesigner_Preview") || 
                 obj.name.StartsWith("Preview_") ||
                 obj.name.StartsWith("PreviewFixed_")))
            {
                DestroyImmediate(obj);
            }
        }
    }
    
    /// <summary>
    /// Level için küpler oluşturur
    /// </summary>
    private void CreateCubesWithColorsForLevel(LevelData level)
    {
        if (puzzleShape.Count == 0) return;
        
        // Bounding box hesapla
        Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        foreach (var pos in puzzleShape)
        {
            minBounds = Vector3.Min(minBounds, pos);
            maxBounds = Vector3.Max(maxBounds, pos);
        }
        
        Vector3 boundsSize = maxBounds - minBounds;
        if (boundsSize.x == 0) boundsSize.x = 1;
        if (boundsSize.y == 0) boundsSize.y = 1;
        if (boundsSize.z == 0) boundsSize.z = 1;
        
        // Sabit küpleri belirle
        HashSet<Vector3Int> fixedPositionSet = new HashSet<Vector3Int>();
        
        // Eğer level'da fixedPositions tanımlıysa onu kullan (boş liste de geçerli - hiç sabit küp yok demek)
        if (level.fixedPositions != null)
        {
            // fixedPositions listesi var - ister dolu ister boş olsun, bunu kullan
            foreach (var pos in level.fixedPositions)
            {
                fixedPositionSet.Add(pos);
            }
            
            Debug.Log($"[PuzzleGenerator] Level {level.levelNumber} '{level.levelName}': {puzzleShape.Count} total cubes, {level.fixedPositions.Count} fixed cubes (JSON/Custom)");
        }
        else
        {
            // fixedPositions null ise (eski format level'lar) ratio ile belirle
            int fixedCount = Mathf.Max(2, Mathf.RoundToInt(puzzleShape.Count * level.fixedCubeRatio));
            List<int> fixedIndices = GetFixedCubeIndices(puzzleShape, fixedCount, minBounds, maxBounds);
            foreach (int idx in fixedIndices)
            {
                fixedPositionSet.Add(puzzleShape[idx]);
            }
            Debug.Log($"[PuzzleGenerator] Level {level.levelNumber} '{level.levelName}': {puzzleShape.Count} total cubes, {fixedCount} fixed cubes (Hardcoded ratio: {level.fixedCubeRatio:F2})");
        }
        
        int totalCubes = puzzleShape.Count;
        
        // Her pozisyon için küp oluştur
        for (int i = 0; i < puzzleShape.Count; i++)
        {
            Vector3Int gridPos = puzzleShape[i];
            
            GameObject cubeObj;
            
            if (cubePrefab != null)
            {
                cubeObj = Instantiate(cubePrefab);
            }
            else
            {
                cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            
            Cube cube = cubeObj.GetComponent<Cube>();
            if (cube == null)
            {
                cube = cubeObj.AddComponent<Cube>();
            }
            
            // Pozisyon ayarla
            cube.gridPosition = gridPos;
            float spacing = GameManager.Instance != null ? GameManager.Instance.cubeSpacing : 1.01f;
            cubeObj.transform.position = new Vector3(
                gridPos.x * spacing,
                gridPos.y * spacing,
                gridPos.z * spacing
            );
            cubeObj.transform.localScale = Vector3.one * 0.98f;  // Küpler arası minimal boşluk
            cubeObj.transform.parent = this.transform;
            
            // I Love Hue tarzı: Grid pozisyonuna göre bilinear gradient renk hesapla
            float u = boundsSize.x > 0 ? (gridPos.x - minBounds.x) / boundsSize.x : 0.5f;
            float v = boundsSize.y > 0 ? (gridPos.y - minBounds.y) / boundsSize.y : 0.5f;
            float w = boundsSize.z > 0 ? (gridPos.z - minBounds.z) / boundsSize.z : 0.5f;
            Color targetColor = CalculateBilinearColor(u, v, w);
            cube.SetTargetColor(targetColor);
            cube.SetColor(targetColor);
            
            // Sabit küp kontrolü
            cube.isFixed = fixedPositionSet.Contains(gridPos);
            
            allCubes.Add(cube);
        }
    }
    
    /// <summary>
    /// Sabit olacak küplerin indekslerini belirler (köşeler ve kenarlar)
    /// </summary>
    private List<int> GetFixedCubeIndices(List<Vector3Int> positions, int count, Vector3 minBounds, Vector3 maxBounds)
    {
        List<int> indices = new List<int>();
        List<(int index, float score)> cornerScores = new List<(int, float)>();
        
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3Int pos = positions[i];
            
            // Köşeye yakınlık skoru hesapla
            float score = 0;
            if (pos.x == minBounds.x || pos.x == maxBounds.x) score += 1;
            if (pos.y == minBounds.y || pos.y == maxBounds.y) score += 1;
            if (pos.z == minBounds.z || pos.z == maxBounds.z) score += 1;
            
            if (score > 0)
            {
                cornerScores.Add((i, score));
            }
        }
        
        // En yüksek skorlu olanları seç
        cornerScores.Sort((a, b) => b.score.CompareTo(a.score));
        
        for (int i = 0; i < Mathf.Min(count, cornerScores.Count); i++)
        {
            indices.Add(cornerScores[i].index);
        }
        
        return indices;
    }

    /// <summary>
    /// Mevcut puzzle'ı temizler
    /// </summary>
    public void ClearPuzzle()
    {
        // Listedeki küpleri temizle
        foreach (var cube in allCubes)
        {
            if (cube != null)
            {
                Destroy(cube.gameObject);
            }
        }
        allCubes.Clear();
        puzzleShape.Clear();
        
        // Ekstra güvenlik: Bu transform'un altındaki tüm çocukları da temizle
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    
    /// <summary>
    /// Resimdeki gibi organik 3D şekil oluşturur
    /// </summary>
    private void GeneratePuzzleShape(int difficulty)
    {
        puzzleShape.Clear();
        
        // Zorluk seviyesine göre boyut ayarla
        int baseSize = 3 + difficulty;
        
        // Merkez noktadan başlayarak organik şekil oluştur
        HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();
        
        // Başlangıç noktası
        Vector3Int start = new Vector3Int(baseSize / 2, baseSize / 2, 0);
        frontier.Enqueue(start);
        usedPositions.Add(start);
        
        int targetCubeCount = 15 + difficulty * 8; // Zorluk arttıkça daha fazla küp
        
        // Büyüme yönleri (izometrik görünüm için ağırlıklı)
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // Sağ
            new Vector3Int(-1, 0, 0),  // Sol
            new Vector3Int(0, 1, 0),   // Yukarı
            new Vector3Int(0, -1, 0),  // Aşağı
            new Vector3Int(0, 0, 1),   // İleri
            new Vector3Int(0, 0, -1),  // Geri
            new Vector3Int(1, 1, 0),   // Çapraz sağ-yukarı
            new Vector3Int(-1, -1, 0), // Çapraz sol-aşağı
            new Vector3Int(1, 0, 1),   // Çapraz sağ-ileri
            new Vector3Int(0, 1, 1),   // Çapraz yukarı-ileri
        };
        
        while (usedPositions.Count < targetCubeCount && frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            
            // Her yönde genişleme şansı
            foreach (var dir in directions)
            {
                Vector3Int newPos = current + dir;
                
                // Sınırları kontrol et
                if (newPos.x < 0 || newPos.x >= baseSize * 2 ||
                    newPos.y < 0 || newPos.y >= baseSize * 2 ||
                    newPos.z < 0 || newPos.z >= baseSize)
                    continue;
                
                if (usedPositions.Contains(newPos))
                    continue;
                
                // Rastgele büyüme (organik şekil için)
                float growthChance = 0.4f;
                
                // Merkeze yakın pozisyonlar daha yüksek şansla eklenir
                float distanceFromCenter = Vector3.Distance(newPos, start);
                growthChance -= distanceFromCenter * 0.03f;
                
                // Aşağı doğru büyüme daha olası (yerçekimi efekti)
                if (dir.y < 0) growthChance += 0.15f;
                
                if (Random.value < growthChance && usedPositions.Count < targetCubeCount)
                {
                    usedPositions.Add(newPos);
                    frontier.Enqueue(newPos);
                }
            }
            
            // Sonsuz döngüyü önlemek için tekrar ekle
            if (frontier.Count < 3 && usedPositions.Count < targetCubeCount)
            {
                var positions = new List<Vector3Int>(usedPositions);
                frontier.Enqueue(positions[Random.Range(0, positions.Count)]);
            }
        }
        
        puzzleShape = new List<Vector3Int>(usedPositions);
        
        // Şekli merkeze taşı
        CenterShape();
    }
    
    /// <summary>
    /// Oluşturulan şekli merkeze taşır
    /// </summary>
    private void CenterShape()
    {
        if (puzzleShape.Count == 0) return;
        
        Vector3 center = Vector3.zero;
        foreach (var pos in puzzleShape)
        {
            center += new Vector3(pos.x, pos.y, pos.z);
        }
        center /= puzzleShape.Count;
        
        Vector3Int offset = new Vector3Int(
            Mathf.RoundToInt(center.x),
            Mathf.RoundToInt(center.y),
            Mathf.RoundToInt(center.z)
        );
        
        for (int i = 0; i < puzzleShape.Count; i++)
        {
            puzzleShape[i] = puzzleShape[i] - offset;
        }
    }
    
    /// <summary>
    /// Küpleri oluşturur ve gradient renkler atar
    /// </summary>
    private void CreateCubesWithColors()
    {
        if (puzzleShape.Count == 0) return;
        
        // Bounding box hesapla
        Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        
        foreach (var pos in puzzleShape)
        {
            minBounds = Vector3.Min(minBounds, pos);
            maxBounds = Vector3.Max(maxBounds, pos);
        }
        
        Vector3 boundsSize = maxBounds - minBounds;
        if (boundsSize.x == 0) boundsSize.x = 1;
        if (boundsSize.y == 0) boundsSize.y = 1;
        if (boundsSize.z == 0) boundsSize.z = 1;
        
        int totalCubes = puzzleShape.Count;
        
        // Her pozisyon için küp oluştur
        for (int i = 0; i < puzzleShape.Count; i++)
        {
            Vector3Int gridPos = puzzleShape[i];
            
            GameObject cubeObj;
            
            if (cubePrefab != null)
            {
                cubeObj = Instantiate(cubePrefab);
            }
            else
            {
                cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            
            Cube cube = cubeObj.GetComponent<Cube>();
            if (cube == null)
            {
                cube = cubeObj.AddComponent<Cube>();
            }
            
            // Pozisyon ayarla
            cube.gridPosition = gridPos;
            float spacing = GameManager.Instance != null ? GameManager.Instance.cubeSpacing : 1.01f;
            cubeObj.transform.position = new Vector3(
                gridPos.x * spacing,
                gridPos.y * spacing,
                gridPos.z * spacing
            );
            cubeObj.transform.localScale = Vector3.one * 0.98f;  // Küpler arası minimal boşluk
            cubeObj.transform.parent = this.transform;
            
            // I Love Hue tarzı: Grid pozisyonuna göre bilinear gradient renk hesapla
            float u2 = boundsSize.x > 0 ? (gridPos.x - minBounds.x) / boundsSize.x : 0.5f;
            float v2 = boundsSize.y > 0 ? (gridPos.y - minBounds.y) / boundsSize.y : 0.5f;
            float w2 = boundsSize.z > 0 ? (gridPos.z - minBounds.z) / boundsSize.z : 0.5f;
            Color targetColor = CalculateBilinearColor(u2, v2, w2);
            cube.SetTargetColor(targetColor);
            cube.SetColor(targetColor); // Başlangıçta doğru renk
            
            // Köşe küpler sabit (ipucu olarak)
            cube.isFixed = IsCornerPosition(gridPos, minBounds, maxBounds);
            
            allCubes.Add(cube);
        }
    }
    
    /// <summary>
    /// I Love Hue tarzı bilinear interpolation ile renk hesaplar
    /// u = normalize X (0-1), v = normalize Y (0-1), w = normalize Z (0-1)
    /// </summary>
    private Color CalculateBilinearColor(float u, float v, float w)
    {
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);
        w = Mathf.Clamp01(w);
        
        if (useColorPalette)
        {
            Color[] palette = ColorPalettes.AllPalettes[currentPaletteIndex % ColorPalettes.AllPalettes.Length];
            return ColorPalettes.GetBilinearColor3D(palette, u, v, w);
        }
        else
        {
            // Fallback: 2 renk arası basit lerp
            float t = (u * 0.5f + v * 0.5f);
            return Color.Lerp(startColor, endColor, t);
        }
    }
    
    /// <summary>
    /// T değerine göre gradient renk hesaplar (geriye uyumluluk)
    /// </summary>
    private Color CalculateGradientColorByT(float t)
    {
        t = Mathf.Clamp01(t);
        
        if (useColorPalette)
        {
            Color[] palette = ColorPalettes.AllPalettes[currentPaletteIndex % ColorPalettes.AllPalettes.Length];
            // 1D fallback: diagonal bilinear
            return ColorPalettes.GetBilinearColor(palette, t, t);
        }
        else
        {
            return Color.Lerp(startColor, endColor, t);
        }
    }
    
    /// <summary>
    /// Sonraki renk paletine geç
    /// </summary>
    public void NextPalette()
    {
        currentPaletteIndex = (currentPaletteIndex + 1) % ColorPalettes.AllPalettes.Length;
    }
    
    /// <summary>
    /// Köşe pozisyonu mu kontrol eder
    /// </summary>
    private bool IsCornerPosition(Vector3Int pos, Vector3 minBounds, Vector3 maxBounds)
    {
        // Sadece birkaç köşe küpünü sabit yap
        int cornerCount = 0;
        if (pos.x == minBounds.x || pos.x == maxBounds.x) cornerCount++;
        if (pos.y == minBounds.y || pos.y == maxBounds.y) cornerCount++;
        if (pos.z == minBounds.z || pos.z == maxBounds.z) cornerCount++;
        
        // En az 2 sınırda olan ve rastgele seçilen küpler sabit
        return cornerCount >= 2 && Random.value < 0.3f;
    }
    
    /// <summary>
    /// Küplerin renklerini karıştırır (sabit olanlar hariç)
    /// </summary>
    public void ShuffleColors()
    {
        // Sabit olmayan küpleri bul
        List<Cube> movableCubes = allCubes.FindAll(c => !c.isFixed);
        
        if (movableCubes.Count < 2) return;
        
        // Hedef renkleri kaydet (çözülmüş durum)
        List<Color> targetColors = new List<Color>();
        foreach (var cube in movableCubes)
        {
            targetColors.Add(cube.targetColor);
        }
        
        // Renkleri topla
        List<Color> colors = new List<Color>();
        foreach (var cube in movableCubes)
        {
            colors.Add(cube.currentColor);
        }
        
        int maxAttempts = 20;
        int attempt = 0;
        bool isSolved = true;
        
        do
        {
            // Fisher-Yates shuffle
            for (int i = colors.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                Color temp = colors[i];
                colors[i] = colors[j];
                colors[j] = temp;
            }
            
            // Çözülmüş durumla aynı mı kontrol et
            isSolved = true;
            int correctCount = 0;
            for (int i = 0; i < colors.Count; i++)
            {
                float dist = Mathf.Sqrt(
                    Mathf.Pow(colors[i].r - targetColors[i].r, 2) +
                    Mathf.Pow(colors[i].g - targetColors[i].g, 2) +
                    Mathf.Pow(colors[i].b - targetColors[i].b, 2)
                );
                if (dist < 0.01f)
                {
                    correctCount++;
                }
            }
            
            // En az %30 küp yanlış pozisyonda olmalı
            float correctRatio = (float)correctCount / colors.Count;
            isSolved = correctRatio > 0.7f;
            
            attempt++;
        }
        while (isSolved && attempt < maxAttempts);
        
        if (isSolved)
        {
            // Hâlâ çözülmüş durumda - basit swap yap
            Color temp = colors[0];
            colors[0] = colors[colors.Count - 1];
            colors[colors.Count - 1] = temp;
            Debug.LogWarning("[PuzzleGenerator] Shuffle could not avoid solved state, forced swap.");
        }
        
        // Karıştırılmış renkleri ata
        for (int i = 0; i < movableCubes.Count; i++)
        {
            movableCubes[i].SetColor(colors[i]);
        }
        
        Debug.Log($"[PuzzleGenerator] ShuffleColors: {movableCubes.Count} cubes shuffled in {attempt} attempt(s).");
    }
    
    /// <summary>
    /// Tüm küplerin doğru pozisyonda olup olmadığını kontrol eder
    /// </summary>
    public bool IsPuzzleSolved()
    {
        foreach (var cube in allCubes)
        {
            if (!cube.IsInCorrectPosition())
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Puzzle'ın tamamlanma yüzdesini hesaplar
    /// </summary>
    public float GetCompletionPercentage()
    {
        if (allCubes.Count == 0) return 0f;
        
        int correctCount = 0;
        foreach (var cube in allCubes)
        {
            if (cube.IsInCorrectPosition())
            {
                correctCount++;
            }
        }
        
        return (float)correctCount / allCubes.Count * 100f;
    }
}
