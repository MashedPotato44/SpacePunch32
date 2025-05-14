using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class BallPointer : MonoBehaviour
{

    [SerializeField]
    private BallMovement ball;
    [SerializeField]
    private Paddle paddle;
    [SerializeField]
    private Rigidbody2D rb2d;

    private Vector3 mousePos;
    private Vector3 pointerVector;



    private void Start()
    {
        transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y + 1f, 0f);

    }
    void FixedUpdate()
    {
        Vector3 mouseScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePos = new Vector3(mouseScreenPos.x, mouseScreenPos.y,0);
        pointerVector = new Vector3(mouseScreenPos.x - ball.transform.position.x, mouseScreenPos.y - ball.transform.position.y,0);
        pointerVector = pointerVector.normalized;
        Debug.Log(ball.transform.position);

        transform.position = ball.transform.position + pointerVector;

    }
}

        /*
        if (paddle.wallCollide) //Fixes ball pointer position when the paddle collides with wall during hold
        {
            rb2d.linearVelocity = Vector2.zero;
        }
        else
        {
            paddle.movementInput = paddle.movement.action.ReadValue<Vector2>();
            //rb2d.linearVelocity = paddle.movementInput * paddle.paddleSpeed;
            Vector2 movement = paddle.movementInput * paddle.paddleSpeed * Time.fixedDeltaTime;
            rb2d.MovePosition(rb2d.position + movement);
        }
        */