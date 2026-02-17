using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// UI yönetimi - Butonlar, paneller ve animasyonlar
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Main UI")]
    public Canvas mainCanvas;
    public CanvasGroup mainCanvasGroup;
    
    [Header("HUD")]
    public Text levelText;
    public Text moveCountText;
    public Text timerText;
    public Slider progressSlider;
    
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject settingsPanel;
    
    [Header("Win Panel")]
    public Text winLevelText;
    public Text winMoveText;
    public Text winTimeText;
    public Text winStarsText;
    public Button winNextButton;
    public Button winRestartButton;
    
    [Header("Buttons")]
    public Button pauseButton;
    public Button settingsButton;
    public Button reshuffleButton;
    
    [Header("Animation")]
    public float fadeSpeed = 2f;
    
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
    
    private void Start()
    {
        SetupUI();
        SetupButtons();
    }
    
    /// <summary>
    /// UI'ı başlangıç durumuna getirir
    /// </summary>
    private void SetupUI()
    {
        // Canvas yoksa oluştur
        if (mainCanvas == null)
        {
            CreateUICanvas();
        }
        
        HideAllPanels();
    }
    
    /// <summary>
    /// Buton eventlerini bağlar
    /// </summary>
    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ToggleSettings);
        
        if (reshuffleButton != null)
            reshuffleButton.onClick.AddListener(() => GameManager.Instance?.ReshuffleColors());
        
        if (winNextButton != null)
            winNextButton.onClick.AddListener(() => GameManager.Instance?.NextLevel());
        
        if (winRestartButton != null)
            winRestartButton.onClick.AddListener(() => GameManager.Instance?.RestartLevel());
    }
    
    /// <summary>
    /// UI Canvas'ı oluşturur
    /// </summary>
    private void CreateUICanvas()
    {
        // Ana Canvas
        GameObject canvasObj = new GameObject("UI Canvas");
        mainCanvas = canvasObj.AddComponent<Canvas>();
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 100;
        
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        
        mainCanvasGroup = canvasObj.AddComponent<CanvasGroup>();
        
        // HUD Panel
        CreateHUD(canvasObj.transform);
        
        // Win Panel
        CreateWinPanel(canvasObj.transform);
    }
    
    /// <summary>
    /// HUD oluşturur
    /// </summary>
    private void CreateHUD(Transform parent)
    {
        // Level Text (sol üst)
        GameObject levelObj = CreateText("Level 1", parent, new Vector2(120, -50), 32);
        levelText = levelObj.GetComponent<Text>();
        levelText.alignment = TextAnchor.MiddleLeft;
        
        // Move Count (sağ üst)
        GameObject moveObj = CreateText("Moves: 0", parent, new Vector2(-120, -50), 24);
        moveCountText = moveObj.GetComponent<Text>();
        moveCountText.alignment = TextAnchor.MiddleRight;
        
        // Progress (alt)
        GameObject progressObj = new GameObject("Progress");
        progressObj.transform.SetParent(parent, false);
        RectTransform progressRect = progressObj.AddComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0.5f, 0);
        progressRect.anchorMax = new Vector2(0.5f, 0);
        progressRect.pivot = new Vector2(0.5f, 0);
        progressRect.anchoredPosition = new Vector2(0, 80);
        progressRect.sizeDelta = new Vector2(300, 10);
        
        progressSlider = progressObj.AddComponent<Slider>();
        progressSlider.interactable = false;
        progressSlider.minValue = 0;
        progressSlider.maxValue = 100;
        
        // Slider background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(progressObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(1, 1, 1, 0.3f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // Slider fill
        GameObject fillAreaObj = new GameObject("Fill Area");
        fillAreaObj.transform.SetParent(progressObj.transform, false);
        RectTransform fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;
        
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color(0.5f, 0.9f, 1f, 0.8f);
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        progressSlider.fillRect = fillRect;
    }
    
    /// <summary>
    /// Kazanma paneli oluşturur
    /// </summary>
    private void CreateWinPanel(Transform parent)
    {
        // Panel
        winPanel = new GameObject("Win Panel");
        winPanel.transform.SetParent(parent, false);
        
        RectTransform panelRect = winPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        Image panelBg = winPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.7f);
        
        // İç panel
        GameObject innerPanel = new GameObject("Inner Panel");
        innerPanel.transform.SetParent(winPanel.transform, false);
        RectTransform innerRect = innerPanel.AddComponent<RectTransform>();
        innerRect.anchorMin = new Vector2(0.5f, 0.5f);
        innerRect.anchorMax = new Vector2(0.5f, 0.5f);
        innerRect.pivot = new Vector2(0.5f, 0.5f);
        innerRect.sizeDelta = new Vector2(400, 350);
        
        Image innerBg = innerPanel.AddComponent<Image>();
        innerBg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        
        // Başlık
        GameObject titleObj = CreateText("Congratulations!", innerPanel.transform, new Vector2(0, 120), 48);
        titleObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        titleObj.GetComponent<Text>().color = new Color(0.5f, 1f, 0.8f);
        
        // Level info
        GameObject levelInfoObj = CreateText("Level 1 Completed", innerPanel.transform, new Vector2(0, 60), 28);
        winLevelText = levelInfoObj.GetComponent<Text>();
        winLevelText.alignment = TextAnchor.MiddleCenter;
        
        // Move info
        GameObject moveInfoObj = CreateText("Moves: 0", innerPanel.transform, new Vector2(0, 20), 24);
        winMoveText = moveInfoObj.GetComponent<Text>();
        winMoveText.alignment = TextAnchor.MiddleCenter;
        winMoveText.color = new Color(0.8f, 0.8f, 0.8f);
        
        // Time info
        GameObject timeInfoObj = CreateText("Time: 0:00", innerPanel.transform, new Vector2(0, -15), 24);
        winTimeText = timeInfoObj.GetComponent<Text>();
        winTimeText.alignment = TextAnchor.MiddleCenter;
        winTimeText.color = new Color(0.8f, 0.8f, 0.8f);
        
        // Next Button
        winNextButton = CreateButton("Next Level", innerPanel.transform, new Vector2(0, -80), new Color(0.3f, 0.8f, 0.5f));
        
        // Restart Button
        winRestartButton = CreateButton("Retry", innerPanel.transform, new Vector2(0, -130), new Color(0.5f, 0.5f, 0.6f));
        
        winPanel.SetActive(false);
    }
    
    /// <summary>
    /// Text objesi oluşturur
    /// </summary>
    private GameObject CreateText(string text, Transform parent, Vector2 position, int fontSize)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(350, fontSize + 20);
        
        Text textComp = textObj.AddComponent<Text>();
        textComp.text = text;
        textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComp.fontSize = fontSize;
        textComp.color = Color.white;
        textComp.alignment = TextAnchor.MiddleCenter;
        
        return textObj;
    }
    
    /// <summary>
    /// Button oluşturur
    /// </summary>
    private Button CreateButton(string text, Transform parent, Vector2 position, Color bgColor)
    {
        GameObject buttonObj = new GameObject("Button");
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(200, 45);
        
        Image img = buttonObj.AddComponent<Image>();
        img.color = bgColor;
        
        Button btn = buttonObj.AddComponent<Button>();
        btn.targetGraphic = img;
        
        // Button text
        GameObject textObj = CreateText(text, buttonObj.transform, Vector2.zero, 22);
        
        return btn;
    }
    
    /// <summary>
    /// Tüm panelleri gizler
    /// </summary>
    public void HideAllPanels()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    
    /// <summary>
    /// HUD'ı günceller
    /// </summary>
    public void UpdateHUD(int level, int moves, float time, float progress)
    {
        if (levelText != null)
            levelText.text = $"Level {level}";
        
        if (moveCountText != null)
            moveCountText.text = $"Moves: {moves}";
        
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timerText.text = $"{minutes}:{seconds:00}";
        }
        
        if (progressSlider != null)
            progressSlider.value = progress;
    }
    
    /// <summary>
    /// Kazanma panelini gösterir
    /// </summary>
    public void ShowWinPanel(int level, int moves, float time)
    {
        if (winPanel == null) return;
        
        winPanel.SetActive(true);
        
        if (winLevelText != null)
            winLevelText.text = $"Level {level} Completed!";
        
        if (winMoveText != null)
            winMoveText.text = $"Moves: {moves}";
        
        if (winTimeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            winTimeText.text = $"Time: {minutes}:{seconds:00}";
        }
        
        StartCoroutine(AnimatePanel(winPanel, true));
    }
    
    /// <summary>
    /// Pause'u açıp kapatır
    /// </summary>
    public void TogglePause()
    {
        if (pausePanel == null) return;
        
        bool isActive = !pausePanel.activeSelf;
        pausePanel.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;
    }
    
    /// <summary>
    /// Ayarları açıp kapatır
    /// </summary>
    public void ToggleSettings()
    {
        if (settingsPanel == null) return;
        
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    
    /// <summary>
    /// Panel animasyonu
    /// </summary>
    private IEnumerator AnimatePanel(GameObject panel, bool show)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();
        
        float start = show ? 0f : 1f;
        float end = show ? 1f : 0f;
        float elapsed = 0f;
        float duration = 0.3f;
        
        cg.alpha = start;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        
        cg.alpha = end;
        
        if (!show)
            panel.SetActive(false);
    }
}
