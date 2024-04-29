using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.CaptainSupplyDrop;
using PoppyMod.Modules;
using PoppyMod.Modules.BaseContent.BaseStates;
using PoppyMod.Modules.Characters;
using PoppyMod.Survivors.Poppy.Components;
using PoppyMod.Survivors.Poppy.SkillStates;
using R2API;
using RoR2;
using RoR2.UI;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using PoppyMod.Modules.BaseStates;
using PoppyMod.Characters.Survivors.Poppy.Components;

namespace PoppyMod.Survivors.Poppy
{
    public class PoppySurvivor : SurvivorBase<PoppySurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "popmodassetbundle"; //if you do not change this, you are giving permission to deprecate the mod

        public override string soundBankName => "PoppySoundBank.sound";

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "PoppyBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "PoppyMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlPoppy";
        public override string displayPrefabName => "mdlPoppyDisplay";

        public const string POPPY_PREFIX = PoppyPlugin.DEVELOPER_PREFIX + "_POP_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => POPPY_PREFIX;
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = POPPY_PREFIX + "NAME",
            subtitleNameToken = POPPY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("poppy_square"),
            bodyColor = new Color(1f, 217f/255f, 122f/255f),
            sortPosition = 100,

            crosshair = Assets.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = PoppyStaticValues.baseHealth,
            healthRegen = 1.5f,
            armor = PoppyStaticValues.baseArmor,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "PopMesh",
                    //material = assetBundle.LoadMaterial("Poppy_Mat"),
                },
                new CustomRendererInfo
                {
                    childName = "PopHammer",
                    //material = assetBundle.LoadMaterial("Poppy_Mat"),
                }
        };

        public override UnlockableDef characterUnlockableDef => PoppyUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new PoppyItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();

        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            PoppyUnlockables.Init();
            PoppyConfig.Init();

            base.InitializeCharacter();

            //PoppyConfig.Init();
            PoppyStates.Init();
            PoppyTokens.Init();

            PoppyAssets.Init(assetBundle);
            PoppyBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeItems();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.gameObject.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(BaseDeath));
            bodyPrefab.gameObject.GetComponent<CharacterMotor>().mass = 300f;
            bodyPrefab.gameObject.AddComponent<VOComponent>();
            bodyPrefab.gameObject.AddComponent<MasteryEmoteComponent>();
            bodyPrefab.gameObject.AddComponent<ArmorPassiveComponent>();
            bodyPrefab.gameObject.AddComponent<HuntressTracker>().maxTrackingDistance = 60f;
            //bodyPrefab.AddComponent<PoppyWeaponComponent>();
        }

        public void AddHitboxes()
        {
            ChildLocator childLocator = characterModelObject.GetComponent<ChildLocator>();

            Transform hitBoxTransform1 = childLocator.FindChild("HammerHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "HammerGroup", hitBoxTransform1);
            Transform hitBoxTransform2 = childLocator.FindChild("AuraHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "AuraGroup", hitBoxTransform2);
            Transform hitBoxTransform3 = childLocator.FindChild("KeeperHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "KeeperGroup", hitBoxTransform3);
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs
            //the main "body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(BasePoppyState), typeof(SpawnTeleporterState));

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Passive");
        }

        #region items
        public void InitializeItems()
        {
            AddShieldy();
        }

        private void AddShieldy()
        {
            Items.shieldyDef = ScriptableObject.CreateInstance<ItemDef>();
            Items.shieldyDef.name = "Shieldy";
            Items.shieldyDef.nameToken = POPPY_PREFIX + "ITEM_SHIELDY_NAME";
            Items.shieldyDef.descriptionToken = POPPY_PREFIX + "ITEM_SHIELDY_DESCRIPTION";
            Items.shieldyDef.loreToken = POPPY_PREFIX + "ITEM_SHIELDY_LORE";
            Items.shieldyDef.pickupToken = POPPY_PREFIX + "ITEM_SHIELDY_PICKUP";

            Items.shieldyDef._itemTierDef = Items.itemTierDef;
            //Items.shieldyDef._itemTierDef._tier = ItemTier.NoTier;
            //Items.shieldyDef._itemTierDef.canScrap = false;

            //Items.shieldyDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            //Items.shieldyDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();
            Items.shieldyDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/SprintArmor/texBucklerIcon.png").WaitForCompletion();
            Items.shieldyDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintArmor/PickupBuckler.prefab").WaitForCompletion();

            Items.shieldyDef.canRemove = true;
            Items.shieldyDef.hidden = false;
            Items.shieldyDef.tags = new ItemTag[] { ItemTag.Utility };

            ItemAPI.Add(new CustomItem(Items.shieldyDef, new ItemDisplayRuleDict(null)));
        }
        #endregion

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkills();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkills()
        {
            SkillLocator skillLocator = prefabCharacterBody.GetComponent<SkillLocator>();
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.icon = assetBundle.LoadAsset<Sprite>("poppy_w_passive");
            skillLocator.passiveSkill.skillNameToken = POPPY_PREFIX + "PASSIVE_STEADFAST_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = POPPY_PREFIX + "PASSIVE_STEADFAST_DESCRIPTION";
        }

        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            //the primary skill is created using a constructor for a typical primary
            //it is also a SteppedSkillDef. Custom Skilldefs are very useful for custom behaviors related to casting a skill. see ror2's different skilldefs for reference
            SteppedSkillDef primarySkillDef1 = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Hammer",
                    POPPY_PREFIX + "PRIMARY_HAMMER_NAME",
                    POPPY_PREFIX + "PRIMARY_HAMMER_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("poppy_q"),
                    new EntityStates.SerializableEntityStateType(typeof(HammerSwing)),
                    "Weapon",
                    false
                ));
            //custom Skilldefs can have additional fields that you can set manually
            primarySkillDef1.stepCount = 3;
            primarySkillDef1.stepGraceDuration = 0.5f;

            Skills.AddPrimarySkills(bodyPrefab, primarySkillDef1);
        }

        private void AddSecondarySkills()
        {
            //here is a basic skill def with all fields accounted for
            HuntressTrackingSkillDef secondarySkillDef1 = Skills.CreateSkillDef<HuntressTrackingSkillDef>(new SkillDefInfo
            {
                skillName = "IronAmbassador",
                skillNameToken = POPPY_PREFIX + "SECONDARY_BUCKLER_NAME",
                skillDescriptionToken = POPPY_PREFIX + "SECONDARY_BUCKLER_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("poppy_passive"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(IronAmbassador)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Frozen,

                baseRechargeInterval = 6f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = false,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });
            
            Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef1);
        }

        private void AddUtiitySkills()
        {
            //here's a skilldef of a typical movement skill. some fields are omitted and will just have default values
            SkillDef utilitySkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HeroicCharge",
                skillNameToken = POPPY_PREFIX + "UTILITY_HEROICCHARGE_NAME",
                skillDescriptionToken = POPPY_PREFIX + "UTILITY_HEROICCHARGE_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_HEAVY", "KEYWORD_STUNNING" },
                skillIcon = assetBundle.LoadAsset<Sprite>("poppy_e"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(HeroicChargeDash)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Frozen,

                baseMaxStock = 1,
                baseRechargeInterval = 8f,

                isCombatSkill = true,
                mustKeyPress = true,
                forceSprintDuringState = true,
                cancelSprintingOnActivation = false,
            });

            SkillDef utilitySkillDef2 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SteadfastPresence",
                skillNameToken = POPPY_PREFIX + "UTILITY_STEADFAST_NAME",
                skillDescriptionToken = POPPY_PREFIX + "UTILITY_STEADFAST_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_STUNNING", "KEYWORD_GROUNDING" },
                skillIcon = assetBundle.LoadAsset<Sprite>("poppy_w"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SteadfastPresence)),
                activationStateMachineName = "Passive",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 8f,

                isCombatSkill = true,
                mustKeyPress = true,
                forceSprintDuringState = true,
            });

            Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef1);
            Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef2);
        }

        private void AddSpecialSkills()
        {
            //a basic skill
            SkillDef specialSkilLDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "KeepersVerdict",
                skillNameToken = POPPY_PREFIX + "SPECIAL_KEEPERSVERDICT_NAME",
                skillDescriptionToken = POPPY_PREFIX + "SPECIAL_KEEPERSVERDICT_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_STUNNING" },
                skillIcon = assetBundle.LoadAsset<Sprite>("poppy_r"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(KeepersVerdictCharge)),
                //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Passive",
                interruptPriority = EntityStates.InterruptPriority.Frozen,

                baseMaxStock = 1,
                baseRechargeInterval = 14f,

                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
            });

            SkillDef specialSkilLDef2 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HammerShock",
                skillNameToken = POPPY_PREFIX + "SPECIAL_HAMMERSHOCK_NAME",
                skillDescriptionToken = POPPY_PREFIX + "SPECIAL_HAMMERSHOCK_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_STUNNING" },
                skillIcon = assetBundle.LoadAsset<Sprite>("poppy_q"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(PreHammerShock)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Frozen,

                baseRechargeInterval = 10f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,

            });

            Skills.AddSpecialSkills(bodyPrefab, specialSkilLDef1);
            Skills.AddSpecialSkills(bodyPrefab, specialSkilLDef2);
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("poppy_square"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            PoppyAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Config.MyConfig.SettingChanged += MyConfig_SettingChanged;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(PoppyBuffs.armorBuff))
            {
                float buffStacks = sender.GetBuffCount(PoppyBuffs.armorBuff);
                args.armorAdd = sender.baseArmor * (buffStacks-1);
            }

            if (sender.HasBuff(PoppyBuffs.speedBuff))
            {
                float buffStacks = sender.GetBuffCount(PoppyBuffs.speedBuff);
                args.baseMoveSpeedAdd = sender.baseMoveSpeed * PoppyConfig.util2SpdConfig.Value * buffStacks;
            }

            try
            {
                if (sender.inventory.GetItemCount(ItemCatalog.FindItemIndex(Items.shieldyDef.name)) >= 1)
                {
                    sender.healthComponent.AddBarrier(sender.healthComponent.fullHealth * PoppyConfig.secondayHPConfig.Value);
                    sender.inventory.RemoveItem(Items.shieldyDef, 1);
                }
            }
            catch
            {
                // Do nothing
            }
        }

        private void MyConfig_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (sender == PoppyConfig.allVolumeConfig)
            {
                AkSoundEngine.SetRTPCValue(3695994288u, PoppyConfig.allVolumeConfig.Value);
            }
        }
    }
}