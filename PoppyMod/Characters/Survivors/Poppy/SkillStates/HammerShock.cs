using EntityStates;
using EntityStates.Loader;
using RoR2;
using UnityEngine;
//Since we are using effects from Commando's Barrage skill, we will also be using the associated namespace
//You can also use Addressables or LegacyResourcesAPI to load whichever effects you like

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class HammerShock : BaseSkillState
    {
        public float airControl = 10.0f;
        public float minimumDuration = 0.1f;
        public float blastRadius = 10.0f;
        public float blastProcCoefficient = 1.0f;
        public float blastDamageCoefficient = PoppyConfig.spec2DmgConfig.Value;
        public float blastForce = 20.0f;
        public Vector3 blastBonusForce = new Vector3(0f, 0f, 5f);
        public float initialVerticalVelocity = 5.0f;
        public float verticalAcceleration = -200.0f;
        public float exitVerticalVelocity = 10.0f;
        public float exitSlowdownCoefficient = 0.5f;
        public GameObject blastImpactPrefab;
        public GameObject fistEffectPrefab;
        private GameObject fistEffectInstance;
        private bool detonateNextFrame = false;
        private float previousAirControl;

        //OnEnter() runs once at the start of the skill
        //All we do here is create a BulletAttack and fire it
        public override void OnEnter()
        {
            base.OnEnter();
            //base.PlayCrossfade("Body", "GroundSlam", 0.2f);
            if (base.isAuthority)
            {
                base.characterMotor.onMovementHit += this.OnMovementHit;
                base.characterMotor.velocity.y = initialVerticalVelocity;
            }
            Util.PlaySound(GroundSlam.enterSoundString, base.gameObject);
            this.previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = airControl;
            PlayCrossfade("FullBody, Override", "HammerShock", 0.2f);
            //this.leftFistEffectInstance = UnityEngine.Object.Instantiate<GameObject>(GroundSlam.fistEffectPrefab, base.FindModelChild("MechHandR"));
        }

        //FixedUpdate() runs almost every frame of the skill
        //Here, we end the skill once it exceeds its intended duration
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.characterMotor)
            {
                base.characterMotor.moveDirection = base.inputBank.moveVector;
                base.characterDirection.moveVector = base.characterMotor.moveDirection;
                CharacterMotor characterMotor = base.characterMotor;
                characterMotor.velocity.y = characterMotor.velocity.y + verticalAcceleration * Time.fixedDeltaTime;
                if (base.fixedAge >= minimumDuration && (this.detonateNextFrame || base.characterMotor.Motor.GroundingStatus.IsStableOnGround))
                {
                    BlastAttack.Result targetsHit = DetonateAuthority();
                    DealMaxHPDamage(targetsHit);
                    if (PoppyConfig.bonkConfig.Value && targetsHit.hitCount != 0)
                    {
                        Util.PlaySound("PlayPoppyBonkBigSFX", gameObject);
                    }
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("FullBody, Override", "HStoIdle", 0.1f);
            Util.PlaySound("PlayPoppyQSFX", gameObject);
            if (base.isAuthority)
            {
                base.characterMotor.onMovementHit -= this.OnMovementHit;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity *= exitSlowdownCoefficient;
                base.characterMotor.velocity.y = exitVerticalVelocity;
            }
            base.characterMotor.airControl = this.previousAirControl;
            //EntityState.Destroy(this.leftFistEffectInstance);
            //EntityState.Destroy(this.rightFistEffectInstance);
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.OnExit();
        }

        private void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            this.detonateNextFrame = true;
        }

        protected BlastAttack.Result DetonateAuthority()
        {
            Vector3 footPosition = base.characterBody.footPosition;
            EffectManager.SpawnEffect(GroundSlam.blastEffectPrefab, new EffectData
            {
                origin = footPosition,
                scale = blastRadius
            }, true);
            return new BlastAttack
            {
                attacker = base.gameObject,
                baseDamage = this.damageStat * blastDamageCoefficient,
                baseForce = blastForce,
                bonusForce = blastBonusForce,
                crit = base.RollCrit(),
                damageType = DamageType.Stun1s,
                falloffModel = BlastAttack.FalloffModel.None,
                procCoefficient = blastProcCoefficient,
                radius = blastRadius,
                position = footPosition,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                impactEffect = EffectCatalog.FindEffectIndexFromPrefab(GroundSlam.blastImpactEffectPrefab),
                teamIndex = base.teamComponent.teamIndex
            }.Fire();
        }

        private void DealMaxHPDamage(BlastAttack.Result enemiesHit)
        {
            try
            {
                foreach (BlastAttack.HitPoint enemy in enemiesHit.hitPoints)
                {
                    HealthComponent enemyHealth = enemy.hurtBox.healthComponent;
                    if (enemyHealth)
                    {
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = gameObject,
                            damage = enemy.hurtBox.healthComponent.fullCombinedHealth * PoppyConfig.spec2HPConfig.Value,
                            damageColorIndex = DamageColorIndex.Item,
                            damageType = DamageType.Generic,
                        };
                        enemyHealth.TakeDamage(damageInfo);
                    }
                }
            }
            catch
            {
                Debug.LogWarning("HammerShock: Error dealing max HP damage.");
            }
        }

        //GetMinimumInterruptPriority() returns the InterruptPriority required to interrupt this skill
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}