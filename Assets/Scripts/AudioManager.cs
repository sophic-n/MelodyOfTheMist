using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip[] bgMusicClips;
    public AudioClip gameOverMusic;
    public AudioClip gameWinMusic;

    [Header("SFX Variations")]
    public AudioClip[] fluteCorrectNotes;
    public AudioClip[] fluteWrongNotes;
    public AudioClip[] sequenceSuccess;
    public AudioClip[] enemySpawns;
    public AudioClip[] enemyDefeated;
    public AudioClip[] energyHit;

    [Header("SFX Cooldown Settings")]
    public float sfxCooldown = 0.1f;

    private Dictionary<string, float> lastSFXPlayTimes = new Dictionary<string, float>();
    private Dictionary<string, int> lastPlayedIndexes = new Dictionary<string, int>();

    private Coroutine sfxLockCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayRandomMusic();
    }

    // ------------------ MUSIC ------------------

    public void PlayRandomMusic()
    {
        if (bgMusicClips == null || bgMusicClips.Length == 0) return;

        int index = Random.Range(0, bgMusicClips.Length);
        musicSource.clip = bgMusicClips[index];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // ------------------ SFX ------------------

    public void PlayRandomSFX(AudioClip[] clips, string key = "")
    {
        if (clips == null || clips.Length == 0) return;

        if (string.IsNullOrEmpty(key))
            key = clips[0].name;

        if (lastSFXPlayTimes.TryGetValue(key, out float lastTime))
        {
            if (Time.time - lastTime < sfxCooldown)
                return;
        }

        int index = GetNonRepeatingIndex(clips.Length, key);
        sfxSource.PlayOneShot(clips[index]);

        lastSFXPlayTimes[key] = Time.time;
        lastPlayedIndexes[key] = index;
    }

    public void PlaySFXNonOverlap(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxLockCoroutine != null)
            StopCoroutine(sfxLockCoroutine);

        sfxLockCoroutine = StartCoroutine(PlaySFXOnceFinished(clip));
    }

    public void PlaySFXNonOverlapRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        int index = Random.Range(0, clips.Length);
        PlaySFXNonOverlap(clips[index]);
    }

    private IEnumerator PlaySFXOnceFinished(AudioClip clip)
    {
        while (sfxSource.isPlaying)
        {
            yield return null;
        }

        sfxSource.PlayOneShot(clip);
    }

    private int GetNonRepeatingIndex(int length, string key)
    {
        int lastIndex = lastPlayedIndexes.ContainsKey(key) ? lastPlayedIndexes[key] : -1;
        int newIndex;

        do
        {
            newIndex = Random.Range(0, length);
        } while (length > 1 && newIndex == lastIndex);

        return newIndex;
    }
}
