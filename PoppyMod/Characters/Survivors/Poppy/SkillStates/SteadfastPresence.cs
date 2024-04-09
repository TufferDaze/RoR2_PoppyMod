using System;
using RoR2;
using R2API;
using EntityStates;

namespace PoppyMod.Survivors.Poppy.SkillStates
{
	public class SteadfastPresence : BaseSkillState
	{
        private float duration = 3f;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}