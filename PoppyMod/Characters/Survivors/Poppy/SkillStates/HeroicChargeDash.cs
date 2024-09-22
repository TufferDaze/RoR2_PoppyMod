using EntityStates;
using EntityStates.Toolbot;
using PoppyMod.Characters.Survivors.Poppy.Components;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
//Since we are using effects from Commando's Barrage skill, we will also be using the associated namespace
//You can also use Addressables or LegacyResourcesAPI to load whichever effects you like

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class HeroicChargeDash : BaseSkillState
    {
        private float duration = 0.25f;
        private OverlapAttack attack;
        private bool inHitPause = false;
        private float hitPauseTimer = 0.01f;
        public static float hitPauseDuration = 0.05f;
        public static float recoilAmplitude = 5.0f;
        public static float massThresholdForKnockback = 300.0f;
        public static float knockbackDamageCoefficient = 1.5f;
        public static float knockbackForce = 10.0f;
        private Vector3 dashDirection;
        private float chargeDamageCoefficient = PoppyConfig.util1DmgConfig.Value;
        public float speedMultiplier = 12.0f;
        private List<HurtBox> victimsStruck = new List<HurtBox>();
        private GrappleComponent grappleCon;

        //OnEnter() runs once at the start of the skill
        //All we do here is create a BulletAttack and fire it
        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                if (inputBank)
                {
                    dashDirection = inputBank.aimDirection;
                }
                characterBody.isSprinting = true;
            }
            if (characterDirection)
            {
                characterDirection.forward = dashDirection;
            }
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "HammerGroup");
            }
            attack = new OverlapAttack();
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = chargeDamageCoefficient * this.damageStat;
            attack.hitEffectPrefab = ToolbotDash.impactEffectPrefab;
            attack.forceVector = Vector3.forward;
            attack.pushAwayForce = 1.0f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            gameObject.layer = LayerIndex.fakeActor.intVal;
            characterMotor.Motor.RebuildCollidableLayers();
            characterMotor.Motor.ForceUnground();
            characterMotor.velocity.y = 0;
            PlayAnimation("FullBody, Override", "HeroicCharge", "Roll.playbackRate", duration);
            Util.PlaySound("PlayPoppyE", gameObject);
            Util.PlaySound("PlayPoppyESFX", gameObject);
        }

        //FixedUpdate() runs almost every frame of the skill
        //Here, we end the skill once it exceeds its intended duration
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
            if (isAuthority)
            {
                if (characterBody)
                {
                    characterBody.isSprinting = true;
                }
                if (!inHitPause)
                {
                    if (characterDirection)
                    {
                        characterDirection.moveVector = dashDirection;
                        if (characterMotor)
                        {
                            characterMotor.rootMotion += GetIdealVelocity() * Time.fixedDeltaTime;
                        }
                    }
                    attack.damage = damageStat * (chargeDamageCoefficient * GetDamageBoostFromSpeed());
                    if (attack.Fire(victimsStruck))
                    {
                        inHitPause = true;
                        hitPauseTimer = hitPauseDuration;
                        AddRecoil(-0.5f * recoilAmplitude, -0.5f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
                        for (int i = 0; i < victimsStruck.Count; i++)
                        {
                            float num = 0f;
                            HurtBox hurtBox = victimsStruck[i];
                            if (hurtBox.healthComponent)
                            {
                                /*CharacterMotor component = hurtBox.healthComponent.GetComponent<CharacterMotor>();
                                if (component)
                                {
                                    num = component.mass;
                                }
                                else
                                {
                                    Rigidbody component2 = hurtBox.healthComponent.GetComponent<Rigidbody>();
                                    if (component2)
                                    {
                                        num = component2.mass;
                                    }
                                }
                                if (num >= massThresholdForKnockback)
                                {
                                    outer.SetNextState(new HeroicChargeImpact
                                    {
                                        victimHealthComponent = hurtBox.healthComponent,
                                        idealDirection = dashDirection,
                                        damageBoostFromSpeed = GetDamageBoostFromSpeed(),
                                        isCrit = attack.isCrit
                                    });
                                    return;
                                }
                                else*/
                                {
                                    grappleCon = hurtBox.healthComponent.body.gameObject.AddComponent<GrappleComponent>();
                                    Transform grappleCarryLocation = gameObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>().GetComponent<ChildLocator>().FindChild("GrappleCarryLocation");
                                    if (grappleCarryLocation)
                                    {
                                        grappleCon.pivotTransform = grappleCarryLocation;
                                    }
                                    else
                                    {
                                        grappleCon.pivotTransform = this.gameObject.transform;
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
                else
                {
                    characterMotor.velocity = Vector3.zero;
                    hitPauseTimer -= Time.fixedDeltaTime;
                    if (hitPauseTimer < 0f)
                    {
                        inHitPause = false;
                    }
                }
            }
        }

        public override void OnExit()
        {
            HandleGrappleRelease();
            characterBody.isSprinting = true;
            gameObject.layer = LayerIndex.defaultLayer.intVal;
            characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
        }

        private Vector3 GetIdealVelocity()
        {
            return dashDirection.normalized * characterBody.moveSpeed * speedMultiplier;
        }

        private float GetDamageBoostFromSpeed()
        {
            return Mathf.Max(1f, characterBody.moveSpeed / characterBody.baseMoveSpeed);
        }

        private void HandleGrappleRelease()
        {
            if (victimsStruck.Count > 0)
            {
                try
                {
                    foreach (HurtBox enemy in victimsStruck)
                    {
                        grappleCon = enemy.healthComponent.body.gameObject.GetComponent<GrappleComponent>();
                        if (grappleCon)
                        {
                            grappleCon.Release();
                        }
                    }
                }
                catch (Exception e)
                {
                    //Debug.LogWarning($"PoppyMod: HeroicCharge: All enemies died before being processed.\n{e}");
                    victimsStruck.RemoveAll(delegate (HurtBox x) { return x == null; });
                    HandleGrappleRelease();
                }
            }
        }

        //GetMinimumInterruptPriority() returns the InterruptPriority required to interrupt this skill
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}