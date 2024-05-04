using System;
using PoppyMod.Modules;
using PoppyMod.Survivors.Poppy.Achievements;
using static UnityEngine.ParticleSystem.PlaybackState;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyTokens
    {
        public static void Init()
        {
            AddPoppyTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Henry.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddPoppyTokens()
        {
            string prefix = PoppySurvivor.POPPY_PREFIX;

            string desc = "Poppy: a small idiot rat thing with a tiny brain and a big heart.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine
                + "< ! > Get up close and personal to do damage." + Environment.NewLine + Environment.NewLine
                + "< ! > Iron Ambassador bounces to up to 3 enemies, increasing damage dealt each time." + Environment.NewLine + Environment.NewLine
                + "< ! > Heroic Charge carries enemies forward." + Environment.NewLine + Environment.NewLine
                + "< ! > Steadfast Presence can be used to keep smaller flying enemies on the ground." + Environment.NewLine + Environment.NewLine
                + "< ! > Enemies hit with Keeper\'s Verdict are launched into the stratosphere." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left to continue her search.";
            string outroFailure = "You are definitely not the hero.";

            Language.Add(prefix + "NAME", "Poppy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Keeper of the Hammer");
            //Language.Add(prefix + "LORE", "There is no place she isn\'t willing to go in order to find the fabled hero.");
            Language.Add(prefix + "LORE", "What the Poppy doin\'? How did she get here?");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_STEADFAST_NAME", "Stubborn to a Fault");
            Language.Add(prefix + "PASSIVE_STEADFAST_DESCRIPTION", $"<style=cIsUtility>Take up to {100f*(1-(100f/(100f+PoppyConfig.passiveConfig.Value*PoppyStaticValues.baseArmor)))}% less damage</style> when below <style=cIsHealth>{100f * PoppyStaticValues.passiveMissingHPThreshhold}% HP.</style>");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_HAMMER_NAME", "Uh... Hammer!");
            Language.Add(prefix + "PRIMARY_HAMMER_DESCRIPTION", $"Swing hammer for <style=cIsDamage>{100f * PoppyConfig.primaryDmgConfig.Value}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_BUCKLER_NAME", "Iron Ambassador");
            Language.Add(prefix + "SECONDARY_BUCKLER_DESCRIPTION", $"Throw a buckler shield for <style=cIsDamage>{100f * PoppyConfig.secondayDmgConfig.Value}% damage</style>. Picking up the shield will grant a <style=cIsHealth>{100f * PoppyConfig.secondayHPConfig.Value}% max HP temporary barrier</style>.");
            #endregion

            #region Utility
            Language.Add("KEYWORD_GROUNDING", $"{Tokens.KeywordText("Grounding", "Forces flying enemies to the ground for a short time.")}") ;
            Language.Add(prefix + "UTILITY_HEROICCHARGE_NAME", "Heroic Charge");
            Language.Add(prefix + "UTILITY_HEROICCHARGE_DESCRIPTION", Tokens.heavyPrefix + " " + Tokens.stunningPrefix + $" Dash forward dealing <style=cIsDamage>{100f * PoppyConfig.util1DmgConfig.Value}% damage</style>.");

            Language.Add(prefix + "UTILITY_STEADFAST_NAME", "Steadfast Presence");
            Language.Add(prefix + "UTILITY_STEADFAST_DESCRIPTION", Tokens.groundingPrefix + " " + Tokens.stunningPrefix + $" Release an aura dealing <style=cIsDamage>{100f * PoppyConfig.util2DmgConfig.Value * 3f}% damage</style> over 3 seconds and speeding you up.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_KEEPERSVERDICT_NAME", "Keeper\'s Verdict");
            Language.Add(prefix + "SPECIAL_KEEPERSVERDICT_DESCRIPTION", Tokens.stunningPrefix + $" Charge a shockwave dealing <style=cIsDamage>{100f * PoppyConfig.spec1MinDmgConfig.Value}%-{100f * PoppyConfig.spec1MaxDmgConfig.Value}% damage</style> and launches enemies up.");

            Language.Add(prefix + "SPECIAL_HAMMERSHOCK_NAME", "Hammer Shock");
            Language.Add(prefix + "SPECIAL_HAMMERSHOCK_DESCRIPTION", Tokens.stunningPrefix + $" Smash the ground dealing <style=cIsDamage>{100f * PoppyConfig.spec2DmgConfig.Value}%+({100f * PoppyConfig.spec2HPConfig.Value}% enemy max HP) damage</style>.");
            #endregion

            #region Items
            Language.Add(prefix + "ITEM_SHIELDY_NAME", "Shieldy");
            Language.Add(prefix + "ITEM_SHIELDY_DESCRIPTION", $"Gain a <style=cIsUtility>temporary barrier</style> for <style=cIsUtility>{100f * PoppyConfig.secondayHPConfig.Value}% max HP</style>.");
            Language.Add(prefix + "ITEM_SHIELDY_LORE", "A small buckler shield that looks tenderly cared for despite the years of wear.");
            Language.Add(prefix + "ITEM_SHIELDY_PICKUP", "Immediately gain a small temporary barrier.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(PoppyMasteryAchievement.identifier), "Poppy: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(PoppyMasteryAchievement.identifier), "As Poppy, beat the game or obliterate on Monsoon.");
            #endregion
        }
    }
}
