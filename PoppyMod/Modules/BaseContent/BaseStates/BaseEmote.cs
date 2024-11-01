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
        public string soundFXString = "";
        public bool isLooping = false;
        public float clipLen = 1f;
        private uint soundFXId;
        private uint[] soundIdArray = new uint[8];

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            if (soundString != "")
            {
                uint count = (uint)soundIdArray.Length;
                AkSoundEngine.GetPlayingIDsFromGameObject(gameObject, ref count, soundIdArray);
                if (IsEmpty(soundIdArray))
                {
                    Util.PlaySound(soundString, gameObject);
                }
                if (soundFXString != "")
                {
                    soundFXId = Util.PlaySound(soundFXString, gameObject);
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
            if (!isLooping)
            {
                if (!isGrounded || IsInputDown() || base.fixedAge >= clipLen)
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
            if (soundFXId != 0u)
            {
                AkSoundEngine.StopPlayingID(soundFXId);
            }
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
                    this.outer.SetInterruptState(new DanceIntroState(), InterruptPriority.Any);
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

        private bool IsEmpty(uint[] idArray)
        {
            foreach (uint id in idArray)
            {
                if (id != 0u)
                {
                    return false;
                }
            }
            return true;
        }
    }
}