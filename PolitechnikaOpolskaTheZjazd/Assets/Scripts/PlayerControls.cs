using System;
using UnityEngine;
using UnityEngine.InputSystem; //korzysta z systemu odczytu klawiszy na klawiaturze w Unity

//Wymusza obecność komponentu Rigidbody2D na obiekcie, do którego jest przypisany ten skrypt
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerControls : MonoBehaviour
{
    [SerializeField] //Umożliwia ustawienie prędkości gracza w edytorze Unity
    private float playerSpeed = 5f; 
    [SerializeField] //Umożliwia ustawienie prędkości gracza w edytorze Unity
    private float animationSpeed = 1.2f; //Umożliwia ustawienie prędkości animacji w edytorze Unity

    private Rigidbody2D rb;
    private Vector2 direction = Vector2.zero;
    private Animator animator;
    private string currentAnimation = "";
    private Vector2 lastDirection = Vector2.down; // Domyślny kierunek patrzenia gracza na dół

    //Kierunek, w którym patrzy gracz (potrzebny np. do wskazania, kogo gracz uderza linijką)
    public Vector2 Facing => lastDirection;

    //Gdy false, gracz nie może się ruszać (np. na ekranie podsumowania zadania)
    public bool InputEnabled { get; set; } = true;

    //Wywoływane przy każdym naciśnięciu klawisza interakcji. Zadania zapisują się tu, żeby reagować na E
    public event Action Interacted;

    // Awake zostaje wywołane przed metodą Start i jest używane do inicjalizacji komponentów
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        if (InputEnabled && Keyboard.current.wKey.isPressed)
        {
            direction += Vector2.up;
        }
        if (InputEnabled && Keyboard.current.sKey.isPressed)
        {
            direction += Vector2.down;
        }
        if (InputEnabled && Keyboard.current.dKey.isPressed)
        {
            direction += Vector2.right;
        }
        if (InputEnabled && Keyboard.current.aKey.isPressed)
        {
            direction += Vector2.left;
        }

        direction = direction.normalized;
        //Normalizacja wektora kierunku, aby uniknąć szybszego poruszania się po przekątnej

        UpdateAnimation();
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
        //Interakcja jest zdarzeniem: każdy, kto jej potrzebuje (np. zadanie egzaminu), zapisuje się do Interacted
        Interacted?.Invoke();
    }


    void UpdateAnimation()
    {
        string nextAnimation;

        if (direction == Vector2.zero)
        {
            if (lastDirection == Vector2.up)
            {
                nextAnimation = "idle_up";
            }
            else if (lastDirection == Vector2.down)
            {
                nextAnimation = "idle_down";
            }
            else if (lastDirection == Vector2.left)
            {
                nextAnimation = "idle_left";
            }
            else // lastDirection == Vector2.right
            {
                nextAnimation = "idle_right";
            }
        }
        else if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                nextAnimation = "move_right";
                lastDirection = Vector2.right;
            }
            else
            {
                nextAnimation = "move_left";
                lastDirection = Vector2.left;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                nextAnimation = "move_up";
                lastDirection = Vector2.up;
            }
            else
            {
                nextAnimation = "move_down";
                lastDirection = Vector2.down;
            }
        }

        if (nextAnimation != currentAnimation)
        {   
            animator.speed = animationSpeed; // Ustawienie prędkości animacji
            animator.Play(nextAnimation, 0, 0f);
            currentAnimation = nextAnimation;
        }
    }

}
