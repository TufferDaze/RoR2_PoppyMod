using System;
using RoR2;
using EntityStates;
using UnityEngine;
using RoR2.Skills;
using PoppyMod.Survivors.Poppy;
using PoppyMod.Modules.BaseStates;

namespace PoppyMod.Modules.BaseContent.BaseStates
{
    public class BasePoppyState : GenericCharacterMain
    {
        private float idleStopwatch;
        private float idleVoiceTimer = 30f;
        private float idleVoiceChance = PoppyConfig.voFreqConfig.Value;
        private float pickupVoiceChance = 0.5f;
        private float passiveStopwatch;
        private float passiveVoiceTimer = 10f;
        private bool passiveVoiceCanFire;
        private uint soundId;
        private bool hasFiredSpawn;
        private CharacterBody body;

        public override void OnEnter()
        {
            base.OnEnter();
            BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            body = GetComponent<CharacterBody>();
        }

        public override void Update()
        {
            base.Update();
            CheckEmote();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            idleStopwatch += Time.fixedDeltaTime;
            passiveStopwatch += Time.fixedDeltaTime;
            if (idleStopwatch >= idleVoiceTimer)
            {
                bool playSound = UnityEngine.Random.value < idleVoiceChance;
                if (playSound)
                {
                    soundId = Util.PlaySound("PlayPoppyIdle", gameObject);
                    idleStopwatch = 0f;
                }
                else
                {
                    idleStopwatch = 0f;
                }
            }
            if (passiveStopwatch >= passiveVoiceTimer || passiveStopwatch < 0f)
            {
                if (passiveVoiceCanFire && body.healthComponent.combinedHealthFraction <= PoppyStaticValues.passiveMissingHPThreshhold)
                {
                    Util.PlaySound("PlayPoppyWPassive", gameObject);
                    passiveStopwatch = 0f;
                    passiveVoiceCanFire = false;
                }
            }
            if (body.healthComponent.combinedHealthFraction > PoppyStaticValues.passiveMissingHPThreshhold)
            {
                passiveVoiceCanFire = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            BossGroup.onBossGroupStartServer -= BossGroup_onBossGroupStartServer;
            GlobalEventManager.OnInteractionsGlobal -= GlobalEventManager_OnInteractionsGlobal;
            AkSoundEngine.StopPlayingID(soundId);
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

        private void BossGroup_onBossGroupStartServer(BossGroup obj)
        {
            if (!hasFiredSpawn)
            {
                hasFiredSpawn = true;
                Util.PlaySound("PlayPoppyBossSpawn", gameObject);
            }
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor sender, IInteractable interactInfo, GameObject interactObject)
        {
            if (interactObject.GetComponent<PurchaseInteraction>())
            {
                Util.PlaySound("PlayPoppyItemBuy", gameObject);
            }
        }
    }
}
