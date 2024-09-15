using RoR2;
using PoppyMod.Modules.Achievements;

namespace PoppyMod.Survivors.Poppy.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, (uint)0)]
    public class PoppyMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = PoppySurvivor.POPPY_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = PoppySurvivor.POPPY_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => PoppySurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}