using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates;
using EntityStates.BrotherMonster;
using UnityEngine.UIElements;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class KeepersVerdictChargeSlam : BaseSkillState
    {
        public static float duration = 0.8f;
        public float stopwatch;
        public static float blastDelay = 0.6f;
        public float damageCoefficient = 6f;
        public static float forceMagnitude = 16f;
        public static float upwardForce;
        public static float radius = 3f;
        public static string attackSoundString;
        //public static string muzzleString;
        public static GameObject slamImpactEffect;
        public static float durationBeforePriorityReduces;
        //public static GameObject waveProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardBodySecondaryFamily.asset").WaitForCompletion();
        public static GameObject waveProjectilePrefab;
        private bool hasFired;
        private float procCoefficient = 0.5f;
        public static float waveProjectileArc = 1f;
        public static int waveProjectileCount = 3;
        public static float waveProjectileDamageCoefficient = 0.1f;
        public static float waveProjectileForce = 0f;
        public static float slamForce = 0f;
        public static GameObject weaponHitEffectPrefab;
        public static NetworkSoundEventDef weaponImpactSound;
        private BlastAttack blastAttack;
        private OverlapAttack weaponAttack;
        private Animator modelAnimator;
        private Transform modelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            stopwatch = 0f;
            waveProjectilePrefab = LegacyResourcesAPI.Load<GameObject>("RoR2/Base/Brother/BrotherSunderWave");
            this.modelAnimator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            if (base.characterDirection)
            {
                base.characterDirection.moveVector = base.GetAimRay().direction;
            }
            if (this.modelTransform)
            {
                AimAnimator component = this.modelTransform.GetComponent<AimAnimator>();
                if (component)
                {
                    component.enabled = true;
                }
            }
            if (base.isAuthority)
            {
                OverlapAttack overlapAttack = new OverlapAttack();
                overlapAttack.attacker = base.gameObject;
                overlapAttack.damage = damageCoefficient * this.damageStat;
                //overlapAttack.hitEffectPrefab = WeaponSlam.weaponHitEffectPrefab;
                overlapAttack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "KeeperGroup");
                overlapAttack.impactSound = WeaponSlam.weaponImpactSound.index;
                overlapAttack.inflictor = base.gameObject;
                overlapAttack.procChainMask = default(ProcChainMask);
                overlapAttack.pushAwayForce = slamForce;
                overlapAttack.procCoefficient = procCoefficient;
                overlapAttack.teamIndex = base.GetTeam();
                this.weaponAttack = overlapAttack;
            }
            Util.PlaySound("PlayPoppyRFire", gameObject);
            PlayAnimation("FullBody, Override", "KeepersVerdictChargeFire");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            //EffectManager.SimpleMuzzleFlash(slamImpactEffect, base.gameObject, muzzleString, false);
            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    base.characterDirection.moveVector = base.characterDirection.forward;
                }
                //Transform transform2 = base.FindModelChild(muzzleString);
                if (!hasFired && stopwatch >= blastDelay)
                {
                    this.weaponAttack.Fire(null);
                    if (PoppyConfig.bonkConfig.Value)
                    {
                        Util.PlaySound("PlayPoppyBonkBigSFX", gameObject);
                    }
                    else
                    {
                        Util.PlaySound("PlayPoppyRChargeFireSFX", gameObject);
                    }
                    hasFired = true;
                    Vector3 footPosition = base.characterBody.footPosition;
                    float num = 90f / (float)waveProjectileCount;
                    Vector3 point = Vector3.ProjectOnPlane(base.inputBank.aimDirection, Vector3.up);
                    for (int i = 0; i < waveProjectileCount; i++)
                    {
                        Vector3 forward = Quaternion.AngleAxis((num * (float)i)-45f, Vector3.up) * point;
                        bool isAuthority2 = base.isAuthority;
                        if (isAuthority2)
                        {
                            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                            {
                                crit = base.RollCrit(),
                                damage = damageStat * (this.damageCoefficient / (float)waveProjectileCount),
                                damageTypeOverride = new DamageType?(DamageType.Stun1s),
                                damageColorIndex = DamageColorIndex.Default,
                                force = waveProjectileForce,
                                owner = base.gameObject,
                                position = footPosition,
                                procChainMask = default(ProcChainMask),
                                projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWave.prefab").WaitForCompletion(),
                                rotation = Util.QuaternionSafeLookRotation(forward),
                                useFuseOverride = false,
                                useSpeedOverride = true,
                                speedOverride = 10f,
                                target = null
                            };
                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                        }
                    }
                }
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (PoppyConfig.rChargeVOConfig.Value)
            {
                Util.PlaySound("PlayPoppyRComment", gameObject);
            }
            if (!GetModelAnimator().GetBool("isMoving"))
            {
                PlayAnimation("FullBody, Override", "KeepersVerdictToIdle");
            }
            else
            {
                PlayAnimation("FullBody, Override", "KeepersVerdictToRun");
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge <= durationBeforePriorityReduces)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Skill;
        }
    }
}
