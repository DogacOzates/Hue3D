using UnityEngine;

/// <summary>
/// Tek bir küpü temsil eder - EYKA tarzı pastel, yumuşak görünüm.
/// Renk, pozisyon ve tıklama işlemlerini yönetir.
/// </summary>
public class Cube : MonoBehaviour
{
    [Header("Cube Properties")]
    public Vector3Int gridPosition;      // Küpün grid'deki pozisyonu
    public Color targetColor;            // Küpün olması gereken doğru renk
    public Color currentColor;           // Küpün şu anki rengi
    public bool isFixed;                 // Sabit küp mü (değiştirilemez)
    
    [Header("EYKA Style Visual")]
    [Range(0f, 1f)]
    public float softness = 0.15f;       // Kenar yumuşaklığı
    [Range(0f, 0.5f)]
    public float glossiness = 0.3f;      // Parlaklık
    
    [Header("Visual")]
    private Material material;
    private MeshRenderer meshRenderer;
    
    [Header("Animation")]
    private Vector3 originalScale;
    private bool isAnimating;
    private bool isSelected;
    
    // Events
    public static event System.Action<Cube> OnCubeClicked;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        
        // EYKA tarzı pastel material oluştur
        CreateSoftPastelMaterial();
        
        originalScale = transform.localScale;
    }
    
    /// <summary>
    /// EYKA tarzı buzlu cam / frosted glass material oluşturur
    /// Yüksek smoothness + hafif emission = parlak pastel görünüm
    /// </summary>
    private void CreateSoftPastelMaterial()
    {
        // Shader.Find artık çalışır çünkü URP shader'lar Always Included Shaders'a eklendi
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        
        if (shader != null)
        {
            material = new Material(shader);
        }
        else
        {
            // Fallback: Mevcut renderer'ın default material'ini kopyala
            if (meshRenderer != null && meshRenderer.sharedMaterial != null 
                && meshRenderer.sharedMaterial.shader != null 
                && meshRenderer.sharedMaterial.shader.name != "Hidden/InternalErrorShader")
            {
                material = new Material(meshRenderer.sharedMaterial);
            }
            else
            {
                // Son çare
                Shader fallback = Shader.Find("Standard");
                if (fallback == null) fallback = Shader.Find("Sprites/Default");
                if (fallback != null)
                    material = new Material(fallback);
                else
                {
                    Debug.LogError("Cube: No shader found!");
                    return;
                }
            }
        }
        
        material.color = Color.gray;
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", Color.gray);
        
        // EYKA tarzı buzlu cam / frosted glass ayarları
        if (material.HasProperty("_Smoothness"))
            material.SetFloat("_Smoothness", 0.75f);
        if (material.HasProperty("_Metallic"))
            material.SetFloat("_Metallic", 0.02f);
        
        // Specular highlights
        if (material.HasProperty("_SpecularHighlights"))
            material.SetFloat("_SpecularHighlights", 1f);
        if (material.HasProperty("_EnvironmentReflections"))
            material.SetFloat("_EnvironmentReflections", 1f);
        
        // Emission ile renkleri canlı ve parlak göster
        material.EnableKeyword("_EMISSION");
        if (material.HasProperty("_EmissionColor"))
            material.SetColor("_EmissionColor", Color.gray * 0.45f);
        
        meshRenderer.material = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
    }
    
    private void Start()
    {
        // Sabit küp görsel stilini ayarla
        UpdateFixedVisual();
    }
    
    /// <summary>
    /// Sabit küpü görsel olarak farklılaştırır - ayrı obje yerine renk/scale ile
    /// </summary>
    public void UpdateFixedMarker()
    {
        UpdateFixedVisual();
    }
    
    private void UpdateFixedVisual()
    {
        // Unlit shader'da smoothness yok - sabit/hareketli farkı yok
    }
    
    /// <summary>
    /// Küpün rengini ayarlar - EYKA tarzı pastel renk + buzlu cam emission
    /// </summary>
    public void SetColor(Color color)
    {
        currentColor = color;
        if (material != null)
        {
            // EYKA tarzı: Rengi biraz daha açık/pastel yap
            Color pastelColor = Color.Lerp(color, Color.white, 0.12f);
            material.color = pastelColor;
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", pastelColor);
            
            // Emission - rengin parlak ışıması
            if (material.HasProperty("_EmissionColor"))
                material.SetColor("_EmissionColor", color * 0.45f);
        }
    }
    
    /// <summary>
    /// Küpün hedef rengini ayarlar (çözüm rengi)
    /// </summary>
    public void SetTargetColor(Color color)
    {
        targetColor = color;
    }
    
    /// <summary>
    /// Küp doğru pozisyonda mı kontrol eder
    /// </summary>
    public bool IsInCorrectPosition()
    {
        return ColorDistance(currentColor, targetColor) < 0.01f;
    }
    
    /// <summary>
    /// İki renk arasındaki mesafeyi hesaplar
    /// </summary>
    private float ColorDistance(Color a, Color b)
    {
        return Mathf.Sqrt(
            Mathf.Pow(a.r - b.r, 2) +
            Mathf.Pow(a.g - b.g, 2) +
            Mathf.Pow(a.b - b.b, 2)
        );
    }
    
    /// <summary>
    /// Küpe tıklandığında çağrılır (InputHandler tarafından)
    /// </summary>
    public void NotifyClicked()
    {
        if (isFixed) return;
        
        OnCubeClicked?.Invoke(this);
    }
    
    /// <summary>
    /// Hover başladığında (InputHandler tarafından)
    /// </summary>
    public void OnHoverEnter()
    {
        if (isFixed || isSelected) return;
        
        StopAllCoroutines();
        StartCoroutine(ScaleAnimation(originalScale * 1.1f, 0.1f));
    }
    
    /// <summary>
    /// Hover bittiğinde (InputHandler tarafından)
    /// </summary>
    public void OnHoverExit()
    {
        if (isFixed || isSelected) return;
        
        StopAllCoroutines();
        StartCoroutine(ScaleAnimation(originalScale, 0.1f));
    }
    
    /// <summary>
    /// Küpü seçili olarak işaretle
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        StopAllCoroutines();
        if (selected)
        {
            StartCoroutine(ScaleAnimation(originalScale * 1.15f, 0.15f));
            // Seçili küpü biraz yukarı kaldır
            StartCoroutine(MoveAnimation(transform.position + Vector3.up * 0.2f, 0.15f));
        }
        else
        {
            StartCoroutine(ScaleAnimation(originalScale, 0.15f));
            // Orijinal pozisyona geri dön
            Vector3 targetPos = new Vector3(
                gridPosition.x * GameManager.Instance.cubeSpacing,
                gridPosition.y * GameManager.Instance.cubeSpacing,
                gridPosition.z * GameManager.Instance.cubeSpacing
            );
            StartCoroutine(MoveAnimation(targetPos, 0.15f));
        }
    }
    
    /// <summary>
    /// Başka bir küple yer değiştir
    /// </summary>
    public void SwapWith(Cube other)
    {
        // Renkleri değiştir
        Color tempColor = this.currentColor;
        this.SetColor(other.currentColor);
        other.SetColor(tempColor);
    }
    
    /// <summary>
    /// Küpü hedef pozisyona animasyonlu hareket ettir
    /// </summary>
    public void AnimateToPosition(Vector3 targetPosition, float duration = 0.3f)
    {
        StartCoroutine(MoveAnimation(targetPosition, duration));
    }
    
    private System.Collections.IEnumerator ScaleAnimation(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EaseOutBack(t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
    
    private System.Collections.IEnumerator MoveAnimation(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = EaseOutCubic(t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        
        transform.position = targetPosition;
    }
    
    // Easing fonksiyonları
    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
    
    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
    
    private void OnDestroy()
    {
        if (material != null)
        {
            Destroy(material);
        }
    }
}
