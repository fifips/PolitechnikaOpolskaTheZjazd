using UnityEngine;

public class CarControls : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 5f;
    public float maxSpeed = 8f;
    public float braking = 8f;
    public float turnSpeed = 120f;

    private Rigidbody2D rb;
    private float currentSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Vertical");
        float turnInput = Input.GetAxisRaw("Horizontal");

        // Przyspieszanie i cofanie
        if (moveInput != 0)
        {
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed, 0,
                braking * Time.fixedDeltaTime
            );
        }

        currentSpeed = Mathf.Clamp(
            currentSpeed, -maxSpeed * 0.5f, maxSpeed
        );

        // Ruch samochodu
        rb.linearVelocity = transform.up * currentSpeed;

        // Skrêcanie zale¿ne od prêdkoœci
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float direction = currentSpeed >= 0 ? 1 : -1;

            float rotation = -turnInput * turnSpeed *
                             direction *
                             Time.fixedDeltaTime;

            rb.MoveRotation(rb.rotation + rotation);
        }
    }
}
