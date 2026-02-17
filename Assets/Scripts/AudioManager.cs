using UnityEngine;

/// <summary>
/// Ses efektleri ve müzik yönetimi
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    
    [Header("Sound Effects")]
    public AudioClip selectSound;
    public AudioClip swapSound;
    public AudioClip correctPlacementSound;
    public AudioClip wrongPlacementSound;
    public AudioClip levelCompleteSound;
    public AudioClip buttonClickSound;
    
    [Header("Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    public bool sfxEnabled = true;
    public bool musicEnabled = true;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        LoadSettings();
        CreateDefaultSounds();
    }
    
    /// <summary>
    /// Audio source'ları başlatır
    /// </summary>
    private void InitializeAudioSources()
    {
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX Source");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }
    }
    
    /// <summary>
    /// Prosedürel ses efektleri oluşturur
    /// </summary>
    private void CreateDefaultSounds()
    {
        // Eğer ses yoksa prosedürel ses oluştur
        if (selectSound == null)
            selectSound = CreateToneClip(440f, 0.1f, 0.3f); // A4 note
        
        if (swapSound == null)
            swapSound = CreateSweepClip(300f, 500f, 0.15f, 0.4f);
        
        if (correctPlacementSound == null)
            correctPlacementSound = CreateToneClip(523f, 0.2f, 0.4f); // C5 note
        
        if (levelCompleteSound == null)
            levelCompleteSound = CreateChordClip(new float[] { 523f, 659f, 784f }, 0.5f, 0.5f); // C major chord
        
        if (buttonClickSound == null)
            buttonClickSound = CreateToneClip(660f, 0.05f, 0.2f);
    }
    
    /// <summary>
    /// Tek tonlu ses oluşturur
    /// </summary>
    private AudioClip CreateToneClip(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        
        AudioClip clip = AudioClip.Create("Tone", sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];
        
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration); // Fade out
            envelope = Mathf.Pow(envelope, 2f);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
    
    /// <summary>
    /// Frekans sweep sesi oluşturur
    /// </summary>
    private AudioClip CreateSweepClip(float startFreq, float endFreq, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        
        AudioClip clip = AudioClip.Create("Sweep", sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];
        
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float normalizedT = t / duration;
            float frequency = Mathf.Lerp(startFreq, endFreq, normalizedT);
            float envelope = 1f - normalizedT;
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
    
    /// <summary>
    /// Akor sesi oluşturur
    /// </summary>
    private AudioClip CreateChordClip(float[] frequencies, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.RoundToInt(sampleRate * duration);
        
        AudioClip clip = AudioClip.Create("Chord", sampleCount, 1, sampleRate, false);
        float[] samples = new float[sampleCount];
        
        float normalizedVolume = volume / frequencies.Length;
        
        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration);
            envelope = Mathf.Pow(envelope, 1.5f);
            
            float sample = 0f;
            foreach (float freq in frequencies)
            {
                sample += Mathf.Sin(2f * Mathf.PI * freq * t);
            }
            
            samples[i] = sample * envelope * normalizedVolume;
        }
        
        clip.SetData(samples, 0);
        return clip;
    }
    
    /// <summary>
    /// Ses efekti çalar
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled || clip == null || sfxSource == null) return;
        
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
    
    /// <summary>
    /// Seçim sesi çalar
    /// </summary>
    public void PlaySelect()
    {
        PlaySFX(selectSound);
    }
    
    /// <summary>
    /// Değiştirme sesi çalar
    /// </summary>
    public void PlaySwap()
    {
        PlaySFX(swapSound);
    }
    
    /// <summary>
    /// Doğru yerleştirme sesi
    /// </summary>
    public void PlayCorrect()
    {
        PlaySFX(correctPlacementSound);
    }
    
    /// <summary>
    /// Level tamamlama sesi
    /// </summary>
    public void PlayLevelComplete()
    {
        PlaySFX(levelCompleteSound);
    }
    
    /// <summary>
    /// Buton tıklama sesi
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }
    
    /// <summary>
    /// SFX sesini ayarlar
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }
    
    /// <summary>
    /// Müzik sesini ayarlar
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        SaveSettings();
    }
    
    /// <summary>
    /// SFX'i açar/kapatır
    /// </summary>
    public void ToggleSFX()
    {
        sfxEnabled = !sfxEnabled;
        SaveSettings();
    }
    
    /// <summary>
    /// Müziği açar/kapatır
    /// </summary>
    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        if (musicSource != null)
        {
            if (musicEnabled)
                musicSource.UnPause();
            else
                musicSource.Pause();
        }
        SaveSettings();
    }
    
    /// <summary>
    /// Ayarları kaydeder
    /// </summary>
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Ayarları yükler
    /// </summary>
    private void LoadSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
}
