using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SFXSource;

    [SerializeField] private AudioData[] BGMClips;
    [SerializeField] private AudioData[] SFXClips;

    public static AudioManager Instance;

    private void Awake()
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

    public void PlayBGM(string name)
    {
        AudioClip clip = GetClip(BGMClips, name);
        if (clip != null)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
        else
        {
            Debug.LogWarning("BGM dengan nama '" + name + "' tidak ditemukan di AudioManager!");
        }
    }

    public void PlaySFX(string name)
    {
        AudioClip clip = GetClip(SFXClips, name);
        if (clip != null)
        {
            SFXSource.PlayOneShot(clip); // Menggunakan PlayOneShot agar SFX tidak saling potong jika diputar cepat
        }
        else
        {
            Debug.LogWarning("SFX dengan nama '" + name + "' tidak ditemukan di AudioManager!");
        }
    }

    // Fungsi untuk memutar AudioClip kustom secara eksklusif (suara lama dihentikan dulu agar tidak tumpang tindih)
    public void PlayCustomSFX(AudioClip clip)
    {
        if (clip != null && SFXSource != null)
        {
            SFXSource.Stop(); // Hentikan suara dialog sebelumnya yang masih berputar
            SFXSource.clip = clip;
            SFXSource.Play(); // Putar suara dialog yang baru
        }
    }

    public void StopBGM()
    {
        if (BGMSource != null) BGMSource.Stop();
    }

    public void StopSFX()
    {
        if (SFXSource != null) SFXSource.Stop();
    }

    private AudioClip GetClip(AudioData[] clipArray, string name)
    {
        if (clipArray == null) return null;

        foreach (AudioData clipData in clipArray)
        {
            if (clipData.name == name)
            {
                return clipData.clip;
            }
        }

        return null;
    }
}

[System.Serializable]
public class AudioData
{
    public string name;
    public AudioClip clip;
}