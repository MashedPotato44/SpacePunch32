using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    private InputActionReference movement; //paddle input controls
    private Vector2 movementInput; //input vectors
    private int paddleSpeed = 12;
    [SerializeField]
    private Rigidbody2D _rb2d;
    [SerializeField]
    private BoxCollider2D box2d;
   
    public float paddleLength;


    private void FixedUpdate()
    {
        _rb2d.linearVelocity = movementInput * paddleSpeed; //input speed calcs
    }
    void Update()
    {
        paddleLength = box2d.size.x * transform.localScale.x;

        movementInput = movement.action.ReadValue<Vector2>(); //reads input vectors
    }

    private void OnEnable()
    {
        movement.action.Enable();
    }

    private void OnDisable()
    {
        movement.action.Disable();
    }
}
