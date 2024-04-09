using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;

        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("SteadfastPressence",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.yellow,
                true,
                false);

        }
    }
}
