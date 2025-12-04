using EntityStates;
using RoR2;
using RoR2.UI;
using RoR2BepInExPack.GameAssetPaths;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class KeepersVerdictCharge : BaseSkillState
    {
        private float baseChargeTime = 4.3f;
        private float maxHoldTime = 7f;
        private float maxChargeTime;
        private float minimumDamageCoefficient = PoppyConfig.spec1MinDmgConfig.Value;
        private float maximumDamageCoefficient = PoppyConfig.spec1MaxDmgConfig.Value;
        private float damageCoefficient = PoppyConfig.spec1MinDmgConfig.Value;
        private float charge;
        private float instantFireThreshhold = 0.1f;
        private float walkSlowCoefficient = 0.5f;
        private uint soundId1;
        private uint soundId2;
        private GameObject chargeEffectPrefab;
        private GameObject crosshairPrefab;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            HandleDamageCoefficients();
            this.maxChargeTime = baseChargeTime / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.isSprinting = false;
            }
            soundId1 = Util.PlaySound("PlayPoppyRCharge", gameObject);
            soundId2 = Util.PlaySound("PlayPoppyRChargeSFX", gameObject);
            PlayAnimation("Gesture, Override", "KeepersVerdictCharge");
            GetModelAnimator().SetBool("isCharging", true);
            chargeEffectPrefab = null;
            crosshairPrefab = EntityStates.Loader.BaseChargeFist.crosshairOverridePrefab;
            crosshairOverrideRequest = null;
            base.characterBody.spreadBloomCurve = AnimationCurve.Linear(0f, 0f, 9.5f, 1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.charge = Mathf.Clamp01(base.fixedAge / this.maxChargeTime);
            this.damageCoefficient = Mathf.Clamp(maximumDamageCoefficient * this.charge, minimumDamageCoefficient, maximumDamageCoefficient);
            base.characterBody.SetSpreadBloom(this.charge, true);
            base.characterBody.SetAimTimer(3f);
            if (!chargeEffectPrefab)
            {
                if (crosshairPrefab && this.crosshairOverrideRequest == null)
                {
                    crosshairPrefab.transform.GetComponent<CrosshairController>().maxSpreadAngle = 0.1f;
                    this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairPrefab, CrosshairUtils.OverridePriority.Skill);
                }
                /*this.chargeEffectPrefab = Object.Instantiate<GameObject>(EntityStates.Loader.BaseChargeFist.chargeVfxPrefab, transform);
                if (chargeEffectPrefab)
                {
                    ScaleParticleSystemDuration component = this.chargeEffectPrefab.transform.GetComponent<ScaleParticleSystemDuration>();
                    Debug.LogWarning("chargeEffectPrefab IS ALIVEEEEEE");
                    if (component)
                    {
                        component.newDuration = (1f - instantFireThreshhold) * this.newMaxChargeTime;
                    }
                }*/
            }
            if (base.fixedAge >= maxHoldTime)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            base.characterMotor.walkSpeedPenaltyCoefficient = walkSlowCoefficient;
            if (base.isAuthority)
            {
                if (!this.ShouldKeepChargingAuthority() && charge < instantFireThreshhold)
                {
                    this.outer.SetNextState(new KeepersVerdictFire
                    {
                        damageCoefficient = this.minimumDamageCoefficient
                    });
                }
                else if (!this.ShouldKeepChargingAuthority() && charge >= instantFireThreshhold)
                {
                    this.outer.SetNextState(new KeepersVerdictChargeSlam
                    {
                        damageCoefficient = this.damageCoefficient, 
                        charge = this.charge
                    });
                }
            }
        }

        public override void OnExit()
        {
            if (chargeEffectPrefab)
            {
                EntityState.Destroy(chargeEffectPrefab);
                Debug.LogWarning("DESTROYED chargeEffectPrefab.");
            }
            CrosshairUtils.OverrideRequest overrideRequest = this.crosshairOverrideRequest;
            if (overrideRequest != null)
            {
                overrideRequest.Dispose();
            }
            AkSoundEngine.StopPlayingID(soundId1);
            AkSoundEngine.StopPlayingID(soundId2);
            GetModelAnimator().SetBool("isCharging", false);
            base.characterMotor.walkSpeedPenaltyCoefficient = 1f;
            base.OnExit();
        }

        protected virtual bool ShouldKeepChargingAuthority()
        {
            return base.IsKeyDownAuthority();
        }

        private void HandleDamageCoefficients()
        {
            if (minimumDamageCoefficient > maximumDamageCoefficient)
            {
                float temp = minimumDamageCoefficient;
                minimumDamageCoefficient = maximumDamageCoefficient;
                maximumDamageCoefficient = temp;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
