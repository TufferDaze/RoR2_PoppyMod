using BepInEx.Configuration;
using UnityEngine;
using JetBrains.Annotations;
using PoppyMod.Modules;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyConfig
    {
        public static ConfigEntry<int> allVolumeConfig;
        public static ConfigEntry<bool> bonkConfig;
        public static ConfigEntry<bool> bossConfig;
        public static ConfigEntry<bool> purchaseVOConfig;
        public static ConfigEntry<bool> shieldyVOConfig;
        public static ConfigEntry<bool> passiveVOConfig;
        public static ConfigEntry<bool> rChargeVOConfig;
        public static ConfigEntry<bool> idleVOConfig;
        public static ConfigEntry<float> voFreqConfig;
        public static ConfigEntry<float> passiveConfig;
        public static ConfigEntry<float> primaryDmgConfig;
        public static ConfigEntry<float> secondayDmgConfig;
        public static ConfigEntry<float> secondayHPConfig;
        public static ConfigEntry<float> util1DmgConfig;
        public static ConfigEntry<float> util2DmgConfig;
        public static ConfigEntry<float> util2SpdConfig;
        public static ConfigEntry<float> spec1MinDmgConfig;
        public static ConfigEntry<float> spec1MaxDmgConfig;
        public static ConfigEntry<float> spec2DmgConfig;
        public static ConfigEntry<float> spec2HPConfig;
        public static ConfigEntry<KeyboardShortcut> jokeConfig;
        public static ConfigEntry<KeyboardShortcut> tauntConfig;
        public static ConfigEntry<KeyboardShortcut> danceConfig;
        public static ConfigEntry<KeyboardShortcut> laughConfig;
        public static ConfigEntry<KeyboardShortcut> masteryConfig;

        public static void Init()
        {
            string volumeSection = "Audio";
            string abilitySection = "Abilities";
            string emoteSection = "Emotes";

            // Volume Control
            allVolumeConfig = Config.BindAndOptionsSlider(volumeSection, "Master Volume", 100, "Set the volume level for voicelines and sfx.", 0, 100);
            bonkConfig = Config.BindAndOptions(volumeSection, "Bonk", false, "Enable only if you are really truely absolutely prepared for maximum bonkage.");
            bossConfig = Config.BindAndOptions(volumeSection, "Boss Chatter", true, "Whether or not to say something when a teleporter boss spawns.");
            purchaseVOConfig = Config.BindAndOptions(volumeSection, "Purchase Chatter", true, "Whether or not to say something when interacting with chests and the like.");
            shieldyVOConfig = Config.BindAndOptions(volumeSection, "Shieldy Pickup Chatter", true, "Whether or not to say something when picking up Shieldy.");
            passiveVOConfig = Config.BindAndOptions(volumeSection, "Passive Chatter", true, "Whether or not to say something when below 50% HP.");
            rChargeVOConfig = Config.BindAndOptions(volumeSection, "Keepers Verdict Chatter", true, "Whether or not to say something when firing a charged Keepers Verdict.");
            idleVOConfig = Config.BindAndOptions(volumeSection, "Idle Chatter", true, "Whether or not the rat says something every 30 seconds.");
            voFreqConfig = Config.BindAndOptionsSlider(volumeSection, "Chatter Freqency", 1f, "Frequency of idle chatter.", 0f, 1f);

            // Passive
            passiveConfig = Config.BindAndOptions<float>(abilitySection, "Passive Armor", PoppyStaticValues.passiveArmorIncreaseCoefficient, "Percentage of your base armor to gain at max passive stacks.");
            
            // Primary
            primaryDmgConfig = Config.BindAndOptions<float>(abilitySection, "Primary Damage", PoppyStaticValues.primaryDamageCoefficient, "Damage coefficient of the primary attack.");

            // Secondary
            secondayDmgConfig = Config.BindAndOptions<float>(abilitySection, "Secondary Damage", PoppyStaticValues.secondaryDamageCoefficient, "Damage coefficient of Iron Ambassador.");
            secondayHPConfig = Config.BindAndOptions<float>(abilitySection, "Secondary Shield", PoppyStaticValues.secondaryHPCoefficient, "Max HP shield coefficient of Shieldy.");

            // Utility #1
            util1DmgConfig = Config.BindAndOptions<float>(abilitySection, "Utility #1 Damage", PoppyStaticValues.utility1DamageCoefficient, "Damage coefficient of Heroic Charge.");

            // Utility #2
            util2DmgConfig = Config.BindAndOptions<float>(abilitySection, "Utility #2 Damage", PoppyStaticValues.utility2DamageCoefficient, "Damage coefficient of Steadfast Presence.");
            util2SpdConfig = Config.BindAndOptions<float>(abilitySection, "Utility #2 Movespeed", PoppyStaticValues.utility2MoveCoefficient, "Percentage of your base movespeed gained during Steadfast Presence.");

            // Special #1
            spec1MinDmgConfig = Config.BindAndOptions<float>(abilitySection, "Special #1 Min Damage", PoppyStaticValues.special1MinDamageCoefficient, "Minimum damage coefficient of Keepers Verdict. If the min > max, then the values are swapped.");
            spec1MaxDmgConfig = Config.BindAndOptions<float>(abilitySection, "Special #1 Max Damage", PoppyStaticValues.special1MaxDamageCoefficient, "Maximum damage coefficient of Keepers Verdict. If the min > max, then the values are swapped.");

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
