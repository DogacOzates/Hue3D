using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using System.Collections;

/// <summary>
/// EYKA tarzı minimal mobil UI yönetimi - pastel tonlarda temiz tasarım
/// </summary>
public class MobileUIManager : MonoBehaviour
{
    public static MobileUIManager Instance { get; private set; }
    
    [Header("References")]
    public Canvas mainCanvas;
    
    [Header("HUD Elements - Minimal")]
    public Text levelText;
    public Image progressFill;
    
    [Header("Bottom Icons - EYKA Style")]
    public Button statsButton;      // İstatistik ikonu (sol)
    public Button menuButton;       // Menü ikonu (orta)
    public Button hintButton;       // İpucu ikonu (sağ)
    
    [Header("Win Panel")]
    public GameObject winPanel;
    public Text winTitleText;
    public Text winStatsText;
    
    [Header("Level Select")]
    public GameObject levelSelectPanel;
    public Button levelSelectButton;
    
    [Header("Theme")]
    public int currentTheme = 0;
    private Color themeAccentColor;
    
    [Header("Settings")]
    public float safeAreaPadding = 20f;
    
    private RectTransform canvasRect;
    private bool isInitialized;
    private bool isShowingSolution;
    private GameObject tapToStartOverlay;
    private Text tapToStartText;
    private Coroutine tapToStartPulse;
    
