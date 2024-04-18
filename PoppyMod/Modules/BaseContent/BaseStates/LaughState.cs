using System;
using EntityStates;
using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
    public class LaughState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "Laugh";
            soundString = "PlayPoppyLaugh";
            isLooping = false;
            clipLen = 2.5f;
            base.OnEnter();
        }
    }
}