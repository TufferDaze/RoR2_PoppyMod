using System;
using System.Collections;
using RoR2;
using UnityEngine;

namespace PoppyMod.Characters.Survivors.Poppy.Components
{
    public class GrappleComponent : MonoBehaviour
    {
        public Transform pivotTransform;
        private CharacterBody body;
        private CapsuleCollider capsuleCollider;
        private CharacterDirection direction;
        private ModelLocator modelLocator;
        private Transform modelTransform;
        private CharacterMotor motor;
        private SphereCollider sphereCollider;
        private float stopwatch;
        private float maxDuration = 0.3f;

        private void Awake()
        {
            //Debug.Log("HeroicChargeGrapple: Grapple component added successfully.");
            try
            {
                stopwatch = 0f;
                body = GetComponent<CharacterBody>();
                motor = GetComponent<CharacterMotor>();
                direction = GetComponent<CharacterDirection>();
                modelLocator = GetComponent<ModelLocator>();
                capsuleCollider = GetComponent<CapsuleCollider>();
                sphereCollider = GetComponent<SphereCollider>();
                if (direction)
                {
                    direction.enabled = false;
                }
                if (capsuleCollider)
                {
                    capsuleCollider.enabled = false;
                }
                if (sphereCollider)
                {
                    sphereCollider.enabled = false;
                }
                if (modelLocator && modelLocator.modelTransform)
                {
                    modelTransform = modelLocator.modelTransform;
                    modelLocator.enabled = false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"HeroicChargeGrapple: Error processing grapple target.\n{e}");
                Release();
            }
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
                motor.disableAirControlUntilCollision = true;
                motor.velocity = Vector3.zero;
                motor.rootMotion = Vector3.zero;
                motor.Motor.SetPosition(pivotTransform.position, true);
            }
            modelTransform.position = pivotTransform.position;
        }

        public void Release()
        {
            if (modelLocator)
            {
                modelLocator.enabled = true;
            }
            if (direction)
            {
                direction.enabled = true;
            }
            if (capsuleCollider)
            {
                capsuleCollider.enabled = true;
            }
            if (sphereCollider)
            {
                sphereCollider.enabled = true;
            }
            Destroy(this);
        }
    }
}
