﻿using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;

namespace PoppyMod.Modules
{
    public static class Config
    {
        public static ConfigFile MyConfig = PoppyPlugin.instance.Config;

        /// <summary>
        /// automatically makes config entries for disabling survivors
        /// </summary>
        /// <param name="section"></param>
        /// <param name="characterName"></param>
        /// <param name="description"></param>
        /// <param name="enabledByDefault"></param>
        public static ConfigEntry<bool> CharacterEnableConfig(string section, string characterName, string description = "", bool enabledByDefault = true)
        {

            if (string.IsNullOrEmpty(description))
            {
                description = "Set to false to disable this character and as much of its code and content as possible";
            }
            return BindAndOptions<bool>(section,
                                        "Enable " + characterName,
                                        enabledByDefault,
                                        description);
        }

        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<T> configEntry = MyConfig.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOption(configEntry, restartRequired);
            }

            return configEntry;
        }
        public static ConfigEntry<float> BindAndOptionsSlider(string section, string name, float defaultValue, string description = "", float min = 0, float max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<float> configEntry = MyConfig.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        public static ConfigEntry<int> BindAndOptionsSlider(string section, string name, int defaultValue, string description = "", int min = 0, int max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<int> configEntry = MyConfig.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        //add risk of options dll to your project libs and uncomment this for a soft dependency
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOption<T>(ConfigEntry<T> entry, bool restartRequired)
        {
            if (entry is ConfigEntry<float>)
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = 0.01f, max = 100f, formatString = "{0:0.00}", restartRequired = restartRequired }));
            }
            if (entry is ConfigEntry<int>)
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, restartRequired));
            }
            if (entry is ConfigEntry<bool>)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(entry as ConfigEntry<bool>, restartRequired));
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<float> entry, float min, float max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = min, max = max, formatString = "{0:0.00}", restartRequired = restartRequired }));
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<int> entry, int min, int max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, new IntSliderConfig() { min = min, max = max, restartRequired = restartRequired }));
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}