
using UnityEngine;

public class CameraFollowCar : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    public float followSpeed = 5f;
    public float lookAhead = 2.5f;

    private Vector3 currentLookAhead;

    void Start()
    {
        currentLookAhead = target.up * lookAhead;
        transform.position = target.position + offset + currentLookAhead;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // Płynna zmiana kierunku kamery
        currentLookAhead = Vector3.Lerp(
            currentLookAhead,
            target.up * lookAhead,
            followSpeed * Time.deltaTime
        );

        // Kamera podąża za samochodem bez opóźnienia
        transform.position = target.position + offset + currentLookAhead;
    }
}
