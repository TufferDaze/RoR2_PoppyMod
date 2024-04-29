using EntityStates;
using PoppyMod.Modules.BaseStates;
using PoppyMod.Modules;
using RoR2;
using System;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.Components
{
	public class VOComponent : MonoBehaviour
	{
        private float idleStopwatch;
        private float idleVoiceTimer = 60f;
        private float idleVoiceChance = PoppyConfig.voFreqConfig.Value;
        private float passiveStopwatch;
        private float passiveVoiceTimer = 30f;
        private bool passiveVoiceCanFire;
        private bool hasFiredSpawn;
        private CharacterBody body;

        public void Awake()
        {
            if (PoppyConfig.bossConfig.Value)
            {
                BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
            }
            if (PoppyConfig.purchaseVOConfig.Value)
            {
                GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
            }
            body = gameObject.GetComponent<CharacterBody>();
        }

        public void Update()
        {
            if (body.inventory)
            {
                if (body.inventory.GetItemCount(Items.shieldyDef) >= 1)
                {
                    if (PoppyConfig.shieldyVOConfig.Value)
                    {
                        Util.PlaySound("PlayPoppyPassiveCollect", gameObject);
                    }
                    Util.PlaySound("PlayPoppyPassiveCollectSFX", gameObject);
                }
            }
        }

        public void FixedUpdate()
        {
            if (PoppyConfig.idleVOConfig.Value && body.outOfCombat)
            {
                HandleIdleVO();
            }
            if (PoppyConfig.passiveVOConfig.Value)
            {
                HandlePassiveVO();
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
                    Util.PlaySound("PlayPoppyIdle", gameObject);
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
            if (interactObject.GetComponent<PurchaseInteraction>() && body.outOfCombat)
            {
                AkSoundEngine.StopAll(gameObject);
                Util.PlaySound("PlayPoppyItemBuy", gameObject);
            }
        }
    }
}