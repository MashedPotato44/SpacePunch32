using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BallMovement : MonoBehaviour
{
    private Vector3 directionVector = Vector3.up;
    private Vector3 worldClickPosition;
    private Vector2 ballTraj;
    private float speed = 5f;
    [SerializeField]
    private Rigidbody2D rb2d;
    [SerializeField]
    private Paddle paddle;
    [SerializeField]
    private Brick brick;
    [SerializeField]
    private CircleCollider2D ball;
    [SerializeField]

    public InputActionAsset inputAction;
    [SerializeField]
    private InputAction release;
    [SerializeField]
    private Animator animator;

    private GameObject reflector;
    private float worldContactPointx;
    private float worldContactPointy;
    //private float bounceError = 0.01f; adjust error for brick collision tuning purposes
    private float localContactPoint;
    private float paddleContact;
    private float maxAngle = 70f;
    private float ballTravelAngle;
    private float ballRadius;
    public bool ballRelease = false;

    private void Start()
    {
        animator.enabled = false;
        ballRadius = ball.radius * transform.localScale.x;
        Vector2 startPos = new Vector2(startPos.x = paddle.transform.position.x, paddle.transform.position.y + (0.5f * paddle.paddleHeight) + ballRadius);
        rb2d.MovePosition(startPos);
    }
    void Update()
    {

        if (transform.position.y <= -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//reset
            brick.brickDamage = 0;
        }
    }

    private void FixedUpdate()
    {
        if (ballRelease)
        {
            rb2d.linearVelocity = directionVector * speed;
        }

        else
        {
            BallHold();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        reflector = collision.gameObject;
        string reflectorName = reflector.tag;

        worldContactPointx = collision.contacts[0].point.x;
        worldContactPointy = collision.contacts[0].point.y;

        switch (reflectorName) 
        {
            case "Paddle":

                PaddleBounce();
                break;

            case "Wall":

                SideBounce();
                break;

            case "Ceiling":

                TopBounce();
                break;

            case "Brick":

                BrickBounce();
                break;
        }
    }

    private void PaddleBounce()
    {
        localContactPoint = paddle.transform.position.x;
        paddleContact = worldContactPointx - localContactPoint;

        float paddleLeftEdge = -1 * (paddle.paddleLength / 2);
        ballTravelAngle = ((((paddleContact - paddleLeftEdge) / paddle.paddleLength) * (maxAngle * 2)) - maxAngle);
        float ballTravelAngleRad = Mathf.Deg2Rad * ballTravelAngle;
        directionVector = new Vector3(Mathf.Sin(ballTravelAngleRad), Mathf.Cos(ballTravelAngleRad), 0);
        if (directionVector.y < Mathf.Sin(maxAngle))
        {
            directionVector.y = Mathf.Sin(maxAngle);
        }
        directionVector = directionVector.normalized; //ball angle control calcs
        StartCoroutine(DisableColliderTemporarily(0.25f)); //Prevents the paddle pushing the ball from the side
    }

    private void SideBounce()
    {
        directionVector.x = directionVector.x * -1;
        directionVector = directionVector.normalized; //reverse x vector when hit ceiling
    }

    private void TopBounce()
    {
        directionVector.y = directionVector.y * -1;
        directionVector = directionVector.normalized; //reverse y vector when hit ceiling
    }

    public void BrickBounce()
    {
        Brick brick = reflector.GetComponent<Brick>();
        if (Mathf.Abs(worldContactPointy - reflector.transform.position.y) <= (0.5 * brick.brickHeight))//bounce error required as collider calculates higher than it should
        {
            SideBounce();
        }
        else
        {
            TopBounce();
        }
        //Debug.Log(Mathf.Abs(worldContactPointy - reflector.transform.position.y));
        
    }

    private void BallHold()
    {
        animator.enabled = false;
        if (paddle.wallCollide) //Fixes ball position when the paddle collides with wall during hold
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }
        else 
        {
            rb2d.constraints = RigidbodyConstraints2D.None;
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            paddle.movementInput = paddle.movement.action.ReadValue<Vector2>();
            rb2d.linearVelocity = paddle.movementInput * paddle.paddleSpeed;
        }
    }
    IEnumerator DisableColliderTemporarily(float duration)
    {
        Collider2D paddleCollider = paddle.GetComponent<Collider2D>();
        Collider2D ballCollider = ball.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(paddleCollider, ballCollider);

        yield return new WaitForSeconds(duration);

        Physics2D.IgnoreCollision(paddleCollider, ballCollider, false); //disables collider to prevent the ball double bouncing
    }

    private void OnEnable()
    {
        var map = inputAction.FindActionMap("PaddleInput");
        release = map.FindAction("Release");
        release.Enable();
        release.performed += OnRelease; //Mouse click to release
    }

    private void OnDisable()
    {
        if (release != null)
        {
            release.performed -= OnRelease;
            release.Disable(); 
        }
    }

    private void OnRelease(InputAction.CallbackContext context)
    {
        animator.enabled = true;
        if (!ballRelease) //restricts the click to move to only happen when ball isn't released yet
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();//calculates direction the ball travels according to mouse click
            worldClickPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldClickPosition.z = 0;
            ballTraj = worldClickPosition - transform.position;


            if (ballTraj.y < 0) //can't click below paddle
            {
                ballRelease = false;
            }

            else if (Mathf.Abs(Mathf.Rad2Deg * Mathf.Atan(ballTraj.x / ballTraj.y)) > maxAngle) //forces ball to travel max angle if clicked to shallow
            {
                ballTraj.y = Mathf.Abs(ballTraj.x) / Mathf.Tan(maxAngle * Mathf.Deg2Rad);
                BallTrajectorySet();
            }
            else
            {
                BallTrajectorySet();
            }
        }
    }

    private void BallTrajectorySet()
    {
        directionVector = ballTraj;
        directionVector = directionVector.normalized;
        ballRelease = true;
    }

    public void BallCentre()
    {
        Vector3 pos = transform.position;
        pos.x = paddle.transform.position.x;
        transform.position = pos;
    }
}
