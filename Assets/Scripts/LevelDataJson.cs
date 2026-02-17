using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// JSON formatında level verisi - Serializable
/// </summary>
[System.Serializable]
public class LevelDataJson
{
    public int levelNumber;
    public string levelName;
    public int colorPaletteIndex;
    public string description;
    
    // Pozisyonlar int array olarak (Vector3Int serializable değil)
    public int[] allPositionsX;
    public int[] allPositionsY;
    public int[] allPositionsZ;
    
    public int[] fixedPositionsX;
    public int[] fixedPositionsY;
    public int[] fixedPositionsZ;
    
    /// <summary>
    /// Vector3Int listesinden JSON formatına çevir
    /// </summary>
    public void SetAllPositions(List<Vector3Int> positions)
    {
        allPositionsX = new int[positions.Count];
        allPositionsY = new int[positions.Count];
        allPositionsZ = new int[positions.Count];
        
        for (int i = 0; i < positions.Count; i++)
        {
            allPositionsX[i] = positions[i].x;
            allPositionsY[i] = positions[i].y;
            allPositionsZ[i] = positions[i].z;
        }
    }
    
    /// <summary>
    /// Sabit küp pozisyonlarını ayarla
    /// </summary>
    public void SetFixedPositions(List<Vector3Int> positions)
    {
        fixedPositionsX = new int[positions.Count];
        fixedPositionsY = new int[positions.Count];
        fixedPositionsZ = new int[positions.Count];
        
        for (int i = 0; i < positions.Count; i++)
        {
            fixedPositionsX[i] = positions[i].x;
            fixedPositionsY[i] = positions[i].y;
            fixedPositionsZ[i] = positions[i].z;
        }
    }
    
    /// <summary>
    /// Tüm küp pozisyonlarını al
    /// </summary>
    public List<Vector3Int> GetAllPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        if (allPositionsX != null)
        {
            for (int i = 0; i < allPositionsX.Length; i++)
            {
                positions.Add(new Vector3Int(allPositionsX[i], allPositionsY[i], allPositionsZ[i]));
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// Sabit küp pozisyonlarını al
    /// </summary>
    public List<Vector3Int> GetFixedPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        if (fixedPositionsX != null)
        {
            for (int i = 0; i < fixedPositionsX.Length; i++)
            {
                positions.Add(new Vector3Int(fixedPositionsX[i], fixedPositionsY[i], fixedPositionsZ[i]));
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// LevelData'ya çevir
    /// </summary>
    public LevelData ToLevelData()
    {
        LevelData data = new LevelData();
        data.levelNumber = levelNumber;
        data.levelName = levelName;
        data.colorPaletteIndex = colorPaletteIndex;
        data.description = description;
        data.customPositions = GetAllPositions();
        data.fixedPositions = GetFixedPositions();
        data.fixedCubeRatio = data.fixedPositions.Count / (float)Mathf.Max(1, data.customPositions.Count);
        return data;
    }
    
    /// <summary>
    /// LevelData'dan oluştur
    /// </summary>
    public static LevelDataJson FromLevelData(LevelData data)
    {
        LevelDataJson json = new LevelDataJson();
        json.levelNumber = data.levelNumber;
        json.levelName = data.levelName;
        json.colorPaletteIndex = data.colorPaletteIndex;
        json.description = data.description;
        json.SetAllPositions(data.customPositions ?? new List<Vector3Int>());
        json.SetFixedPositions(data.fixedPositions ?? new List<Vector3Int>());
        return json;
    }
}

/// <summary>
/// Level JSON dosyalarını yöneten yardımcı sınıf
/// </summary>
public static class LevelJsonManager
{
    private const string LEVELS_FOLDER = "Levels";
    private const string LEVELS_PATH = "Assets/Resources/Levels";
    
    /// <summary>
    /// Level'ı JSON dosyasına kaydet
    /// </summary>
    public static void SaveLevel(LevelDataJson levelData)
    {
        // Klasörü oluştur
        if (!Directory.Exists(LEVELS_PATH))
        {
            Directory.CreateDirectory(LEVELS_PATH);
        }
        
        string fileName = $"level_{levelData.levelNumber:D2}.json";
        string filePath = Path.Combine(LEVELS_PATH, fileName);
        
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(filePath, json);
        
        Debug.Log($"[LevelJsonManager] Level {levelData.levelNumber} saved: {filePath}");
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    
    /// <summary>
    /// JSON dosyasından level yükle
    /// </summary>
    public static LevelDataJson LoadLevel(int levelNumber)
    {
        string fileName = $"level_{levelNumber:D2}";
        TextAsset textAsset = Resources.Load<TextAsset>($"{LEVELS_FOLDER}/{fileName}");
        
        if (textAsset != null)
        {
            LevelDataJson data = JsonUtility.FromJson<LevelDataJson>(textAsset.text);
            Debug.Log($"[LevelJsonManager] Level {levelNumber} loaded from JSON: {data.levelName}, {data.GetAllPositions().Count} cubes");
            return data;
        }
        
        return null;
    }
    
    /// <summary>
    /// Tüm JSON level'larını yükle
    /// </summary>
    public static List<LevelDataJson> LoadAllLevels()
    {
        List<LevelDataJson> levels = new List<LevelDataJson>();
        
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>(LEVELS_FOLDER);
        
        foreach (var asset in textAssets)
        {
            if (asset.name.StartsWith("level_"))
            {
                try
                {
                    LevelDataJson data = JsonUtility.FromJson<LevelDataJson>(asset.text);
                    if (data != null)
                    {
                        levels.Add(data);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[LevelJsonManager] JSON parse error: {asset.name} - {e.Message}");
                }
            }
        }
        
        // Numaraya göre sırala
        levels.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));
        
        Debug.Log($"[LevelJsonManager] {levels.Count} custom levels loaded");
        return levels;
    }
    
    /// <summary>
    /// JSON level dosyası var mı kontrol et
    /// </summary>
    public static bool LevelExists(int levelNumber)
    {
        string fileName = $"level_{levelNumber:D2}";
        TextAsset textAsset = Resources.Load<TextAsset>($"{LEVELS_FOLDER}/{fileName}");
        return textAsset != null;
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// Editor'da dosya yolundan level yükle
    /// </summary>
    public static LevelDataJson LoadLevelFromFile(int levelNumber)
    {
        string fileName = $"level_{levelNumber:D2}.json";
        string filePath = Path.Combine(LEVELS_PATH, fileName);
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<LevelDataJson>(json);
        }
        
        return null;
    }
    
    /// <summary>
    /// Editor'da tüm JSON dosyalarını listele
    /// </summary>
    public static List<string> GetAllLevelFiles()
    {
        List<string> files = new List<string>();
        
        if (Directory.Exists(LEVELS_PATH))
        {
            string[] jsonFiles = Directory.GetFiles(LEVELS_PATH, "level_*.json");
            foreach (var file in jsonFiles)
            {
                files.Add(Path.GetFileNameWithoutExtension(file));
            }
        }
        
        files.Sort();
        return files;
    }
#endif
}
