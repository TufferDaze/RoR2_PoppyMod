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
        public static ConfigEntry<bool> idleVOConfig;
        public static ConfigEntry<float> voFreqConfig;
        public static ConfigEntry<bool> bonkConfig;
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

        public static void Init()
        {
            string section = "Poppy";

            // Volume Control
            allVolumeConfig = Config.BindAndOptionsSlider(section, "Master Volume", 100, "Set the total volume level.", 0, 100);
            idleVOConfig = Config.BindAndOptions(section, "Idle Chatter", true, "Whether or not the rat says something every 30 seconds.");
            voFreqConfig = Config.BindAndOptionsSlider(section, "Chatter Freqency", 1f, "Frequency of idle chatter.", 0f, 1f);
            bonkConfig = Config.BindAndOptions(section, "Bonk", false, "Enable only if you are truely prepared for maximum bonkage.");

            // Passive
            passiveConfig = Config.BindAndOptions<float>(section, "Passive Armor", PoppyStaticValues.passiveArmorIncreaseCoefficient, "Percentage of your base armor to gain at max passive stacks. Default is 2000%.");
            
            // Primary
            primaryDmgConfig = Config.BindAndOptions<float>(section, "Primary Damage", PoppyStaticValues.primaryDamageCoefficient, "Damage coefficient of primary attack.");

            // Secondary
            secondayDmgConfig = Config.BindAndOptions<float>(section, "Secondary Damage", PoppyStaticValues.secondaryDamageCoefficient, "Damage coefficient of the secondary skill.");
            secondayHPConfig = Config.BindAndOptions<float>(section, "Secondary Shield", PoppyStaticValues.secondaryHPCoefficient, "Max HP shield coefficient of the shield item.");

            // Utility #1
            util1DmgConfig = Config.BindAndOptions<float>(section, "Utility #1 Damage", PoppyStaticValues.utility1DamageCoefficient, "Damage coefficient of the utility skill.");

            // Utility #2
            util2DmgConfig = Config.BindAndOptions<float>(section, "Utility #2 Damage", PoppyStaticValues.utility2DamageCoefficient, "Damage coefficient of the utility skill.");
            util2SpdConfig = Config.BindAndOptions<float>(section, "Utility #2 Movespeed", PoppyStaticValues.utility2MoveCoefficient, "Percentage of your base movespeed to gain.");

            // Special #1
            spec1MinDmgConfig = Config.BindAndOptions<float>(section, "Special #1 Min Damage", PoppyStaticValues.special1MinDamageCoefficient, "Minimum damage coefficient of the special skill.");
            spec1MaxDmgConfig = Config.BindAndOptions<float>(section, "Special #1 Max Damage", PoppyStaticValues.special1MaxDamageCoefficient, "Maximum damage coefficient of the special skill.");

            // Special #2
            spec2DmgConfig = Config.BindAndOptions<float>(section, "Special #2 Damage", PoppyStaticValues.special2DamageCoefficient, "Damage coefficient of the special skill.");
            spec2HPConfig = Config.BindAndOptions<float>(section, "Special #2 HP Damage", PoppyStaticValues.special2HPDamageCoefficient, "Percentage of enemy max HP to deal as damage.");

            // Emote Keybinds
            jokeConfig = Config.MyConfig.Bind(new ConfigDefinition(section, "Joke Keybind"), new KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftControl), null);
            tauntConfig = Config.MyConfig.Bind(new ConfigDefinition(section, "Taunt Keybind"), new KeyboardShortcut(KeyCode.Alpha2, KeyCode.LeftControl), null);
            danceConfig = Config.MyConfig.Bind(new ConfigDefinition(section, "Dance Keybind"), new KeyboardShortcut(KeyCode.Alpha3, KeyCode.LeftControl), null);
            laughConfig = Config.MyConfig.Bind(new ConfigDefinition(section, "Laugh Keybind"), new KeyboardShortcut(KeyCode.Alpha4, KeyCode.LeftControl), null);
            ModSettingsManager.AddOption(new KeyBindOption(jokeConfig));
            ModSettingsManager.AddOption(new KeyBindOption(tauntConfig));
            ModSettingsManager.AddOption(new KeyBindOption(danceConfig));
            ModSettingsManager.AddOption(new KeyBindOption(laughConfig));
        }
    }
}
