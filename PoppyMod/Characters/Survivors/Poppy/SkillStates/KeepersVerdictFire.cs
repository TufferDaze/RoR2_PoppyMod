using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.SkillStates
{

	public class KeepersVerdictFire : BaseSkillState
	{
		public float damageCoefficient;
        private Transform modelTransform;
        private float attackDelay = 0.25f;
        private float procCoefficient = PoppyStaticValues.special1SlamProcCoefficient;
        private float upForceCoefficient = PoppyConfig.spec1FireForceConfig.Value;
        private OverlapAttack attack;
        private List<HurtBox> enemiesHit = new List<HurtBox>();
        private float duration = 1f;

		public override void OnEnter()
		{
			base.OnEnter();
            modelTransform = base.GetModelTransform();
            attack = new OverlapAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = GetTeam(),
                damage = damageStat * damageCoefficient,
                damageType = DamageType.Stun1s,
                forceVector = new Vector3(0, upForceCoefficient, 0),
                hitBoxGroup = Array.Find<HitBoxGroup>(this.modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "KeeperGroup"),
                procCoefficient = procCoefficient
            };
            Util.PlaySound("PlayPoppyRFire", gameObject);
            Util.PlaySound("PlayPoppyRFireSFX", gameObject);
            PlayAnimation("Fullbody, Override", "KeepersVerdictFire");
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= attackDelay)
            {
                if (attack.Fire(enemiesHit))
                {
                    if (PoppyConfig.bonkConfig.Value)
                    {
                        Util.PlaySound("PlayPoppyBonkBigSFX", gameObject);
                    }
                    else
                    {
                        Util.PlaySound("PlayPoppyRFireHitSFX", gameObject);
                    }
                }
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