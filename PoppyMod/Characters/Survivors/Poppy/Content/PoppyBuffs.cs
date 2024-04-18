using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;
        public static BuffDef speedBuff;

        public static void Init(AssetBundle assetBundle)
        {
            // For passive armor boost
            armorBuff = Modules.Content.CreateAndAddBuff("SteadfastPressence",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/ArmorBoost").iconSprite,
                Color.yellow,
                true,
                false);

            // For Steadfast Presence speed boost
            speedBuff = Modules.Content.CreateAndAddBuff("SteadfastPressence",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite,
                Color.yellow,
                true,
                false);
        }
    }
}
