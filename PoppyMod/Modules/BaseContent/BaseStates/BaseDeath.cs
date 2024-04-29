using EntityStates;
using RoR2;
using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
	public class BaseDeath : GenericCharacterDeath
	{
        private float funnyDeathThreshhold = 0.25f;
        private string animString = "";

        public override void OnEnter()
        {
            base.OnEnter();
            if (Random.value <= funnyDeathThreshhold)
            {
                animString = "Death2";
            }
            else
            {
                animString = "Death1";
            }
            PlayCrossfade("FullBody, Override", animString, 0.1f);
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