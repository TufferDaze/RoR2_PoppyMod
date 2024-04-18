using System;
using EntityStates;
using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
    public class TauntState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "Taunt";
            soundString = "PlayPoppyTaunt";
            isLooping = false;
            clipLen = 5.333f;
            base.OnEnter();
        }
    }
}