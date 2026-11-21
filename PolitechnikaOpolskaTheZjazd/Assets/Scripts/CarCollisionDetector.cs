
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarCollisionDetector : MonoBehaviour
{
    [Header("Dźwięki kolizji")]
    [SerializeField] private AudioClip[] crashSounds;

    private CarControls carControls;
    private AudioSource audioSource;

    void Awake()
    {
        carControls = GetComponent<CarControls>();
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("ParkedCar"))
            return;

        carControls.StopImmediately();

        if (crashSounds != null && crashSounds.Length > 0)
        {
            AudioClip sound = crashSounds[Random.Range(0, crashSounds.Length)];

            if (sound != null)
                audioSource.PlayOneShot(sound);
        }

        Debug.LogWarning("Uderzyłeś w zaparkowany samochód!");
    }
}
