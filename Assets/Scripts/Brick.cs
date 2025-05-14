using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class Brick : MonoBehaviour
{
    public float brickLength;
    public float brickHeight;
    public int brickDamage = 0;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BoxCollider2D box2d;
    [SerializeField]
    private Sprite[] brickSprites;


    // Update is called once per frame
    void Update()
    {
        brickLength = box2d.size.x * transform.localScale.x;
        brickHeight = box2d.size.y * transform.localScale.y;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            brickDamage++;
            spriteRenderer.sprite = brickSprites[brickDamage];
            StartCoroutine(DisableBrickCollider());
            if (brickDamage == 3)
            {
                Destroy(gameObject);
            }
        }
    }
    IEnumerator DisableBrickCollider()
    {
        box2d.enabled = false;
        for (int i = 0; i < 3; i++) //waits 5 frames
        {
            yield return null;
        }
        box2d.enabled = true;
    }
}
