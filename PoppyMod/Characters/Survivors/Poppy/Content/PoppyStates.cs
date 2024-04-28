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
            Modules.Content.AddEntityState(typeof(KeepersVerdictFire));
            Modules.Content.AddEntityState(typeof(KeepersVerdictCharge));
            Modules.Content.AddEntityState(typeof(KeepersVerdictChargeSlam));

            Modules.Content.AddEntityState(typeof(PreHammerShock));
            Modules.Content.AddEntityState(typeof(HammerShock));

            //Modules.Content.AddEntityState(typeof(ThrowBomb));

            // Main States
            Modules.Content.AddEntityState(typeof(BasePoppyState));
            Modules.Content.AddEntityState(typeof(BaseDeath));

            // Emotes
            Modules.Content.AddEntityState(typeof(JokeState));
            Modules.Content.AddEntityState(typeof(TauntState));
            Modules.Content.AddEntityState(typeof(DanceIntroState));
            Modules.Content.AddEntityState(typeof(DanceLoopState));
            Modules.Content.AddEntityState(typeof(LaughState));
        }
    }
}
