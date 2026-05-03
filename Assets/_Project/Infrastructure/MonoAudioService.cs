using UnityEngine;

public class MonoAudioService : MonoBehaviour
{
    [SerializeField] private AudioClipData _clipData;
    [SerializeField] private AudioSource _sfxSource;

    private AudioServiceImpl _service;

    public AudioServiceBase Service
    {
        get
        {
            if (_service == null)
                _service = new AudioServiceImpl(_clipData, _sfxSource);
            return _service;
        }
    }

    private void Awake()
    {
        _service = new AudioServiceImpl(_clipData, _sfxSource);
    }
}

public class AudioServiceImpl : AudioServiceBase
{
    private readonly AudioClipData _clipData;
    private readonly AudioSource _source;

    public AudioServiceImpl(AudioClipData clipData, AudioSource source)
    {
        _clipData = clipData;
        _source = source;
    }

    public override void Play(SoundType soundType)
    {
        var clip = _clipData.GetClip(soundType);
        if (clip == null)
        {
            Debug.LogWarning($"[AudioService] No clip for {soundType}");
            return;
        }
        _source.PlayOneShot(clip, _clipData.GetVolume(soundType));
    }

    public override void SetMasterVolume(float volume)
    {
        _source.volume = volume;
    }
}