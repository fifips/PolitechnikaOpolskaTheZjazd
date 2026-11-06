
using UnityEngine;
using UnityEngine.InputSystem; //korzysta z systemu odczytu klawiszy na klawiaturze w Unity

//Wymusza obecność komponentu Rigidbody2D na obiekcie, do którego jest przypisany ten skrypt
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerControls : MonoBehaviour
{
    [SerializeField] //Umożliwia ustawienie prędkości gracza w edytorze Unity
    private float playerSpeed = 5f; 

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.zero;

    // Awake zostaje wywołane przed metodą Start i jest używane do inicjalizacji komponentów
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update zostaje wywołane raz na klatkę
    void Update()
    {
        direction = Vector2.zero;

        //Interakcja (klawisz E)
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }

        //Ruch gracza (klawisze WASD)
        if (Keyboard.current.wKey.isPressed)
        {
            direction += Vector2.up;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            direction += Vector2.down;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            direction += Vector2.right;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            direction += Vector2.left;
        }

        direction = direction.normalized;
        //Normalizacja wektora kierunku, aby uniknąć szybszego poruszania się po przekątnej
    }

    void FixedUpdate()
    {
        Move(direction);
    }

    void Move(Vector2 direction)
    {
        rb.linearVelocity = direction * playerSpeed;
        //Gracz porusza się w kierunku wektora direction, z prędkością playerSpeed
    }

    void Interact()
    {
        // PLACEHOLDER
        Debug.Log("Interakcja z obiektem! Placeholder! Do zaimplementowania!");
    }
}
