using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDirection : MonoBehaviour 
{

    public Transform muzzle;




	
	// Update is called once per frame
	void Update () 
	{
        transform.position = muzzle.TransformPoint(Vector3.zero);
        transform.forward = muzzle.TransformDirection(Vector3.forward);

	}

    private void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            Vector3 direction = other.transform.position - transform.position;
            direction = direction.normalized;
            other.GetComponent<Rigidbody>().AddForce(direction * 50);
        }
    }
}
