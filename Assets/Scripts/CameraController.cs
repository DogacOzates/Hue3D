using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Kamera kontrolü - İzometrik görünüm, döndürme ve zoom
/// </summary>
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    [Header("Orbit Settings")]
    public float rotationSpeed = 5f;
    public float smoothSpeed = 10f;
    public float autoRotateSpeed = 2f;
    public bool autoRotate = false;
    
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = 8f;
    public float maxZoom = 35f;
    public float defaultZoom = 20f;
    
    [Header("Touch Settings")]
    public float touchRotationSensitivity = 0.3f;
    public float touchZoomSensitivity = 0.05f;
    
    [Header("Camera Position")]
    public Vector3 targetPosition = Vector3.zero;
    public float currentDistance = 20f;
    public float currentAngleX = 35f;  // Dikey açı (yukarıdan bakış)
    public float currentAngleY = -145f;  // Yatay açı
    
    private float targetAngleX;
    private float targetAngleY;
    private float targetDistance;
    
    private Vector2 lastMousePosition;
    private bool isDragging;
    
    // Touch için
    private Vector2 lastTouchPosition;
    private float lastTouchDistance;
    
    // Input System references
    private Mouse mouse;
    private Keyboard keyboard;
    private Touchscreen touchscreen;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void Start()
    {
        // Input devices
        mouse = Mouse.current;
        keyboard = Keyboard.current;
        touchscreen = Touchscreen.current;
        
        // Başlangıç değerlerini ayarla
        targetAngleX = currentAngleX;
        targetAngleY = currentAngleY;
        targetDistance = defaultZoom;
        currentDistance = defaultZoom;
        
        // Kamerayı orthographic yap (izometrik görünüm için)
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = defaultZoom / 2f;
        }
        
        UpdateCameraPosition();
    }
    
    private void Update()
    {
        // Refresh input devices
        mouse = Mouse.current;
        keyboard = Keyboard.current;
        touchscreen = Touchscreen.current;
        
        HandleInput();
        
        if (autoRotate && !isDragging)
        {
            targetAngleY += autoRotateSpeed * Time.deltaTime;
        }
        
        // Smooth interpolation
        currentAngleX = Mathf.LerpAngle(currentAngleX, targetAngleX, Time.deltaTime * smoothSpeed);
        currentAngleY = Mathf.LerpAngle(currentAngleY, targetAngleY, Time.deltaTime * smoothSpeed);
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * smoothSpeed);
        
        UpdateCameraPosition();
    }
    
    /// <summary>
    /// Girişleri işler (mouse ve touch)
    /// </summary>
    private void HandleInput()
    {
        // Mouse kontrolü (PC/Editor için)
        HandleMouseInput();
        
        // Touch kontrolü InputHandler tarafından yönetiliyor
        // InputHandler, RotateCamera() ve ZoomCamera() metodlarını çağırır
    }
    
    /// <summary>
    /// Mouse girişini işler (New Input System)
    /// </summary>
    private void HandleMouseInput()
    {
        if (mouse == null) return;
        
        // Sağ tık ile döndürme
        if (mouse.rightButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastMousePosition = mouse.position.ReadValue();
        }
        
        if (mouse.rightButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
        
        if (isDragging && mouse.rightButton.isPressed)
        {
            Vector2 currentPos = mouse.position.ReadValue();
            Vector2 delta = currentPos - lastMousePosition;
            
            targetAngleY += delta.x * rotationSpeed * 0.1f;
            targetAngleX -= delta.y * rotationSpeed * 0.1f;
            
            // Dikey açıyı sınırla (gimbal lock önlemek için minimal sınır)
            targetAngleX = Mathf.Clamp(targetAngleX, -89f, 89f);
            
            lastMousePosition = currentPos;
        }
        
        // Mouse wheel ile zoom
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeed * 0.01f;
            targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);
            
            // Orthographic size'ı da güncelle
            Camera cam = GetComponent<Camera>();
            if (cam != null && cam.orthographic)
            {
                cam.orthographicSize = targetDistance / 2f;
            }
        }
    }
    
    /// <summary>
    /// Touch girişini işler (mobil için - New Input System)
    /// </summary>
    private void HandleTouchInput()
    {
        if (touchscreen == null) return;
        
        var touches = touchscreen.touches;
        int touchCount = 0;
        
        foreach (var touch in touches)
        {
            if (touch.isInProgress) touchCount++;
        }
        
        if (touchCount == 1)
        {
            // Tek parmak - döndürme
            foreach (var touch in touches)
            {
                if (!touch.isInProgress) continue;
                
                var phase = touch.phase.ReadValue();
                Vector2 position = touch.position.ReadValue();
                
                if (phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    lastTouchPosition = position;
                    isDragging = true;
                }
                else if (phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    Vector2 delta = position - lastTouchPosition;
                    
                    targetAngleY += delta.x * touchRotationSensitivity;
                    targetAngleX -= delta.y * touchRotationSensitivity;
                    
                    targetAngleX = Mathf.Clamp(targetAngleX, -89f, 89f);
                    
                    lastTouchPosition = position;
                }
                else if (phase == UnityEngine.InputSystem.TouchPhase.Ended || 
                         phase == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    isDragging = false;
                }
                break;
            }
        }
        else if (touchCount == 2)
        {
            // İki parmak - zoom
            Vector2 pos1 = Vector2.zero, pos2 = Vector2.zero;
            bool foundFirst = false;
            UnityEngine.InputSystem.TouchPhase phase1 = default, phase2 = default;
            
            foreach (var touch in touches)
            {
                if (!touch.isInProgress) continue;
                
                if (!foundFirst)
                {
                    pos1 = touch.position.ReadValue();
                    phase1 = touch.phase.ReadValue();
                    foundFirst = true;
                }
                else
                {
                    pos2 = touch.position.ReadValue();
                    phase2 = touch.phase.ReadValue();
                    break;
                }
            }
            
            float currentTouchDistance = Vector2.Distance(pos1, pos2);
            
            if (phase1 == UnityEngine.InputSystem.TouchPhase.Began || 
                phase2 == UnityEngine.InputSystem.TouchPhase.Began)
            {
                lastTouchDistance = currentTouchDistance;
            }
            else if (phase1 == UnityEngine.InputSystem.TouchPhase.Moved || 
                     phase2 == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float deltaDistance = currentTouchDistance - lastTouchDistance;
                
                targetDistance -= deltaDistance * touchZoomSensitivity;
                targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);
                
                Camera cam = GetComponent<Camera>();
                if (cam != null && cam.orthographic)
                {
                    cam.orthographicSize = targetDistance / 2f;
                }
                
                lastTouchDistance = currentTouchDistance;
            }
        }
    }
    
    /// <summary>
    /// Kamera pozisyonunu günceller
    /// </summary>
    private void UpdateCameraPosition()
    {
        // Açıları radyana çevir
        float radX = currentAngleX * Mathf.Deg2Rad;
        float radY = currentAngleY * Mathf.Deg2Rad;
        
        // Küresel koordinatları hesapla
        Vector3 offset = new Vector3(
            Mathf.Sin(radY) * Mathf.Cos(radX) * currentDistance,
            Mathf.Sin(radX) * currentDistance,
            Mathf.Cos(radY) * Mathf.Cos(radX) * currentDistance
        );
        
        transform.position = targetPosition + offset;
        transform.LookAt(targetPosition);
    }
    
    /// <summary>
    /// Puzzle'a odaklanır
    /// </summary>
    public void FocusOnPuzzle(List<Cube> cubes)
    {
        if (cubes == null || cubes.Count == 0) return;
        
        // Merkez noktayı hesapla
        Vector3 center = Vector3.zero;
        Vector3 min = Vector3.one * float.MaxValue;
        Vector3 max = Vector3.one * float.MinValue;
        
        foreach (var cube in cubes)
        {
            center += cube.transform.position;
            min = Vector3.Min(min, cube.transform.position);
            max = Vector3.Max(max, cube.transform.position);
        }
        
        center /= cubes.Count;
        targetPosition = center;
        
        // Boyuta göre zoom ayarla (daha uzaktan bak)
        float size = Vector3.Distance(min, max);
        targetDistance = Mathf.Max(size * 2.5f + 5f, minZoom);
        targetDistance = Mathf.Min(targetDistance, maxZoom);
        
        // Orthographic size güncelle
        Camera cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = targetDistance / 2f;
        }
        
        // İzometrik açıya sıfırla
        targetAngleX = 35f;
        targetAngleY = -145f;
    }
    
    /// <summary>
    /// Kamerayı döndürür (InputHandler'dan çağrılır)
    /// </summary>
    public void RotateCamera(float deltaX, float deltaY)
    {
        targetAngleY += deltaX;
        targetAngleX += deltaY;
        targetAngleX = Mathf.Clamp(targetAngleX, -89f, 89f);
    }
    
    /// <summary>
    /// Kamerayı zoom yapar (InputHandler'dan çağrılır)
    /// </summary>
    public void ZoomCamera(float delta)
    {
        targetDistance += delta;
        targetDistance = Mathf.Clamp(targetDistance, minZoom, maxZoom);
        
        Camera cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            cam.orthographicSize = targetDistance / 2f;
        }
    }
    
    /// <summary>
    /// Kamerayı belirli bir açıya döndürür
    /// </summary>
    public void SetViewAngle(float angleX, float angleY)
    {
        targetAngleX = Mathf.Clamp(angleX, -89f, 89f);
        targetAngleY = angleY;
    }
    
    /// <summary>
    /// Ön görünüme geç
    /// </summary>
    public void SetFrontView()
    {
        SetViewAngle(0f, 0f);
    }
    
    /// <summary>
    /// Üst görünüme geç
    /// </summary>
    public void SetTopView()
    {
        SetViewAngle(90f, 0f);
    }
    
    /// <summary>
    /// İzometrik görünüme geç
    /// </summary>
    public void SetIsometricView()
    {
        SetViewAngle(30f, 45f);
    }
}
