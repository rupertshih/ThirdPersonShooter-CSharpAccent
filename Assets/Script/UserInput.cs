using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour 
{
	public bool walkByDefault = false;
    public float aimingWeight; // camera
    public bool aim;            // camera
    public bool lookInCameraDirection;
    public ParticleSystem particleSys;

    //IK stuff
    public Transform spine;
    public float aimingZ = 213.46f;
    public float aimingX = -65.93f;
    public float aimingY = 20.1f;
    public float point = 30f;


    private CharacterMovement charMove;
	private Transform cam;
	private Vector3 camForward;
	private Vector3 move;
    private Vector3 lookPos;



    Animator anim;





	// Use this for initialization
	void Start ()
	{
		if (Camera.main != null)
		{
			cam = Camera.main.transform;
		}

		charMove = GetComponent<CharacterMovement>();
        anim = GetComponent<Animator>();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("JoyHorizontal");
        float vertical = Input.GetAxis("JoyVertical");

        if (!aim) // do as usual
        { 
            if (cam != null)
            {
                camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
                move = vertical * camForward + horizontal * cam.right;
            }
            else
            {
                move = vertical * Vector3.forward + horizontal * Vector3.right;
            }
        }
        else // unable to move but look at where the camera is looking at
        {
            move = Vector3.zero;
            Vector3 dir = lookPos - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);

            anim.SetFloat("Forward", vertical);
            anim.SetFloat("Turn", horizontal);
        }


		if (move.magnitude > 1)  // make sure that the movement is normalized
		{
			move.Normalize ();
		}

        bool walkToogle = Input.GetButton("WalkRun") || aim; // Input.GetKey (KeyCode.LeftShift);

		float walkMultiplier = 1f;
		if (walkByDefault) 
		{
			if (walkToogle) 
			{
				walkMultiplier = 1f; // run
			} 
			else 
			{
				walkMultiplier = 0.5f; // walk
			}
		}
		else 
		{
			if (walkToogle) 
			{
				walkMultiplier = 0.5f; // walk
			} 
			else 
			{
				walkMultiplier = 1f; // run
			}
		}

        lookPos = lookInCameraDirection && cam != null ? transform.position + cam.forward * 100 : transform.position + transform.forward * 100;

		move *= walkMultiplier;
		charMove.Move(move, aim, lookPos);
	}


    private void Update()
    {
        aim = Input.GetButton("Aim"); //Input.GetMouseButton(1); // right mouse click to zoom in
        if (aim)
        {
            if(Input.GetAxisRaw("Fire1") < 0) // right 3rd axis of joystick
            {
                anim.SetTrigger("Fire");
                particleSys.Emit(1);
            }
        }
    }


    private void LateUpdate()
    {

        aimingWeight = Mathf.MoveTowards(aimingWeight, (aim) ? 1f : 0f, Time.deltaTime * 5);

        Vector3 normalState = new Vector3(0, 0, -2f);
        Vector3 aimingState = new Vector3(0, 0, -0.5f);

        Vector3 pos = Vector3.Lerp(normalState, aimingState, aimingWeight);

        cam.transform.localPosition = pos;

        if (aim)
        {
            Vector3 eulerAngleOffset = Vector3.zero;
            eulerAngleOffset = new Vector3(aimingX, aimingY, aimingZ);

            Ray ray = new Ray(cam.position, cam.forward);
            Vector3 lookPosition = ray.GetPoint(point);
            spine.LookAt(lookPosition);
            spine.Rotate(eulerAngleOffset, Space.Self);
        }
    }
}
