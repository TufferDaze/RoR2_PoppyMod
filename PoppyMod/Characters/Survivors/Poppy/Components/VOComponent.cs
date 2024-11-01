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
        private float shieldSoundStopwatch;
        private float shieldSoundTimer = 0.5f;
        private bool shieldVoiceCanFire = true;
        private bool passiveVoiceCanFire;
        private bool hasFiredSpawn;
        private bool hasFiredDefeat;
        private CharacterBody body;

        public void Awake()
        {
            if (PoppyConfig.bossConfig.Value)
            {
                BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
                BossGroup.onBossGroupDefeatedServer += BossGroup_onBossGroupDefeatedServer; ;
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
                if (body.inventory.GetItemCount(Items.shieldyDef) >= 1 && shieldVoiceCanFire)
                {
                    if (PoppyConfig.shieldyVOConfig.Value)
                    {
                        Util.PlaySound("PlayPoppyPassiveCollect", gameObject);
                    }
                    Util.PlaySound("PlayPoppyPassiveCollectSFX", gameObject);
                    shieldVoiceCanFire = false;
                }
            }
        }

        public void FixedUpdate()
        {
            shieldSoundStopwatch += Time.fixedDeltaTime;
            if (shieldSoundStopwatch >= shieldSoundTimer)
            {
                shieldVoiceCanFire = true;
                shieldSoundStopwatch = 0f;
            }
            if (PoppyConfig.idleVOConfig.Value && body.outOfDanger)
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
                if (obj.bestObservedName == "BrotherBody")
                {
                    Util.PlaySound("PlayPoppyBossSpawnMoon", gameObject);
                }
                else
                {
                    Util.PlaySound("PlayPoppyBossSpawn", gameObject);
                }
            }
        }

        private void BossGroup_onBossGroupDefeatedServer(BossGroup obj)
        {
            if (!hasFiredDefeat)
            {
                hasFiredDefeat = true;
                AkSoundEngine.StopAll(gameObject);
                if (obj && (obj.bestObservedName == "BrotherBody"))
                {
                    Util.PlaySound("PlayPoppyBossDeathMoon", gameObject);
                }
            }
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor sender, IInteractable interactInfo, GameObject interactObject)
        {
            try
            {
                if (interactObject.GetComponent<PurchaseInteraction>() && body.outOfDanger && ReferenceEquals(sender.gameObject, gameObject))
                {
                    AkSoundEngine.StopAll(gameObject);
                    Util.PlaySound("PlayPoppyItemBuy", gameObject);
                }
            }
            catch (Exception e)
            {
                //Debug.LogError($"{e}PoppyMod: VOComponent: CharacterBody body component null");
            }
        }
    }
}