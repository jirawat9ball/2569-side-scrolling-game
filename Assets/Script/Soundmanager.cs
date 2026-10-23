using UnityEngine;

public class Soundmanager : MonoBehaviour
{

    public static Soundmanager instance;
    public AudioSource musicSource;
    public AudioSource VfxClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        VfxClip = gameObject.AddComponent<AudioSource>();
    }

    public void PlayVFX(AudioClip clip)
    {
        VfxClip.PlayOneShot(clip);
    }

}
