using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    [SerializeField] private AudioClip[] playlist;
    
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        PlayNextTrack();
    }

    private void Update()
    {
        if(!audioSource.isPlaying) PlayNextTrack();
    }

    private void PlayNextTrack()
    {
        if (playlist.Length <= 0) return;
        if(currentTrackIndex >= playlist.Length) currentTrackIndex = 0;

        audioSource.clip = playlist[currentTrackIndex];
        audioSource.Play();

        currentTrackIndex++;
    }

    public float GetVolume() => audioSource.volume;
    public void SetVolume(float value) => audioSource.volume = value;
}
