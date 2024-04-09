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
        private float baseChargeTime = 5f;
        private float maxHoldTime = 10f;
        private float newMaxChargeTime;
        private float minimumDamageCoefficient = 6f;
        private float maximumDamageCoefficient = 36f;
        private float damageCoefficient = 6f;
        private float charge;
        private float chargeThreshhold = 1.0f;
        private float walkSlowCoefficient = 0.5f;
        private Transform chargeVfxInstanceTransform;
        public static GameObject crosshairOverridePrefab;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        public static string chargeVfxChildLocatorName;
        public static GameObject chargeVfxPrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            this.newMaxChargeTime = baseChargeTime / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.isSprinting = false;
            }
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
                Transform transform = base.FindModelChild(chargeVfxChildLocatorName);
                if (transform)
                {
                    this.chargeVfxInstanceTransform = UnityEngine.Object.Instantiate<GameObject>(chargeVfxPrefab, transform).transform;
                    ScaleParticleSystemDuration component = this.chargeVfxInstanceTransform.GetComponent<ScaleParticleSystemDuration>();
                    if (component)
                    {
                        component.newDuration = (1f - chargeThreshhold) * this.newMaxChargeTime;
                    }
                }
            }
            if (this.chargeVfxInstanceTransform)
            {
                base.characterMotor.walkSpeedPenaltyCoefficient = walkSlowCoefficient;
            }
            if (base.isAuthority)
            {
                this.AuthorityFixedUpdate();
            }
        }

        private void AuthorityFixedUpdate()
        {
            if (!this.ShouldKeepChargingAuthority())
            {
                this.outer.SetNextState(this.GetNextStateAuthority());
            }
        }

        protected virtual bool ShouldKeepChargingAuthority()
        {
            return base.IsKeyDownAuthority();
        }

        protected virtual EntityState GetNextStateAuthority()
        {
            return new KeepersVerdictSlam
            {
                damageCoefficient = this.damageCoefficient
            };
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
