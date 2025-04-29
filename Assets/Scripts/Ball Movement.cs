using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class BallMovement : MonoBehaviour
{
    private Vector3 directionVector = Vector3.down;
    private float speed = 4f;
    [SerializeField]
    private Rigidbody2D rb2d;
    [SerializeField]
    private Paddle paddle;
    [SerializeField]
    private Brick brick;
    [SerializeField]
    private CircleCollider2D ball;

    private GameObject reflector;
    private float worldContactPointx;
    private float worldContactPointy;
    private float localContactPoint;
    private float paddleContact;
    private float maxAngle = 70f;
    private float ballTravelAngle;


    void Update()
    {

        if (transform.position.y <= -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//reset
        }
       
    }

    private void FixedUpdate()
    {
        rb2d.linearVelocity = directionVector * speed;
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
        StartCoroutine(DisableColliderTemporarily(1f));
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
        //Debug.Log($"{worldContactPointx - reflector.transform.position.x},{worldContactPointy - reflector.transform.position.y}");
        //Debug.Log(Mathf.Abs(worldContactPointx - reflector.transform.position.x));
        Brick brick = reflector.GetComponent<Brick>();
        if (Mathf.Abs(worldContactPointx - reflector.transform.position.x) <= 0.5 * brick.brickLength)
        {
            TopBounce();
        }
        else
        {
            SideBounce();
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
}
