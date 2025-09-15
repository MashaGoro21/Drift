using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CarSound : MonoBehaviourPun, IPunObservable
{
    [Header("Speed & Pitch Settings")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;

    private float currentSpeed;
    private float pitchFromCar;
    
    private Rigidbody rb;
    private AudioSource audioSource;

    private bool isMine;
    private float networkPitch;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = SaveSystem.GetFloat(PrefsKeys.SFX_VOLUME);
        isMine = photonView.IsMine;
    }

    private void Update()
    {
        if(isMine) EngineSound();
        else
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, networkPitch, Time.deltaTime * 5f);
    }

    private void EngineSound()
    {
        currentSpeed = rb.velocity.magnitude;
        pitchFromCar = currentSpeed / 50f;

        if(currentSpeed < minSpeed)
        {
            audioSource.pitch = minPitch;
            return;
        }

        if(currentSpeed > maxSpeed)
        {
            audioSource.pitch = maxPitch;
            return;
        }

        audioSource.pitch = minPitch + pitchFromCar;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(audioSource.pitch);
        else networkPitch = (float)stream.ReceiveNext();
    }
}
