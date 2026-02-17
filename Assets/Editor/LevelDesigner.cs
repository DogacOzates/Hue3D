using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Hue3D Level Designer - Temiz ve düzgün çalışan versiyon
/// Window > Hue3D > Level Designer ile açılır
/// </summary>
public class LevelDesigner : EditorWindow
{
    // ===== LEVEL VERİLERİ =====
    private List<Vector3Int> cubePositions = new List<Vector3Int>();
    private List<Vector3Int> fixedCubePositions = new List<Vector3Int>();  // Sabit küpler
    private string levelName = "New Level";
    private int levelNumber = 1;
    private int paletteIndex = 0;
    
    // ===== KÜP YERLEŞTİRME MODU =====
    private enum CubePlacementMode { Normal, Fixed }
    private CubePlacementMode placementMode = CubePlacementMode.Normal;
    
    // ===== ARKA PLAN AYARLARI =====
    private Color bgTopColor = new Color(0.78f, 0.58f, 0.42f);
    private Color bgBottomColor = new Color(0.55f, 0.38f, 0.28f);
    private int bgPresetIndex = 0;
    
    // ===== CUSTOM RENK PALETİ =====
    private List<Color> customPalette = new List<Color>();
    private bool showPaletteEditor = true;
    
    // ===== HAZIR RENK PALETLERİ =====
    // I Love Hue tarzı: Her palette 4 köşe rengi [topLeft, topRight, bottomLeft, bottomRight]
    private static readonly Color[][] cubePalettes = new Color[][]
    {
        // 0: Sunset Warmth
        new Color[] { new Color(1.00f, 0.72f, 0.53f), new Color(0.96f, 0.45f, 0.55f), new Color(0.98f, 0.84f, 0.42f), new Color(0.95f, 0.58f, 0.38f) },
        // 1: Ocean Depths
        new Color[] { new Color(0.55f, 0.82f, 0.90f), new Color(0.20f, 0.72f, 0.70f), new Color(0.25f, 0.45f, 0.75f), new Color(0.15f, 0.60f, 0.72f) },
        // 2: Spring Meadow
        new Color[] { new Color(0.70f, 0.88f, 0.55f), new Color(0.98f, 0.90f, 0.42f), new Color(0.28f, 0.68f, 0.58f), new Color(0.52f, 0.78f, 0.48f) },
        // 3: Berry Garden
        new Color[] { new Color(0.90f, 0.62f, 0.75f), new Color(0.72f, 0.50f, 0.78f), new Color(0.95f, 0.42f, 0.55f), new Color(0.55f, 0.30f, 0.68f) },
        // 4: Desert Dusk
        new Color[] { new Color(0.95f, 0.85f, 0.58f), new Color(0.88f, 0.60f, 0.42f), new Color(0.78f, 0.65f, 0.35f), new Color(0.92f, 0.55f, 0.45f) },
        // 5: Northern Lights
        new Color[] { new Color(0.72f, 0.88f, 0.82f), new Color(0.60f, 0.72f, 0.85f), new Color(0.55f, 0.62f, 0.80f), new Color(0.88f, 0.72f, 0.82f) },
    };
    
    // Arka plan presetleri
    private static readonly Color[][] bgPresets = new Color[][]
    {
        new Color[] { new Color(0.72f, 0.48f, 0.35f), new Color(0.45f, 0.28f, 0.22f) }, // Sunset
        new Color[] { new Color(0.28f, 0.45f, 0.60f), new Color(0.12f, 0.22f, 0.38f) }, // Ocean
        new Color[] { new Color(0.48f, 0.58f, 0.38f), new Color(0.28f, 0.38f, 0.25f) }, // Spring
        new Color[] { new Color(0.58f, 0.38f, 0.52f), new Color(0.35f, 0.22f, 0.32f) }, // Berry
        new Color[] { new Color(0.72f, 0.55f, 0.35f), new Color(0.48f, 0.35f, 0.22f) }, // Desert
        new Color[] { new Color(0.50f, 0.55f, 0.62f), new Color(0.28f, 0.32f, 0.40f) }, // Northern
    };
    
    private static readonly string[] bgPresetNames = new string[]
    {
        "Sunset (Warm)", "Ocean (Blue)", "Spring (Green)", 
        "Berry (Purple)", "Desert (Gold)", "Northern (Gray)"
    };
    
    // ===== GRID AYARLARI =====
    private int gridSize = 10;
    private int currentLayer = 0; // Z ekseni katmanı
    private float cellSize = 28f;
    
    // ===== GÖRÜNÜM =====
    private Vector2 mainScroll;
    private Vector2 levelListScroll;
    
    // ===== PREVIEW =====
    private GameObject previewParent;
    private bool showPreview = true;
    
    // ===== SCENE VIEW EDİT =====
    private bool sceneViewEditMode = false;
    private int sceneViewLayer = 0;  // Hangi Y katmanında edit yapılıyor
    
    // ===== MEVCUT LEVEL LİSTESİ =====
    private List<LevelInfo> existingLevels = new List<LevelInfo>();
    private int selectedExistingLevel = -1;
    
    // ===== PALET İSİMLERİ =====
    private static readonly string[] paletteNames = new string[]
    {
        "Sunset Warmth", "Ocean Depths", "Spring Meadow", 
        "Berry Garden", "Desert Dusk", "Northern Lights",
        "Autumn Forest", "Tropical Sunrise", "Lavender Fields",
        "Midnight City", "Coral Reef", "Golden Hour",
        "Arctic Ice", "Volcanic Ash", "Cherry Blossom", "Emerald Cave"
    };
    
    private struct LevelInfo
    {
        public int number;
        public string name;
        public int cubeCount;
    }
    
