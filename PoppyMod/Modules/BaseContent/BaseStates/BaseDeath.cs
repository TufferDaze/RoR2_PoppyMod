using System;
using EntityStates;
using RoR2;
using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
	public class BaseDeath : GenericCharacterDeath
	{
        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("FullBody, Override", "Death", 0.1f);
            Util.PlaySound("PlayPoppyDeath", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}