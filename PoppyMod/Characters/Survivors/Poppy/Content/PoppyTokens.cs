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
             + "< ! > Use your crowd control to make enemies want to obliterate themselves." + Environment.NewLine + Environment.NewLine
             //+ "< ! > Enemies killed with Keeper\'s Verdict are launched into the stratosphere. ;)" + Environment.NewLine + Environment.NewLine
             + "< ! > Keeper\'s Verdict and Steadfast Presence are not done yet." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left to continue her search.";
            string outroFailure = "SHE DIED :,[";

            Language.Add(prefix + "NAME", "Poppy");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Keeper of the Hammer");
            Language.Add(prefix + "LORE", "There is no place she isn\'t willing to go in order to find the hero.");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_STEADFAST_NAME", "Stubborn to a Fault");
            Language.Add(prefix + "PASSIVE_STEADFAST_DESCRIPTION", $"<style=cIsUtility>Take up to {100f*(1-(100f/(100f+PoppyStaticValues.passiveArmorIncreaseCoefficient*PoppyConfig.baseArmor)))}% less damage</style> when below <style=cIsHealth>{100f * PoppyStaticValues.passiveMissingHPThreshhold}% HP.</style>");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_HAMMER_NAME", "Uh... Hammer!");
            Language.Add(prefix + "PRIMARY_HAMMER_DESCRIPTION", $"Swing hammer for <style=cIsDamage>{100f * PoppyStaticValues.primaryDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_BUCKLER_NAME", "Iron Ambassador");
            Language.Add(prefix + "SECONDARY_BUCKLER_DESCRIPTION", $"Throw a buckler shield for <style=cIsDamage>{100f * PoppyStaticValues.secondaryDamageCoefficient}% damage</style>. Picking up the shield will grant a <style=cIsHealth>{100f * PoppyStaticValues.secondaryHPCoefficient}% max HP temporary barrier</style>.");
            #endregion

            #region Utility
            Language.Add("KEYWORD_GROUNDING", $"{Tokens.KeywordText("Grounding", "Forces flying enemies to the ground for a short time.")}") ;
            Language.Add(prefix + "UTILITY_HEROICCHARGE_NAME", "Heroic Charge");
            Language.Add(prefix + "UTILITY_HEROICCHARGE_DESCRIPTION", Tokens.heavyPrefix + " " + Tokens.stunningPrefix + $" Dash forward dealing <style=cIsDamage>{100f * PoppyStaticValues.utility1DamageCoefficient}% damage</style>.");

            Language.Add(prefix + "UTILITY_STEADFAST_NAME", "Steadfast Presence");
            Language.Add(prefix + "UTILITY_STEADFAST_DESCRIPTION", Tokens.stunningPrefix + " " + Tokens.groundingPrefix + $" Release an aura dealing <style=cIsDamage>{100f * PoppyStaticValues.utility2DamageCoefficient * 2f}% damage per second</style> for 3 seconds.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_KEEPERSVERDICT_NAME", "Keeper\'s Verdict");
            Language.Add(prefix + "SPECIAL_KEEPERSVERDICT_DESCRIPTION", Tokens.stunningPrefix + $" Charge a shockwave dealing <style=cIsDamage>{100f * PoppyStaticValues.special1MinDamageCoefficient}%-{100f * PoppyStaticValues.special1MaxDamageCoefficient}% damage</style> and pulling enemies toward Poppy.");

            Language.Add(prefix + "SPECIAL_HAMMERSHOCK_NAME", "Hammer Shock");
            Language.Add(prefix + "SPECIAL_HAMMERSHOCK_DESCRIPTION", Tokens.stunningPrefix + $" Smash the ground dealing <style=cIsDamage>{100f * PoppyStaticValues.special2DamageCoefficient}%+({100f * PoppyStaticValues.special2HPDamageCoefficient}% enemy max HP) damage</style>.");
            #endregion

            #region Items
            Language.Add(prefix + "ITEM_SHIELDY_NAME", "Shieldy");
            Language.Add(prefix + "ITEM_SHIELDY_DESCRIPTION", $"Gain a <style=cIsUtility>temporary barrier</style> for <style=cIsUtility>{100f * PoppyStaticValues.secondaryHPCoefficient}% max HP</style>.");
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
