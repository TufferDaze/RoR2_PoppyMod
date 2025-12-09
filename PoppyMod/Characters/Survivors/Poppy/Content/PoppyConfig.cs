using BepInEx.Configuration;
using UnityEngine;
using PoppyMod.Modules;
using RiskOfOptions;
using RiskOfOptions.Options;
using System.Runtime.CompilerServices;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyConfig
    {
        public static ConfigEntry<bool> bonkConfig;
        public static ConfigEntry<bool> bossConfig;
        public static ConfigEntry<bool> purchaseVOConfig;
        public static ConfigEntry<bool> shieldyVOConfig;
        public static ConfigEntry<bool> shieldyChatMsgConfig;
        public static ConfigEntry<bool> passiveVOConfig;
        public static ConfigEntry<bool> rChargeVOConfig;
        public static ConfigEntry<bool> idleVOConfig;
        public static ConfigEntry<float> voFreqConfig;
        public static ConfigEntry<int> baseHPConfig;
        public static ConfigEntry<int> baseArmorConfig;
        public static ConfigEntry<float> baseDamageConfig;
        public static ConfigEntry<float> baseCritConfig;
        public static ConfigEntry<float> baseAttackSpeedConfig;
        public static ConfigEntry<float> baseMoveSpeedConfig;
        public static ConfigEntry<float> passiveConfig;
        public static ConfigEntry<float> primaryDmgConfig;
        public static ConfigEntry<float> secondayDmgConfig;
        public static ConfigEntry<float> secondayHPConfig;
        public static ConfigEntry<float> util1DmgConfig;
        public static ConfigEntry<float> util2DmgConfig;
        public static ConfigEntry<float> util2SpdConfig;
        public static ConfigEntry<float> spec1MinDmgConfig;
        public static ConfigEntry<float> spec1MaxDmgConfig;
        public static ConfigEntry<float> spec1FireForceConfig;
        public static ConfigEntry<float> spec1SlamForceConfig;
        public static ConfigEntry<float> spec1WaveForceConfig;
        public static ConfigEntry<float> spec2DmgConfig;
        public static ConfigEntry<float> spec2HPConfig;
        public static ConfigEntry<KeyboardShortcut> jokeConfig;
        public static ConfigEntry<KeyboardShortcut> tauntConfig;
        public static ConfigEntry<KeyboardShortcut> danceConfig;
        public static ConfigEntry<KeyboardShortcut> laughConfig;
        public static ConfigEntry<KeyboardShortcut> masteryConfig;

        public static void Init()
        {
            string audioSection = "Audio";
            string statSection = "Stats";
            string abilitySection = "Abilities";
            string emoteSection = "Emotes";

            ModSettingsManager.SetModIcon(PoppySurvivor.instance.assetBundle.LoadAsset<Sprite>("poppy_square"));
            ModSettingsManager.SetModDescription("Small idiot rat thing.");

            // Audio Control
            bonkConfig = Config.BindAndOptions(audioSection, "Bonk", false, "Enable only if you are really truely absolutely prepared for maximum bonkage.");
            bossConfig = Config.BindAndOptions(audioSection, "Boss Chatter", true, "Whether or not to say something when a teleporter boss spawns.", true);
            purchaseVOConfig = Config.BindAndOptions(audioSection, "Purchase Chatter", true, "Whether or not to say something when interacting with chests and the like.", true);
            shieldyVOConfig = Config.BindAndOptions(audioSection, "Shieldy Pickup Chatter", true, "Whether or not to say something when picking up Shieldy.");
            shieldyChatMsgConfig = Config.BindAndOptions(audioSection, "Shieldy Pickup Message", false, "Whether or not the pickup message shows up in chat.", true);
            passiveVOConfig = Config.BindAndOptions(audioSection, "Passive Chatter", true, "Whether or not to say something when below 50% HP.");
            rChargeVOConfig = Config.BindAndOptions(audioSection, "Keepers Verdict Chatter", true, "Whether or not to say something when firing a charged Keepers Verdict.");
            idleVOConfig = Config.BindAndOptions(audioSection, "Idle Chatter", true, "Whether or not the rat says something every 30 seconds.");
            voFreqConfig = Config.BindAndOptionsSlider(audioSection, "Chatter Freqency", 1f, "Frequency of idle chatter.", 0f, 1f);

            // Stats
            baseHPConfig = Config.BindAndOptionsSlider(statSection, "Base HP", PoppyStaticValues.baseHealth, "", 1, 1000, true);
            baseArmorConfig = Config.BindAndOptionsSlider(statSection, "Base Armor", PoppyStaticValues.baseArmor, "", 1, 1000, true);
            baseDamageConfig = Config.BindAndOptionsSlider(statSection, "Base Damage", PoppyStaticValues.baseDamage, "", 1f, 1000f, true);
            baseCritConfig = Config.BindAndOptions<float>(statSection, "Base Crit", PoppyStaticValues.baseCrit, "", true);
            baseAttackSpeedConfig = Config.BindAndOptionsSlider(statSection, "Base Attack Speed", PoppyStaticValues.baseAttackSpeed, "", 1f, 1000f, true);
            baseMoveSpeedConfig = Config.BindAndOptionsSlider(statSection, "Base Move Speed", PoppyStaticValues.baseMoveSpeed, "", 1f, 1000f, true);

            // Passive
            passiveConfig = Config.BindAndOptions<float>(abilitySection, "Passive Armor", PoppyStaticValues.passiveArmorIncreaseCoefficient, "Percentage of your base armor to gain at max passive stacks.");
            
            // Primary
            primaryDmgConfig = Config.BindAndOptions<float>(abilitySection, "Primary Damage", PoppyStaticValues.primaryDamageCoefficient, "Damage coefficient of the primary attack.");

            // Secondary
            secondayDmgConfig = Config.BindAndOptions<float>(abilitySection, "Secondary Damage", PoppyStaticValues.secondaryDamageCoefficient, "Damage coefficient of Iron Ambassador.");
            secondayHPConfig = Config.BindAndOptions<float>(abilitySection, "Secondary Shield", PoppyStaticValues.secondaryHPBarrierCoefficient, "Max HP shield coefficient of Shieldy.");

            // Utility #1
            util1DmgConfig = Config.BindAndOptions<float>(abilitySection, "Utility #1 Damage", PoppyStaticValues.utility1DamageCoefficient, "Damage coefficient of Heroic Charge.");

            // Utility #2
            util2DmgConfig = Config.BindAndOptions<float>(abilitySection, "Utility #2 Damage", PoppyStaticValues.utility2DamageCoefficient, "Damage coefficient of Steadfast Presence.");
            util2SpdConfig = Config.BindAndOptions<float>(abilitySection, "Utility #2 Movespeed", PoppyStaticValues.utility2MoveCoefficient, "Percentage of your base movespeed gained during Steadfast Presence.");

            // Special #1
            spec1MinDmgConfig = Config.BindAndOptions<float>(abilitySection, "Special #1 Min Damage", PoppyStaticValues.special1MinDamageCoefficient, "Minimum damage coefficient of Keeper\'s Verdict. If the min > max, then the values are swapped.");
            spec1MaxDmgConfig = Config.BindAndOptions<float>(abilitySection, "Special #1 Max Damage", PoppyStaticValues.special1MaxDamageCoefficient, "Maximum damage coefficient of Keeper\'s Verdict. If the min > max, then the values are swapped.");
            spec1FireForceConfig = Config.BindAndOptionsSlider(abilitySection, "Special #1 Uncharged Force", PoppyStaticValues.special1FireForce, "Uncharged force of Keeper\'s Verdict. Applies to direct hits.", 100f, 10000f);
            spec1SlamForceConfig = Config.BindAndOptionsSlider(abilitySection, "Special #1 Slam Force", PoppyStaticValues.special1ChargeSlamForce, "Slam force of Keeper\'s Verdict. Applies to direct hits.", 100f, 10000f);
            spec1WaveForceConfig = Config.BindAndOptionsSlider(abilitySection, "Special #1 Wave Force", PoppyStaticValues.special1ChargeWaveForce, "Shockwave force of Keeper\'s Verdict. Applies to shockwaves.", 100f, 10000f);

            // Special #2
            spec2DmgConfig = Config.BindAndOptions<float>(abilitySection, "Special #2 Damage", PoppyStaticValues.special2DamageCoefficient, "Damage coefficient of Hammer Shock.");
            spec2HPConfig = Config.BindAndOptions<float>(abilitySection, "Special #2 HP Damage", PoppyStaticValues.special2HPDamageCoefficient, "Percentage of enemy max HP to deal as damage.");

            // Emote Keybinds
            jokeConfig = Config.MyConfig.Bind(new ConfigDefinition(emoteSection, "Joke Keybind"), new KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftControl), null);
            tauntConfig = Config.MyConfig.Bind(new ConfigDefinition(emoteSection, "Taunt Keybind"), new KeyboardShortcut(KeyCode.Alpha2, KeyCode.LeftControl), null);
            danceConfig = Config.MyConfig.Bind(new ConfigDefinition(emoteSection, "Dance Keybind"), new KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftControl), null);
            laughConfig = Config.MyConfig.Bind(new ConfigDefinition(emoteSection, "Laugh Keybind"), new KeyboardShortcut(KeyCode.Alpha4, KeyCode.LeftControl), null);
            masteryConfig = Config.MyConfig.Bind(new ConfigDefinition(emoteSection, "Mastery Keybind"), new KeyboardShortcut(KeyCode.Alpha6, KeyCode.LeftControl), null);
            ModSettingsManager.AddOption(new KeyBindOption(jokeConfig));
            ModSettingsManager.AddOption(new KeyBindOption(tauntConfig));
            ModSettingsManager.AddOption(new KeyBindOption(danceConfig));
            ModSettingsManager.AddOption(new KeyBindOption(laughConfig));
            ModSettingsManager.AddOption(new KeyBindOption(masteryConfig));
        }
    }
}
