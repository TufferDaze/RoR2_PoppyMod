using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates;
using EntityStates.BrotherMonster;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class KeepersVerdictSlam : BaseSkillState
    {
        public static float duration = 3.5f;
        public float damageCoefficient = 6f;
        public static float forceMagnitude = 16f;
        public static float upwardForce;
        public static float radius = 3f;
        public static string attackSoundString;
        //public static string muzzleString;
        public static GameObject slamImpactEffect;
        public static float durationBeforePriorityReduces;
        public static GameObject waveProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWave.prefab").WaitForCompletion();
        public static float waveProjectileArc = 1f;
        public static int waveProjectileCount = 10;
        public static float waveProjectileDamageCoefficient = 0.1f;
        public static float waveProjectileForce = 10f;
        public static float weaponForce = 10f;
        public static GameObject weaponHitEffectPrefab;
        public static NetworkSoundEventDef weaponImpactSound;
        private BlastAttack blastAttack;
        private OverlapAttack weaponAttack;
        private Animator modelAnimator;
        private Transform modelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelAnimator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            Util.PlayAttackSpeedSound(attackSoundString, base.gameObject, this.attackSpeedStat);
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
                overlapAttack.damage = WeaponSlam.damageCoefficient * this.damageStat;
                overlapAttack.damageColorIndex = DamageColorIndex.Default;
                overlapAttack.damageType = DamageType.Generic;
                overlapAttack.hitEffectPrefab = WeaponSlam.weaponHitEffectPrefab;
                overlapAttack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge");
                overlapAttack.impactSound = WeaponSlam.weaponImpactSound.index;
                overlapAttack.inflictor = base.gameObject;
                overlapAttack.procChainMask = default(ProcChainMask);
                overlapAttack.pushAwayForce = WeaponSlam.weaponForce;
                overlapAttack.procCoefficient = 1f;
                overlapAttack.teamIndex = base.GetTeam();
                this.weaponAttack = overlapAttack;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                this.weaponAttack.Fire(null);
            }
            //EffectManager.SimpleMuzzleFlash(slamImpactEffect, base.gameObject, muzzleString, false);
            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    base.characterDirection.moveVector = base.characterDirection.forward;
                }
                if (this.modelTransform)
                {
                    //Transform transform = base.FindModelChild(muzzleString);
                    Transform transform = base.transform;
                    if (transform)
                    {
                        this.blastAttack = new BlastAttack();
                        this.blastAttack.attacker = base.gameObject;
                        this.blastAttack.inflictor = base.gameObject;
                        this.blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                        this.blastAttack.baseDamage = this.damageStat * damageCoefficient;
                        this.blastAttack.baseForce = forceMagnitude;
                        this.blastAttack.position = transform.position;
                        this.blastAttack.radius = radius;
                        this.blastAttack.bonusForce = new Vector3(0f, upwardForce, 0f);
                        this.blastAttack.Fire();
                    }
                }
                if (PhaseCounter.instance && PhaseCounter.instance.phase == 3)
                {
                    //Transform transform2 = base.FindModelChild(muzzleString);
                    Transform transform2 = base.transform;
                    float num = waveProjectileArc / (float)waveProjectileCount;
                    Vector3 point = Vector3.ProjectOnPlane(base.characterDirection.forward, Vector3.up);
                    Vector3 position = base.characterBody.footPosition;
                    if (transform2)
                    {
                        position = transform2.position;
                    }
                    for (int i = 0; i < waveProjectileCount; i++)
                    {
                        Vector3 forward = Quaternion.AngleAxis(num * ((float)i - (float)waveProjectileCount / 2f), Vector3.up) * point;
                        ProjectileManager.instance.FireProjectile(
                            waveProjectilePrefab, 
                            position, 
                            Util.QuaternionSafeLookRotation(forward), 
                            base.gameObject, 
                            base.characterBody.damage * waveProjectileDamageCoefficient, 
                            waveProjectileForce, 
                            Util.CheckRoll(base.characterBody.crit, base.characterBody.master), 
                            DamageColorIndex.Default, 
                            null, 
                            -1f
                        );
                    }
                }
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
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
