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

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;


    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // 싱글톤 유지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        // 오디오 소스 구성
        if (bgmSource != null && bgmGroup != null)
            bgmSource.outputAudioMixerGroup = bgmGroup;

        if (sfxSource != null && sfxGroup != null)
            sfxSource.outputAudioMixerGroup = sfxGroup;
    }

    // BGM 재생
    public void PlayBGM(string clipName, float fadeTime = 1f)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null) return;

        if (bgmSource.isPlaying)
            StartCoroutine(FadeOutIn(bgmSource, clip, fadeTime));
        else
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    // SFX 재생
    public void PlaySFX(string clipName)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null) return;

        sfxSource.PlayOneShot(clip);
    }

    // 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("BGM", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
        else
            bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20);
        else
            sfxSource.volume = volume;
    }

    // 페이드 전환
    private System.Collections.IEnumerator FadeOutIn(AudioSource source, AudioClip newClip, float time)
    {
        float startVol = source.volume;
        float t = 0f;
        while (t < time)
        {
            source.volume = Mathf.Lerp(startVol, 0f, t / time);
            t += Time.deltaTime;
            yield return null;
        }

        source.Stop();
        source.clip = newClip;
        source.Play();

        t = 0f;
        while (t < time)
        {
            source.volume = Mathf.Lerp(0f, startVol, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }

    // 클립 로드 (캐싱)
    private AudioClip LoadClip(string clipName)
    {
        if (clipCache.ContainsKey(clipName))
            return clipCache[clipName];

        AudioClip clip = Resources.Load<AudioClip>($"Sound/{clipName}");
        if (clip == null)
        {
            Debug.LogWarning($"[SoundManager] 사운드 파일을 찾을 수 없습니다: {clipName}");
            return null;
        }

        clipCache.Add(clipName, clip);
        return clip;
    }
    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
}