    // Tutorial
    private GameObject tutorialOverlay;
    private Text tutorialText;
    private int tutorialStep; // 0=not tutorial, 1=select first, 2=select second, 3=done
    public bool IsTutorialActive => tutorialStep > 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        CreateMobileUI();
        isInitialized = true;
    }
    
    /// <summary>
    /// EYKA tarzı minimal UI oluşturur
    /// </summary>
    private void CreateMobileUI()
    {
        // Canvas oluştur
        GameObject canvasObj = new GameObject("MobileUI Canvas");
        canvasObj.transform.SetParent(transform);
        
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        canvasRect = canvasObj.GetComponent<RectTransform>();
        
        // EventSystem oluştur
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<InputSystemUIInputModule>();
        }
        
        // Safe area container
        GameObject safeArea = CreateSafeAreaContainer(canvasObj.transform);
        
        // EYKA tarzı minimal üst HUD
        CreateMinimalTopHUD(safeArea.transform);
        
        // EYKA tarzı alt ikon bar
        CreateBottomIconBar(safeArea.transform);
        
        // Win Panel
        CreateWinPanel(canvasObj.transform);
        
        // Başlangıç temasını uygula
        UpdateUITheme(0);
    }
    
    /// <summary>
    /// Safe area container oluşturur
    /// </summary>
    private GameObject CreateSafeAreaContainer(Transform parent)
    {
        GameObject safeArea = new GameObject("SafeArea");
        safeArea.transform.SetParent(parent, false);
        
        RectTransform rect = safeArea.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        SafeAreaFitter fitter = safeArea.AddComponent<SafeAreaFitter>();
        
        return safeArea;
    }
    
    /// <summary>
    /// Üst HUD - EYKA: Sadece level numarası, çok küçük ve şeffaf (tıklanabilir)
    /// </summary>
    private void CreateMinimalTopHUD(Transform parent)
    {
        // Üst bar - tamamen şeffaf, sadece level numarası
        GameObject topBar = new GameObject("TopBar");
        topBar.transform.SetParent(parent, false);
        
        RectTransform topRect = topBar.AddComponent<RectTransform>();
        topRect.anchorMin = new Vector2(0, 1);
        topRect.anchorMax = new Vector2(1, 1);
        topRect.pivot = new Vector2(0.5f, 1);
        topRect.anchoredPosition = new Vector2(0, -20);
        topRect.sizeDelta = new Vector2(0, 50);
        
        // Level numarası - ortada, çok küçük ve ince
        GameObject levelBtnObj = new GameObject("LevelButton");
        levelBtnObj.transform.SetParent(topBar.transform, false);
        
        RectTransform levelBtnRect = levelBtnObj.AddComponent<RectTransform>();
        levelBtnRect.anchorMin = new Vector2(0.5f, 0.5f);
        levelBtnRect.anchorMax = new Vector2(0.5f, 0.5f);
        levelBtnRect.pivot = new Vector2(0.5f, 0.5f);
        levelBtnRect.anchoredPosition = Vector2.zero;
        levelBtnRect.sizeDelta = new Vector2(280, 70);
        
        levelText = levelBtnObj.AddComponent<Text>();
        levelText.text = "";
        levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        levelText.fontSize = 48;
        levelText.color = new Color(1f, 1f, 1f, 0.5f);
        levelText.alignment = TextAnchor.MiddleCenter;
        levelText.fontStyle = FontStyle.Normal;
        
        // Tıklanabilir yap
        levelSelectButton = levelBtnObj.AddComponent<Button>();
        levelSelectButton.targetGraphic = null;
        levelSelectButton.onClick.AddListener(ToggleLevelSelectPanel);
        
        // Level seçim paneli oluştur (başlangıçta gizli)
        CreateLevelSelectPanel(parent);
    }
    
    /// <summary>
    /// Level seçim panelini oluşturur
    /// </summary>
    private void CreateLevelSelectPanel(Transform parent)
    {
        try
        {
            // Ana panel - ekranı kaplar
            levelSelectPanel = new GameObject("LevelSelectPanel");
            levelSelectPanel.transform.SetParent(parent, false);
            
            RectTransform panelRect = levelSelectPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Yarı saydam arka plan - EYKA tarzı frosted overlay
            Image bgImage = levelSelectPanel.AddComponent<Image>();
            bgImage.color = new Color(0.92f, 0.90f, 0.95f, 0.97f);
            
            // Kapatma butonu - sağ üst
            GameObject closeBtn = new GameObject("CloseButton");
            closeBtn.transform.SetParent(levelSelectPanel.transform, false);
            
            RectTransform closeRect = closeBtn.AddComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(1, 1);
            closeRect.anchorMax = new Vector2(1, 1);
            closeRect.pivot = new Vector2(1, 1);
            closeRect.anchoredPosition = new Vector2(-30, -50);
            closeRect.sizeDelta = new Vector2(80, 80);
            
            Image closeBg = closeBtn.AddComponent<Image>();
            closeBg.color = new Color(0.85f, 0.75f, 0.80f, 0.8f);  // EYKA soft rose close button
            
            GameObject closeTextObj = new GameObject("CloseText");
            closeTextObj.transform.SetParent(closeBtn.transform, false);
            RectTransform closeTextRect = closeTextObj.AddComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            
            Text closeText = closeTextObj.AddComponent<Text>();
            closeText.text = "X";
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = 40;
            closeText.color = Color.white;
            closeText.alignment = TextAnchor.MiddleCenter;
            closeText.fontStyle = FontStyle.Bold;
            
            Button closeBtnComp = closeBtn.AddComponent<Button>();
            closeBtnComp.targetGraphic = closeBg;
            closeBtnComp.onClick.AddListener(() => levelSelectPanel.SetActive(false));
            
            // Viewport - maskeleme alanı
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(levelSelectPanel.transform, false);
            
            RectTransform viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = new Vector2(0.05f, 0.08f);
            viewportRect.anchorMax = new Vector2(0.95f, 0.88f);
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.anchoredPosition = Vector2.zero;
            
            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(0, 0, 0, 0.01f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            
            // Content - grid butonları burada
            GameObject gridContainer = new GameObject("Content");
            gridContainer.transform.SetParent(viewport.transform, false);
            
            RectTransform gridRect = gridContainer.AddComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0, 1);
            gridRect.anchorMax = new Vector2(1, 1);
            gridRect.pivot = new Vector2(0.5f, 1);
            gridRect.anchoredPosition = Vector2.zero;
            gridRect.sizeDelta = new Vector2(0, 0);
            
            ContentSizeFitter fitter = gridContainer.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            // GridLayoutGroup ile level butonları
            GridLayoutGroup grid = gridContainer.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(80, 80);
            grid.spacing = new Vector2(12, 12);
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.padding = new RectOffset(10, 10, 10, 10);
            
            // ScrollRect - viewport üstünde değil, panel üstünde
            ScrollRect scroll = levelSelectPanel.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.viewport = viewportRect;
            scroll.content = gridRect;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.elasticity = 0.1f;
            scroll.scrollSensitivity = 30f;
            
            // Level butonlarını oluştur
            int totalLevels = 100;
            try { totalLevels = LevelManager.TotalLevels > 0 ? LevelManager.TotalLevels : 100; } catch { }
            
            for (int i = 1; i <= totalLevels; i++)
            {
                CreateSimpleLevelButton(gridContainer.transform, i);
            }
            
            // Layout'u zorla güncelle
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRect);
            
            // Başlangıçta gizli
            levelSelectPanel.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Level select panel creation error: {e.Message}");
        }
    }
    
    /// <summary>
    /// Basit level butonu oluşturur
    /// </summary>
    private void CreateSimpleLevelButton(Transform parent, int levelNumber)
    {
        GameObject btnObj = new GameObject($"Level_{levelNumber}");
        btnObj.transform.SetParent(parent, false);
        
        // Arka plan - EYKA pastel
        Image btnBg = btnObj.AddComponent<Image>();
        btnBg.color = new Color(0.82f, 0.78f, 0.90f, 0.4f);
        
        // Level numarası
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text levelNum = textObj.AddComponent<Text>();
        levelNum.text = levelNumber.ToString();
        levelNum.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        levelNum.fontSize = 28;
        levelNum.color = new Color(0.45f, 0.38f, 0.55f, 0.9f);  // EYKA soft purple text
        levelNum.alignment = TextAnchor.MiddleCenter;
        levelNum.fontStyle = FontStyle.Normal;  // İnce, bold değil
        
        // Tıklanabilir
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnBg;
        
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.82f, 0.78f, 0.90f, 0.4f);
        colors.highlightedColor = new Color(0.82f, 0.78f, 0.90f, 0.6f);
        colors.pressedColor = new Color(0.82f, 0.78f, 0.90f, 0.8f);
        btn.colors = colors;
        
        int level = levelNumber;
        btn.onClick.AddListener(() => {
            GameManager.Instance?.StartLevel(level);
            levelSelectPanel.SetActive(false);
        });
    }
    
    /// <summary>
    /// Level seçim panelini aç/kapat
    /// </summary>
    private void ToggleLevelSelectPanel()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(!levelSelectPanel.activeSelf);
        }
    }
    
    /// <summary>
    /// EYKA tarzı accent renk (altın/sarı)
    /// </summary>
    private Color GetAccentColor()
    {
        return Color.white;  // Beyaz
    }
    
    /// <summary>
    /// EYKA tarzı ince progress bar
    /// </summary>
    private GameObject CreateMinimalProgressBar(Transform parent)
    {
        // Progress bar kullanmıyoruz EYKA tarzında
        return null;
    }
    
    /// <summary>
    /// Alt buton barı - EYKA tarzı: 3 ince ikon, eşit aralıklı, tema rengiyle
    /// Sol: bar chart (restart), Orta: hamburger menü (hint), Sağ: daire (solution)
    /// </summary>
    private void CreateBottomIconBar(Transform parent)
    {
        // Alt ikon container - ekranın altına tam yayılmış
        GameObject bottomBar = new GameObject("BottomIconBar");
        bottomBar.transform.SetParent(parent, false);
        
        RectTransform barRect = bottomBar.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.05f, 0);
        barRect.anchorMax = new Vector2(0.95f, 0);
        barRect.pivot = new Vector2(0.5f, 0);
        barRect.anchoredPosition = new Vector2(0, 25);
        barRect.sizeDelta = new Vector2(0, 90);
        
        HorizontalLayoutGroup hLayout = bottomBar.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 40;
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = false;
        hLayout.padding = new RectOffset(40, 40, 0, 0);
        
        // Sol ikon - Bar chart (restart) - EYKA tarzı ince çizgiler
        GameObject leftIcon = CreateEykaIcon(bottomBar.transform, "RestartIcon", EykaIconType.BarChart);
        statsButton = leftIcon.GetComponent<Button>();
        statsButton.onClick.AddListener(() => GameManager.Instance?.RestartLevel());
        
        // Orta ikon - Hamburger menü (bir doğru hamle)
        GameObject centerIcon = CreateEykaIcon(bottomBar.transform, "HintMoveIcon", EykaIconType.HamburgerMenu);
        menuButton = centerIcon.GetComponent<Button>();
        menuButton.onClick.AddListener(() => GameManager.Instance?.MakeOneCorrectMove());
        
        // Sağ ikon - Daire (çözümü göster)
        GameObject rightIcon = CreateEykaIcon(bottomBar.transform, "SolutionIcon", EykaIconType.Circle);
        hintButton = rightIcon.GetComponent<Button>();
        hintButton.onClick.AddListener(OnHintButtonClicked);
    }
    
    /// <summary>
    /// EYKA ikon tipleri
    /// </summary>
    private enum EykaIconType
    {
        BarChart,       // ||| - 3 dikey çizgi (farklı boyda)
        HamburgerMenu,  // ☰  - 3 yatay çizgi
        Circle          // ○  - İnce daire
    }
    
    /// <summary>
    /// EYKA tarzı ince çizgi ikon oluşturur (sprite yerine procedural)
    /// </summary>
    private GameObject CreateEykaIcon(Transform parent, string name, EykaIconType iconType)
    {
        // Ana container
        GameObject iconObj = new GameObject(name);
        iconObj.transform.SetParent(parent, false);
        
        RectTransform rect = iconObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(88, 78);
        
        // Şeffaf raycast target
        Image bgImage = iconObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0);  // Tamamen şeffaf arka plan
        bgImage.raycastTarget = true;
        
        // Layout element
        LayoutElement layout = iconObj.AddComponent<LayoutElement>();
        layout.preferredWidth = 80;
        layout.preferredHeight = 70;
        
        // İkon texture'ı oluştur - BEYAZ çizgi, Image.color ile renklendirilecek
        Texture2D iconTex = CreateEykaIconTexture(iconType, 128, new Color(1f, 1f, 1f, 1f));
        
        // İkon görseli (child)
        GameObject iconVisual = new GameObject("Icon");
        iconVisual.transform.SetParent(iconObj.transform, false);
        
        RectTransform iconRect = iconVisual.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(72, 72);  // Büyük ikon
        
        Image iconImage = iconVisual.AddComponent<Image>();
        Sprite iconSprite = Sprite.Create(iconTex, new Rect(0, 0, iconTex.width, iconTex.height), new Vector2(0.5f, 0.5f), 100f);
        iconImage.sprite = iconSprite;
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;
        
        // Tıklanabilir yap
        Button btn = iconObj.AddComponent<Button>();
        btn.targetGraphic = bgImage;
        
        // Hover/press renkleri - çok hafif
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0, 0, 0, 0);
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.05f);
        colors.pressedColor = new Color(1f, 1f, 1f, 0.1f);
        btn.colors = colors;
        
        return iconObj;
    }
    
    /// <summary>
    /// EYKA tarzı ince çizgi ikon texture'ı oluşturur
    /// </summary>
    private Texture2D CreateEykaIconTexture(EykaIconType type, int size, Color lineColor)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = new Color(0, 0, 0, 0);
        
        // Tüm pikselleri temizle
        Color[] clearPixels = new Color[size * size];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = clear;
        tex.SetPixels(clearPixels);
        
        float lineWidth = 4f;  // Daha kalın çizgi
        float center = size / 2f;
        float margin = size * 0.2f;
        float top = size - margin;
        float bottom = margin;
        float left = margin;
        float right = size - margin;
        
        switch (type)
        {
            case EykaIconType.BarChart:
                // ||| - 3 dikey çizgi, farklı yüksekliklerde
                float barSpacing = (right - left) / 4f;
                DrawLine(tex, size, left + barSpacing, bottom + (top-bottom)*0.25f, left + barSpacing, top, lineWidth, lineColor);
                DrawLine(tex, size, center, bottom, center, top, lineWidth, lineColor);
                DrawLine(tex, size, right - barSpacing, bottom + (top-bottom)*0.4f, right - barSpacing, top, lineWidth, lineColor);
                break;
                
            case EykaIconType.HamburgerMenu:
                // ☰ - 3 yatay çizgi
                float lineSpacing = (top - bottom) / 4f;
                DrawLine(tex, size, left, bottom + lineSpacing, right, bottom + lineSpacing, lineWidth, lineColor);
                DrawLine(tex, size, left, center, right, center, lineWidth, lineColor);
                DrawLine(tex, size, left, top - lineSpacing, right, top - lineSpacing, lineWidth, lineColor);
                break;
                
            case EykaIconType.Circle:
                // ○ - Daire
                float radius = (right - left) / 2f * 0.85f;
                DrawCircle(tex, size, center, center, radius, lineWidth, lineColor);
                break;
        }
        
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        return tex;
    }
    
    /// <summary>
    /// Texture üzerine anti-aliased çizgi çizer
    /// </summary>
    private void DrawLine(Texture2D tex, int texSize, float x1, float y1, float x2, float y2, float width, Color color)
    {
        float length = Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        float dx = (x2 - x1) / length;
        float dy = (y2 - y1) / length;
        
        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                // Noktanın çizgiye mesafesini hesapla
                float px = x - x1;
                float py = y - y1;
                float dot = px * dx + py * dy;
                dot = Mathf.Clamp(dot, 0, length);
                
                float closestX = x1 + dot * dx;
                float closestY = y1 + dot * dy;
                
                float dist = Mathf.Sqrt((x - closestX) * (x - closestX) + (y - closestY) * (y - closestY));
                
                if (dist < width + 1f)
                {
                    float alpha = Mathf.Clamp01(1f - (dist - width + 0.5f));
                    Color existing = tex.GetPixel(x, y);
                    Color blended = new Color(color.r, color.g, color.b, Mathf.Max(existing.a, color.a * alpha));
                    tex.SetPixel(x, y, blended);
                }
            }
        }
    }
    
    /// <summary>
    /// Texture üzerine anti-aliased daire çizer
    /// </summary>
    private void DrawCircle(Texture2D tex, int texSize, float cx, float cy, float radius, float width, Color color)
    {
        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                float ringDist = Mathf.Abs(dist - radius);
                
                if (ringDist < width + 1f)
                {
                    float alpha = Mathf.Clamp01(1f - (ringDist - width * 0.5f + 0.5f));
                    Color existing = tex.GetPixel(x, y);
                    Color blended = new Color(color.r, color.g, color.b, Mathf.Max(existing.a, color.a * alpha));
                    tex.SetPixel(x, y, blended);
                }
            }
        }
    }
    
    private void OnStatsClicked()
    {
        // İstatistik göster (gelecekte implement edilebilir)
        Debug.Log("Stats clicked");
    }
    
    private void OnMenuClicked()
    {
        // Menü göster (gelecekte implement edilebilir)
        Debug.Log("Menu clicked");
    }
    
    /// <summary>
    /// Hint butonuna tıklandığında - EYKA tarzı
    /// </summary>
    private void OnHintButtonClicked()
    {
        // Preview modunda hint kullanılamaz
        if (GameManager.Instance != null && GameManager.Instance.isPreviewingLevel) return;
        
        Transform iconChild = hintButton?.transform.Find("Icon");
        Image hintImage = iconChild?.GetComponent<Image>();
        
        if (!isShowingSolution)
        {
            // Çözümü göster
            GameManager.Instance?.ShowSolution();
            isShowingSolution = true;
            if (hintImage != null)
            {
                hintImage.color = new Color(0.5f, 0.9f, 0.6f, 0.9f);  // Aktif yeşil tint
            }
        }
        else
        {
            // Oyuna devam et
            GameManager.Instance?.HideSolution();
            isShowingSolution = false;
            // İkon rengini tema rengine döndür
            UpdateIconColors();
        }
    }
    
    /// <summary>
    /// Hint butonunu resetle
    /// </summary>
    public void ResetHintButton()
    {
        isShowingSolution = false;
        UpdateIconColors();
    }
    
    /// <summary>
    /// "Tap to start" overlay'ını göster
    /// </summary>
    public void ShowTapToStart()
    {
        if (tapToStartOverlay == null)
        {
            CreateTapToStartOverlay();
        }
        tapToStartOverlay.SetActive(true);
        
        // Pulse animation
        if (tapToStartPulse != null) StopCoroutine(tapToStartPulse);
        tapToStartPulse = StartCoroutine(PulseTapToStartText());
    }
    
    /// <summary>
    /// "Tap to start" overlay'ını gizle
    /// </summary>
    public void HideTapToStart()
    {
        if (tapToStartPulse != null)
        {
            StopCoroutine(tapToStartPulse);
            tapToStartPulse = null;
        }
        if (tapToStartOverlay != null)
        {
            tapToStartOverlay.SetActive(false);
        }
    }
    
    /// <summary>
    /// Tap to start overlay oluştur
    /// </summary>
    private void CreateTapToStartOverlay()
    {
        tapToStartOverlay = new GameObject("TapToStartOverlay");
        tapToStartOverlay.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform rect = tapToStartOverlay.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // Saydam - raycast yakalamaz, sadece text gösterir
        // (GameManager zaten tap algılıyor)
        
        // Ekranın ortasına yakın, büyük text
        GameObject textObj = new GameObject("TapText");
        textObj.transform.SetParent(tapToStartOverlay.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0, 420);
        textRect.sizeDelta = new Vector2(700, 100);
        
        tapToStartText = textObj.AddComponent<Text>();
        tapToStartText.text = "tap to start";
        tapToStartText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tapToStartText.fontSize = 60;
        tapToStartText.fontStyle = FontStyle.Normal;
        tapToStartText.alignment = TextAnchor.MiddleCenter;
        tapToStartText.color = new Color(0.55f, 0.45f, 0.62f, 0.85f);
        tapToStartText.horizontalOverflow = HorizontalWrapMode.Overflow;
        tapToStartText.verticalOverflow = VerticalWrapMode.Overflow;
    }
    
    /// <summary>
    /// Tap to start text pulse effect
    /// </summary>
    private IEnumerator PulseTapToStartText()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime * 1.5f;
            float alpha = Mathf.Lerp(0.5f, 0.9f, (Mathf.Sin(t) + 1f) * 0.5f);
            if (tapToStartText != null)
            {
                tapToStartText.color = new Color(
                    tapToStartText.color.r,
                    tapToStartText.color.g,
                    tapToStartText.color.b,
                    alpha);
            }
            yield return null;
        }
    }
    
    // ========================================
    // TUTORIAL
    // ========================================
    
    /// <summary>
    /// Tutorial preview açıklaması - solved state'de gösterilir
    /// </summary>
    public void ShowTutorialPreview()
    {
        tutorialStep = 0; // henüz oyun başlamadı
        
        if (tutorialOverlay == null)
        {
            CreateTutorialOverlay();
        }
        tutorialOverlay.SetActive(true);
        tutorialText.text = "This is the solved state\nRemember the colors!";
        StartCoroutine(FadeTutorialText());
    }
    
    /// <summary>
    /// Tutorial overlay'ını başlat
    /// </summary>
    public void ShowTutorial()
    {
        tutorialStep = 1;
        
        if (tutorialOverlay == null)
        {
            CreateTutorialOverlay();
        }
        tutorialOverlay.SetActive(true);
        UpdateTutorialText();
    }
    
    /// <summary>
    /// Tutorial gizle
    /// </summary>
    public void HideTutorial()
    {
        tutorialStep = 0;
        if (tutorialOverlay != null)
        {
            tutorialOverlay.SetActive(false);
        }
    }
    
    /// <summary>
    /// Tutorial adımını ilerlet
    /// </summary>
    public void AdvanceTutorial()
    {
        tutorialStep++;
        if (tutorialStep > 3)
        {
            HideTutorial();
            return;
        }
        UpdateTutorialText();
    }
    
    /// <summary>
    /// Tutorial step'ine göre text güncelle
    /// </summary>
    private void UpdateTutorialText()
    {
        if (tutorialText == null) return;
        
        switch (tutorialStep)
        {
            case 1:
                tutorialText.text = "Tap a colored cube\nto select it";
                break;
            case 2:
                tutorialText.text = "Now tap another cube\nto swap their colors";
                break;
            case 3:
                tutorialText.text = "Match all colors\nto their correct positions!";
                break;
        }
        
        // Fade in effect
        StartCoroutine(FadeTutorialText());
    }
    
    /// <summary>
    /// Tutorial overlay oluştur
    /// </summary>
    private void CreateTutorialOverlay()
    {
        tutorialOverlay = new GameObject("TutorialOverlay");
        tutorialOverlay.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform rect = tutorialOverlay.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        // Alt kısımda tutorial text
        GameObject textObj = new GameObject("TutorialText");
        textObj.transform.SetParent(tutorialOverlay.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0f);
        textRect.anchorMax = new Vector2(0.5f, 0f);
        textRect.pivot = new Vector2(0.5f, 0f);
        textRect.anchoredPosition = new Vector2(0, 450);
        textRect.sizeDelta = new Vector2(600, 100);
        
        tutorialText = textObj.AddComponent<Text>();
        tutorialText.text = "";
        tutorialText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        tutorialText.fontSize = 38;
        tutorialText.fontStyle = FontStyle.Normal;
        tutorialText.alignment = TextAnchor.MiddleCenter;
        tutorialText.color = new Color(0.55f, 0.45f, 0.62f, 0.85f);
        tutorialText.lineSpacing = 1.2f;
        tutorialText.horizontalOverflow = HorizontalWrapMode.Overflow;
        tutorialText.verticalOverflow = VerticalWrapMode.Overflow;
    }
    
    /// <summary>
    /// Tutorial text fade in
    /// </summary>
    private IEnumerator FadeTutorialText()
    {
        if (tutorialText == null) yield break;
        
        Color c = tutorialText.color;
        tutorialText.color = new Color(c.r, c.g, c.b, 0f);
        
        float elapsed = 0f;
        while (elapsed < 0.4f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 0.85f, elapsed / 0.4f);
            if (tutorialText != null)
                tutorialText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }
    
    /// <summary>
    /// UI temasını günceller - EYKA tarzı: ikonlar ve text tema rengiyle uyumlu
    /// </summary>
    public void UpdateUITheme(int themeIndex)
    {
        currentTheme = themeIndex % ColorPalettes.AllPalettes.Length;
        
        // Tema accent rengi al - EYKA: gradient'in alt rengi (pastel ton)
        var (_, bottomColor) = ColorPalettes.GetBackgroundGradient(currentTheme);
        themeAccentColor = bottomColor;
        
        // Progress bar rengini güncelle
        if (progressFill != null)
        {
            Color progressColor = themeAccentColor;
            progressColor.a = 0.6f;
            progressFill.color = progressColor;
        }
        
        // İkon ve text renklerini tema ile uyumlu güncelle
        UpdateIconColors();
    }
    
    /// <summary>
    /// İkon renklerini tema ile uyumlu günceller - EYKA tarzı: ikonlar arka plan tonu
    /// </summary>
    private void UpdateIconColors()
    {
        // EYKA tarzı: İkon rengi gradient alt renginin belirgin koyu tonu
        Color.RGBToHSV(themeAccentColor, out float h, out float s, out float v);
        Color iconColor = Color.HSVToRGB(h, Mathf.Clamp(s * 2f, 0.15f, 0.6f), Mathf.Clamp(v * 0.65f, 0.35f, 0.7f));
        iconColor.a = 1f;
        
        // Level text'ini de tema rengiyle güncelle
        if (levelText != null)
        {
            levelText.color = new Color(iconColor.r, iconColor.g, iconColor.b, 0.65f);
        }
        
        // İkonların child Image'larını güncelle
        UpdateIconChildColor(statsButton, iconColor);
        UpdateIconChildColor(menuButton, iconColor);
        if (!isShowingSolution)
        {
            UpdateIconChildColor(hintButton, iconColor);
        }
    }
    
    /// <summary>
    /// Bir butonun child ikon Image'ını renklendirir
    /// </summary>
    private void UpdateIconChildColor(Button button, Color color)
    {
        if (button == null) return;
        
        // Child'daki "Icon" objesini bul
        Transform iconChild = button.transform.Find("Icon");
        if (iconChild != null)
        {
            Image img = iconChild.GetComponent<Image>();
            if (img != null) img.color = color;
        }
    }
    
    /// <summary>
    /// Progress bar oluşturur (eski - kullanılmıyor)
    /// </summary>
    private GameObject CreateProgressBar(Transform parent)
    {
        GameObject progressObj = new GameObject("ProgressBar");
        progressObj.transform.SetParent(parent, false);
        
        RectTransform rect = progressObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 20);
        
        LayoutElement layout = progressObj.AddComponent<LayoutElement>();
        layout.flexibleWidth = 2;
        layout.preferredHeight = 20;
        
        // Background
        Image bgImage = progressObj.AddComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0.2f);
        
        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(progressObj.transform, false);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0, 1); // Başlangıçta 0 genişlik
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        progressFill = fillObj.AddComponent<Image>();
        progressFill.color = new Color(0.4f, 0.9f, 0.6f, 0.9f);
        
        return progressObj;
    }
    
    /// <summary>
    /// Win Panel oluşturur - Professional design
    /// </summary>
    private void CreateWinPanel(Transform parent)
    {
        Color accentColor = new Color(0.55f, 0.45f, 0.62f);       // purple theme
        Color accentLight = new Color(0.55f, 0.45f, 0.62f, 0.12f); // subtle bg
        
        // Full screen overlay
        winPanel = new GameObject("WinPanel");
        winPanel.transform.SetParent(parent, false);
        
        RectTransform panelRect = winPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelBg = winPanel.AddComponent<Image>();
        panelBg.color = new Color(0.94f, 0.91f, 0.96f, 0.94f);
        panelBg.raycastTarget = true;
        
        // Center container
        GameObject innerContainer = new GameObject("Container");
        innerContainer.transform.SetParent(winPanel.transform, false);
        
        RectTransform innerRect = innerContainer.AddComponent<RectTransform>();
        innerRect.anchorMin = new Vector2(0.5f, 0.5f);
        innerRect.anchorMax = new Vector2(0.5f, 0.5f);
        innerRect.pivot = new Vector2(0.5f, 0.5f);
        innerRect.sizeDelta = new Vector2(320, 340);
        
        // --- Checkmark circle background ---
        GameObject checkCircle = new GameObject("CheckCircle");
        checkCircle.transform.SetParent(innerContainer.transform, false);
        RectTransform circleRect = checkCircle.AddComponent<RectTransform>();
        circleRect.anchorMin = new Vector2(0.5f, 1f);
        circleRect.anchorMax = new Vector2(0.5f, 1f);
        circleRect.pivot = new Vector2(0.5f, 0.5f);
        circleRect.anchoredPosition = new Vector2(0, -56);
        circleRect.sizeDelta = new Vector2(88, 88);
        
        Image circleImg = checkCircle.AddComponent<Image>();
        Texture2D circleTex = CreateSmoothCircleTexture(128, Color.white);
        circleImg.sprite = Sprite.Create(circleTex, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        circleImg.color = accentLight;
        
        // Checkmark text inside circle
        GameObject checkObj = new GameObject("Checkmark");
        checkObj.transform.SetParent(checkCircle.transform, false);
        RectTransform checkRect = checkObj.AddComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;
        
        Text checkText = checkObj.AddComponent<Text>();
        checkText.text = "\u2713";
        checkText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        checkText.fontSize = 44;
        checkText.color = accentColor;
        checkText.alignment = TextAnchor.MiddleCenter;
        
        // --- Level info text ---
        GameObject statsObj = new GameObject("StatsText");
        statsObj.transform.SetParent(innerContainer.transform, false);
        RectTransform statsRect = statsObj.AddComponent<RectTransform>();
        statsRect.anchorMin = new Vector2(0.5f, 1f);
        statsRect.anchorMax = new Vector2(0.5f, 1f);
        statsRect.pivot = new Vector2(0.5f, 1f);
        statsRect.anchoredPosition = new Vector2(0, -115);
        statsRect.sizeDelta = new Vector2(300, 70);
        
        winStatsText = statsObj.AddComponent<Text>();
        winStatsText.text = "Level 1\n4 Moves";
        winStatsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        winStatsText.fontSize = 22;
        winStatsText.lineSpacing = 1.3f;
        winStatsText.color = new Color(0.48f, 0.40f, 0.55f);
        winStatsText.alignment = TextAnchor.MiddleCenter;
        winStatsText.horizontalOverflow = HorizontalWrapMode.Overflow;
        winStatsText.verticalOverflow = VerticalWrapMode.Overflow;
        
        // --- Hidden title (used by ShowWinPanel) ---
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(innerContainer.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = Vector2.zero;
        winTitleText = titleObj.AddComponent<Text>();
        winTitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        winTitleText.fontSize = 1;
        winTitleText.color = Color.clear;
        
        // Make entire panel clickable -> next level
        Button panelButton = winPanel.AddComponent<Button>();
        panelButton.targetGraphic = panelBg;
        ColorBlock panelColors = panelButton.colors;
        panelColors.normalColor = Color.white;
        panelColors.highlightedColor = Color.white;
        panelColors.pressedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
        panelColors.fadeDuration = 0.1f;
        panelButton.colors = panelColors;
        panelButton.onClick.AddListener(() => {
            HideWinPanel();
            GameManager.Instance?.NextLevel();
        });
        
        winPanel.SetActive(false);
    }
    
    /// <summary>
    /// Creates a smooth anti-aliased circle texture
    /// </summary>
    private Texture2D CreateSmoothCircleTexture(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float radius = size * 0.5f;
        float edge = 1.5f; // anti-alias edge width
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(radius, radius));
                float alpha = Mathf.Clamp01((radius - dist) / edge);
                tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return tex;
    }
    
    /// <summary>
    /// Text objesi oluşturur
    /// </summary>
    private GameObject CreateTextObject(string text, Transform parent, int fontSize, TextAnchor anchor)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, fontSize + 20);
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.fontSize = fontSize;
        textComp.color = new Color(0.45f, 0.38f, 0.55f);  // EYKA soft purple text
        textComp.alignment = anchor;
        textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
        textComp.verticalOverflow = VerticalWrapMode.Overflow;
        
        // Layout element ekle
        LayoutElement layout = textObj.AddComponent<LayoutElement>();
        layout.flexibleWidth = 1;
        layout.preferredHeight = fontSize + 20;
        
        return textObj;
    }
    
    /// <summary>
    /// Button oluşturur
    /// </summary>
    private Button CreateButton(string text, Transform parent, Color bgColor)
    {
        GameObject buttonObj = new GameObject("Button_" + text);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 60);
        
        Image bg = buttonObj.AddComponent<Image>();
        bg.color = bgColor;
        bg.raycastTarget = true;
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = bg;
        button.interactable = true;
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.fontSize = 24;  // EYKA: daha küçük font
        textComp.color = new Color(0.40f, 0.35f, 0.50f);  // EYKA soft purple
        textComp.alignment = TextAnchor.MiddleCenter;
        
        // Layout element
        LayoutElement layout = buttonObj.AddComponent<LayoutElement>();
        layout.preferredHeight = 60;
        layout.preferredWidth = 300;
        
        return button;
    }
    
    /// <summary>
    /// HUD'ı günceller - EYKA tarzı ultra minimal
    /// </summary>
    public void UpdateHUD(int level, string levelName, int moves, float progress)
    {
        if (!isInitialized) return;
        
        // EYKA: Sadece level numarası, çok küçük
        if (levelText != null)
            levelText.text = $"{level}";  // Sadece numara, "Level" yazmaya gerek yok
        
        if (progressFill != null)
        {
            RectTransform fillRect = progressFill.GetComponent<RectTransform>();
            fillRect.anchorMax = new Vector2(progress / 100f, 1);
        }
    }
    
    /// <summary>
    /// Win panel gösterir
    /// </summary>
    public void ShowWinPanel(int level, string levelName, int moves)
    {
        if (winPanel == null) return;
        
        winPanel.SetActive(true);
        
        if (winStatsText != null)
            winStatsText.text = $"Level {level} \u2022 {levelName}\n{moves} Moves";
        
        StartCoroutine(AnimateWinPanel());
    }
    
    /// <summary>
    /// Win panel gizler
    /// </summary>
    public void HideWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }
    
    /// <summary>
    /// Win panel animasyonu
    /// </summary>
    private IEnumerator AnimateWinPanel()
    {
        CanvasGroup cg = winPanel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = winPanel.AddComponent<CanvasGroup>();
        
        cg.alpha = 0;
        cg.blocksRaycasts = true;
        cg.interactable = true;
        
        float elapsed = 0;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            cg.alpha = elapsed / 0.3f;
            yield return null;
        }
        
        cg.alpha = 1;
    }
}

/// <summary>
/// Safe Area ayarlayıcı (iPhone notch için)
/// </summary>
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }
    
    private void Update()
    {
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }
    
    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
