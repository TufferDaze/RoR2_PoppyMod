using System;
using EntityStates;
using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
    public class DanceState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "IdleToDance";
            isLooping = true;
            clipLen = 0;
            base.OnEnter();
        }
    }
}