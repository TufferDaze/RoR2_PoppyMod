using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using EntityStates;
using EntityStates.BrotherMonster;
using UnityEngine.AddressableAssets;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class KeepersVerdictChargeSlam : BaseSkillState
    {
        public float duration = 0.8f;
        public float stopwatch;
        public float blastDelay = 0.6f;
        public float damageCoefficient;
        public float forceMagnitude = 16f;
        public float upwardForce;
        public float radius = 3f;
        public string attackSoundString;
        public string muzzleString = "Weapon3";
        public GameObject slamImpactEffect;
        public GameObject waveProjectilePrefab;
        private bool hasFired;
        private float slamProcCoefficient = PoppyStaticValues.special1SlamProcCoefficient;
        private float waveProcCoefficient = PoppyStaticValues.special1WaveProcCoefficient;
        public float waveProjectileArc = 1f;
        public int waveProjectileCount = 3;
        public float slamForceDirect = PoppyConfig.spec1SlamForceConfig.Value;
        public float slamForceWave = PoppyConfig.spec1WaveForceConfig.Value;
        public float charge;
        public NetworkSoundEventDef weaponImpactSound;
        private OverlapAttack weaponAttack;
        private Animator modelAnimator;
        private Transform modelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            stopwatch = 0f;
            slamImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderGroundSlam.prefab").WaitForCompletion();
            waveProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWave.prefab").WaitForCompletion();
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
                overlapAttack.hitEffectPrefab = WeaponSlam.weaponHitEffectPrefab;
                overlapAttack.hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "KeeperGroup");
                overlapAttack.impactSound = WeaponSlam.weaponImpactSound.index;
                overlapAttack.inflictor = base.gameObject;
                overlapAttack.procChainMask = default(ProcChainMask);
                overlapAttack.forceVector = new Vector3(0, charge * slamForceDirect, 0);
                overlapAttack.procCoefficient = slamProcCoefficient;
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
            if (base.isAuthority)
            {
                if (base.characterDirection)
                {
                    base.characterDirection.moveVector = base.characterDirection.forward;
                }
                if (!hasFired && stopwatch >= blastDelay)
                {
                    EffectManager.SimpleMuzzleFlash(slamImpactEffect, gameObject, muzzleString, false);
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
                    FireWave();
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

        private void FireWave()
        {
            float num = 90f / (float)waveProjectileCount;
            Vector3 point = Vector3.ProjectOnPlane(base.inputBank.aimDirection, Vector3.up);
            Vector3 footPosition = base.characterBody.footPosition;
            for (int i = 0; i < waveProjectileCount; i++)
            {
                Vector3 forward = Quaternion.AngleAxis((num * (float)i) - 45f, Vector3.up) * point;
                bool isAuthority2 = base.isAuthority;
                if (isAuthority2)
                {
                    waveProjectilePrefab.GetComponent<ProjectileOverlapAttack>().forceVector = new Vector3(0f, charge * slamForceWave, 0f);

                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        crit = base.RollCrit(),
                        damage = base.damageStat * (this.damageCoefficient / (float)waveProjectileCount),
                        damageTypeOverride = new DamageType?(DamageType.Stun1s),
                        damageColorIndex = DamageColorIndex.Default,
                        force = 0f,
                        owner = base.gameObject,
                        position = footPosition,
                        procChainMask = default(ProcChainMask),
                        projectilePrefab = waveProjectilePrefab,
                        rotation = Util.QuaternionSafeLookRotation(forward),
                        useFuseOverride = false,
                        useSpeedOverride = false,
                        target = null
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            }
        }

        //public override InterruptPriority GetMinimumInterruptPriority()
        //{
        //    if (base.fixedAge <= durationBeforePriorityReduces)
        //    {
        //        return InterruptPriority.PrioritySkill;
        //    }
        //    return InterruptPriority.Skill;
        //}
    }
}
