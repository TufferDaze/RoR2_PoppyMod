using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.Components
{
	public class GroundingComponent : MonoBehaviour
	{
		private CharacterBody body;
		private CharacterMotor motor;
		private RigidbodyMotor rigidMotor;
        private float maxDuration = PoppyStaticValues.utility2GroundingDuration;
		private float stopwatch;
		private float motorDownForce;
		private float rigidDownForce = PoppyStaticValues.utility2RigidDownForce;

		private void Awake()
		{
			//Debug.Log("GroundingComponent: Grounding component added successfully!");
			body = GetComponent<CharacterBody>();
			motor = GetComponent<CharacterMotor>();
            stopwatch = 0f;

            if (motor)
			{
				//motor.isFlying = false;
				//motor.useGravity = true;
			}
			else
			{
                rigidMotor = GetComponent<RigidbodyMotor>();
            }
		}

		private void FixedUpdate()
		{
            stopwatch += Time.fixedDeltaTime;
			//Debug.Log("GroundingComponent: Stopwatch is at " + stopwatch);
            if (stopwatch >= maxDuration || !body.healthComponent.alive)
            {
                Release();
            }
			if (motor)
			{
				motor.rootMotion += Vector3.downVector * 10f * Time.fixedDeltaTime;
			}
			else if (rigidMotor)
			{
				rigidMotor.rigid.AddForce(Vector3.downVector * rigidDownForce, ForceMode.VelocityChange);
            }
        }

		private void Release()
		{
			if (motor)
			{
				motor.ApplyForce(Vector3.upVector * 10f, true);
			}
			else
			{
                rigidMotor.rigid.AddForce(Vector3.upVector * 15f, ForceMode.VelocityChange);
            }
			//Debug.LogWarning("GroundingComponent: Removing Grounding component from " + gameObject);
			Destroy(this);
		}
	}
}