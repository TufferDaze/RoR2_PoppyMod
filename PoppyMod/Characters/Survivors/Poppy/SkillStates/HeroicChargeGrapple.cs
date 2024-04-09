using System;
using System.Collections;
using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class HeroicChargeGrapple : MonoBehaviour
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
        private float maxDuration = 0.25f;

        private void Awake()
        {
            Debug.Log("HeroicChargeGrapple: Grapple component added successfully.");
            try
            {
                stopwatch = 0f;
                body = GetComponent<CharacterBody>();
                motor = GetComponent<CharacterMotor>();
                direction = GetComponent<CharacterDirection>();
                modelLocator = GetComponent<ModelLocator>();
                capsuleCollider = GetComponent<CapsuleCollider>();
                sphereCollider = GetComponent<SphereCollider>();
                if (this.direction)
                {
                    this.direction.enabled = false;
                }
                if (this.capsuleCollider)
                {
                    this.capsuleCollider.enabled = false;
                }
                if (this.sphereCollider)
                {
                    this.sphereCollider.enabled = false;
                }
                if (this.modelLocator && this.modelLocator.modelTransform)
                {
                    this.modelTransform = this.modelLocator.modelTransform;
                    this.modelLocator.enabled = false;
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
            try
            {
                if (motor)
                {
                    motor.disableAirControlUntilCollision = true;
                    motor.velocity = Vector3.zero;
                    motor.rootMotion = Vector3.zero;
                    motor.Motor.SetPosition(pivotTransform.position, true);
                }
                modelTransform.position = pivotTransform.position;
            }
            catch (Exception e)
            {
                Debug.LogError($"HeroicChargeGrapple: Target transform is null.\n{e}");
            }
        }

        IEnumerator TimeRelease()
        {
            yield return new WaitForSecondsRealtime(1f);
            Release();
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
