using System;
using RoR2;
using EntityStates;
using UnityEngine;
using RoR2.Skills;
using PoppyMod.Survivors.Poppy;
using PoppyMod.Modules.BaseStates;
using PoppyMod.Modules.Characters;

namespace PoppyMod.Modules.BaseContent.BaseStates
{
    public class BasePoppyState : GenericCharacterMain
    {
        private float idleStopwatch;
        private float idleVoiceTimer = 60f;
        private float idleVoiceChance = PoppyConfig.voFreqConfig.Value;
        private float passiveStopwatch;
        private float passiveVoiceTimer = 10f;
        private bool passiveVoiceCanFire;
        private uint soundId;
        private bool hasFiredSpawn;
        private CharacterBody body;
        private GameObject masteryEmotePrefab;
        private GameObject masteryEmoteInstance;
        private Transform masteryTransform;
        private Animator masteryAnimator;
        private float masteryCoolDown = 1.7f;
        private float masteryStopwatch;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
            if (PoppyConfig.purchaseVOConfig.Value)
            {
                GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            }
            body = gameObject.GetComponent<CharacterBody>();
            childLocator = gameObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>().GetComponent<ChildLocator>();
            masteryTransform = childLocator.FindChild("MasteryEmoteLocation");
            if (!masteryEmotePrefab)
            {
                masteryEmotePrefab = PoppyAssets.masteryEmote;
                masteryEmoteInstance = UnityEngine.Object.Instantiate(masteryEmotePrefab, masteryTransform);
            }
            if (masteryEmoteInstance)
            {
                masteryAnimator = masteryEmoteInstance.GetComponent<Animator>();
            }
        }

        public override void Update()
        {
            base.Update();
            CheckEmote();
            if (body.inventory)
            {
                if (body.inventory.GetItemCount(Items.shieldyDef) >= 1)
                {
                    if (PoppyConfig.shieldyVOConfig.Value)
                    {
                        AkSoundEngine.StopAll(gameObject);
                        Util.PlaySound("PlayPoppyPassiveCollect", gameObject);
                    }
                    Util.PlaySound("PlayPoppyPassiveCollectSFX", gameObject);
                }
            }
            if (masteryEmoteInstance)
            {
                masteryEmoteInstance.transform.LookAt(Camera.main.transform);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            masteryStopwatch += Time.fixedDeltaTime;
            if (PoppyConfig.idleVOConfig.Value)
            {
                HandleIdleVO();
            }
            if (PoppyConfig.passiveVOConfig.Value)
            {
                HandlePassiveVO();
            }
        }

        public override void OnExit()
        {
            BossGroup.onBossGroupStartServer -= BossGroup_onBossGroupStartServer;
            GlobalEventManager.OnInteractionsGlobal -= GlobalEventManager_OnInteractionsGlobal;
            AkSoundEngine.StopPlayingID(soundId);
            Destroy(masteryEmoteInstance);
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
            if (isAuthority)
            {
                if (Config.GetKeyPressed(PoppyConfig.masteryConfig) && masteryStopwatch >= masteryCoolDown)
                {
                    masteryStopwatch = 0f;
                    masteryAnimator.Update(0f);
                    int layerIndex = masteryAnimator.GetLayerIndex("Base Layer");
                    masteryAnimator.PlayInFixedTime("MasteryEmote", layerIndex, 0f);
                    masteryAnimator.Update(0f);
                    Util.PlaySound("PlayPoppyMastery", gameObject);
                }
            }
        }

        private void HandleIdleVO()
        {
            idleStopwatch += Time.fixedDeltaTime;
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
        }

        private void HandlePassiveVO()
        {
            passiveStopwatch += Time.fixedDeltaTime;
            if (PoppyConfig.passiveVOConfig.Value && (passiveStopwatch >= passiveVoiceTimer || passiveStopwatch < 0f))
            {
                if (passiveVoiceCanFire && body.healthComponent.combinedHealthFraction <= PoppyStaticValues.passiveMissingHPThreshhold)
                {
                    AkSoundEngine.StopAll(gameObject);
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

        private void BossGroup_onBossGroupStartServer(BossGroup obj)
        {
            if (!hasFiredSpawn)
            {
                hasFiredSpawn = true;
                AkSoundEngine.StopAll(gameObject);
                Util.PlaySound("PlayPoppyBossSpawn", gameObject);
            }
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor sender, IInteractable interactInfo, GameObject interactObject)
        {
            if (interactObject.GetComponent<PurchaseInteraction>())
            {
                AkSoundEngine.StopAll(gameObject);
                Util.PlaySound("PlayPoppyItemBuy", gameObject);
            }
        }
    }
}
