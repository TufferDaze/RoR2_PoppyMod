using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy
{
    public static class PoppyAI
    {
        public static void Init(GameObject bodyPrefab, string masterName)
        {
            GameObject master = Modules.Prefabs.CreateBlankMasterPrefab(bodyPrefab, masterName);

            BaseAI baseAI = master.GetComponent<BaseAI>();
            baseAI.aimVectorDampTime = 0.1f;
            baseAI.aimVectorMaxSpeed = 360;

            //mouse over these fields for tooltips
            AISkillDriver primaryDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            primaryDriver.customName = "Use Primary";
            primaryDriver.skillSlot = SkillSlot.Primary;
            primaryDriver.requiredSkill = null; //usually used when you have skills that override other skillslots like engi harpoons
            primaryDriver.requireSkillReady = false; //usually false for primaries
            primaryDriver.requireEquipmentReady = false;
            primaryDriver.minUserHealthFraction = float.NegativeInfinity;
            primaryDriver.maxUserHealthFraction = float.PositiveInfinity;
            primaryDriver.minTargetHealthFraction = float.NegativeInfinity;
            primaryDriver.maxTargetHealthFraction = float.PositiveInfinity;
            primaryDriver.minDistance = 0;
            primaryDriver.maxDistance = 6;
            primaryDriver.selectionRequiresTargetLoS = false;
            primaryDriver.selectionRequiresOnGround = false;
            primaryDriver.selectionRequiresAimTarget = false;
            primaryDriver.maxTimesSelected = -1;

            //Behavior
            primaryDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            primaryDriver.activationRequiresTargetLoS = false;
            primaryDriver.activationRequiresAimTargetLoS = false;
            primaryDriver.activationRequiresAimConfirmation = false;
            primaryDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            primaryDriver.moveInputScale = 1;
            primaryDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            primaryDriver.ignoreNodeGraph = false; //will chase relentlessly but be kind of stupid
            primaryDriver.shouldSprint = true;
            primaryDriver.shouldFireEquipment = false;
            primaryDriver.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous; 

            //Transition Behavior
            primaryDriver.driverUpdateTimerOverride = -1;
            primaryDriver.resetCurrentEnemyOnNextDriverSelection = false;
            primaryDriver.noRepeat = false;
            primaryDriver.nextHighPriorityOverride = null;

            //some fields omitted that aren't commonly changed. will be set to default values
            AISkillDriver secondaryDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            secondaryDriver.customName = "Use Secondary";
            secondaryDriver.skillSlot = SkillSlot.Secondary;
            secondaryDriver.requireSkillReady = true;
            secondaryDriver.minDistance = 0;
            secondaryDriver.maxDistance = 60;
            secondaryDriver.selectionRequiresTargetLoS = false;
            secondaryDriver.selectionRequiresOnGround = false;
            secondaryDriver.selectionRequiresAimTarget = false;
            secondaryDriver.maxTimesSelected = -1;

            //Behavior
            secondaryDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            secondaryDriver.activationRequiresTargetLoS = true;
            secondaryDriver.activationRequiresAimTargetLoS = true;
            secondaryDriver.activationRequiresAimConfirmation = true;
            secondaryDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            secondaryDriver.moveInputScale = 1;
            secondaryDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            secondaryDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold; 
            
            AISkillDriver utilDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            utilDriver.customName = "Use Utility";
            utilDriver.skillSlot = SkillSlot.Utility;
            utilDriver.requireSkillReady = true;
            utilDriver.minDistance = 0;
            utilDriver.maxDistance = 10;
            utilDriver.selectionRequiresTargetLoS = false;
            utilDriver.selectionRequiresOnGround = false;
            utilDriver.selectionRequiresAimTarget = false;
            utilDriver.maxTimesSelected = -1;

            //Behavior
            utilDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            utilDriver.activationRequiresTargetLoS = false;
            utilDriver.activationRequiresAimTargetLoS = false;
            utilDriver.activationRequiresAimConfirmation = false;
            utilDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            utilDriver.moveInputScale = 1;
            utilDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            utilDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver specDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            specDriver.customName = "Use Special";
            specDriver.skillSlot = SkillSlot.Special;
            specDriver.requireSkillReady = true;
            specDriver.minDistance = 0;
            specDriver.maxDistance = 10;
            specDriver.selectionRequiresTargetLoS = false;
            specDriver.selectionRequiresOnGround = false;
            specDriver.selectionRequiresAimTarget = false;
            specDriver.maxTimesSelected = -1;

            //Behavior
            specDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            specDriver.activationRequiresTargetLoS = true;
            specDriver.activationRequiresAimTargetLoS = false;
            specDriver.activationRequiresAimConfirmation = false;
            specDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            specDriver.moveInputScale = 1;
            specDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            specDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chaseDriver = master.AddComponent<AISkillDriver>();
            //Selection Conditions
            chaseDriver.customName = "Chase";
            chaseDriver.skillSlot = SkillSlot.None;
            chaseDriver.requireSkillReady = false;
            chaseDriver.minDistance = 0;
            chaseDriver.maxDistance = float.PositiveInfinity;

            //Behavior
            chaseDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseDriver.activationRequiresTargetLoS = false;
            chaseDriver.activationRequiresAimTargetLoS = false;
            chaseDriver.activationRequiresAimConfirmation = false;
            chaseDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseDriver.moveInputScale = 1;
            chaseDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseDriver.shouldSprint = true;
            chaseDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            //recommend taking these for a spin in game, messing with them in runtimeinspector to get a feel for what they should do at certain ranges and such
        }
    }
}
