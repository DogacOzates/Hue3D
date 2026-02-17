using UnityEngine;

/// <summary>
/// Sahneyi otomatik olarak kurar - EYKA tarzı pastel görünüm.
/// </summary>
public class SceneSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    
    [Header("Game Settings")]
    [Range(1, 50)]
    public int startingLevel = 1;  // Hangi level'den başlasın
    
    [Header("Visual Settings - EYKA Style")]
    public int currentColorTheme = 0;  // 0=Lavender, 1=CoralBlush, 2=WarmSand, 3=FreshMint, 4=ArcticFrost, 5=SunsetRose
    public bool useGradientBackground = true;
    
    [Header("Created Objects")]
    public GameObject gameManagerObject;
    public GameObject puzzleGeneratorObject;
    public GameObject uiManagerObject;
    public GameObject inputHandlerObject;
    public GameObject gradientBackgroundObject;
    public GameObject particleSystemObject;
    public Light mainLight;
    public Light fillLight;
    
    private Camera mainCam;
    
    private void Start()
    {
        if (setupOnStart)
        {
            SetupScene();
        }
    }
    
    [ContextMenu("Setup Scene")]
    public void SetupScene()
    {
        SetupCamera();
        SetupLighting();
        SetupManagers();
        SetupGradientBackground();
        SetupBackgroundParticles();
    }
    
    /// <summary>
    /// Tema rengini değiştirir ve arka planı günceller
    /// </summary>
    public void SetColorTheme(int themeIndex)
    {
        currentColorTheme = themeIndex % ColorPalettes.AllPalettes.Length;
        UpdateBackgroundForTheme(currentColorTheme);
    }
    
    /// <summary>
    /// Kamerayı ayarlar
    /// </summary>
    private void SetupCamera()
    {
        mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            camObj.AddComponent<AudioListener>();
        }
        
        // CameraController ekle
        if (mainCam.GetComponent<CameraController>() == null)
        {
            mainCam.gameObject.AddComponent<CameraController>();
        }
        
        // Kamera ayarları - EYKA tarzı
        mainCam.clearFlags = CameraClearFlags.SolidColor;
        mainCam.backgroundColor = ColorPalettes.GetBackgroundColor(currentColorTheme);
        mainCam.orthographic = true;
        mainCam.orthographicSize = 6f;
        mainCam.nearClipPlane = 0.1f;
        mainCam.farClipPlane = 100f;
        
        // Başlangıç pozisyonu - izometrik açı
        mainCam.transform.position = new Vector3(10, 10, 10);
        mainCam.transform.LookAt(Vector3.zero);
    }
    
    /// <summary>
    /// EYKA tarzı yumuşak ışıklandırma - güçlü ambient, minimal directional
    /// Küpler parlak pastel görünsün, gölge yok
    /// </summary>
    private void SetupLighting()
    {
        // Ana ışık - çok hafif, sadece minimal derinlik hissi için
        if (mainLight == null)
        {
            GameObject lightObj = new GameObject("Main Light");
            mainLight = lightObj.AddComponent<Light>();
            mainLight.type = LightType.Directional;
            mainLight.color = new Color(1f, 0.99f, 0.97f);  // Çok hafif warm white
            mainLight.intensity = 0.25f;  // EYKA: yumuşak vurgu ışığı
            mainLight.shadows = LightShadows.None;
            lightObj.transform.rotation = Quaternion.Euler(45, -30, 0);
        }
        
        // Dolgu ışığı - karşı taraftan dengelemek için
        if (fillLight == null)
        {
            GameObject fillObj = new GameObject("Fill Light");
            fillLight = fillObj.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.color = new Color(0.97f, 0.97f, 1f);  // Çok hafif cool white
            fillLight.intensity = 0.20f;
            fillLight.shadows = LightShadows.None;
            fillObj.transform.rotation = Quaternion.Euler(35, 150, 0);
        }
        
        // Ambient ayarları - EYKA: Çok güçlü ambient, her açıdan eşit aydınlatma
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.98f, 0.97f, 0.99f);  // Neredeyse beyaz ambient
        
        RenderSettings.fog = false;
    }
    
    /// <summary>
    /// Manager objelerini oluşturur
    /// </summary>
    private void SetupManagers()
    {
        // GameManager
        if (GameManager.Instance == null)
        {
            gameManagerObject = new GameObject("GameManager");
            GameManager gm = gameManagerObject.AddComponent<GameManager>();
            gm.startingLevel = startingLevel;
        }
        else
        {
            gameManagerObject = GameManager.Instance.gameObject;
            GameManager.Instance.startingLevel = startingLevel;
        }
        
        // PuzzleGenerator
        if (PuzzleGenerator.Instance == null)
        {
            puzzleGeneratorObject = new GameObject("PuzzleGenerator");
            puzzleGeneratorObject.AddComponent<PuzzleGenerator>();
        }
        else
        {
            puzzleGeneratorObject = PuzzleGenerator.Instance.gameObject;
        }
        
        // InputHandler (yeni touch/input sistemi)
        InputHandler inputHandler = FindAnyObjectByType<InputHandler>();
        if (inputHandler == null)
        {
            inputHandlerObject = new GameObject("InputHandler");
            inputHandler = inputHandlerObject.AddComponent<InputHandler>();
        }
        else
        {
            inputHandlerObject = inputHandler.gameObject;
        }
        
        // MobileUIManager (Canvas oluşturur)
        MobileUIManager mobileUI = FindAnyObjectByType<MobileUIManager>();
        if (mobileUI == null)
        {
            uiManagerObject = new GameObject("MobileUIManager");
            mobileUI = uiManagerObject.AddComponent<MobileUIManager>();
        }
        else
        {
            uiManagerObject = mobileUI.gameObject;
        }
        
        // Referansları bağla
        GameManager gmInstance = GameManager.Instance;
        if (gmInstance != null)
        {
            gmInstance.puzzleGenerator = PuzzleGenerator.Instance;
            gmInstance.cameraController = Camera.main?.GetComponent<CameraController>();
            gmInstance.inputHandler = inputHandler;
            gmInstance.mobileUI = mobileUI;
        }
    }
    
    /// <summary>
    /// EYKA tarzı gradient arka plan oluşturur
    /// </summary>
    private void SetupGradientBackground()
    {
        if (useGradientBackground)
        {
            CreateGradientBackground(currentColorTheme);
        }
        else
        {
            // Solid color background
            if (mainCam != null)
            {
                mainCam.backgroundColor = ColorPalettes.GetBackgroundColor(currentColorTheme);
            }
        }
    }
    
    /// <summary>
    /// EYKA tarzı gradient arka plan quad'ı oluşturur
    /// Üst: beyaza yakın, Alt: yumuşak pastel ton
    /// </summary>
    private void CreateGradientBackground(int themeIndex)
    {
        // Eski gradient'i temizle
        if (gradientBackgroundObject != null)
        {
            Destroy(gradientBackgroundObject);
        }
        
        // EYKA tarzı gradient renkleri al (beyaz → yumuşak pastel)
        var (topColor, bottomColor) = ColorPalettes.GetBackgroundGradient(themeIndex);
        
        // Kamera solid color'ı gradient'in üst kısmına yakın olsun (beyaza yakın)
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = Color.Lerp(topColor, bottomColor, 0.3f);
        }
        
        // Gradient quad oluştur (ekran boyutunda)
        gradientBackgroundObject = new GameObject("GradientBackground");
        MeshFilter meshFilter = gradientBackgroundObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gradientBackgroundObject.AddComponent<MeshRenderer>();
        
        // Basit quad mesh
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            new Vector3(-50, -50, 0),  // Sol alt
            new Vector3(50, -50, 0),   // Sağ alt
            new Vector3(-50, 50, 0),   // Sol üst
            new Vector3(50, 50, 0)     // Sağ üst
        };
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),  // Sol alt
            new Vector2(1, 0),  // Sağ alt
            new Vector2(0, 1),  // Sol üst
            new Vector2(1, 1)   // Sağ üst
        };
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        
        // Gradient texture oluştur (vertex color yerine - URP uyumlu)
        Texture2D gradientTex = new Texture2D(1, 256, TextureFormat.RGBA32, false);
        for (int y = 0; y < 256; y++)
        {
            float t = y / 255f;
            Color c = Color.Lerp(bottomColor, topColor, t);
            gradientTex.SetPixel(0, y, c);
        }
        gradientTex.Apply();
        gradientTex.wrapMode = TextureWrapMode.Clamp;
        gradientTex.filterMode = FilterMode.Bilinear;
        
        // URP Unlit material - texture ile gradient
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            shader = Shader.Find("Unlit/Texture");
        
        Material gradientMat = new Material(shader);
        gradientMat.SetTexture("_BaseMap", gradientTex);
        gradientMat.SetColor("_BaseColor", Color.white);
        meshRenderer.material = gradientMat;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        
        // Kameranın child'ı yap - her açıda kameranın arkasında kalsın
        if (mainCam != null)
        {
            gradientBackgroundObject.transform.SetParent(mainCam.transform);
            gradientBackgroundObject.transform.localPosition = new Vector3(0, 0, mainCam.farClipPlane - 1f);
            gradientBackgroundObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            gradientBackgroundObject.transform.position = new Vector3(0, 0, 40);
        }
    }
    
    /// <summary>
    /// Arka planı tema için günceller
    /// </summary>
    public void UpdateBackgroundForTheme(int themeIndex)
    {
        if (useGradientBackground)
        {
            CreateGradientBackground(themeIndex);
        }
        else
        {
            if (mainCam != null)
            {
                mainCam.backgroundColor = ColorPalettes.GetBackgroundColor(themeIndex);
            }
        }
        
        // MobileUIManager'a da bildir
        if (MobileUIManager.Instance != null)
        {
            MobileUIManager.Instance.UpdateUITheme(themeIndex);
        }
    }
    
    /// <summary>
    /// EYKA tarzı hafif parçacık efekti - arka planda yavaşça süzülen noktalar
    /// </summary>
    private void SetupBackgroundParticles()
    {
        // Eski particle system'i temizle
        if (particleSystemObject != null)
        {
            Destroy(particleSystemObject);
        }
        
        particleSystemObject = new GameObject("BackgroundParticles");
        ParticleSystem ps = particleSystemObject.AddComponent<ParticleSystem>();
        
        // Kameranın child'ı yap - kamera ile birlikte hareket etsin
        if (mainCam != null)
        {
            particleSystemObject.transform.SetParent(mainCam.transform);
            particleSystemObject.transform.localPosition = new Vector3(0, 0, 15);  // Kameranın önünde
            particleSystemObject.transform.localRotation = Quaternion.identity;
        }
        else
        {
            particleSystemObject.transform.position = new Vector3(0, 0, -5);
            particleSystemObject.transform.rotation = Quaternion.identity;
        }
        
        Debug.Log("BackgroundParticles created: " + particleSystemObject.transform.position);
        
        // Main module - EYKA tarzı çok yumuşak, yavaş parçacıklar
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(8f, 15f);  // Uzun ömür
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.4f);  // Çok yavaş
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);  // Daha büyük
        main.maxParticles = 60;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.01f;  // Çok hafif yukarı süzülme
        main.playOnAwake = true;
        main.loop = true;
        
        // Renk - beyaz, görünür saydamlık
        Color particleColor = new Color(1f, 1f, 1f, 0.6f);
        main.startColor = particleColor;
        
        // Emission - çok seyrek (EYKA minimalizmi)
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 4f;
        
        // Shape - geniş alan, kamera önünde
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(25, 20, 1);  // Tüm ekranı kapla
        shape.position = new Vector3(0, -5, 0);  // Alttan biraz daha çok
        
        // Velocity - EYKA: çok yavaş yukarı süzülme
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.1f, 0.1f);  // Minimal yatay salınım
        velocity.y = new ParticleSystem.MinMaxCurve(0.15f, 0.5f);  // Yavaş yükselme
        velocity.z = new ParticleSystem.MinMaxCurve(-0.03f, 0.03f);
        
        // Noise - doğal yalpalama (gazoz baloncuğu hareketi)
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.3f;
        noise.frequency = 0.5f;
        noise.scrollSpeed = 0.3f;
        noise.damping = true;
        noise.octaveCount = 2;
        
        // Size - baloncuklar yukarı çıktıkça biraz büyür
        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.5f);
        sizeCurve.AddKey(0.3f, 0.8f);
        sizeCurve.AddKey(0.7f, 1.0f);
        sizeCurve.AddKey(0.95f, 1.1f);
        sizeCurve.AddKey(1f, 0f);  // Son anda patlayıp kaybolma
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        
        // Alpha - belirip yumuşak kaybolma
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.1f), new GradientAlphaKey(0.9f, 0.75f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;
        
        // Renderer
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        
        // Yuvarlak parçacık için özel material oluştur
        Material particleMat = CreateCircleParticleMaterial();
        if (particleMat != null)
        {
            renderer.material = particleMat;
            renderer.trailMaterial = particleMat;
        }
        
        renderer.sortingOrder = 5;  // Arka planın önünde görünsün
        
        Debug.Log("Particle System started, material: " + (renderer.material != null ? renderer.material.name : "null"));
        
        // Başlat
        ps.Play();
        
        Debug.Log("Particle System running: " + ps.isPlaying + ", particle count: " + ps.particleCount);
    }
    
    /// <summary>
    /// Yuvarlak parçacık için yumuşak daire texture'ı oluşturur
    /// </summary>
    private Texture2D CreateCircleTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = size / 2f;
        float maxRadius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center + 0.5f;
                float dy = y - center + 0.5f;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                // Normalize distance (0 = center, 1 = edge)
                float normalizedDist = distance / maxRadius;
                
                // Smooth falloff - radial gradient
                float alpha = 1f - Mathf.SmoothStep(0f, 1f, normalizedDist);
                
                // Daha yumuşak kenar
                alpha = alpha * alpha;
                
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
    }
    
    /// <summary>
    /// Yuvarlak parçacık için URP uyumlu material oluşturur
    /// </summary>
    private Material CreateCircleParticleMaterial()
    {
        // Önce URP Particles shader'ını dene (en iyi sonuç)
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        
        // URP yoksa standart particles shader
        if (shader == null)
            shader = Shader.Find("Particles/Standard Unlit");
        
        // O da yoksa legacy shader
        if (shader == null)
            shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended");
        
        if (shader == null)
        {
            Debug.LogWarning("Particle shader not found!");
            return null;
        }
        
        Material mat = new Material(shader);
        
        // Yuvarlak texture oluştur
        Texture2D circleTexture = CreateCircleTexture(64);
        
        // Material ayarları
        mat.SetTexture("_BaseMap", circleTexture);
        mat.SetTexture("_MainTex", circleTexture);
        
        // Renk - beyaz (particle color'dan gelecek)
        mat.SetColor("_BaseColor", Color.white);
        mat.SetColor("_Color", Color.white);
        
        // Transparency/Blending
        mat.SetFloat("_Surface", 1); // Transparent
        mat.SetFloat("_Blend", 0);   // Alpha blend
        mat.SetFloat("_AlphaClip", 0);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0);
        
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.renderQueue = 3000; // Transparent queue
        
        mat.name = "CircleParticleMaterial";
        
        Debug.Log("Circle particle material created: " + shader.name);
        
        return mat;
    }
    
    [ContextMenu("Clear Scene")]
    public void ClearScene()
    {
        if (PuzzleGenerator.Instance != null)
        {
            PuzzleGenerator.Instance.ClearPuzzle();
        }
    }
    
    [ContextMenu("Regenerate Puzzle")]
    public void RegeneratePuzzle()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartLevel();
        }
    }
    
    [ContextMenu("Next Theme")]
    public void NextTheme()
    {
        SetColorTheme(currentColorTheme + 1);
    }
}
