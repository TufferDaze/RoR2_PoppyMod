using EntityStates;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Skills;
using R2API;
using PoppyMod.Modules.Characters;
using PoppyMod.Modules.BaseContent.BaseStates;
using System;
using System.Linq;
using System.Collections.Generic;
using RoR2.Orbs;
using PoppyMod.Modules;
using RoR2.CharacterAI;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class IronAmbassador : BaseState
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
        private bool attemptedFire;
        private Transform mdlTransform;
        private HurtBox initialTarget;
        private ChildLocator childLocator;
        private Animator animator;
        private HuntressTracker tracker;

		public override void OnEnter()
		{
			base.OnEnter();
            tracker = GetComponent<HuntressTracker>();
            initialTarget = tracker.GetTrackingTarget();
            stopwatch = 0f;
            attemptedFire = false;
            duration = baseDuration / attackSpeedStat;
            animator = base.GetModelAnimator();
            mdlTransform = base.GetModelTransform();
            childLocator = mdlTransform.GetComponent<ChildLocator>();
            characterMotor.Motor.ForceUnground();
            if (tracker && hopStrength != 0)
            {
                characterMotor.velocity.y = hopStrength;
            }
            PlayAnimation("FullBody, Override", "IronAmbassador", "IronAmbassador.playbackRate", duration);
            Util.PlayAttackSpeedSound("PlayPoppyPassiveThrow", gameObject, attackSpeedStat);
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
            if (stopwatch >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireBuckler()
        {
            if (!NetworkServer.active || attemptedFire)
            {
                return;
            }
            attemptedFire = true;
            ReboundProjectile bucklerAttack = new ReboundProjectile();
            bucklerAttack.damageValue = damageStat * damageCoefficient;
            bucklerAttack.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            bucklerAttack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            bucklerAttack.attacker = gameObject;
            bucklerAttack.procCoefficient = procCoefficient;
            bucklerAttack.bouncesRemaining = maxBounces;
            bucklerAttack.speed = travelSpeed;
            bucklerAttack.bouncedObjects = new List<HealthComponent>();
            bucklerAttack.range = bounceRange;
            bucklerAttack.damageCoefficientPerBounce = bounceDamageIncreaseCoefficient;
            HurtBox hurtBox = initialTarget;
            if (hurtBox)
            {
                Transform transform = childLocator.FindChild("HandL");
                bucklerAttack.origin = transform.position;
                bucklerAttack.target = hurtBox;
                OrbManager.instance.AddOrb(bucklerAttack);
            }
        }
    }
}