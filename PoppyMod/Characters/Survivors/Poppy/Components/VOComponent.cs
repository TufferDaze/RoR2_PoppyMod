using EntityStates.Missions.BrotherEncounter;
using PoppyMod.Modules;
using RoR2;
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
        private bool hasFiredBossSpawn;
        private static int firedEnter = 0;
        private static int firedDeath = 0;
        private CharacterBody body;

        public void Awake()
        {
            if (PoppyConfig.bossConfig.Value)
            {
                BossGroup.onBossGroupStartServer += BossGroup_onBossGroupStartServer;
                On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
                On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter += BossDeath_OnEnter;
            }
            if (PoppyConfig.purchaseVOConfig.Value)
            {
                On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            }
            body = gameObject.GetComponent<CharacterBody>();
            firedEnter = 0;
            firedDeath = 0;
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
                }
                idleStopwatch = 0f;
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
            if (!hasFiredBossSpawn)
            {
                hasFiredBossSpawn = true;
                AkSoundEngine.StopAll(gameObject);
                Util.PlaySound("PlayPoppyBossSpawn", gameObject);
            }
        }

        private void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, Phase1 self)
        {
            if (firedEnter == 0)
            {
                firedEnter++;
                AkSoundEngine.StopAll(gameObject);
                Util.PlaySound("PlayPoppyBossSpawnMoon", gameObject);
            }
            orig(self);
        }

        private void BossDeath_OnEnter(On.EntityStates.Missions.BrotherEncounter.BossDeath.orig_OnEnter orig, BossDeath self)
        {
            if (firedDeath == 0)
            {
                firedDeath++;
                AkSoundEngine.StopAll(gameObject);
                Util.PlaySound("PlayPoppyBossDeathMoon", gameObject);
            }
            orig(self);
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            GameObject cachedRef = LocalUserManager.GetFirstLocalUser().cachedBodyObject;
            if (body.outOfDanger && self.CanBeAffordedByInteractor(activator) && ReferenceEquals(activator.gameObject, cachedRef))
            {
                //Debug.Log("VOComponent: PurchaseInteract Fired.");
                AkSoundEngine.StopAll(cachedRef);
                Util.PlaySound("PlayPoppyItemBuy", cachedRef);
            }
            orig(self, activator);
        }
    }
}