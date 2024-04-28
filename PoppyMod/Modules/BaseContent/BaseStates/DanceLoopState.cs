using System;
using EntityStates;
using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
    public class DanceLoopState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "Dance";
            isLooping = true;
            clipLen = 0;
            base.OnEnter();
        }
    }
}