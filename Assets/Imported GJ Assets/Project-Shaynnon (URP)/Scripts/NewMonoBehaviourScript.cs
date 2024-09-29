using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private float speedX;

    [SerializeField]
    private float speedY;

    [SerializeField]
    private float gravity;

    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!grounded && body.velocity.y < 0)
            body.AddForce(new Vector2(0.0f, gravity));
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        body.velocity = new Vector2(horizontalInput * speedX, body.velocity.y);

        //flip player when moving left to right

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(4,4,0);


        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-4,4,0);


        if (Input.GetKey(KeyCode.W) && grounded)
            Jump();

        anim.SetBool("walk", horizontalInput != 0);
        anim.SetBool("grounded", grounded);
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, speedY);
        anim.SetTrigger("jump");
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            grounded = true;
    }
}