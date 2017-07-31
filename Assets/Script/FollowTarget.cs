using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FollowTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] bool autoTargetPlayer = true;





    protected virtual void Start()
    {
        if (autoTargetPlayer)
        {
            FindTargetPlayer();
        }
    }
    

    void FixedUpdate()
    {
        if(autoTargetPlayer && (target == null || !target.gameObject.activeSelf))
        {
            FindTargetPlayer();
        }

        if (target != null && (target.GetComponent<Rigidbody>() != null || !target.GetComponent<Rigidbody>().isKinematic))
        {
            Follow(Time.deltaTime);
        }
    }

    protected abstract void Follow(float deltaTime);


    public void FindTargetPlayer()
    {
        if (target == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag("Player");

            if (targetObj)
            {
                SetTarget(targetObj.transform);
            }
        }
    }

    public virtual void SetTarget(Transform newTransform)
    {
        target = newTransform;
    }

    public Transform Target{get{ return this.target; } }
}
