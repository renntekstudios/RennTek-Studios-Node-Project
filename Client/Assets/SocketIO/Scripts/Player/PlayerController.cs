using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;


public class PlayerController : MonoBehaviour {

    
	PlayerName playerName;
	public Vector3 position;
	private Vector3 prePosition_ = Vector3.zero;
	private Quaternion preRotation_;
	public string	id;
	
	private TestSocketIO3 socket;
	public float verticalSpeed = 3f;
	public float rotateSpeed = 20f;
	public bool m_jump;
	Rigidbody myRigidbody;
	public float jumpForce;
    private bool canJump = true;
    public bool isGrounded;
	// Use this for initialization
	void Start () {
	
	    
		socket = FindObjectOfType (typeof(TestSocketIO3)) as TestSocketIO3;
		playerName = GetComponentInChildren<PlayerName> () as PlayerName;
		playerName.setName(this.name);

		jumpForce = 200f;
		myRigidbody = GetComponent<Rigidbody> ();
		//this .name = playerName;
		
	}
	
	// Update is called once per frame
	void Update () {
	   
	   Move();
	
	}

    public void Move()
    {

        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        m_jump = CrossPlatformInputManager.GetButtonDown("Jump");


        if (v > 0)//up button or joystick
            transform.Translate(new Vector3(0, 0, 1 * verticalSpeed * Time.deltaTime));
        if (v < 0)//down button or joystick
            transform.Translate(new Vector3(0, 0, -1 * verticalSpeed * Time.deltaTime));


        if (h > 0)//right button or joystick
            this.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        if (h < 0)//left button or joystick
            this.transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);

        //user press any button(Up or down) and player moving
        if (v != 0)
        {
            socket.EmitPosition(transform.position);//call method NetworkSocketIO.EmitPosition for transmit new  player position to all clients in game
        }

        //user press any button(Right or Left) and player moving
        if (h != 0)//if new rotation
        {
            socket.EmitRotation(transform.rotation);//call method NetworkSocketIO.EmitRotation for transmit new  player rotation to all clients in game
        }

        //jump

        if (m_jump && canJump && isGrounded)
        {
            canJump = false;
            isGrounded = false;
            socket.EmitJump(transform.position);
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
            Debug.Log("JUMP");
        }

        if (isGrounded) { canJump = true; }

        if (!canJump) { socket.EmitJump(transform.position); }
	}

    void OnCollisionEnter(Collision col) { if (!isGrounded) { if (col.gameObject.name == "Ground") { isGrounded = true; } } }
}
