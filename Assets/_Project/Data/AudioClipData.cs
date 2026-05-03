using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipData", menuName = "Sokoban/Audio Clip Data")]
public class AudioClipData : ScriptableObject
{
    [SerializeField] private SoundEntry[] _entries;

    public AudioClip GetClip(SoundType type)
    {
        foreach (var entry in _entries)
            if (entry.Type == type) return entry.Clip;
        return null;
    }

    public float GetVolume(SoundType type)
    {
        foreach (var entry in _entries)
            if (entry.Type == type) return entry.Volume;
        return 1f;
    }
}

[Serializable]
public class SoundEntry
{
    public SoundType Type;
    public AudioClip Clip;
    [Range(0f, 1f)] public float Volume = 1f;
}