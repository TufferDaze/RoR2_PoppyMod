using System;
using RoR2;
using R2API;
using EntityStates;
using EntityStates.Toolbot;
using System.Collections.Generic;
using UnityEngine;
using PoppyMod.Survivors.Poppy.Components;
using EntityStates.Huntress;
using PoppyMod.Modules.Characters;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class SteadfastPresence : BaseSkillState
	{
        private float duration = 3f;
        private OverlapAttack attack;
        private GameObject hitImpactPrefab;
        private CharacterBody body;
        private HitBoxGroup hitBoxGroup;
        private float initialTime;
        private bool isCrit;
        private bool inHitPause = false;
        private bool hasBlocked;
        private float hitPauseTimer;
        private Transform indicatorInstance;
        public static float hitPauseDuration = 0.25f;
        private float damageCoefficient = PoppyConfig.util2DmgConfig.Value * hitPauseDuration;
        private float procCoefficient = 0.75f;
        private List<HurtBox> enemiesHit = new List<HurtBox>();
        private CameraTargetParams.CameraParamsOverrideHandle handle;

        public override void OnEnter()
        {
            base.OnEnter();
            initialTime = Time.fixedTime;
            hasBlocked = false;
            hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            body = GetComponent<CharacterBody>();
            if (body)
            {
                body.AddTimedBuff(PoppyBuffs.speedBuff, duration);
                body.isSprinting = true;
            }
            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "AuraGroup");
            }
            isCrit = RollCrit();
            CreateNewAttack();
            CreateIndicator();
            Util.PlaySound("PlayPoppyW", gameObject);
            Util.PlaySound("PlayPoppyWSFX", gameObject);
            if (!GetModelAnimator().GetBool("isMoving"))
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
                                Rigidbody component2 = hurtBox.healthComponent.GetComponent<Rigidbody>();
                                GameObject enemyBody = hurtBox.healthComponent.body.gameObject;
                                if ((component || component2)
                                    && !enemyBody.GetComponent<GroundingComponent>()
                                    && !component.netIsGrounded
                                    && !enemyBody.GetComponent<CharacterBody>().isChampion)
                                {
                                    // add grounding component to this enemy
                                    enemyBody.AddComponent<GroundingComponent>();
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
                            Debug.LogError("hurtBox null!");
                        }
                    }
                    CreateNewAttack();
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
            base.cameraTargetParams.RemoveParamsOverride(this.handle);
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
                forceVector = Vector3.downVector,
                pushAwayForce = 0f,
                hitBoxGroup = this.hitBoxGroup,
                isCrit = this.isCrit
            };
        }

        private void CreateIndicator()
        {
            indicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab).transform;
            indicatorInstance.localScale = Vector3.one * 8f;
            indicatorInstance.transform.position = transform.position;
        }

        private void UpdateIndicator()
        {
            indicatorInstance.transform.position = transform.position;
        }

        private void OverrideCamera()
        {
            CameraTargetParams cameraTargetParams = base.cameraTargetParams;
            CharacterCameraParamsData cameraParamsData = cameraTargetParams.currentCameraParamsData;
            float scalar = 1f+(float)Math.Pow(Time.fixedTime - this.initialTime, 1.5);
            Vector3 modifierVector = new Vector3(0f, 0f, -1f);
            cameraParamsData.idealLocalCameraPos = PoppySurvivor.instance.bodyInfo.cameraPivotPosition + scalar * modifierVector;
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