using System;
using RoR2;
using EntityStates;
using UnityEngine;
using RoR2.Skills;

namespace PoppyMod.Modules.BaseContent.BaseStates
{
    public class BasePoppyState : GenericCharacterMain
    {
        /*private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = base.GetComponent<Animator>();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.HandleSecondarySkill();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void HandleSecondarySkill()
        {
            InputBankTest.ButtonState buttonState = base.inputBank.skill2;
            GenericSkill skill = base.skillLocator.secondary;

            if (!buttonState.down || !skill)
            {
                return;
            }
            if (skill.mustKeyPress && buttonState.hasPressBeenClaimed)
            {
                return;
            }
            if (tracker.GetTrackingTarget() && CanExecuteSkill(skill))
            {
                Debug.Log("BasePoppyState: This is being fired!!");
                if (skill.stock < 1)
                {
                    skill.AddOneStock();
                }
                skill.ExecuteIfReady();
                buttonState.hasPressBeenClaimed = true;
            }
            else
            {
                skill.RemoveAllStocks();
            }
        }*/
    }
}
