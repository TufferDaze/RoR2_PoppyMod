using EntityStates;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Orbs;
using PoppyMod.Modules;
using System.Collections.Generic;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class IronAmbassador : BaseSkillState
	{
        private float stopwatch;
        private float baseDuration = 1.2f;
        private float duration;
        private float hopStrength = 5f;
        private float antiGravStrength = 30f;
        private float procCoefficient = 0.8f;
        private float bounceDamageIncreaseCoefficient = 1.3f;
        private float damageCoefficient = PoppyConfig.secondayDmgConfig.Value;
        private int maxBounces = 2;
        private float travelSpeed = 50f;
        private float bounceRange = 20f;
        private float selfInterruptPercentTime = 0.5f;
        private bool attemptedFire;
        private bool hasFired;
        private Transform mdlTransform;
        private HurtBox initialTarget;
        private ChildLocator childLocator;
        private Animator animator;
        private HuntressTracker tracker;

		public override void OnEnter()
		{
			base.OnEnter();
            tracker = GetComponent<HuntressTracker>();
            if (tracker && isAuthority)
            {
                initialTarget = tracker.GetTrackingTarget();
            }
            stopwatch = 0f;
            attemptedFire = false;
            hasFired = false;
            duration = baseDuration / attackSpeedStat;
            animator = base.GetModelAnimator();
            mdlTransform = base.GetModelTransform();
            childLocator = mdlTransform.GetComponent<ChildLocator>();
            characterMotor.Motor.ForceUnground();
            if (characterMotor && hopStrength != 0)
            {
                characterMotor.velocity.y = hopStrength;
            }
            PlayAnimation("FullBody, Override", "IronAmbassador", "IronAmbassador.playbackRate", duration);
            Util.PlaySound("PlayPoppyPassiveThrow", gameObject);
            Util.PlayAttackSpeedSound("PlayPoppyPassiveThrowSFX", gameObject, attackSpeedStat);
            characterBody.SetAimTimer(duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (!attemptedFire && stopwatch >= (animator.GetFloat("IronAmbassador.fire")/attackSpeedStat))
            {
                FireBuckler();
            }
            characterMotor.velocity.y += antiGravStrength * Time.fixedDeltaTime * (1f - stopwatch / duration);
            if (stopwatch >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!attemptedFire)
            {
                //Debug.LogWarning("IronAmbassador: OnExit fire used.");
                FireBuckler();
            }
            if (!hasFired && NetworkServer.active)
            {
                skillLocator.secondary.AddOneStock();
            }
        }

        private void FireBuckler()
        {
            if (!NetworkServer.active || attemptedFire)
            {
                return;
            }
            attemptedFire = true;
            ShieldyProjectile bucklerAttack = new ShieldyProjectile();
            bucklerAttack.procCoefficient = procCoefficient;
            bucklerAttack.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            bucklerAttack.damageValue = damageStat * damageCoefficient;
            bucklerAttack.damageType = DamageType.Generic;
            bucklerAttack.damageCoefficientPerBounce = bounceDamageIncreaseCoefficient;
            bucklerAttack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            bucklerAttack.attacker = base.gameObject;
            bucklerAttack.inflictor = base.gameObject;
            bucklerAttack.bouncesRemaining = maxBounces;
            bucklerAttack.speed = travelSpeed;
            bucklerAttack.bouncedObjects = new List<HealthComponent>();
            bucklerAttack.range = bounceRange;
            bucklerAttack.killConfirmed = false;
            HurtBox hurtBox = initialTarget;
            if (hurtBox)
            {
                hasFired = true;
                Transform transform = childLocator.FindChild("HandL");
                bucklerAttack.origin = transform.position;
                bucklerAttack.target = hurtBox;
                OrbManager.instance.AddOrb(bucklerAttack);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (stopwatch >= duration * selfInterruptPercentTime)
            {
                return InterruptPriority.Any;
            }
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(this.initialTarget));
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            this.initialTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }
    }
}