    [MenuItem("Window/Hue3D/Level Designer")]
    public static void ShowWindow()
    {
        LevelDesigner window = GetWindow<LevelDesigner>("Level Designer");
        window.minSize = new Vector2(700, 600);
        window.Show();
    }
    
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        LoadExistingLevels();
    }
    
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        ClearPreview();
    }
    
    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        // Play moduna girerken veya çıkarken preview'ı temizle
        if (state == PlayModeStateChange.ExitingEditMode || 
            state == PlayModeStateChange.EnteredEditMode)
        {
            ClearPreview();
            CleanupAllPreviewObjects(); // Ekstra güvenlik
            sceneViewEditMode = false;
        }
    }
    
    /// <summary>
    /// Sahnedeki tüm preview objelerini agresif şekilde temizler
    /// </summary>
    private void CleanupAllPreviewObjects()
    {
        // İsme göre tüm preview objelerini bul ve sil
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj != null && (obj.name.StartsWith("_LevelDesigner_Preview") || 
                               obj.name.StartsWith("Preview_") ||
                               obj.name.StartsWith("PreviewFixed_")))
            {
                Object.DestroyImmediate(obj);
            }
        }
    }
    
    private void OnGUI()
    {
        // Play modunda GUI çizme
        if (EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Level Designer is disabled in Play mode.", MessageType.Info);
            return;
        }
        
        EditorGUILayout.BeginHorizontal();
        
        // Sol panel - Level listesi
        DrawLevelListPanel();
        
        // Orta panel - Grid tasarım
        DrawDesignPanel();
        
        // Sağ panel - Ayarlar
        DrawSettingsPanel();
        
        EditorGUILayout.EndHorizontal();
        
        // Preview güncelle
        if (showPreview && cubePositions.Count > 0)
        {
            UpdatePreview();
        }
    }
    
    // ========================================
    // SOL PANEL - LEVEL LİSTESİ
    // ========================================
    private void DrawLevelListPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(180));
        
        EditorGUILayout.LabelField("Mevcut Leveller", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🔄 Listeyi Yenile"))
        {
            LoadExistingLevels();
        }
        
        EditorGUILayout.Space(5);
        
        levelListScroll = EditorGUILayout.BeginScrollView(levelListScroll, GUILayout.Height(400));
        
        for (int i = 0; i < existingLevels.Count; i++)
        {
            var level = existingLevels[i];
            bool isSelected = (selectedExistingLevel == i);
            
            GUI.backgroundColor = isSelected ? Color.cyan : Color.white;
            
            if (GUILayout.Button($"#{level.number}: {level.name} ({level.cubeCount})", 
                isSelected ? EditorStyles.helpBox : EditorStyles.miniButton))
            {
                selectedExistingLevel = i;
                LoadLevelFromManager(level.number);
            }
            
            GUI.backgroundColor = Color.white;
        }
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.Space(10);
        
        // Yeni level oluştur
        EditorGUILayout.LabelField("New Level", EditorStyles.boldLabel);
        
        if (GUILayout.Button("➕ New Empty Level", GUILayout.Height(25)))
        {
            ClearDesign();
            levelNumber = existingLevels.Count > 0 ? existingLevels[existingLevels.Count - 1].number + 1 : 1;
            levelName = $"Level {levelNumber}";
            selectedExistingLevel = -1;
        }
        
        EditorGUILayout.EndVertical();
    }
    
    // ========================================
    // ORTA PANEL - TASARIM GRİD
    // ========================================
    private void DrawDesignPanel()
    {
        EditorGUILayout.BeginVertical();
        
        // Üst toolbar - Katman ve küp tipi seçimi
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        EditorGUILayout.LabelField($"Katman (Z): {currentLayer}", GUILayout.Width(100));
        
        if (GUILayout.Button("◀", EditorStyles.toolbarButton, GUILayout.Width(25)))
            currentLayer = Mathf.Max(0, currentLayer - 1);
        
        if (GUILayout.Button("▶", EditorStyles.toolbarButton, GUILayout.Width(25)))
            currentLayer = Mathf.Min(gridSize - 1, currentLayer + 1);
        
        GUILayout.Space(20);
        
        // Küp tipi seçimi
        EditorGUILayout.LabelField("Place:", GUILayout.Width(55));
        
        GUI.backgroundColor = placementMode == CubePlacementMode.Normal ? new Color(0.5f, 0.8f, 0.5f) : Color.white;
        if (GUILayout.Button("Normal", EditorStyles.toolbarButton, GUILayout.Width(55)))
            placementMode = CubePlacementMode.Normal;
        
        GUI.backgroundColor = placementMode == CubePlacementMode.Fixed ? new Color(0.8f, 0.6f, 0.3f) : Color.white;
        if (GUILayout.Button("Fixed", EditorStyles.toolbarButton, GUILayout.Width(45)))
            placementMode = CubePlacementMode.Fixed;
        
        GUI.backgroundColor = Color.white;
        
        GUILayout.FlexibleSpace();
        
        EditorGUILayout.LabelField($"Normal: {cubePositions.Count} | Fixed: {fixedCubePositions.Count}", GUILayout.Width(130));
        
        if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            if (EditorUtility.DisplayDialog("Clear", "Are you sure you want to delete all cubes?", "Yes", "No"))
            {
                ClearDesign();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // Grid çizimi
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
        
        Rect gridRect = GUILayoutUtility.GetRect(gridSize * cellSize + 40, gridSize * cellSize + 40);
        
        // Arka plan
        EditorGUI.DrawRect(gridRect, new Color(0.15f, 0.15f, 0.15f));
        
        // Grid hücreleri
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Rect cellRect = new Rect(
                    gridRect.x + 20 + x * cellSize,
                    gridRect.y + 20 + (gridSize - 1 - y) * cellSize, // Y ters çevrildi
                    cellSize - 2,
                    cellSize - 2
                );
                
                Vector3Int pos = new Vector3Int(x, y, currentLayer);
                bool hasNormalCube = cubePositions.Contains(pos);
                bool hasFixedCube = fixedCubePositions.Contains(pos);
                bool hasCube = hasNormalCube || hasFixedCube;
                
                // Diğer katmanlardaki küpler
                bool hasOtherLayerCube = false;
                bool hasOtherLayerFixed = false;
                for (int z = 0; z < gridSize; z++)
                {
                    if (z != currentLayer)
                    {
                        if (cubePositions.Contains(new Vector3Int(x, y, z)))
                            hasOtherLayerCube = true;
                        if (fixedCubePositions.Contains(new Vector3Int(x, y, z)))
                            hasOtherLayerFixed = true;
                    }
                }
                
                // Hücre rengi
                Color cellColor;
                if (hasFixedCube)
                {
                    // Sabit küp - turuncu kenar ile göster
                    cellColor = GetCubeColor(pos);
                }
                else if (hasNormalCube)
                {
                    cellColor = GetCubeColor(pos);
                }
                else if (hasOtherLayerFixed)
                {
                    cellColor = new Color(0.4f, 0.3f, 0.2f); // Diğer katman sabit
                }
                else if (hasOtherLayerCube)
                {
                    cellColor = new Color(0.3f, 0.3f, 0.35f); // Diğer katman normal
                }
                else
                {
                    cellColor = new Color(0.2f, 0.2f, 0.22f); // Boş hücre
                }
                
                EditorGUI.DrawRect(cellRect, cellColor);
                
                // Sabit küp işareti - turuncu çerçeve
                if (hasFixedCube)
                {
                    Rect borderRect = new Rect(cellRect.x - 1, cellRect.y - 1, cellRect.width + 2, cellRect.height + 2);
                    EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.y, borderRect.width, 2), new Color(1f, 0.6f, 0.2f));
                    EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.yMax - 2, borderRect.width, 2), new Color(1f, 0.6f, 0.2f));
                    EditorGUI.DrawRect(new Rect(borderRect.x, borderRect.y, 2, borderRect.height), new Color(1f, 0.6f, 0.2f));
                    EditorGUI.DrawRect(new Rect(borderRect.xMax - 2, borderRect.y, 2, borderRect.height), new Color(1f, 0.6f, 0.2f));
                    
                    // Merkeze küçük turuncu nokta
                    Rect dotRect = new Rect(cellRect.center.x - 3, cellRect.center.y - 3, 6, 6);
                    EditorGUI.DrawRect(dotRect, new Color(1f, 0.6f, 0.2f));
                }
                
                // Kenarlık
                Handles.color = hasCube ? Color.white : new Color(0.3f, 0.3f, 0.3f);
                Handles.DrawWireDisc(cellRect.center, Vector3.forward, 1);
                
                // Tıklama kontrolü
                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.button == 0) // Sol tık - ekle/kaldır
                    {
                        if (placementMode == CubePlacementMode.Fixed)
                        {
                            // Sabit küp modu
                            if (hasFixedCube)
                            {
                                fixedCubePositions.Remove(pos);
                            }
                            else
                            {
                                // Eğer normal küp varsa, onu sabit yap
                                if (hasNormalCube)
                                    cubePositions.Remove(pos);
                                fixedCubePositions.Add(pos);
                            }
                        }
                        else
                        {
                            // Normal küp modu
                            if (hasNormalCube)
                            {
                                cubePositions.Remove(pos);
                            }
                            else
                            {
                                // Eğer sabit küp varsa, onu normal yap
                                if (hasFixedCube)
                                    fixedCubePositions.Remove(pos);
                                cubePositions.Add(pos);
                            }
                        }
                        
                        Event.current.Use();
                        Repaint();
                    }
                    else if (Event.current.button == 1) // Sağ tık - küpü sil
                    {
                        cubePositions.Remove(pos);
                        fixedCubePositions.Remove(pos);
                        Event.current.Use();
                        Repaint();
                    }
                }
            }
        }
        
        // Koordinat göstergeleri
        GUI.color = Color.gray;
        for (int i = 0; i < gridSize; i++)
        {
            // X ekseni
            GUI.Label(new Rect(gridRect.x + 20 + i * cellSize + cellSize/2 - 5, gridRect.y + 5, 20, 15), i.ToString());
            // Y ekseni
            GUI.Label(new Rect(gridRect.x + 2, gridRect.y + 20 + (gridSize - 1 - i) * cellSize + cellSize/2 - 7, 15, 15), i.ToString());
        }
        GUI.color = Color.white;
        
        EditorGUILayout.EndScrollView();
        
        EditorGUILayout.EndVertical();
    }
    
    // ========================================
    // SAĞ PANEL - AYARLAR
    // ========================================
    private void DrawSettingsPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(220));
        
        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        levelNumber = EditorGUILayout.IntField("Level No:", levelNumber);
        levelName = EditorGUILayout.TextField("Name:", levelName);
        
        EditorGUILayout.Space(10);
        
        // ===== RENK PALETİ =====
        showPaletteEditor = EditorGUILayout.Foldout(showPaletteEditor, "Color Palette", true);
        
        if (showPaletteEditor)
        {
            EditorGUI.indentLevel++;
            
            // Preset seçimi
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Preset:", GUILayout.Width(50));
            int newPaletteIndex = EditorGUILayout.Popup(paletteIndex, paletteNames);
            if (newPaletteIndex != paletteIndex)
            {
                paletteIndex = newPaletteIndex;
                // Custom palette'i preset ile doldur
                LoadPresetToPalette(paletteIndex);
                // Arka plan presetini de eşleştir
                if (paletteIndex < bgPresets.Length)
                {
                    bgPresetIndex = paletteIndex;
                    bgTopColor = bgPresets[paletteIndex][0];
                    bgBottomColor = bgPresets[paletteIndex][1];
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // Custom renk düzenleme
            EditorGUILayout.LabelField("Colors (set individually):");
            
            // Renk yoksa başlangıç renkleri ekle
            if (customPalette.Count == 0)
            {
                LoadPresetToPalette(paletteIndex);
            }
            
            // Her renk için ColorField
            for (int i = 0; i < customPalette.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                customPalette[i] = EditorGUILayout.ColorField($"Color {i + 1}:", customPalette[i]);
                
                // Renk sil butonu
                if (customPalette.Count > 2)
                {
                    if (GUILayout.Button("✕", GUILayout.Width(22)))
                    {
                        customPalette.RemoveAt(i);
                        break;
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            // Renk ekle/sil butonları
            EditorGUILayout.BeginHorizontal();
            
            if (customPalette.Count < 8)
            {
                if (GUILayout.Button("+ Add Color"))
                {
                    // Son rengin hafif varyasyonunu ekle
                    Color lastColor = customPalette.Count > 0 ? customPalette[customPalette.Count - 1] : Color.white;
                    customPalette.Add(new Color(lastColor.r * 0.9f, lastColor.g * 0.9f, lastColor.b * 0.9f));
                }
            }
            
            if (GUILayout.Button("Reset"))
            {
                LoadPresetToPalette(paletteIndex);
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Palet önizleme (gradient şerit)
            EditorGUILayout.Space(5);
            Rect palettePreviewRect = GUILayoutUtility.GetRect(180, 25);
            
            if (customPalette.Count > 0)
            {
                float segmentWidth = palettePreviewRect.width / customPalette.Count;
                for (int i = 0; i < customPalette.Count; i++)
                {
                    Rect segmentRect = new Rect(palettePreviewRect.x + i * segmentWidth, palettePreviewRect.y, segmentWidth, 25);
                    EditorGUI.DrawRect(segmentRect, customPalette[i]);
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space(10);
        
        // ===== ARKA PLAN RENKLERİ =====
        EditorGUILayout.LabelField("Arka Plan", EditorStyles.boldLabel);
        
        int newBgPreset = EditorGUILayout.Popup("Preset:", bgPresetIndex, bgPresetNames);
        if (newBgPreset != bgPresetIndex)
        {
            bgPresetIndex = newBgPreset;
            bgTopColor = bgPresets[bgPresetIndex][0];
            bgBottomColor = bgPresets[bgPresetIndex][1];
        }
        
        bgTopColor = EditorGUILayout.ColorField("Top Color:", bgTopColor);
        bgBottomColor = EditorGUILayout.ColorField("Bottom Color:", bgBottomColor);
        
        // Arka plan önizleme (gradient)
        Rect bgPreviewRect = GUILayoutUtility.GetRect(180, 40);
        for (int i = 0; i < 40; i++)
        {
            float t = i / 39f;
            Color lineColor = Color.Lerp(bgTopColor, bgBottomColor, t);
            Rect lineRect = new Rect(bgPreviewRect.x, bgPreviewRect.y + i, bgPreviewRect.width, 1);
            EditorGUI.DrawRect(lineRect, lineColor);
        }
        
        EditorGUILayout.Space(10);
        
        // Grid ayarları
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        gridSize = EditorGUILayout.IntSlider("Grid Size:", gridSize, 5, 20);
        cellSize = EditorGUILayout.Slider("Cell Size:", cellSize, 15f, 40f);
        
        EditorGUILayout.Space(10);
        
        // Preview
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        showPreview = EditorGUILayout.Toggle("3D Preview:", showPreview);
        
        if (!showPreview)
        {
            ClearPreview();
        }
        
        EditorGUILayout.Space(10);
        
        // Scene View Edit Mode
        EditorGUILayout.LabelField("Scene View Edit", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        sceneViewEditMode = EditorGUILayout.Toggle("Edit Mode:", sceneViewEditMode);
        if (EditorGUI.EndChangeCheck() && sceneViewEditMode)
        {
            // Focus to Scene View
            SceneView.lastActiveSceneView?.Focus();
        }
        
        if (sceneViewEditMode)
        {
            sceneViewLayer = EditorGUILayout.IntSlider("Y Layer:", sceneViewLayer, 0, 10);
            
            EditorGUILayout.HelpBox(
                "Left Click: Add Cube\n" +
                "Shift + Left Click: Remove Cube\n" +
                "Ctrl + Left Click: Add Fixed Cube\n" +
                "Scroll: Change Layer", 
                MessageType.Info);
        }
        
        EditorGUILayout.Space(20);
        
        // KAYDET BUTONLARI
        EditorGUILayout.LabelField("Save", EditorStyles.boldLabel);
        
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("💾 KAYDET", GUILayout.Height(35)))
        {
            SaveLevel();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(5);
        
        // TEST ET BUTONU
        GUI.backgroundColor = new Color(0.3f, 0.6f, 1f);
        if (GUILayout.Button("▶️ TEST ET", GUILayout.Height(35)))
        {
            TestLevel();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space(5);
        
        if (GUILayout.Button("📋 Kodu Kopyala", GUILayout.Height(25)))
        {
            string code = GenerateLevelCode();
            EditorGUIUtility.systemCopyBuffer = code;
            EditorUtility.DisplayDialog("Copied", "Level code copied to clipboard!", "OK");
        }
        
        EditorGUILayout.Space(20);
        
        // Kod önizleme
        EditorGUILayout.LabelField("Code Preview:", EditorStyles.boldLabel);
        
        string preview = GenerateLevelCode();
        if (preview.Length > 500)
            preview = preview.Substring(0, 500) + "...";
        
        EditorGUILayout.TextArea(preview, GUILayout.Height(120));
        
        EditorGUILayout.EndVertical();
    }
    
    // ========================================
    // KAYDETME İŞLEMLERİ - JSON TABANLI
    // ========================================
    private void SaveLevel()
    {
        if (cubePositions.Count == 0 && fixedCubePositions.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Add at least one cube!", "OK");
            return;
        }
        
        // Tüm küpler = normal + sabit
        List<Vector3Int> allCubes = new List<Vector3Int>();
        allCubes.AddRange(cubePositions);
        allCubes.AddRange(fixedCubePositions);
        
        // Pozisyonları normalize et (minimum 0,0,0'dan başla)
        Vector3Int min = GetMinBounds();
        
        List<Vector3Int> normalizedAll = new List<Vector3Int>();
        List<Vector3Int> normalizedFixed = new List<Vector3Int>();
        
        foreach (var pos in allCubes)
        {
            normalizedAll.Add(new Vector3Int(pos.x - min.x, pos.y - min.y, pos.z - min.z));
        }
        
        foreach (var pos in fixedCubePositions)
        {
            normalizedFixed.Add(new Vector3Int(pos.x - min.x, pos.y - min.y, pos.z - min.z));
        }
        
        // JSON objesi oluştur
        LevelDataJson jsonData = new LevelDataJson();
        jsonData.levelNumber = levelNumber;
        jsonData.levelName = levelName;
        jsonData.colorPaletteIndex = paletteIndex;
        jsonData.description = levelName;
        jsonData.SetAllPositions(normalizedAll);
        jsonData.SetFixedPositions(normalizedFixed);
        
        // Mevcut level var mı kontrol et
        if (LevelJsonManager.LevelExists(levelNumber))
        {
            if (!EditorUtility.DisplayDialog("Level Exists", 
                $"Level {levelNumber} already exists. Do you want to overwrite?", "Yes", "No"))
            {
                return;
            }
        }
        
        // JSON'a kaydet
        LevelJsonManager.SaveLevel(jsonData);
        
        // Listeyi güncelle
        LoadExistingLevels();
        
        EditorUtility.DisplayDialog("Success!", 
            $"Level {levelNumber} '{levelName}' saved as JSON!\n\n" +
            $"Total cubes: {normalizedAll.Count}\n" +
            $"Fixed cubes: {normalizedFixed.Count}\n" +
            $"Normal cubes: {normalizedAll.Count - normalizedFixed.Count}\n\n" +
            $"You can now press Play!", "OK");
    }
    
    /// <summary>
    /// Level'ı kaydedip test moduna girer
    /// </summary>
    private void TestLevel()
    {
        // Önce kaydet
        if (cubePositions.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "Add at least one cube to test!", "OK");
            return;
        }
        
        SaveLevel();
        
        // Preview küplerini temizle (önemli!)
        ClearPreview();
        CleanupAllPreviewObjects();
        
        // Test level'ı dosyaya yaz (en güvenilir yöntem)
        string testLevelFile = System.IO.Path.Combine(Application.dataPath, "Resources/Levels/test_level.txt");
        System.IO.File.WriteAllText(testLevelFile, levelNumber.ToString());
        Debug.Log($"[LevelDesigner] Test level {levelNumber} written to file: {testLevelFile}");
        
        // EditorPrefs'e de yaz (backup)
        EditorPrefs.SetInt("Hue3D_TestLevel", levelNumber);
        
        // Asset refresh ve Play moduna gir
        AssetDatabase.Refresh();
        EditorApplication.delayCall += () =>
        {
            EditorApplication.isPlaying = true;
        };
    }

    private int FindLevelEnd(string content, int startPos)
    {
        int depth = 0;
        bool inString = false;
        bool started = false;
        
        for (int i = startPos; i < content.Length; i++)
        {
            char c = content[i];
            
            // String kontrolü
            if (c == '"' && (i == 0 || content[i-1] != '\\'))
                inString = !inString;
            
            if (!inString)
            {
                if (c == '(' || c == '{')
                {
                    depth++;
                    started = true;
                }
                else if (c == ')' || c == '}')
                {
                    depth--;
                    if (started && depth == 0)
                    {
                        return i + 1;
                    }
                }
            }
        }
        
        return -1;
    }
    
    private int FindLastLevelEnd(string content)
    {
        // levels dizisinin kapanış noktasını bul - "};" pattern'ını ara
        // CreateLevel veya CreateLevelWithFixed çağrılarının sonunu bul
        
        // Önce "levels = new LevelData[]" bloğunun sonunu bul
        int levelsArrayStart = content.IndexOf("levels = new LevelData[]");
        if (levelsArrayStart < 0) return -1;
        
        // Bu blok içindeki son CreateLevel çağrısını bul
        int searchEnd = content.IndexOf("};", levelsArrayStart);
        if (searchEnd < 0) return -1;
        
        // Son CreateLevelWithFixed veya CreateLevel çağrısını bul (hangisi daha sondaysa)
        string searchArea = content.Substring(levelsArrayStart, searchEnd - levelsArrayStart);
        
        int lastWithFixed = searchArea.LastIndexOf("CreateLevelWithFixed(");
        int lastNormal = searchArea.LastIndexOf("CreateLevel(");
        
        // Hangisi daha sonda?
        int lastIndex;
        if (lastWithFixed > lastNormal && lastWithFixed >= 0)
        {
            lastIndex = levelsArrayStart + lastWithFixed;
        }
        else if (lastNormal >= 0)
        {
            lastIndex = levelsArrayStart + lastNormal;
        }
        else
        {
            return -1;
        }
        
        return FindLevelEnd(content, lastIndex);
    }
    
    private string GenerateLevelCode()
    {
        // Tüm küpler = normal + sabit
        List<Vector3Int> allCubes = new List<Vector3Int>();
        allCubes.AddRange(cubePositions);
        allCubes.AddRange(fixedCubePositions);
        
        if (allCubes.Count == 0)
            return "// No cubes";
        
        // Pozisyonları normalize et (minimum 0,0,0'dan başla)
        Vector3Int min = GetMinBounds();
        
        List<Vector3Int> normalizedAll = new List<Vector3Int>();
        List<Vector3Int> normalizedFixed = new List<Vector3Int>();
        
        foreach (var pos in allCubes)
        {
            normalizedAll.Add(new Vector3Int(pos.x - min.x, pos.y - min.y, pos.z - min.z));
        }
        
        foreach (var pos in fixedCubePositions)
        {
            normalizedFixed.Add(new Vector3Int(pos.x - min.x, pos.y - min.y, pos.z - min.z));
        }
        
        // Sırala
        normalizedAll.Sort((a, b) => {
            if (a.z != b.z) return a.z.CompareTo(b.z);
            if (a.y != b.y) return a.y.CompareTo(b.y);
            return a.x.CompareTo(b.x);
        });
        
        normalizedFixed.Sort((a, b) => {
            if (a.z != b.z) return a.z.CompareTo(b.z);
            if (a.y != b.y) return a.y.CompareTo(b.y);
            return a.x.CompareTo(b.x);
        });
        
        StringBuilder sb = new StringBuilder();
        
        // Yeni format: CreateLevelWithFixed(num, name, palette, desc, positions, fixedPositions)
        sb.Append($"CreateLevelWithFixed({levelNumber}, \"{levelName}\", {paletteIndex}, \"{levelName}\",\n");
        
        // Tüm küpler
        sb.Append("                new Vector3Int[] {\n");
        for (int i = 0; i < normalizedAll.Count; i++)
        {
            var pos = normalizedAll[i];
            if (i % 5 == 0) sb.Append("                    ");
            sb.Append($"new Vector3Int({pos.x},{pos.y},{pos.z})");
            if (i < normalizedAll.Count - 1) sb.Append(", ");
            if (i % 5 == 4 || i == normalizedAll.Count - 1) sb.Append("\n");
        }
        sb.Append("                },\n");
        
        // Sabit küpler
        sb.Append("                new Vector3Int[] {\n");
        if (normalizedFixed.Count > 0)
        {
            for (int i = 0; i < normalizedFixed.Count; i++)
            {
                var pos = normalizedFixed[i];
                if (i % 5 == 0) sb.Append("                    ");
                sb.Append($"new Vector3Int({pos.x},{pos.y},{pos.z})");
                if (i < normalizedFixed.Count - 1) sb.Append(", ");
                if (i % 5 == 4 || i == normalizedFixed.Count - 1) sb.Append("\n");
            }
        }
        sb.Append("                })");
        
        return sb.ToString();
    }
    
    // ========================================
    // LEVEL YÜKLEME - JSON + HARDCODED
    // ========================================
    private void LoadExistingLevels()
    {
        existingLevels.Clear();
        
        // Önce JSON dosyalarını yükle
        List<string> jsonFiles = LevelJsonManager.GetAllLevelFiles();
        foreach (var fileName in jsonFiles)
        {
            // level_XX formatından numarayı çıkar
            if (fileName.StartsWith("level_") && fileName.Length >= 8)
            {
                string numStr = fileName.Substring(6);
                if (int.TryParse(numStr, out int levelNum))
                {
                    LevelDataJson jsonData = LevelJsonManager.LoadLevelFromFile(levelNum);
                    if (jsonData != null)
                    {
                        LevelInfo info = new LevelInfo();
                        info.number = jsonData.levelNumber;
                        info.name = jsonData.levelName + " (JSON)";
                        info.cubeCount = jsonData.GetAllPositions().Count;
                        existingLevels.Add(info);
                    }
                }
            }
        }
        
        // Sonra LevelManager.cs'deki hardcoded level'ları yükle
        string path = "Assets/Scripts/LevelManager.cs";
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            
            // CreateLevel çağrılarını bul
            Regex regexOld = new Regex(@"CreateLevel\s*\(\s*(\d+)\s*,\s*""([^""]+)""\s*,\s*\d+\s*,\s*[\d.]+f?\s*,\s*""[^""]*""\s*,\s*new\s+Vector3Int\s*\[\s*\]\s*\{([^}]+)\}");
            
            foreach (Match match in regexOld.Matches(content))
            {
                int levelNum = int.Parse(match.Groups[1].Value);
                
                // JSON'da yoksa ekle
                if (!existingLevels.Exists(l => l.number == levelNum))
                {
                    LevelInfo info = new LevelInfo();
                    info.number = levelNum;
                    info.name = match.Groups[2].Value;
                    
                    string positions = match.Groups[3].Value;
                    info.cubeCount = Regex.Matches(positions, @"Vector3Int").Count;
                    
                    existingLevels.Add(info);
                }
            }
            
            // CreateLevelWithFixed çağrılarını bul
            Regex regexNew = new Regex(@"CreateLevelWithFixed\s*\(\s*(\d+)\s*,\s*""([^""]+)""\s*,\s*\d+\s*,\s*""[^""]*""\s*,\s*new\s+Vector3Int\s*\[\s*\]\s*\{([^}]+)\}");
            
            foreach (Match match in regexNew.Matches(content))
            {
                int levelNum = int.Parse(match.Groups[1].Value);
                
                // JSON'da yoksa ekle
                if (!existingLevels.Exists(l => l.number == levelNum))
                {
                    LevelInfo info = new LevelInfo();
                    info.number = levelNum;
                    info.name = match.Groups[2].Value;
                    
                    string positions = match.Groups[3].Value;
                    info.cubeCount = Regex.Matches(positions, @"Vector3Int").Count;
                    
                    existingLevels.Add(info);
                }
            }
        }
        
        // Sırala
        existingLevels.Sort((a, b) => a.number.CompareTo(b.number));
    }
    
    private void LoadLevelFromManager(int levelNum)
    {
        // Önce JSON'dan yüklemeyi dene
        LevelDataJson jsonData = LevelJsonManager.LoadLevelFromFile(levelNum);
        if (jsonData != null)
        {
            LoadFromJsonData(jsonData);
            return;
        }
        
        // JSON yoksa LevelManager.cs'den oku
        LoadFromLevelManagerCS(levelNum);
    }
    
    private void LoadFromJsonData(LevelDataJson jsonData)
    {
        levelNumber = jsonData.levelNumber;
        levelName = jsonData.levelName;
        paletteIndex = jsonData.colorPaletteIndex;
        
        // Arka plan renklerini palette'e göre ayarla
        if (paletteIndex < bgPresets.Length)
        {
            bgPresetIndex = paletteIndex;
            bgTopColor = bgPresets[paletteIndex][0];
            bgBottomColor = bgPresets[paletteIndex][1];
        }
        
        // Custom palette'i preset'ten yükle
        LoadPresetToPalette(paletteIndex);
        
        // Pozisyonları yükle
        cubePositions.Clear();
        fixedCubePositions.Clear();
        
        List<Vector3Int> allPositions = jsonData.GetAllPositions();
        List<Vector3Int> fixedPositions = jsonData.GetFixedPositions();
        HashSet<Vector3Int> fixedSet = new HashSet<Vector3Int>(fixedPositions);
        
        foreach (var pos in allPositions)
        {
            if (fixedSet.Contains(pos))
            {
                fixedCubePositions.Add(pos);
            }
            else
            {
                cubePositions.Add(pos);
            }
        }
        
        // İlk katmana git
        currentLayer = 0;
        
        Debug.Log($"[LevelDesigner] Level {levelNumber} loaded from JSON: {cubePositions.Count} normal, {fixedCubePositions.Count} fixed cubes");
        
        Repaint();
    }
    
    private void LoadFromLevelManagerCS(int levelNum)
    {
        string path = "Assets/Scripts/LevelManager.cs";
        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("Hata", $"Level {levelNum} not found!", "OK");
            return;
        }
        
        string content = File.ReadAllText(path);
        
        // Önce CreateLevelWithFixed formatını dene
        string patternWithFixed = $@"CreateLevelWithFixed\s*\(\s*{levelNum}\s*,\s*""([^""]+)""\s*,\s*(\d+)\s*,\s*""[^""]*""\s*,\s*new\s+Vector3Int\s*\[\s*\]\s*\{{([^}}]+)\}}\s*,\s*new\s+Vector3Int\s*\[\s*\]\s*\{{([^}}]*)\}}";
        Match match = Regex.Match(content, patternWithFixed);
        bool isWithFixed = match.Success;
        
        if (!match.Success)
        {
            // Eski CreateLevel formatını dene
            string patternOld = $@"CreateLevel\s*\(\s*{levelNum}\s*,\s*""([^""]+)""\s*,\s*(\d+)\s*,\s*([\d.]+)f?\s*,\s*""[^""]*""\s*,\s*new\s+Vector3Int\s*\[\s*\]\s*\{{([^}}]+)\}}";
            match = Regex.Match(content, patternOld);
        }
        
        if (!match.Success)
        {
            EditorUtility.DisplayDialog("Hata", $"Level {levelNum} not found!", "OK");
            return;
        }
        
        // Bilgileri yükle
        levelNumber = levelNum;
        levelName = match.Groups[1].Value;
        paletteIndex = int.Parse(match.Groups[2].Value);
        
        // Arka plan renklerini palette'e göre ayarla
        if (paletteIndex < bgPresets.Length)
        {
            bgPresetIndex = paletteIndex;
            bgTopColor = bgPresets[paletteIndex][0];
            bgBottomColor = bgPresets[paletteIndex][1];
        }
        
        // Custom palette'i preset'ten yükle
        LoadPresetToPalette(paletteIndex);
        
        // Pozisyonları yükle
        cubePositions.Clear();
        fixedCubePositions.Clear();
        
        Regex posRegex = new Regex(@"Vector3Int\s*\(\s*(-?\d+)\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*\)");
        
        // Tüm küp pozisyonları - CreateLevelWithFixed için grup 3, CreateLevel için grup 4
        string positionsStr = isWithFixed ? match.Groups[3].Value : match.Groups[4].Value;
        foreach (Match posMatch in posRegex.Matches(positionsStr))
        {
            int x = int.Parse(posMatch.Groups[1].Value);
            int y = int.Parse(posMatch.Groups[2].Value);
            int z = int.Parse(posMatch.Groups[3].Value);
            cubePositions.Add(new Vector3Int(x, y, z));
        }
        
        // Sabit küp pozisyonları (CreateLevelWithFixed formatında)
        if (isWithFixed && match.Groups.Count > 4)
        {
            string fixedPosStr = match.Groups[4].Value;
            HashSet<Vector3Int> fixedSet = new HashSet<Vector3Int>();
            
            foreach (Match posMatch in posRegex.Matches(fixedPosStr))
            {
                int x = int.Parse(posMatch.Groups[1].Value);
                int y = int.Parse(posMatch.Groups[2].Value);
                int z = int.Parse(posMatch.Groups[3].Value);
                fixedSet.Add(new Vector3Int(x, y, z));
            }
            
            // cubePositions'dan fixed olanları ayır
            foreach (var pos in fixedSet)
            {
                if (cubePositions.Contains(pos))
                {
                    cubePositions.Remove(pos);
                    fixedCubePositions.Add(pos);
                }
            }
        }
        
        // İlk katmana git
        currentLayer = 0;
        
        Debug.Log($"[LevelDesigner] Level {levelNumber} loaded from LevelManager.cs: {cubePositions.Count} normal, {fixedCubePositions.Count} fixed cubes");
        
        Repaint();
    }
    
    // ========================================
    // YARDIMCI METODLAR
    // ========================================
    
    /// <summary>
    /// Preset paletini custom palette'e yükler - ColorPalettes ile senkronize
    /// </summary>
    private void LoadPresetToPalette(int presetIndex)
    {
        customPalette.Clear();
        
        // ColorPalettes.AllPalettes kullan (PuzzleGenerator ile aynı kaynak)
        if (presetIndex >= 0 && presetIndex < ColorPalettes.AllPalettes.Length)
        {
            foreach (var color in ColorPalettes.AllPalettes[presetIndex])
            {
                customPalette.Add(color);
            }
        }
        else
        {
            // Varsayılan - ilk palette
            foreach (var color in ColorPalettes.AllPalettes[0])
            {
                customPalette.Add(color);
            }
        }
    }
    
    private void ClearDesign()
    {
        cubePositions.Clear();
        fixedCubePositions.Clear();
        currentLayer = 0;
        LoadPresetToPalette(paletteIndex); // Palette'i sıfırla
        ClearPreview();
        Repaint();
    }
    
    private Vector3Int GetMinBounds()
    {
        var allPositions = new List<Vector3Int>(cubePositions);
        allPositions.AddRange(fixedCubePositions);
        
        if (allPositions.Count == 0) return Vector3Int.zero;
        
        int minX = int.MaxValue, minY = int.MaxValue, minZ = int.MaxValue;
        
        foreach (var pos in allPositions)
        {
            minX = Mathf.Min(minX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            minZ = Mathf.Min(minZ, pos.z);
        }
        
        return new Vector3Int(minX, minY, minZ);
    }
    
    private Color GetCubeColor(Vector3Int pos)
    {
        // Custom palette kullan (yoksa preset'ten yükle)
        if (customPalette.Count == 0)
        {
            LoadPresetToPalette(paletteIndex);
        }
        
        if (customPalette.Count == 0)
            return Color.gray;
        
        if (customPalette.Count == 1)
            return customPalette[0];
        
        // Tüm pozisyonları topla
        var allPositions = new List<Vector3Int>(cubePositions);
        allPositions.AddRange(fixedCubePositions);
        
        if (allPositions.Count <= 1)
            return customPalette[0];
        
        // I Love Hue tarzı: Bilinear interpolation (pozisyon bazlı)
        Vector3Int min = GetMinBounds();
        Vector3Int max = GetMaxBounds();
        float rangeX = Mathf.Max(1, max.x - min.x);
        float rangeY = Mathf.Max(1, max.y - min.y);
        float rangeZ = Mathf.Max(1, max.z - min.z);
        
        float u = (pos.x - min.x) / rangeX;
        float v = (pos.y - min.y) / rangeY;
        float w = (pos.z - min.z) / rangeZ;
        
        // 4 köşe palette ise bilinear, değilse eski gradient
        Color[] paletteArray = customPalette.ToArray();
        if (paletteArray.Length >= 4)
        {
            return ColorPalettes.GetBilinearColor3D(paletteArray, u, v, w);
        }
        else
        {
            float t = u * 0.4f + v * 0.4f + w * 0.2f;
            return ColorPalettes.GetGradientFromPalette(paletteArray, t);
        }
    }
    
    private Vector3Int GetMaxBounds()
    {
        var allPositions = new List<Vector3Int>(cubePositions);
        allPositions.AddRange(fixedCubePositions);
        
        if (allPositions.Count == 0) return Vector3Int.zero;
        
        int maxX = int.MinValue, maxY = int.MinValue, maxZ = int.MinValue;
        
        foreach (var pos in allPositions)
        {
            maxX = Mathf.Max(maxX, pos.x);
            maxY = Mathf.Max(maxY, pos.y);
            maxZ = Mathf.Max(maxZ, pos.z);
        }
        
        return new Vector3Int(maxX, maxY, maxZ);
    }
    
    // ========================================
    // 3D PREVIEW
    // ========================================
    private void UpdatePreview()
    {
        // Play modunda preview oluşturma!
        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            ClearPreview();
            return;
        }
        
        ClearPreview();
        
        if (cubePositions.Count == 0 && fixedCubePositions.Count == 0) return;
        
        previewParent = new GameObject("_LevelDesigner_Preview_");
        previewParent.hideFlags = HideFlags.HideAndDontSave;
        
        // Normal küpler
        foreach (var pos in cubePositions)
        {
            CreatePreviewCube(pos, GetCubeColor(pos), false);
        }
        
        // Sabit küpler (daha koyu renk ve marker ile)
        foreach (var pos in fixedCubePositions)
        {
            CreatePreviewCube(pos, GetCubeColor(pos), true);
        }
        
        SceneView.RepaintAll();
    }
    
    private void CreatePreviewCube(Vector3Int pos, Color color, bool isFixed)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = isFixed ? $"PreviewFixed_{pos.x}_{pos.y}_{pos.z}" : $"Preview_{pos.x}_{pos.y}_{pos.z}";
        cube.transform.parent = previewParent.transform;
        cube.transform.position = new Vector3(pos.x * 1.02f, pos.y * 1.02f, pos.z * 1.02f);
        cube.transform.localScale = Vector3.one * 0.98f;
        cube.hideFlags = HideFlags.HideAndDontSave;
        
        // Collider kaldır
        var collider = cube.GetComponent<Collider>();
        if (collider != null)
            Object.DestroyImmediate(collider);
        
        // Renk ver
        var renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? 
                                       Shader.Find("Standard") ?? 
                                       Shader.Find("Unlit/Color"));
            
            if (isFixed)
            {
                // Sabit küpler için biraz daha koyu renk
                mat.color = Color.Lerp(color, Color.black, 0.15f);
            }
            else
            {
                mat.color = color;
            }
            
            mat.hideFlags = HideFlags.HideAndDontSave;
            renderer.sharedMaterial = mat;
        }
    }
    
    private void ClearPreview()
    {
        if (previewParent != null)
        {
            // Önce child'ları temizle
            while (previewParent.transform.childCount > 0)
            {
                var child = previewParent.transform.GetChild(0);
                var renderer = child.GetComponent<Renderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    Object.DestroyImmediate(renderer.sharedMaterial);
                }
                Object.DestroyImmediate(child.gameObject);
            }
            
            Object.DestroyImmediate(previewParent);
            previewParent = null;
        }
    }
    
    // ========================================
    // SCENE VIEW
    // ========================================
    private void OnSceneGUI(SceneView sceneView)
    {
        // Edit modu açıksa grid ve etkileşim göster
        if (sceneViewEditMode)
        {
            DrawSceneViewGrid(sceneView);
            HandleSceneViewInput(sceneView);
        }
        
        if (!showPreview || (cubePositions.Count == 0 && fixedCubePositions.Count == 0)) return;
        
        // Tüm pozisyonları birleştir
        var allPositions = new List<Vector3Int>(cubePositions);
        allPositions.AddRange(fixedCubePositions);
        
        if (allPositions.Count == 0) return;
        
        // Merkez noktasını göster
        Vector3 center = Vector3.zero;
        foreach (var pos in allPositions)
        {
            center += new Vector3(pos.x, pos.y, pos.z);
        }
        center /= allPositions.Count;
        
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(center, Vector3.up, 0.5f);
        
        // Katman göstergesi
        Handles.color = new Color(1, 1, 0, 0.2f);
        Handles.DrawSolidDisc(new Vector3(center.x, 0, currentLayer), Vector3.up, 0.3f);
    }
    
    /// <summary>
    /// Scene View'da edit grid'i çizer - Profesyonel level designer görünümü
    /// </summary>
    private void DrawSceneViewGrid(SceneView sceneView)
    {
        int gridExtent = 12;
        float y = sceneViewLayer;
        
        // ========== ZEMIN GRID ==========
        // Ana grid çizgileri (her birim)
        Handles.color = new Color(0.4f, 0.4f, 0.4f, 0.4f);
        for (int x = -gridExtent; x <= gridExtent; x++)
        {
            float alpha = (x == 0) ? 0.8f : 0.25f;
            Handles.color = new Color(0.5f, 0.5f, 0.5f, alpha);
            Handles.DrawLine(
                new Vector3(x, y, -gridExtent),
                new Vector3(x, y, gridExtent));
        }
        for (int z = -gridExtent; z <= gridExtent; z++)
        {
            float alpha = (z == 0) ? 0.8f : 0.25f;
            Handles.color = new Color(0.5f, 0.5f, 0.5f, alpha);
            Handles.DrawLine(
                new Vector3(-gridExtent, y, z),
                new Vector3(gridExtent, y, z));
        }
        
        // 5 birimlik major grid çizgileri
        Handles.color = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        for (int x = -gridExtent; x <= gridExtent; x += 5)
        {
            if (x == 0) continue;
            Handles.DrawLine(
                new Vector3(x, y, -gridExtent),
                new Vector3(x, y, gridExtent));
        }
        for (int z = -gridExtent; z <= gridExtent; z += 5)
        {
            if (z == 0) continue;
            Handles.DrawLine(
                new Vector3(-gridExtent, y, z),
                new Vector3(gridExtent, y, z));
        }
        
        // ========== EKSEN GÖSTERGELERİ ==========
        float axisLength = gridExtent + 1;
        
        // X Ekseni (Kırmızı)
        Handles.color = new Color(1f, 0.3f, 0.3f, 0.9f);
        Handles.DrawLine(new Vector3(0, y, 0), new Vector3(axisLength, y, 0));
        Handles.ConeHandleCap(0, new Vector3(axisLength, y, 0), Quaternion.LookRotation(Vector3.right), 0.3f, EventType.Repaint);
        
        // Z Ekseni (Mavi)
        Handles.color = new Color(0.3f, 0.5f, 1f, 0.9f);
        Handles.DrawLine(new Vector3(0, y, 0), new Vector3(0, y, axisLength));
        Handles.ConeHandleCap(0, new Vector3(0, y, axisLength), Quaternion.LookRotation(Vector3.forward), 0.3f, EventType.Repaint);
        
        // Y Ekseni (Yeşil) - mevcut katmandan yukarı
        Handles.color = new Color(0.3f, 1f, 0.3f, 0.9f);
        Handles.DrawLine(new Vector3(0, y, 0), new Vector3(0, y + 5, 0));
        Handles.ConeHandleCap(0, new Vector3(0, y + 5, 0), Quaternion.LookRotation(Vector3.up), 0.3f, EventType.Repaint);
        
        // Orijin noktası
        Handles.color = Color.white;
        Handles.SphereHandleCap(0, new Vector3(0, y, 0), Quaternion.identity, 0.2f, EventType.Repaint);
        
        // ========== KATMAN DÜZLEM GÖSTERGESİ ==========
        // Aktif katman için yarı saydam zemin
        Vector3[] planeVerts = new Vector3[]
        {
            new Vector3(-gridExtent, y, -gridExtent),
            new Vector3(gridExtent, y, -gridExtent),
            new Vector3(gridExtent, y, gridExtent),
            new Vector3(-gridExtent, y, gridExtent)
        };
        Handles.DrawSolidRectangleWithOutline(planeVerts, new Color(0.3f, 0.6f, 1f, 0.03f), new Color(0.3f, 0.6f, 1f, 0.2f));
        
        // ========== DİĞER KATMANLARDAKİ KÜPLER (GHOST) ==========
        foreach (var pos in cubePositions)
        {
            if (pos.y != sceneViewLayer)
            {
                DrawGhostCube(pos, new Color(0.5f, 0.8f, 0.5f, 0.15f));
            }
        }
        foreach (var pos in fixedCubePositions)
        {
            if (pos.y != sceneViewLayer)
            {
                DrawGhostCube(pos, new Color(1f, 0.6f, 0.3f, 0.15f));
            }
        }
        
        // ========== AKTİF KATMANDAKİ KÜPLER ==========
        foreach (var pos in cubePositions)
        {
            if (pos.y == sceneViewLayer)
            {
                DrawFilledCube(pos, new Color(0.4f, 0.9f, 0.4f, 0.7f), new Color(0.2f, 0.7f, 0.2f, 1f));
            }
        }
        foreach (var pos in fixedCubePositions)
        {
            if (pos.y == sceneViewLayer)
            {
                DrawFilledCube(pos, new Color(1f, 0.6f, 0.2f, 0.7f), new Color(0.9f, 0.4f, 0.1f, 1f));
                // Sabit küp işareti (X)
                DrawFixedMarker(pos);
            }
        }
        
        // ========== GUI PANEL ==========
        Handles.BeginGUI();
        
        // Üst sol panel - bilgi
        float panelWidth = 220;
        float panelHeight = 140;
        GUI.Box(new Rect(5, 5, panelWidth, panelHeight), "");
        
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 14;
        titleStyle.normal.textColor = new Color(0.3f, 0.8f, 1f);
        GUI.Label(new Rect(15, 10, 200, 25), "⚙ LEVEL DESIGNER", titleStyle);
        
        GUIStyle infoStyle = new GUIStyle(EditorStyles.label);
        infoStyle.fontSize = 12;
        infoStyle.normal.textColor = Color.white;
        
        GUI.Label(new Rect(15, 35, 200, 20), $"Y Layer: {sceneViewLayer}", infoStyle);
        GUI.Label(new Rect(15, 55, 200, 20), $"Normal Cubes: {cubePositions.Count}", infoStyle);
        GUI.Label(new Rect(15, 75, 200, 20), $"Fixed Cubes: {fixedCubePositions.Count}", infoStyle);
        GUI.Label(new Rect(15, 95, 200, 20), $"Total: {cubePositions.Count + fixedCubePositions.Count}", infoStyle);
        
        // Kontroller
        GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel);
        smallStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        GUI.Label(new Rect(15, 118, 200, 20), "Click:Add | Shift:Remove | Ctrl:Fixed", smallStyle);
        
        // Sağ üst - Renk lejandı
        float legendX = sceneView.position.width - 130;
        GUI.Box(new Rect(legendX, 5, 120, 70), "");
        
        GUIStyle legendStyle = new GUIStyle(EditorStyles.miniLabel);
        legendStyle.normal.textColor = Color.white;
        
        // Yeşil kutu
        GUI.color = new Color(0.4f, 0.9f, 0.4f);
        GUI.DrawTexture(new Rect(legendX + 10, 15, 15, 15), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(legendX + 30, 13, 80, 20), "Normal", legendStyle);
        
        // Turuncu kutu
        GUI.color = new Color(1f, 0.6f, 0.2f);
        GUI.DrawTexture(new Rect(legendX + 10, 38, 15, 15), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(legendX + 30, 36, 80, 20), "Fixed", legendStyle);
        
        // Cyan kutu
        GUI.color = new Color(0f, 1f, 1f);
        GUI.DrawTexture(new Rect(legendX + 10, 58, 15, 15), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(legendX + 30, 56, 80, 20), "Cursor", legendStyle);
        
        Handles.EndGUI();
    }
    
    /// <summary>
    /// Dolgulu küp çizer
    /// </summary>
    private void DrawFilledCube(Vector3Int pos, Color fillColor, Color outlineColor)
    {
        Vector3 center = new Vector3(pos.x, pos.y, pos.z);
        Vector3 size = Vector3.one * 0.95f;
        
        // 6 yüz çiz
        Vector3 halfSize = size * 0.5f;
        
        // Üst yüz
        Vector3[] topFace = new Vector3[]
        {
            center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, halfSize.z)
        };
        Handles.DrawSolidRectangleWithOutline(topFace, fillColor, outlineColor);
        
        // Alt yüz
        Vector3[] bottomFace = new Vector3[]
        {
            center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z)
        };
        Handles.DrawSolidRectangleWithOutline(bottomFace, fillColor, outlineColor);
        
        // Ön yüz
        Vector3[] frontFace = new Vector3[]
        {
            center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, halfSize.z)
        };
        Handles.DrawSolidRectangleWithOutline(frontFace, fillColor, outlineColor);
        
        // Arka yüz
        Vector3[] backFace = new Vector3[]
        {
            center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, -halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z)
        };
        Handles.DrawSolidRectangleWithOutline(backFace, fillColor, outlineColor);
        
        // Sol yüz
        Vector3[] leftFace = new Vector3[]
        {
            center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, halfSize.z),
            center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z)
        };
        Handles.DrawSolidRectangleWithOutline(leftFace, fillColor, outlineColor);
        
        // Sağ yüz
        Vector3[] rightFace = new Vector3[]
        {
            center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
            center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, halfSize.z),
            center + new Vector3(halfSize.x, halfSize.y, -halfSize.z)
        };
        Handles.DrawSolidRectangleWithOutline(rightFace, fillColor, outlineColor);
    }
    
    /// <summary>
    /// Ghost (hayalet) küp çizer - diğer katmanlar için
    /// </summary>
    private void DrawGhostCube(Vector3Int pos, Color color)
    {
        Handles.color = color;
        Vector3 center = new Vector3(pos.x, pos.y, pos.z);
        Handles.DrawWireCube(center, Vector3.one * 0.95f);
    }
    
    /// <summary>
    /// Sabit küp işareti (X) çizer
    /// </summary>
    private void DrawFixedMarker(Vector3Int pos)
    {
        Handles.color = new Color(1f, 0.3f, 0.1f, 1f);
        Vector3 center = new Vector3(pos.x, pos.y + 0.5f, pos.z);
        float size = 0.2f;
        
        // X işareti
        Handles.DrawLine(center + new Vector3(-size, 0, -size), center + new Vector3(size, 0, size));
        Handles.DrawLine(center + new Vector3(-size, 0, size), center + new Vector3(size, 0, -size));
    }
    
    /// <summary>
    /// Küp outline çizer
    /// </summary>
    private void DrawCubeOutline(Vector3Int pos, Color color)
    {
        Handles.color = color;
        Vector3 center = new Vector3(pos.x, pos.y, pos.z);
        Handles.DrawWireCube(center, Vector3.one);
    }
    
    /// <summary>
    /// Scene View'da mouse input'u işler
    /// </summary>
    private void HandleSceneViewInput(SceneView sceneView)
    {
        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        
        // Layout event'te hemen default control'u al - diğer tool'ların önüne geç
        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlId);
        }
        
        // Scroll ile katman değiştir
        if (e.type == EventType.ScrollWheel)
        {
            sceneViewLayer += (e.delta.y > 0) ? -1 : 1;
            sceneViewLayer = Mathf.Clamp(sceneViewLayer, 0, 10);
            e.Use();
            Repaint();
            return;
        }
        
        // Mouse pozisyonunu dünya koordinatına çevir
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        float y = sceneViewLayer;
        
        // Ray'in Y=sceneViewLayer düzlemiyle kesişimini bul
        Plane plane = new Plane(Vector3.up, new Vector3(0, y, 0));
        float distance;
        if (!plane.Raycast(ray, out distance)) return;
        
        Vector3 hitPoint = ray.GetPoint(distance);
        int gridX = Mathf.RoundToInt(hitPoint.x);
        int gridZ = Mathf.RoundToInt(hitPoint.z);
        Vector3Int gridPos = new Vector3Int(gridX, sceneViewLayer, gridZ);
        
        // Grid sınırları içinde mi kontrol et
        int gridExtent = 12;
        if (Mathf.Abs(gridX) > gridExtent || Mathf.Abs(gridZ) > gridExtent) return;
        
        // Hover pozisyonunu göster - dolgulu imleç küpü
        bool existsAtHover = cubePositions.Contains(gridPos) || fixedCubePositions.Contains(gridPos);
        Color hoverFill = existsAtHover ? new Color(1f, 0.3f, 0.3f, 0.4f) : new Color(0f, 1f, 1f, 0.3f);
        Color hoverOutline = existsAtHover ? new Color(1f, 0.2f, 0.2f, 0.9f) : new Color(0f, 1f, 1f, 0.9f);
        DrawFilledCube(gridPos, hoverFill, hoverOutline);
        
        // Koordinat göstergesi
        Handles.BeginGUI();
        Vector2 guiPos = HandleUtility.WorldToGUIPoint(new Vector3(gridX, y + 1.2f, gridZ));
        GUIStyle coordStyle = new GUIStyle(EditorStyles.boldLabel);
        coordStyle.normal.textColor = Color.white;
        coordStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(guiPos.x - 40, guiPos.y - 10, 80, 20), $"({gridX}, {sceneViewLayer}, {gridZ})", coordStyle);
        Handles.EndGUI();
        
        // Mouse click
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            bool isFixed = cubePositions.Contains(gridPos) == false && fixedCubePositions.Contains(gridPos);
            bool exists = cubePositions.Contains(gridPos) || fixedCubePositions.Contains(gridPos);
            
            if (e.shift)
            {
                // Shift + Click = Sil
                if (cubePositions.Contains(gridPos))
                {
                    cubePositions.Remove(gridPos);
                }
                if (fixedCubePositions.Contains(gridPos))
                {
                    fixedCubePositions.Remove(gridPos);
                }
            }
            else if (e.control)
            {
                // Ctrl + Click = Sabit küp ekle/toggle
                if (fixedCubePositions.Contains(gridPos))
                {
                    // Sabit küpü normal küpe çevir
                    fixedCubePositions.Remove(gridPos);
                    cubePositions.Add(gridPos);
                }
                else if (cubePositions.Contains(gridPos))
                {
                    // Normal küpü sabit küpe çevir
                    cubePositions.Remove(gridPos);
                    fixedCubePositions.Add(gridPos);
                }
                else
                {
                    // Yeni sabit küp ekle
                    fixedCubePositions.Add(gridPos);
                }
            }
            else
            {
                // Normal Click = Ekle/Sil toggle
                if (exists)
                {
                    cubePositions.Remove(gridPos);
                    fixedCubePositions.Remove(gridPos);
                }
                else
                {
                    cubePositions.Add(gridPos);
                }
            }
            
            UpdatePreview();
            Repaint();
            e.Use();
            GUIUtility.hotControl = controlId;
        }
        
        if (e.type == EventType.MouseUp && e.button == 0)
        {
            GUIUtility.hotControl = 0;
            e.Use();
        }
        
        // MouseDrag'i de yakala ki kamera hareket etmesin
        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            e.Use();
        }
        
        sceneView.Repaint();
    }
}
