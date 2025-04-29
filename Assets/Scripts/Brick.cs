using UnityEngine;
using UnityEngine.UIElements;

public class Brick : MonoBehaviour
{
    public float brickLength;
    public float brickHeight;

    [SerializeField]
    private BoxCollider2D box2d;


    // Update is called once per frame
    void Update()
    {
        brickLength = box2d.size.x * transform.localScale.x;
        brickHeight = box2d.size.y * transform.localScale.y;
        //Debug.Log(brickHeight);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Destroy(gameObject);
            Debug.Log(brickLength);
        }
    }
}
