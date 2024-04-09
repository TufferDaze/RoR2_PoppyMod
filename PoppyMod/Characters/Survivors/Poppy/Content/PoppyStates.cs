using PoppyMod.Modules.BaseContent.BaseStates;
using PoppyMod.Modules.BaseStates;
using PoppyMod.Survivors.Poppy.SkillStates;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyStates
    {
        public static void Init()
        {
            // Primary
            Modules.Content.AddEntityState(typeof(HammerSwing));

            // Secondary
            Modules.Content.AddEntityState(typeof(IronAmbassador));

            // Utility
            Modules.Content.AddEntityState(typeof(HeroicChargeDash));
            Modules.Content.AddEntityState(typeof(HeroicChargeImpact));

            Modules.Content.AddEntityState(typeof(SteadfastPresence));

            // Special
            Modules.Content.AddEntityState(typeof(KeepersVerdictCharge));

            Modules.Content.AddEntityState(typeof(PreHammerShock));
            Modules.Content.AddEntityState(typeof(HammerShock));

            Modules.Content.AddEntityState(typeof(ThrowBomb));

            // Other
            //Modules.Content.AddEntityState(typeof(BasePoppyState));
            Modules.Content.AddEntityState(typeof(BaseDeath));
        }
    }
}
