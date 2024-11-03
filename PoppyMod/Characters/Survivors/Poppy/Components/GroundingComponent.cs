using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.Components
{
	public class GroundingComponent : MonoBehaviour
	{
		private CharacterBody body;
		private CharacterMotor motor;
		private RigidbodyMotor rigidMotor;
		private float maxDuration = 4f;
		private float stopwatch;
		private float downForce = 10f;

		private void Awake()
		{
			//Debug.Log("GroundingComponent: Grounding component added successfully!");
			body = GetComponent<CharacterBody>();
			motor = GetComponent<CharacterMotor>();
			if (!motor)
			{
				rigidMotor = GetComponent<RigidbodyMotor>();
			}
			stopwatch = 0f;
		}

		private void FixedUpdate()
		{
            stopwatch += Time.fixedDeltaTime;
            if (!body.healthComponent.alive || stopwatch >= maxDuration)
            {
                Release();
            }
			if (motor)
			{
				motor.rootMotion += Vector3.downVector * downForce * Time.fixedDeltaTime;
			}
			else if (rigidMotor)
			{
				rigidMotor.rootMotion += Vector3.downVector * downForce * Time.fixedDeltaTime;

            }
        }

		private void Release()
		{
			Destroy(this);
		}
	}
}