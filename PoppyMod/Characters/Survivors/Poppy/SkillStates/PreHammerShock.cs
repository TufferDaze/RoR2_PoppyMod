using EntityStates;
using RoR2;
using UnityEngine;
//Since we are using effects from Commando's Barrage skill, we will also be using the associated namespace
//You can also use Addressables or LegacyResourcesAPI to load whichever effects you like
using EntityStates.Commando.CommandoWeapon;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
    public class PreHammerShock : BaseCharacterMain
    {
        private float duration = 0.0f;
        public float baseDuration = 0.334f;
        public float upwardVelocity = 40.0f;

        //OnEnter() runs once at the start of the skill
        //All we do here is create a BulletAttack and fire it
        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            this.duration = baseDuration / this.attackSpeedStat;
            //base.PlayAnimation("Body", "PreGroundSlam", "GroundSlam.playbackRate", this.duration);
            //Util.PlaySound([skillname].enterSoundString, base.gameObject);
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.disableAirControlUntilCollision = false;
            base.characterMotor.velocity.y = upwardVelocity;
            PlayAnimation("FullBody, Override", "PreHammerShock", "PreHammerShock.playbackRate", duration);
            Util.PlayAttackSpeedSound("PlayPoppyQ", gameObject, attackSpeedStat);
        }

        //FixedUpdate() runs almost every frame of the skill
        //Here, we end the skill once it exceeds its intended duration
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.moveDirection = base.inputBank.moveVector;
            if (base.fixedAge > this.duration)
            {
                this.outer.SetNextState(new HammerShock());
            }
        }

        //GetMinimumInterruptPriority() returns the InterruptPriority required to interrupt this skill
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}