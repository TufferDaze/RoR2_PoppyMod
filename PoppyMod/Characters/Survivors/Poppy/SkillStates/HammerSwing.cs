﻿using PoppyMod.Modules.BaseStates;
using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class HammerSwing : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            hitboxGroupName = "HammerGroup";

            damageType = DamageType.Generic;
            damageCoefficient = PoppyStaticValues.primaryDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 2f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.6f;

            hitStopDuration = 0.012f;
            attackRecoil = 0.5f;
            hitHopVelocity = 4f;

            swingSoundString = "HenrySwordSwing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Slash.playbackRate";
            swingEffectPrefab = PoppyAssets.hammerSwingEffect;
            hitEffectPrefab = PoppyAssets.hammerHitImpactEffect;

            impactSound = PoppyAssets.hammerHitSoundEvent.index;
            Util.PlayAttackSpeedSound("PlayPoppyAttack", gameObject, attackSpeedStat);

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Attack" + (1 + swingIndex), playbackRateParam, duration, 0.1f * duration);
            if (isGrounded & !GetModelAnimator().GetBool("isMoving"))
            {
                PlayCrossfade("FullBody, Override", "Attack" + (1 + swingIndex), playbackRateParam, duration, 0.1f * duration);
            }
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}