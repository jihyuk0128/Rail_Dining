using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    // BGM 여러개 지원
    private List<AudioSource> bgmSources = new List<AudioSource>();
    private int bgmChannelCount = 2; // 동시에 2개의 BGM 재생 가능

    // SFX 풀 
    private List<AudioSource> sfxSources = new List<AudioSource>();
    private int sfxPoolSize = 10; // 동시에 10개의 SFX 재생 가능

    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;

    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateBGMSources();
        CreateSFXPool();
    }

    //  BGM 초기화 (복수 채널)
    private void CreateBGMSources()
    {
        for (int i = 0; i < bgmChannelCount; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.outputAudioMixerGroup = bgmGroup;
            bgmSources.Add(src);
        }
    }


    //  SFX 풀 생성
    private void CreateSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.outputAudioMixerGroup = sfxGroup;
            sfxSources.Add(src);
        }
    }

    //  BGM 재생 (어떤 채널에서든 재생 가능)
    public void PlayBGM(string clipName, int channel = 0)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null) return;

        if (channel < 0 || channel >= bgmSources.Count)
        {
            Debug.LogWarning("[SoundManager] BGM 채널 번호 오류");
            return;
        }

        AudioSource src = bgmSources[channel];
        src.clip = clip;
        src.Play();
    }

    //  특정 BGM 채널 정지
    public void StopBGM(int channel = 0)
    {
        if (channel < 0 || channel >= bgmSources.Count) return;
        bgmSources[channel].Stop();
    }


    //  모든 BGM 정지
    public void StopAllBGM()
    {
        foreach (var src in bgmSources)
            src.Stop();
    }

    //  SFX 재생 (풀에서 빈 AudioSource 사용)
    public void PlaySFX(string clipName)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null) return;

        AudioSource freeSrc = sfxSources.Find(s => !s.isPlaying);

        if (freeSrc == null)
        {
            // 전부 재생중이면 첫번째 것을 덮어씌움
            freeSrc = sfxSources[0];
        }

        freeSrc.PlayOneShot(clip);
    }

    // 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (audioMixer != null)
            audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (audioMixer != null)
            audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
    }

    // 클립 로드 (캐싱)
    private AudioClip LoadClip(string clipName)
    {
        if (clipCache.ContainsKey(clipName))
            return clipCache[clipName];

        AudioClip clip = Resources.Load<AudioClip>($"Sound/{clipName}");
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] 사운드 파일 없음: {clipName}");
            return null;
        }

        clipCache.Add(clipName, clip);
        return clip;
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
}