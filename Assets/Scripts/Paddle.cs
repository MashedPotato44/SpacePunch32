using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    public InputActionReference movement; //paddle input controls
    public Vector2 movementInput; //input vectors
    public int paddleSpeed = 12;
    [SerializeField]
    private Rigidbody2D _rb2d;
    [SerializeField]
    private BoxCollider2D box2d;
    [SerializeField]
    private BallMovement ball;
    public bool wallCollide = false;
   
    public float paddleLength;
    public float paddleHeight;


    private void FixedUpdate()
    {
        movementInput = movement.action.ReadValue<Vector2>(); //reads input vectors
        _rb2d.linearVelocity = movementInput * paddleSpeed; //input speed calcs
    }
    void Update()
    {
        paddleLength = box2d.size.x * transform.localScale.x;
        paddleHeight = box2d.size.y * transform.localScale.y;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Wall" && !ball.ballRelease)
        {
            ball.BallCentre();
            wallCollide = true;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && !ball.ballRelease)
        {
            ball.BallCentre();
            wallCollide = false;
        }
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
