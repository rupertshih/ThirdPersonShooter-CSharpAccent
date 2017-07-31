using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour 
{

	float moveSpeedMultiplier = 1f;
	float stationaryTurnSpeed = 180f;
	float movingTurnSpeed = 360f;
    float turnAmount;
    float forwardAmount;
    float jumpPower = 10f;
    float autoTurnthreshold = 10f;
    float autoTurnSpeed = 20f;


    bool onGround;
    bool aim;


    Animator anim;

    
    Rigidbody rigidbody;

    Vector3 moveInput;
    Vector3 velocity;
    Vector3 currentLookPos;


    
	IComparer rayHitComparer;




	// Use this for initialization
	void Start () 
	{
		SetupAnimator ();
		rigidbody = GetComponent<Rigidbody>();
	}
	

	void SetupAnimator ()
	{
		anim = GetComponent<Animator>();

		foreach (Animator childAnimator in GetComponentsInChildren<Animator>()) {
			if (childAnimator != anim) 
			{
				anim.avatar = childAnimator.avatar;
				Destroy(childAnimator);
				break;				
			}
		}
	}

	void OnAnimatorMove ()
	{
		if (onGround && Time.deltaTime > 0) 
		{
			Vector3 v = (anim.deltaPosition * moveSpeedMultiplier)/Time.deltaTime;
			v.y = rigidbody.velocity.y;
			rigidbody.velocity = v;
		}
	}

	class RayHitComparer:IComparer
	{
		public int Compare (object x, object y)
		{
			return((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}

	void GroundCheck ()
	{
		Ray ray = new Ray (transform.position + Vector3.up * 0.1f, -Vector3.up);
		RaycastHit[] hits = Physics.RaycastAll (ray, 0.5f);
		rayHitComparer = new RayHitComparer ();

		System.Array.Sort (hits, rayHitComparer);

		if (velocity.y < jumpPower * 0.5f) 
		{
			//onGround = false;
			rigidbody.useGravity = true;
			foreach (var hit in hits) 
			{
				if (!hit.collider.isTrigger) 
				{
					if (velocity.y <= 0) 
					{
						rigidbody.position = Vector3.MoveTowards(rigidbody.position, hit.point, Time.deltaTime * 5);
					}

					onGround = true;
					rigidbody.useGravity = false;

					break;
				}
			}
		}
	}

	void ConvertMoveInput()
	{
		Vector3 localMove = transform.InverseTransformDirection(moveInput);

		turnAmount = Mathf.Atan2(localMove.x, localMove.z);
		forwardAmount = localMove.z;
	}

	void UpdateAnimator ()
	{
		anim.applyRootMotion = true;

		anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
		anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        anim.SetBool("Aim", aim);
    }

	void ApplyExtraTrunRotation ()
	{
		float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
		transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);		
	}

	public void Move (Vector3 move, bool aim, Vector3 lookPos)
	{
		if (move.magnitude > 1) 
		{
			move.Normalize ();
		}

		this.moveInput = move;
        this.aim = aim;
        this.currentLookPos = lookPos;

		velocity = rigidbody.velocity;
		ConvertMoveInput();
        if (!aim)
        {
            TurnTowardsCameraForward();
            ApplyExtraTrunRotation();
        }

		GroundCheck();
		UpdateAnimator();
		
	}

    void TurnTowardsCameraForward()
    {
        if(Mathf.Abs(forwardAmount) < 0.01f)
        {
            Vector3 lookDelta = transform.InverseTransformDirection(currentLookPos - transform.position);

            float lookAngle = Mathf.Atan2(lookDelta.x, lookDelta.z) * Mathf.Rad2Deg;

            if(Mathf.Abs(lookAngle) > autoTurnthreshold)
            {
                turnAmount += lookAngle * autoTurnSpeed * 0.001f;
            }
        }
    }
}
