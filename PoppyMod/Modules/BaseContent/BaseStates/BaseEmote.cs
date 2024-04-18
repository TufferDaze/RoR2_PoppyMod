using System;
using UnityEngine;
using RoR2;
using EntityStates;
using PoppyMod.Survivors.Poppy;

namespace PoppyMod.Modules.BaseStates
{
	public class BaseEmote : BaseState
	{
        public Animator animator;
        public string layerName = "FullBody, Override";
        public string animName = "";
        public string soundString = "";
        public bool isLooping = false;
        public float clipLen;
        private float stopwatch;
        private uint[] soundIdArray = new uint[10];

        public override void OnEnter()
        {
            base.OnEnter();
            stopwatch = 0f;
            animator = GetModelAnimator();
            if (soundString != "")
            {
                uint count = (uint)soundIdArray.Length;
                AkSoundEngine.GetPlayingIDsFromGameObject(gameObject, ref count, soundIdArray);
                if (soundIdArray[0] == 0u)
                {
                    Util.PlaySound(soundString, gameObject);
                }
            }
            if (animName != "")
            {
                PlayAnimation(layerName, animName);
            }
		}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (!isLooping)
            {
                if (!isGrounded || IsInputDown() || stopwatch >= clipLen)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            else
            {
                if (!isGrounded || IsInputDown())
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
            CheckEmote();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void CheckEmote()
        {
            if (isAuthority && isGrounded)
            {
                if (Config.GetKeyPressed(PoppyConfig.jokeConfig))
                {
                    this.outer.SetInterruptState(new JokeState(), InterruptPriority.Any);
                    return;
                }
                if (Config.GetKeyPressed(PoppyConfig.tauntConfig))
                {
                    this.outer.SetInterruptState(new TauntState(), InterruptPriority.Any);
                    return;
                }
                if (Config.GetKeyPressed(PoppyConfig.danceConfig))
                {
                    this.outer.SetInterruptState(new DanceState(), InterruptPriority.Any);
                    return;
                }
                if (Config.GetKeyPressed(PoppyConfig.laughConfig))
                {
                    this.outer.SetInterruptState(new LaughState(), InterruptPriority.Any);
                    return;
                }
            }
        }

        private bool IsInputDown()
        {
            if (inputBank.skill1.down || inputBank.skill2.down || inputBank.skill3.down || inputBank.skill4.down || inputBank.jump.down || inputBank.moveVector != Vector3.zero)
            {
                return true;
            }
            return false;
        }
    }
}