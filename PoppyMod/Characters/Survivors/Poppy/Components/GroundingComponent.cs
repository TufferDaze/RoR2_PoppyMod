using RoR2;
using System;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.Components
{
	public class GroundingComponent : MonoBehaviour
	{
		private CharacterBody body;
		private CharacterMotor motor;
		private float maxDuration = 4f;
		private float stopwatch;
		private float downForce = PoppyStaticValues.groundingSpeed;

		private void Awake()
		{
			//Debug.Log("GroundingComponent: Grounding component added successfully!");
			body = GetComponent<CharacterBody>();
			motor = GetComponent<CharacterMotor>();
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
        }

		private void Release()
		{
			Destroy(this);
		}
	}
}