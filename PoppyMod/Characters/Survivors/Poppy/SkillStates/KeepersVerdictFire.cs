using EntityStates;
using EntityStates.BrotherMonster;
using RoR2;
using System;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.SkillStates
{

	public class KeepersVerdictFire : BaseSkillState
	{
		public float damageCoefficient;
        private Transform modelTransform;
        private float attackDelay = 0.10f;
        private float procCoefficient = 1f;
        private float stopwatch;
        private OverlapAttack attack;
        private float duration = 1f;

		public override void OnEnter()
		{
			base.OnEnter();
            stopwatch = 0f;
            modelTransform = base.GetModelTransform();
            attack = new OverlapAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = GetTeam(),
                damage = damageStat * damageCoefficient,
                forceVector = Vector3.upVector * 50f,
                hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "KeeperGroup"),
                impactSound = WeaponSlam.weaponImpactSound.index,
                procCoefficient = procCoefficient
            };
            Util.PlaySound("PlayPoppyRFire", gameObject);
            if (PoppyConfig.bonkConfig.Value)
            {
                Util.PlaySound("PlayPoppyBonkBigSFX", gameObject);
            }
            else
            {
                Util.PlaySound("PlayPoppyRFireSFX", gameObject);
            }
            PlayAnimation("Fullbody, Override", "KeepersVerdictFire");
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (base.isAuthority && stopwatch >= attackDelay)
            {
                attack.Fire();
            }
            if (base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}