using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

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

    public static void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume)
    {
        if (Instance == null || clip == null) { return; }
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
