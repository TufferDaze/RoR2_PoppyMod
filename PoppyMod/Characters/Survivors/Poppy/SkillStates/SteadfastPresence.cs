using System;
using RoR2;
using EntityStates;
using EntityStates.Toolbot;
using System.Collections.Generic;
using UnityEngine;
using PoppyMod.Survivors.Poppy.Components;
using EntityStates.Huntress;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class SteadfastPresence : BaseSkillState
	{
        private float duration = 3f;
        private OverlapAttack attack;
        //private GameObject hitImpactPrefab;
        private CharacterBody body;
        private HitBoxGroup hitBoxGroup;
        private float initialTime;
        private bool inHitPause = false;
        private bool hasBlocked;
        private float hitPauseTimer;
        private Transform indicatorInstance;
        public static float hitPauseDuration = 0.2f;
        private float damageCoefficient;
        private float procCoefficient = PoppyStaticValues.utility2ProcCoefficient;
        private List<HurtBox> enemiesHit = new List<HurtBox>();
        private CameraTargetParams.CameraParamsOverrideHandle handle;
        
        private float totalDmg = 0f;
        private int fireTimes = 0;

        public override void OnEnter()
        {
            base.OnEnter();
            initialTime = Time.fixedDeltaTime;
            hasBlocked = false;
            hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            body = GetComponent<CharacterBody>();
            if (body)
            {
                body.AddTimedBuff(PoppyBuffs.steadfastSpeedBuff, duration);
                body.isSprinting = true;
            }
            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "AuraGroup");
            }
            damageCoefficient = (PoppyConfig.util2DmgConfig.Value / duration) * duration/13; // TODO calculate denominator
            CreateNewAttack();
            CreateIndicator();
            Util.PlaySound("PlayPoppyW", gameObject);
            Util.PlaySound("PlayPoppyWSFX", gameObject);
            if (inputBank.moveVector == Vector3.zero)
            {
                PlayAnimation("FullBody, Override", "SteadfastPresenceIdle");
                //PlayCrossfade("FullBody, Override", "SteadfastPresenceIdle", "Roll.PlaybackRate", duration, 0.1f * duration);
            }
            else
            {
                PlayAnimation("FullBody, Override", "SteadfastPresenceRun");
                //PlayCrossfade("FullBody, Override", "SteadfastPresenceRun", "Roll.PlaybackRate", duration, 0.1f * duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            enemiesHit.Clear();
            OverrideCamera();
            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
            if (isAuthority && !inHitPause)
            {
                if (attack.Fire(enemiesHit))
                {
                    totalDmg += damageCoefficient;
                    fireTimes++;
                    //Debug.LogWarning("Steadfast Presence DAMAGE: " + totalDmg);
                    //Debug.LogWarning("Steadfast Presence FIRE NUM: " + fireTimes);
                    inHitPause = true;
                    hitPauseTimer = hitPauseDuration;
                    for (int i = 0; i < enemiesHit.Count; i++)
                    {
                        HurtBox hurtBox = enemiesHit[i];
                        try
                        {
                            if (hurtBox.healthComponent)
                            {
                                CharacterMotor component = hurtBox.healthComponent.GetComponent<CharacterMotor>();
                                RigidbodyMotor component2 = hurtBox.healthComponent.GetComponent<RigidbodyMotor>();
                                bool isFlying = false;
                                if (component)
                                {
                                    isFlying = !component.netIsGrounded || (component.isFlying && !component.useGravity);
                                }
                                else if (component2)
                                {
                                    isFlying = true;
                                }
                                GameObject enemyBody = hurtBox.healthComponent.body.gameObject;
                                if (!enemyBody.GetComponent<GroundingComponent>()
                                    && isFlying
                                    && !enemyBody.GetComponent<CharacterBody>().isChampion)
                                {
                                    enemyBody.AddComponent<GroundingComponent>(); // Add grounding component to this enemy
                                    if (!hasBlocked)
                                    {
                                        hasBlocked = true;
                                        Util.PlaySound("PlayPoppyWBlock", gameObject);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Debug.LogError("healthComponent null!"); // Health component can sometimes be null here for some reason :P
                        }
                    }
                    attack.ResetIgnoredHealthComponents();
                    return;
                }
            }
            else
            {
                hitPauseTimer -= Time.fixedDeltaTime;
                if (hitPauseTimer < 0f)
                {
                    inHitPause = false;
                }
            }
            UpdateIndicator();
        }

        public override void OnExit()
        {
            if (indicatorInstance)
            {
                Destroy(indicatorInstance.gameObject);
            }
            base.cameraTargetParams.RemoveParamsOverride(this.handle, 1f);
            base.OnExit();
        }

        private void CreateNewAttack()
        {
            attack = new OverlapAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = GetTeam(),
                damage = damageCoefficient * this.damageStat,
                procCoefficient = this.procCoefficient,
                damageType = DamageType.Stun1s,
                hitEffectPrefab = ToolbotDash.impactEffectPrefab,
                forceVector = new Vector3(0, 0, 0),
                pushAwayForce = 0f,
                hitBoxGroup = this.hitBoxGroup,
                isCrit = RollCrit()
            };
        }

        private void CreateIndicator()
        {
            indicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab).transform;
            if (indicatorInstance)
            {
                indicatorInstance.localScale = Vector3.one * 8f;
                indicatorInstance.transform.position = transform.position;
            }
            else
            {
                Debug.LogError("SteadfastPressence: Could not load Arrowrain indicator.");
            }
        }

        private void UpdateIndicator()
        {
            if (indicatorInstance)
            {
                indicatorInstance.transform.position = transform.position;
            }
        }

        private void OverrideCamera()
        {
            CameraTargetParams cameraTargetParams = base.cameraTargetParams;
            CharacterCameraParamsData cameraParamsData = cameraTargetParams.currentCameraParamsData;
            float scalar = (float)Math.Pow(1f + Time.fixedDeltaTime - this.initialTime, 2);
            Vector3 modifierVector = new Vector3(0f, 0f, -1f);
            cameraParamsData.idealLocalCameraPos = new Vector3(0f, 0f, -12f) + scalar * modifierVector;
            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = cameraParamsData,
                priority = 0f
            };
            this.handle = cameraTargetParams.AddParamsOverride(request);
            base.cameraTargetParams.RemoveParamsOverride(this.handle, 1f);
        }
    }
}