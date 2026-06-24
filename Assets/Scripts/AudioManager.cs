using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMatch()
    {
        if (audioSource == null) return;
        // Generate a simple beep sound procedurally
        audioSource.PlayOneShot(GenerateBeep(660f, 0.1f));
    }

    public void PlaySwap()
    {
        if (audioSource == null) return;
        audioSource.PlayOneShot(GenerateBeep(440f, 0.05f));
    }

    public void PlayFail()
    {
        if (audioSource == null) return;
        audioSource.PlayOneShot(GenerateBeep(220f, 0.15f));
    }

    AudioClip GenerateBeep(float frequency, float duration)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        AudioClip clip = AudioClip.Create("beep", samples, 1, sampleRate, false);
        float[] data = new float[samples];
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = 1f - (t / duration);
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * envelope * 0.5f;
        }
        clip.SetData(data, 0);
        return clip;
    }
}
