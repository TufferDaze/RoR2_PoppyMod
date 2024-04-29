using EntityStates;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class KeepersVerdictCharge : BaseSkillState
    {
        private float baseChargeTime = 4f;
        private float maxHoldTime = 7f;
        private float newMaxChargeTime;
        private float minimumDamageCoefficient = PoppyConfig.spec1MinDmgConfig.Value;
        private float maximumDamageCoefficient = PoppyConfig.spec1MaxDmgConfig.Value;
        private float damageCoefficient = PoppyConfig.spec1MinDmgConfig.Value;
        private float charge;
        private float instantFireThreshhold = 0.1f;
        private float chargeThreshhold = 1.0f;
        private float walkSlowCoefficient = 0.5f;
        private uint soundId1;
        private uint soundId2;
        private Transform chargeVfxInstanceTransform;
        public static GameObject crosshairOverridePrefab;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        public static string chargeVfxChildLocatorName;
        public static GameObject chargeVfxPrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            HandleDamageCoefficients();
            this.newMaxChargeTime = baseChargeTime / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.isSprinting = false;
            }
            soundId1 = Util.PlaySound("PlayPoppyRCharge", gameObject);
            soundId2 = Util.PlaySound("PlayPoppyRChargeSFX", gameObject);
            /*if (GetModelAnimator().GetBool("isMoving"))
            {
                PlayAnimation("FullBody, Override", "KeepersVerdictChargeRun");
            }
            else
            {
                PlayAnimation("FullBody, Override", "KeepersVerdictChargeIdle");
            }*/
            PlayAnimation("Gesture, Override", "KeepersVerdictCharge");
            GetModelAnimator().SetBool("isCharging", true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.charge = Mathf.Clamp01(base.fixedAge / this.newMaxChargeTime);
            this.damageCoefficient = Mathf.Clamp(maximumDamageCoefficient * this.charge, minimumDamageCoefficient, maximumDamageCoefficient);
            base.characterBody.SetSpreadBloom(this.charge, true);
            base.characterBody.SetAimTimer(3f);
            if (base.fixedAge >= maxHoldTime)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            if (this.charge >= chargeThreshhold && !this.chargeVfxInstanceTransform && chargeVfxPrefab)
            {
                if (crosshairOverridePrefab && this.crosshairOverrideRequest == null)
                {
                    this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
                }
                /*Transform transform = base.FindModelChild(chargeVfxChildLocatorName);
                if (transform)
                {
                    this.chargeVfxInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(chargeVfxPrefab, transform).transform;
                    ScaleParticleSystemDuration component = this.chargeVfxInstanceTransform.GetComponent<ScaleParticleSystemDuration>();
                    if (component)
                    {
                        component.newDuration = (1f - chargeThreshhold) * this.newMaxChargeTime;
                    }
                }*/
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
                        damageCoefficient = this.damageCoefficient
                    });
                }
            }
        }

        public override void OnExit()
        {
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
