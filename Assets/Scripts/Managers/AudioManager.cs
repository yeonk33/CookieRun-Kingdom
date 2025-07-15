using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioClip[] _bgms;
    [SerializeField] private AudioClip[] _sfxs;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Init();
        Play();
    }

    private void Init()
    {
        _bgmSource = GetComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f); // 기본값 0.5f
    }

    private void Play()
    {
        _bgmSource.clip = _bgms[(int)BGM.Main]; // 기본 BGM 설정
        _bgmSource.Play();
    }

    public void OnClickMute()
    {
        _bgmSource.mute = !_bgmSource.mute;
    }
}

public enum BGM
{
    Main, Arena
}
