using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Mobil ve masaüstü için küp seçme ve input yönetimi
/// </summary>
public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }
    
    [Header("Settings")]
    public LayerMask cubeLayerMask = -1;
    public float dragThreshold = 20f; // Piksel cinsinden - mobilde parmak kayması için yeterli
    public float rotationSensitivity = 0.5f; // Artırıldı
    public float zoomSensitivity = 0.02f;
    
    private Camera mainCamera;
    private Vector2 touchStartPosition;
    private Vector2 lastTouchPosition;
    private float lastPinchDistance;
    private bool isDragging;
    private Cube hoveredCube;
    
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
    
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    
    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            return;
        }
        
        // Debug: Her 60 frame'de bir durumu logla

        
        // Önce Touchscreen.current ile dene (daha güvenilir)
        if (Touchscreen.current != null)
        {
            HandleTouchscreenInput();
        }
        // Mouse input (editor/masaüstü)
        else if (Mouse.current != null)
        {
            HandleMouseInput();
        }
    }
    
    /// <summary>
    /// Touchscreen input - New Input System
    /// </summary>
    private void HandleTouchscreenInput()
    {
        var touchscreen = Touchscreen.current;
        var primaryTouch = touchscreen.primaryTouch;
        
        // Touch sayısını kontrol et
        int touchCount = 0;
        foreach (var touch in touchscreen.touches)
        {
            if (touch.press.isPressed)
                touchCount++;
        }
        
        // ÖNEMLİ: wasReleasedThisFrame kontrolü dış blokta olmalı!
        // Çünkü parmak kaldırıldığında isPressed=false ve touchCount=0 olur,
        // iç bloğa hiç girilmez.
        if (primaryTouch.press.wasReleasedThisFrame)
        {
            if (!isDragging)
            {
                // Preview modunda: ekranın herhangi yerine tap = oyuna başla
                if (GameManager.Instance != null && GameManager.Instance.isPreviewingLevel)
                {
                    GameManager.Instance.BeginPlayAfterPreview();
                }
                else
                {
                    // Release anında touchStartPosition kullan (daha güvenilir)
                    TrySelectCubeAtPosition(touchStartPosition);
                }
            }
            isDragging = false;
            return;
        }
        
        // Tek parmak - basılıyken
        if (touchCount == 1 || primaryTouch.press.isPressed)
        {
            Vector2 position = primaryTouch.position.ReadValue();
            
            if (primaryTouch.press.wasPressedThisFrame)
            {
                touchStartPosition = position;
                lastTouchPosition = position;
                isDragging = false;
            }
            
            if (primaryTouch.press.isPressed)
            {
                Vector2 delta = position - lastTouchPosition;
                float totalDistance = Vector2.Distance(position, touchStartPosition);
                
                if (totalDistance > dragThreshold)
                {
                    isDragging = true;
                    
                    // Kamerayı döndür - sadece drag modundayken
                    if (delta.magnitude > 0.5f)
                    {
                        CameraController cam = CameraController.Instance;
                        if (cam == null)
                            cam = FindAnyObjectByType<CameraController>();
                        
                        if (cam != null)
                        {
                            cam.RotateCamera(delta.x * rotationSensitivity, -delta.y * rotationSensitivity);
                        }
                    }
                }
                
                lastTouchPosition = position;
            }
        }
        
        // İki parmak pinch zoom
        if (touchCount >= 2)
        {
            isDragging = true;
            
            // İki touch pozisyonunu bul
            Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;
            int found = 0;
            foreach (var touch in touchscreen.touches)
            {
                if (touch.press.isPressed)
                {
                    if (found == 0) pos0 = touch.position.ReadValue();
                    else if (found == 1) pos1 = touch.position.ReadValue();
                    found++;
                    if (found >= 2) break;
                }
            }
            
            if (found >= 2)
            {
                float currentPinchDist = Vector2.Distance(pos0, pos1);
                
                if (lastPinchDistance < 0.1f)
                {
                    // İlk frame — sadece kaydet
                    lastPinchDistance = currentPinchDist;
                }
                else
                {
                    float pinchDelta = currentPinchDist - lastPinchDistance;
                    
                    if (Mathf.Abs(pinchDelta) > 1f)
                    {
                        CameraController cam = CameraController.Instance;
                        if (cam == null)
                            cam = FindAnyObjectByType<CameraController>();
                        
                        if (cam != null)
                        {
                            cam.ZoomCamera(-pinchDelta * zoomSensitivity);
                        }
                    }
                    
                    lastPinchDistance = currentPinchDist;
                }
            }
        }
        else
        {
            // İki parmak bırakıldığında reset
            lastPinchDistance = 0f;
        }
    }
    
    /// <summary>
    /// Touch input işler (iOS/Android)
    /// </summary>
    private void HandleTouchInput()
    {
        int touchCount = Touch.activeTouches.Count;
        
        // Tek parmak - döndürme veya seçim
        if (touchCount == 1)
        {
            var touch = Touch.activeTouches[0];
            
            switch (touch.phase)
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    touchStartPosition = touch.screenPosition;
                    lastTouchPosition = touch.screenPosition;
                    isDragging = false;

                    break;
                    
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    // Sürükleme kontrolü
                    float distance = Vector2.Distance(touch.screenPosition, touchStartPosition);
                    
                    // Her zaman döndür (drag threshold'u geçtiyse)
                    Vector2 delta = touch.screenPosition - lastTouchPosition;
                    
                    if (distance > dragThreshold)
                    {
                        isDragging = true;
                    }
                    
                    // Kamerayı döndür
                    CameraController cam = CameraController.Instance;
                    if (cam == null)
                    {
                        cam = FindAnyObjectByType<CameraController>();
                    }
                    
                    if (cam != null && delta.magnitude > 0.1f)
                    {
                        cam.RotateCamera(delta.x * rotationSensitivity, -delta.y * rotationSensitivity);

                    }
                    
                    lastTouchPosition = touch.screenPosition;
                    break;
                    
                case UnityEngine.InputSystem.TouchPhase.Ended:
                    // Sürükleme değilse tıklama sayılır
                    if (!isDragging)
                    {
                        // Preview modunda: ekranın herhangi yerine tap = oyuna başla
                        if (GameManager.Instance != null && GameManager.Instance.isPreviewingLevel)
                        {
                            GameManager.Instance.BeginPlayAfterPreview();
                        }
                        else
                        {
                            TrySelectCubeAtPosition(touch.screenPosition);
                        }
                    }
                    isDragging = false;
                    break;
            }
        }
        // İki parmak - zoom
        else if (touchCount >= 2)
        {
            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];
            
            float currentPinchDistance = Vector2.Distance(touch0.screenPosition, touch1.screenPosition);
            
            if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Began || 
                touch1.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                lastPinchDistance = currentPinchDistance;
            }
            else if (touch0.phase == UnityEngine.InputSystem.TouchPhase.Moved || 
                     touch1.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float pinchDelta = currentPinchDistance - lastPinchDistance;
                
                if (CameraController.Instance != null)
                {
                    CameraController.Instance.ZoomCamera(-pinchDelta * zoomSensitivity);
                }
                
                lastPinchDistance = currentPinchDistance;
            }
            
            isDragging = true; // Zoom sırasında seçim yapılmasın
        }
    }
    
    /// <summary>
    /// Mouse input işler (Editor/PC)
    /// </summary>
    private void HandleMouseInput()
    {
        var mouse = Mouse.current;
        Vector2 currentPos = mouse.position.ReadValue();
        
        // Sol tık: seçim veya döndürme
        if (mouse.leftButton.wasPressedThisFrame)
        {
            touchStartPosition = currentPos;
            lastTouchPosition = currentPos;
            isDragging = false;
        }
        
        if (mouse.leftButton.isPressed)
        {
            Vector2 delta = currentPos - lastTouchPosition;
            float totalDistance = Vector2.Distance(currentPos, touchStartPosition);
            
            if (totalDistance > dragThreshold)
            {
                isDragging = true;
                
                // Sol tık sürükle = döndür
                CameraController cam = CameraController.Instance ?? FindAnyObjectByType<CameraController>();
                if (cam != null && delta.magnitude > 0.5f)
                {
                    cam.RotateCamera(delta.x * rotationSensitivity, -delta.y * rotationSensitivity);
                }
            }
            
            lastTouchPosition = currentPos;
        }
        
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            if (!isDragging)
            {
                // Preview modunda: herhangi yere tıkla = oyuna başla
                if (GameManager.Instance != null && GameManager.Instance.isPreviewingLevel)
                {
                    GameManager.Instance.BeginPlayAfterPreview();
                }
                else
                {
                    TrySelectCubeAtPosition(currentPos);
                }
            }
            isDragging = false;
        }
        
        // Sağ tık: sadece döndürme
        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = currentPos - lastTouchPosition;
            CameraController cam = CameraController.Instance ?? FindAnyObjectByType<CameraController>();
            if (cam != null && delta.magnitude > 0.5f)
            {
                cam.RotateCamera(delta.x * rotationSensitivity * 2f, -delta.y * rotationSensitivity * 2f);
            }
            lastTouchPosition = currentPos;
        }
        
        if (mouse.rightButton.wasPressedThisFrame)
        {
            lastTouchPosition = currentPos;
        }
        
        // Hover efekti (sadece PC)
        UpdateHoverEffect(currentPos);
    }
    
    /// <summary>
    /// Pozisyondaki küpü seçmeye çalışır
    /// </summary>
    private void TrySelectCubeAtPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100f, cubeLayerMask))
        {
            Cube cube = hit.collider.GetComponent<Cube>();
            if (cube != null && !cube.isFixed)
            {
                // GameManager'a bildir
                cube.NotifyClicked();
            }
        }
    }
    
    /// <summary>
    /// Hover efektini günceller
    /// </summary>
    private void UpdateHoverEffect(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        
        Cube newHovered = null;
        
        if (Physics.Raycast(ray, out hit, 100f, cubeLayerMask))
        {
            newHovered = hit.collider.GetComponent<Cube>();
        }
        
        // Hover değişti mi?
        if (newHovered != hoveredCube)
        {
            if (hoveredCube != null)
            {
                hoveredCube.OnHoverExit();
            }
            
            hoveredCube = newHovered;
            
            if (hoveredCube != null && !hoveredCube.isFixed)
            {
                hoveredCube.OnHoverEnter();
            }
        }
    }
}
