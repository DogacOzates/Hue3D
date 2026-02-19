using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Ana oyun yöneticisi - Oyun akışı, skor ve UI yönetimi
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public float cubeSpacing = 1.01f;  // Küpler arası minimal boşluk
    [Range(1, 50)]
    public int startingLevel = 1;  // Inspector'dan başlangıç level'i seç
    public int currentLevel = 1;
    public int maxLevel = 50;
    
    [Header("Game State")]
    public bool isPlaying;
    public int moveCount;
    public float playTime;
    
    [Header("References")]
    public PuzzleGenerator puzzleGenerator;
    public CameraController cameraController;
    public InputHandler inputHandler;
    public MobileUIManager mobileUI;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip selectSound;
    public AudioClip swapSound;
    public AudioClip winSound;
    
    // Seçili küp
    private Cube selectedCube;
    
    // Current level data
    private LevelData currentLevelData;
    
    // Çözüm gösterme için kaydedilmiş renkler
    private Dictionary<Cube, Color> savedColors = new Dictionary<Cube, Color>();
    private bool isShowingSolution;
    
    // Level preview state - shows solved state before play
    public bool isPreviewingLevel { get; private set; }
    
    // Animation state
    private bool isAnimating;
    private Coroutine spawnAnimCoroutine;
    private Coroutine shuffleCoroutine;
    
    private void Awake()
    {
        Debug.Log($"[GameManager] Awake called, Instance = {(Instance == null ? "null" : "exists")}");
        
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[GameManager] Set as Instance");
        }
        else
        {
            Debug.Log("[GameManager] Duplicate! Destroying");
            Destroy(gameObject);
            return;
        }
    }
    
    private void OnEnable()
    {
        Cube.OnCubeClicked += HandleCubeClicked;
    }
    
    private void OnDisable()
    {
        Cube.OnCubeClicked -= HandleCubeClicked;
    }
    
    private void Start()
    {
        Debug.Log("[GameManager] Start began");
        
        // NÜKLEER TEMİZLİK - Sahnedeki TÜM eski küpleri ve preview objelerini sil
        NuclearCleanup();
        
        // Level'leri yeniden yükle (Editor'deki değişiklikler için)
        LevelManager.ReloadLevels();
        
#if UNITY_EDITOR
        // Test level dosyasını kontrol et (en güvenilir yöntem)
        string testLevelFile = System.IO.Path.Combine(Application.dataPath, "Resources/Levels/test_level.txt");
        if (System.IO.File.Exists(testLevelFile))
        {
            string content = System.IO.File.ReadAllText(testLevelFile).Trim();
            if (int.TryParse(content, out int testLevel) && testLevel > 0)
            {
                startingLevel = testLevel;
                Debug.Log($"[GameManager] ✓ Test level {testLevel} loaded from file!");
            }
            // Dosyayı sil (tek kullanımlık)
            System.IO.File.Delete(testLevelFile);
        }
        else
        {
            // Fallback: EditorPrefs
            int editorTestLevel = UnityEditor.EditorPrefs.GetInt("Hue3D_TestLevel", -1);
            if (editorTestLevel > 0)
            {
                startingLevel = editorTestLevel;
                Debug.Log($"[GameManager] ✓ Test level {editorTestLevel} loaded from EditorPrefs!");
                UnityEditor.EditorPrefs.DeleteKey("Hue3D_TestLevel");
            }
            else
            {
                Debug.Log($"[GameManager] No test level, startingLevel = {startingLevel}");
            }
        }
#endif
        
        // PuzzleGenerator'ı bul veya oluştur
        if (puzzleGenerator == null)
        {
            puzzleGenerator = FindFirstObjectByType<PuzzleGenerator>();
            if (puzzleGenerator == null)
            {
                GameObject genObj = new GameObject("PuzzleGenerator");
                puzzleGenerator = genObj.AddComponent<PuzzleGenerator>();
            }
        }
        
        // CameraController'ı bul veya oluştur
        if (cameraController == null)
        {
            cameraController = FindFirstObjectByType<CameraController>();
            if (cameraController == null && Camera.main != null)
            {
                cameraController = Camera.main.gameObject.AddComponent<CameraController>();
            }
        }
        
        // InputHandler oluştur
        if (inputHandler == null)
        {
            inputHandler = FindFirstObjectByType<InputHandler>();
            if (inputHandler == null)
            {
                GameObject inputObj = new GameObject("InputHandler");
                inputHandler = inputObj.AddComponent<InputHandler>();
            }
        }
        
        // MobileUIManager oluştur
        if (mobileUI == null)
        {
            mobileUI = FindFirstObjectByType<MobileUIManager>();
            if (mobileUI == null)
            {
                GameObject uiObj = new GameObject("MobileUIManager");
                mobileUI = uiObj.AddComponent<MobileUIManager>();
            }
        }
        
        // AudioSource oluştur
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Oyunu başlat
        StartNewGame();
    }
    
    private void Update()
    {
        if (isPlaying)
        {
            playTime += Time.deltaTime;
            UpdateUI();
        }
    }
    
    /// <summary>
    /// Yeni oyun başlatır
    /// </summary>
    public void StartNewGame()
    {
        Debug.Log($"[GameManager] StartNewGame called, startingLevel = {startingLevel}");
        
        // Custom JSON level'lar da dahil olmak üzere max level'ı al
        int maxAvailable = LevelManager.MaxLevelNumber;
        currentLevel = Mathf.Clamp(startingLevel, 1, Mathf.Max(maxAvailable, startingLevel));
        maxLevel = maxAvailable;
        StartLevel(currentLevel);
    }
    
    /// <summary>
    /// Belirli bir level'ı başlatır - önce çözülmüş hali gösterilir
    /// </summary>
    public void StartLevel(int level)
    {
        currentLevel = level;
        moveCount = 0;
        playTime = 0f;
        isPlaying = false; // Preview sırasında oyun başlamaz
        isPreviewingLevel = true;
        selectedCube = null;
        
        // Sahnedeki tüm eski küpleri temizle (güvenlik için)
        CleanupOldCubes();
        
        // Level verisini al ve sakla
        currentLevelData = LevelManager.GetLevel(level);
        
        Debug.Log($"[GameManager] Starting level {level}: {currentLevelData.levelName} (preview mode)");
        
        // Tema rengini güncelle (level'a göre)
        int themeIndex = currentLevelData.colorPaletteIndex % ColorPalettes.AllPalettes.Length;
        UpdateThemeForLevel(themeIndex);
        
        // Puzzle oluştur - karıştırmadan (çözülmüş hali göster)
        puzzleGenerator.GeneratePuzzleFromLevel(currentLevelData, shuffle: false);
        
        // Kamerayı ayarla
        if (cameraController != null)
        {
            cameraController.FocusOnPuzzle(puzzleGenerator.allCubes);
        }
        
        // Hint butonunu resetle
        if (mobileUI != null)
        {
            mobileUI.ResetHintButton();
            mobileUI.HideTutorial(); // Önceki tutorial'u kapat
        }
        
        UpdateUI();
        
        // Önceki animasyonları durdur
        if (shuffleCoroutine != null) StopCoroutine(shuffleCoroutine);
        shuffleCoroutine = null;
        if (spawnAnimCoroutine != null) StopCoroutine(spawnAnimCoroutine);
        isAnimating = false;
        
        // Spawn animasyonu başlat
        spawnAnimCoroutine = StartCoroutine(SpawnCubesAnimation());
    }
    
    /// <summary>
    /// Preview'dan oyuna geçiş - küpleri küçült, karıştır, tekrar büyüt
    /// </summary>
    public void BeginPlayAfterPreview()
    {
        if (!isPreviewingLevel) return;
        if (isAnimating) return;
        
        isPreviewingLevel = false;
        
        // Tap to start overlay'ı gizle
        if (mobileUI != null)
        {
            mobileUI.HideTapToStart();
        }
        
        shuffleCoroutine = StartCoroutine(ShuffleTransitionAnimation());
    }
    
    /// <summary>
    /// Düzenli → karışık geçiş animasyonu: küçül, karıştır, büyü
    /// </summary>
    private IEnumerator ShuffleTransitionAnimation()
    {
        isAnimating = true;
        
        var cubes = new List<Cube>(puzzleGenerator.allCubes);
        if (cubes.Count == 0)
        {
            isAnimating = false;
            isPlaying = true;
            yield break;
        }
        
        // Merkez hesapla
        Vector3 center = Vector3.zero;
        foreach (var c in cubes)
        {
            if (c != null) center += c.transform.position;
        }
        center /= cubes.Count;
        
        // Dıştan içe küçült
        var shrinkOrder = new List<Cube>(cubes);
        shrinkOrder.RemoveAll(c => c == null);
        shrinkOrder.Sort((a, b) => {
            if (a == null || b == null) return 0;
            float distA = Vector3.Distance(a.transform.position, center);
            float distB = Vector3.Distance(b.transform.position, center);
            return distB.CompareTo(distA);
        });
        
        int batchSize = Mathf.Max(1, shrinkOrder.Count / 10);
        for (int i = 0; i < shrinkOrder.Count; i++)
        {
            if (shrinkOrder[i] != null)
            {
                StartCoroutine(AnimateCubeScale(shrinkOrder[i].transform, Vector3.one * 0.98f, Vector3.one * 0.001f, 0.18f));
            }
            if ((i + 1) % batchSize == 0)
            {
                yield return new WaitForSeconds(0.025f);
            }
        }
        
        yield return new WaitForSeconds(0.22f);
        
        // Renkleri karıştır (küpler görünmezken)
        puzzleGenerator.ShuffleColors();
        
        // İçten dışa büyüt
        var growOrder = new List<Cube>(cubes);
        growOrder.RemoveAll(c => c == null);
        growOrder.Sort((a, b) => {
            if (a == null || b == null) return 0;
            float distA = Vector3.Distance(a.transform.position, center);
            float distB = Vector3.Distance(b.transform.position, center);
            return distA.CompareTo(distB);
        });
        
        for (int i = 0; i < growOrder.Count; i++)
        {
            if (growOrder[i] != null)
            {
                StartCoroutine(AnimateCubeScale(growOrder[i].transform, Vector3.one * 0.001f, Vector3.one * 0.98f, 0.2f));
            }
            if ((i + 1) % batchSize == 0)
            {
                yield return new WaitForSeconds(0.025f);
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        
        isAnimating = false;
        isPlaying = true;
        
        // Tutorial level ise tutorial göster
        if (currentLevelData != null && currentLevelData.levelName == "Tutorial" && mobileUI != null)
        {
            mobileUI.ShowTutorial();
        }
        
        Debug.Log($"[GameManager] Level {currentLevel} started! Cubes shuffled.");
        UpdateUI();
    }
    
    /// <summary>
    /// Level için tema günceller (arka plan + UI)
    /// </summary>
    private void UpdateThemeForLevel(int themeIndex)
    {
        // SceneSetup'ı bul ve arka planı güncelle
        SceneSetup sceneSetup = FindFirstObjectByType<SceneSetup>();
        if (sceneSetup != null)
        {
            sceneSetup.SetColorTheme(themeIndex);
        }
        else
        {
            // SceneSetup yoksa sadece kamera arka planını güncelle
            Camera.main.backgroundColor = ColorPalettes.GetBackgroundColor(themeIndex);
            
            // MobileUIManager'ı güncelle
            if (mobileUI != null)
            {
                mobileUI.UpdateUITheme(themeIndex);
            }
        }
    }
    
    /// <summary>
    /// Küp tıklamasını işler
    /// </summary>
    private void HandleCubeClicked(Cube clickedCube)
    {
        // Preview modunda küp tıklaması da oyunu başlatsın
        if (isPreviewingLevel)
        {
            BeginPlayAfterPreview();
            return;
        }
        if (!isPlaying) return;
        if (isShowingSolution) return; // Çözüm gösterilirken hamle yapılamaz
        if (clickedCube.isFixed) return;
        
        PlaySound(selectSound);
        
        if (selectedCube == null)
        {
            // İlk küp seçimi
            selectedCube = clickedCube;
            selectedCube.SetSelected(true);
            
            // Tutorial: ilk küp seçildi
            if (mobileUI != null && mobileUI.IsTutorialActive)
            {
                mobileUI.AdvanceTutorial(); // step 1 -> 2
            }
        }
        else if (selectedCube == clickedCube)
        {
            // Aynı küpe tekrar tıklandı - seçimi kaldır
            DeselectCube();
        }
        else
        {
            // İkinci küp seçildi - yer değiştir
            
            // Tutorial: swap yapıldı
            if (mobileUI != null && mobileUI.IsTutorialActive)
            {
                mobileUI.AdvanceTutorial(); // step 2 -> 3 (veya biter)
            }
            
            SwapCubes(selectedCube, clickedCube);
        }
    }
    
    /// <summary>
    /// İki küpün renklerini değiştirir
    /// </summary>
    private void SwapCubes(Cube cube1, Cube cube2)
    {
        PlaySound(swapSound);
        
        // Renkleri değiştir
        cube1.SwapWith(cube2);
        
        // Seçimi kaldır
        cube1.SetSelected(false);
        selectedCube = null;
        
        // Hamle sayısını artır
        moveCount++;
        
        // Kazanma kontrolü
        StartCoroutine(CheckWinCondition());
    }
    
    /// <summary>
    /// Seçili küpü kaldırır
    /// </summary>
    private void DeselectCube()
    {
        if (selectedCube != null)
        {
            selectedCube.SetSelected(false);
            selectedCube = null;
        }
    }
    
    /// <summary>
    /// Kazanma durumunu kontrol eder
    /// </summary>
    private IEnumerator CheckWinCondition()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (puzzleGenerator.IsPuzzleSolved())
        {
            OnLevelComplete();
        }
        
        UpdateUI();
    }
    
    /// <summary>
    /// Level tamamlandığında çağrılır
    /// </summary>
    private void OnLevelComplete()
    {
        isPlaying = false;
        PlaySound(winSound);
        
        // Kazanma animasyonu
        StartCoroutine(WinAnimation());
    }
    
    /// <summary>
    /// Kazanma animasyonu - küpler küçülerek kaybolur
    /// </summary>
    private IEnumerator WinAnimation()
    {
        isAnimating = true;
        
        yield return new WaitForSeconds(0.3f);
        
        // Küpleri merkeze olan uzaklığa göre sırala (dıştan içe kaybolsun)
        var cubes = new List<Cube>(puzzleGenerator.allCubes);
        Vector3 center = Vector3.zero;
        foreach (var c in cubes)
        {
            if (c != null) center += c.transform.position;
        }
        if (cubes.Count > 0) center /= cubes.Count;
        
        cubes.RemoveAll(c => c == null);
        cubes.Sort((a, b) => {
            if (a == null || b == null) return 0;
            float distA = Vector3.Distance(a.transform.position, center);
            float distB = Vector3.Distance(b.transform.position, center);
            return distB.CompareTo(distA); // Dıştan içe
        });
        
        // Küpleri grup grup küçült (daha hızlı animasyon)
        int batchSize = Mathf.Max(1, cubes.Count / 12);
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] != null)
            {
                StartCoroutine(AnimateCubeScale(cubes[i].transform, Vector3.one * 0.98f, Vector3.one * 0.001f, 0.25f));
            }
            
            if ((i + 1) % batchSize == 0)
            {
                yield return new WaitForSeconds(0.03f);
            }
        }
        
        yield return new WaitForSeconds(0.35f);
        
        isAnimating = false;
        
        // Direkt sonraki bölüme geç
        NextLevel();
    }
    
    /// <summary>
    /// Küplerin spawn animasyonu - küçükten büyüğe sırayla
    /// </summary>
    private IEnumerator SpawnCubesAnimation()
    {
        isAnimating = true;
        
        var cubes = new List<Cube>(puzzleGenerator.allCubes);
        if (cubes.Count == 0)
        {
            isAnimating = false;
            yield break;
        }
        
        // Tüm küpleri başlangıçta görünmez yap
        Vector3 tinyScale = Vector3.one * 0.001f;
        foreach (var cube in cubes)
        {
            if (cube != null)
            {
                cube.transform.localScale = tinyScale;
            }
        }
        
        // Küpleri merkeze olan uzaklığa göre sırala (içten dışa belirsin)
        Vector3 center = Vector3.zero;
        foreach (var c in cubes)
        {
            if (c != null) center += c.transform.position;
        }
        center /= cubes.Count;
        
        cubes.RemoveAll(c => c == null);
        cubes.Sort((a, b) => {
            if (a == null || b == null) return 0;
            float distA = Vector3.Distance(a.transform.position, center);
            float distB = Vector3.Distance(b.transform.position, center);
            return distA.CompareTo(distB); // İçten dışa
        });
        
        // Küpleri grup grup büyüt
        int batchSize = Mathf.Max(1, cubes.Count / 12);
        for (int i = 0; i < cubes.Count; i++)
        {
            if (cubes[i] != null)
            {
                StartCoroutine(AnimateCubeScale(cubes[i].transform, Vector3.one * 0.001f, Vector3.one * 0.98f, 0.2f));
            }
            
            if ((i + 1) % batchSize == 0)
            {
                yield return new WaitForSeconds(0.03f);
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        
        isAnimating = false;
        isPreviewingLevel = true;
        
        // Spawn bittikten sonra tap to start göster
        if (mobileUI != null)
        {
            // Tutorial level ise preview açıklaması göster
            if (currentLevelData != null && currentLevelData.levelName == "Tutorial")
            {
                mobileUI.ShowTutorialPreview();
            }
            mobileUI.ShowTapToStart();
        }
    }
    
    /// <summary>
    /// Küp ölçek animasyonu - elastic overshoot efekti ile
    /// </summary>
    private IEnumerator AnimateCubeScale(Transform cubeTransform, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Elastic ease-out (hafif bounce)
            float ease;
            if (to.magnitude > from.magnitude)
            {
                // Büyüme: overshoot efekti
                ease = 1f - Mathf.Pow(1f - t, 3f) * Mathf.Cos(t * Mathf.PI * 0.5f);
                ease = Mathf.Clamp(ease, 0f, 1.08f); // Hafif overshoot
            }
            else
            {
                // Küçülme: ease-in
                ease = 1f - Mathf.Pow(1f - t, 2f);
                ease = t * t;
            }
            
            if (cubeTransform != null)
            {
                cubeTransform.localScale = Vector3.LerpUnclamped(from, to, ease);
            }
            
            yield return null;
        }
        
        if (cubeTransform != null)
        {
            cubeTransform.localScale = to;
        }
    }
    
    /// <summary>
    /// Sonraki level'a geç
    /// </summary>
    public void NextLevel()
    {
        if (currentLevel < maxLevel)
        {
            StartLevel(currentLevel + 1);
        }
        else
        {
            // Tüm leveller tamamlandı
            StartNewGame();
        }
    }
    
    /// <summary>
    /// Mevcut level'ı yeniden başlat
    /// </summary>
    public void RestartLevel()
    {
        DeselectCube();
        HideSolution(); // Çözüm gösteriliyorsa kapat
        StartLevel(currentLevel);
    }
    
    /// <summary>
    /// Bir doğru hamle yapar - yanlış renkteki bir küpü doğru yerine koyar
    /// </summary>
    public void MakeOneCorrectMove()
    {
        if (!isPlaying) return;
        if (isShowingSolution) return;
        if (puzzleGenerator == null) return;
        
        DeselectCube();
        
        // Yanlış renkteki küpleri bul
        var wrongCubes = puzzleGenerator.allCubes.FindAll(c => c != null && !c.isFixed && c.currentColor != c.targetColor);
        
        if (wrongCubes.Count < 2) return; // En az 2 yanlış küp olmalı (swap için)
        
        // İlk yanlış küpü al
        Cube cube1 = wrongCubes[0];
        
        // cube1'in hedef rengine sahip olan başka bir yanlış küp bul
        Cube cube2 = wrongCubes.Find(c => c != cube1 && c.currentColor == cube1.targetColor);
        
        if (cube2 == null)
        {
            // Direkt eşleşme bulunamadı, herhangi iki yanlış küpü swap et
            cube2 = wrongCubes[1];
        }
        
        // Swap yap
        SwapCubes(cube1, cube2);
        PlaySound(swapSound);
    }

    /// <summary>
    /// Çözümü gösterir - tüm küpler doğru renklerine döner
    /// </summary>
    public void ShowSolution()
    {
        if (isShowingSolution) return;
        if (puzzleGenerator == null) return;
        
        isShowingSolution = true;
        savedColors.Clear();
        
        // Mevcut renkleri kaydet ve hedef renkleri göster
        foreach (Cube cube in puzzleGenerator.allCubes)
        {
            if (cube != null)
            {
                savedColors[cube] = cube.currentColor;
                cube.SetColor(cube.targetColor);
            }
        }
    }
    
    /// <summary>
    /// Çözümü gizler - küpler eski renklerine döner
    /// </summary>
    public void HideSolution()
    {
        if (!isShowingSolution) return;
        
        isShowingSolution = false;
        
        // Kaydedilen renkleri geri yükle
        foreach (var kvp in savedColors)
        {
            if (kvp.Key != null)
            {
                kvp.Key.SetColor(kvp.Value);
            }
        }
        
        savedColors.Clear();
    }
    
    /// <summary>
    /// UI'ı günceller
    /// </summary>
    private void UpdateUI()
    {
        if (mobileUI != null && currentLevelData != null)
        {
            float completion = puzzleGenerator.GetCompletionPercentage();
            mobileUI.UpdateHUD(currentLevel, currentLevelData.levelName, moveCount, completion);
        }
    }
    
    /// <summary>
    /// Ses çalar
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// Puzzle'ı çözer (debug için)
    /// </summary>
    [ContextMenu("Solve Puzzle")]
    public void SolvePuzzle()
    {
        foreach (var cube in puzzleGenerator.allCubes)
        {
            cube.SetColor(cube.targetColor);
        }
        OnLevelComplete();
    }
    
    /// <summary>
    /// Sahnedeki eski küpleri temizler
    /// </summary>
    private void CleanupOldCubes()
    {
        // Sahnedeki tüm Cube objelerini bul
        Cube[] existingCubes = FindObjectsByType<Cube>(FindObjectsSortMode.None);
        
        if (existingCubes.Length > 0)
        {
            Debug.Log($"[GameManager] Found {existingCubes.Length} old cubes in scene, cleaning up...");
            foreach (var cube in existingCubes)
            {
                if (cube != null)
                {
                    Destroy(cube.gameObject);
                }
            }
        }
        
        // Editor preview objelerini de temizle
        CleanupPreviewObjects();
        
        // PuzzleGenerator'ın listesini de temizle
        if (puzzleGenerator != null)
        {
            puzzleGenerator.allCubes.Clear();
            puzzleGenerator.puzzleShape.Clear();
        }
    }
    
    /// <summary>
    /// Editor'dan kalan preview objelerini temizler
    /// </summary>
    private void CleanupPreviewObjects()
    {
        // İsme göre preview objelerini bul ve sil - HideFlags dahil
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int cleanedCount = 0;
        
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.scene.isLoaded && 
                (obj.name.StartsWith("_LevelDesigner_Preview") || 
                 obj.name.StartsWith("Preview_") ||
                 obj.name.StartsWith("PreviewFixed_") ||
                 obj.name.Contains("LevelDesigner")))
            {
                Debug.Log($"[GameManager] Deleting preview object: {obj.name}");
                DestroyImmediate(obj);
                cleanedCount++;
            }
        }
        
        if (cleanedCount > 0)
        {
            Debug.Log($"[GameManager] {cleanedCount} preview objects cleaned up");
        }
    }
    
    /// <summary>
    /// NÜKLEER TEMİZLİK - Play moduna girerken sahnedeki TÜM küpleri siler
    /// </summary>
    private void NuclearCleanup()
    {
        int totalCleaned = 0;
        
        // 1. Tüm Cube component'larını bul ve sil
        Cube[] allCubes = FindObjectsByType<Cube>(FindObjectsSortMode.None);
        Debug.Log($"[GameManager] NuclearCleanup: Found {allCubes.Length} Cubes found");
        
        foreach (var cube in allCubes)
        {
            if (cube != null)
            {
                DestroyImmediate(cube.gameObject);
                totalCleaned++;
            }
        }
        
        // 2. Preview parent objelerini sil
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj != null && obj.scene.isLoaded)
            {
                // Preview objeleri
                if (obj.name.StartsWith("_LevelDesigner_Preview") || 
                    obj.name.StartsWith("Preview_") ||
                    obj.name.StartsWith("PreviewFixed_") ||
                    obj.name.Contains("LevelDesigner"))
                {
                    DestroyImmediate(obj);
                    totalCleaned++;
                }
                
                // Cube isimli objeler (prefab'tan spawn olmuş olabilir)
                if (obj.name == "Cube" || obj.name.StartsWith("Cube("))
                {
                    // Sadece Cube component'ı varsa sil
                    if (obj.GetComponent<Cube>() != null || obj.GetComponent<MeshRenderer>() != null)
                    {
                        DestroyImmediate(obj);
                        totalCleaned++;
                    }
                }
            }
        }
        
        // 3. PuzzleGenerator altındaki tüm child'ları temizle
        if (PuzzleGenerator.Instance != null)
        {
            for (int i = PuzzleGenerator.Instance.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(PuzzleGenerator.Instance.transform.GetChild(i).gameObject);
                totalCleaned++;
            }
            PuzzleGenerator.Instance.allCubes.Clear();
            PuzzleGenerator.Instance.puzzleShape.Clear();
        }
        
        Debug.Log($"[GameManager] NuclearCleanup complete: {totalCleaned} objects deleted");
    }
    
    /// <summary>
    /// Renkleri yeniden karıştırır
    /// </summary>
    [ContextMenu("Shuffle Colors")]
    public void ReshuffleColors()
    {
        DeselectCube();
        puzzleGenerator.ShuffleColors();
        moveCount = 0;
        UpdateUI();
    }
}
