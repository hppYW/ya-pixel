using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Setting")]

    public float JumpForce;
    public float MoveSpeed;

    [Header("Reference")]

    public Rigidbody2D rigid;
    public bool isGrounded = true;
    private float MoveInput;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();   
    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        if(rigid.linearVelocityX > MoveSpeed) // right max speed
        {
            rigid.linearVelocity = new Vector2(MoveSpeed, rigid.linearVelocityY);
        }
        else if (rigid.linearVelocityX < MoveSpeed*(-1))// left max speed
        {
            rigid.linearVelocity = new Vector2(MoveSpeed*(-1), rigid.linearVelocityY);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded){
            rigid.AddForceY(JumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
        MoveInput = Input.GetAxisRaw("Horizontal"); 
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Tilemap"){
            isGrounded = true;
        }
    }
}
