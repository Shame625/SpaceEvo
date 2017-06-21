using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float RotateSpeed = 20;

	public float speed = 0;
	public float maxSpeed = 0;
	float Acceleration = 0;
	float Deceleration = 0;

	//Debug stuff, I hope...
	public bool rotateLeft = false;
	public bool rotateRight = false;
	public bool goForward = false;
	public bool goBackwards = false;


	void Update () 
	{
		if (Input.GetKey(KeyCode.A) || rotateLeft)
			transform.Rotate(Vector3.forward * RotateSpeed * Time.deltaTime);
		else if (Input.GetKey(KeyCode.D) || rotateRight)
			transform.Rotate(-Vector3.forward * RotateSpeed * Time.deltaTime);

		if (Input.GetKey (KeyCode.S) || goBackwards)
		{
			if (speed < maxSpeed)
				speed = speed - Acceleration/4 * Time.deltaTime;
		}
		else if (Input.GetKey (KeyCode.W) || goForward)
		{
			if(speed > -maxSpeed)
				speed = speed + Acceleration * Time.deltaTime;
		}
		else
		{
			if(speed > Deceleration * Time.deltaTime)
				speed = speed - Deceleration * Time.deltaTime;
			else if(speed < -Deceleration * Time.deltaTime)
				speed = speed + Deceleration * Time.deltaTime;
			else
				speed = 0;
		}

		if (speed > maxSpeed) {
			speed = maxSpeed;
		} else if (speed < -maxSpeed) {
			speed = -maxSpeed;
		}
		transform.position += transform.up * speed * Time.deltaTime;
	}

    public void SetSpeed(float max, float acc)
    {
        maxSpeed = max;
        Acceleration = acc;
        Deceleration = acc;
    }

    public void SetRotation(float rotation)
    {
        RotateSpeed = rotation;
    }
}
