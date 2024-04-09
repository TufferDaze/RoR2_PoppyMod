using System;
using RoR2;
using PoppyMod.Modules.Characters;
using UnityEngine;
using UnityEngine.Networking;
using PoppyMod.Survivors.Poppy;

namespace PoppyMod.Modules.BaseContent.BaseStates
{
    public class ArmorPassive : NetworkBehaviour
    {
        private CharacterBody body;
        private float missingHPThreshhold = PoppyStaticValues.passiveMissingHPThreshhold;
        private float armorIncreaseCoef = PoppyStaticValues.passiveArmorIncreaseCoefficient;

        public void Awake()
        {
            body = GetComponent<CharacterBody>();
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                FixedServerUpdate();
            }
        }

        private void FixedServerUpdate()
        {
            float healthPercentage = GetCurrentHealthPercent();
            if (healthPercentage <= missingHPThreshhold || body.HasBuff(PoppyBuffs.armorBuff))
            {
                ManagePassiveStack();
            }
        }

        private void ManagePassiveStack()
        {
            float graphOffset = 1.25f * armorIncreaseCoef;
            float graphSlope = 2.5f * armorIncreaseCoef;
            float healthPercentage = GetCurrentHealthPercent();
            
            int maxStacks = (int)(graphOffset - (graphSlope * healthPercentage));
            int currentStacks = body.GetBuffCount(PoppyBuffs.armorBuff);

            if (currentStacks < maxStacks)
            {
                body.AddBuff(PoppyBuffs.armorBuff);
            }
            if (currentStacks > (int)armorIncreaseCoef || currentStacks > maxStacks)
            {
                body.RemoveBuff(PoppyBuffs.armorBuff);
            }
        }

        public float GetCurrentHealthPercent()
        {
            return body.healthComponent.combinedHealthFraction;
        }
    }
}