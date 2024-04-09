using PoppyMod.Survivors.Poppy.Achievements;
using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                PoppyMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(PoppyMasteryAchievement.identifier),
                PoppySurvivor.instance.assetBundle.LoadAsset<Sprite>("texMasteryAchievement"));
        }
    }
}
