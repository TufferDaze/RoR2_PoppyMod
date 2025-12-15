using PoppyMod.Modules.BaseStates;
using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class HammerSwing : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            hitboxGroupName = "HammerGroup";

            float bonusDmg = PoppyStaticValues.primaryDamageBonusCoefficient * Mathf.Floor(Mathf.Max(0f, characterBody.maxBonusHealth) / PoppyStaticValues.primaryDamageBonusHPRequirement);

            damageType = DamageType.Generic;
            damageCoefficient = PoppyConfig.primaryDmgConfig.Value;
            procCoefficient = PoppyStaticValues.primaryProcCoefficient;
            //Debug.LogWarning("HP Bonus on Primary: " + bonusDmg);
            //Debug.LogWarning("Bonus HP: " + characterBody.maxBonusHealth);
            //Debug.LogWarning("Base HP: " + characterBody.maxHealth);
            pushForce = 300f;
            bonusForce = Vector3.zero;
            baseDuration = 2f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.2f;
            attackMoveStartPercentTime = 0.175f;
            attackEndPercentTime = 0.3f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 0.6f;

            hitStopDuration = 0.05f;
            attackRecoil = 0.5f;
            hitHopVelocity = 4f;
            forceForwardVelocity = false;
            forwardVelocityCurve = AnimationCurve.Linear(0f, 0f, 1f, 0.5f);

            if (PoppyConfig.bonkConfig.Value)
            {
                hitSoundString = "PlayPoppyBonkSmallSFX";
            }
            else
            {
                hitSoundString = "PlayPoppyAttackHitSFX";
            }
            swingSoundString = "PlayPoppyAttackSFX";
            muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            playbackRateParam = "Primary.playbackRate";
            swingEffectPrefab = PoppyAssets.hammerSwingEffect;
            hitEffectPrefab = PoppyAssets.hammerHitImpactEffect;

            impactSound = PoppyAssets.hammerHitSoundEvent.index;
            Util.PlaySound("PlayPoppyAttack", gameObject);
            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            if (GetIsCrit())
            {
                PlayCrossfade("FullBody, Override", "AttackCrit", playbackRateParam, duration, 0.1f * duration);
            }
            else
            {
                PlayCrossfade("Gesture, Override", "Attack" + (1 + swingIndex), playbackRateParam, duration, 0.1f * duration);
                if (isGrounded & !GetModelAnimator().GetBool("isMoving"))
                {
                    PlayCrossfade("FullBody, Override", "Attack" + (1 + swingIndex), playbackRateParam, duration, 0.1f * duration);
                }
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