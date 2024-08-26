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
using System.Net.NetworkInformation;
using MonoMod.Cil;

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
                    childName = "BASE_PopMesh",
                    //material = assetBundle.LoadMaterial("Poppy_Mat"),
                },
                new CustomRendererInfo
                {
                    childName = "BASE_PopHam",
                    //material = assetBundle.LoadMaterial("Poppy_Mat"),
                },
                new CustomRendererInfo
                {
                    childName = "CAF_PopHamEyes",
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
            Items.shieldyDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/SprintArmor/texBucklerIcon.png").WaitForCompletion();
            Items.shieldyDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintArmor/PickupBuckler.prefab").WaitForCompletion();

            Items.shieldyDef.canRemove = true;
            Items.shieldyDef.hidden = false;
            Items.shieldyDef.tags = new ItemTag[] { ItemTag.Utility, ItemTag.WorldUnique };

            //ItemAPI.Add(new CustomItem(Items.shieldyDef, new ItemDisplayRuleDict(null)));
            ItemAPI.Add(new CustomItem(Items.shieldyDef, new ItemDisplayRuleDict(
                new ItemDisplayRule[]
                {
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                        childName = "LowerArmR",
                        localPos = new Vector3(-0.005f, 0.285f, 0.0074f),
                        localAngles = new Vector3(358.4802f, 192.347f, 88.4811f),
                        localScale = new Vector3(0.3351f, 0.3351f, 0.3351f),
                        limbMask = LimbFlags.None
                    }
                }
            )));
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
            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "BASE_PopMesh",
                "BASE_PopHam");

            defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("CAF_PopHamEyes"),
                    shouldActivate = false,
                }
            };

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region NoxSkin

            //creating a new skindef as we did before
            SkinDef noxSkin = Skins.CreateSkinDef(POPPY_PREFIX + "NOX_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            noxSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "NOX_PopMesh",
                "NOX_PopHam");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            noxSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("NOX_PopMat");
            noxSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("NOX_PopMat");

            skins.Add(noxSkin);

            #endregion

            #region LolSkin
            // Skin def
            SkinDef lolSkin = Skins.CreateSkinDef(POPPY_PREFIX + "LOL_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            lolSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "LOL_PopMesh",
                "LOL_PopHam");

            // Materials
            lolSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("LOL_PopMat");
            lolSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("LOL_PopMat");

            // Add skin
            skins.Add(lolSkin);
            #endregion

            #region BlaSkin
            // Skin def
            SkinDef blaSkin = Skins.CreateSkinDef(POPPY_PREFIX + "BLA_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            blaSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "BLA_PopMesh",
                "BLA_PopHam");

            // Materials
            blaSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("BLA_PopMat");
            blaSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("BLA_PopMat");

            // Add skin
            skins.Add(blaSkin);
            #endregion

            #region RagSkin
            // Skin def
            SkinDef ragSkin = Skins.CreateSkinDef(POPPY_PREFIX + "RAG_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            ragSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "RAG_PopMesh",
                "RAG_PopHam");

            // Materials
            ragSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("RAG_PopMat");
            ragSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("RAG_PopMat");

            // Add skin
            skins.Add(ragSkin);
            #endregion

            #region RegSkin
            // Skin def
            SkinDef regSkin = Skins.CreateSkinDef(POPPY_PREFIX + "REG_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            regSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "REG_PopMesh",
                "REG_PopHam");

            // Materials
            regSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("REG_PopMat");
            regSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("REG_PopMat");

            // Add skin
            skins.Add(regSkin);
            #endregion

            #region ScrSkin
            // Skin def
            SkinDef scrSkin = Skins.CreateSkinDef(POPPY_PREFIX + "SCR_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            scrSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "SCR_PopMesh",
                "SCR_PopHam");

            // Materials
            scrSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("SCR_PopMat");
            scrSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("SCR_PopMat");

            // Add skin
            skins.Add(scrSkin);
            #endregion

            #region StrSkin
            // Skin def
            SkinDef strSkin = Skins.CreateSkinDef(POPPY_PREFIX + "STR_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            strSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "STR_PopMesh",
                "STR_PopHam");

            // Materials
            strSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("STR_PopMat");
            strSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("STR_PopMat");

            // Add skin
            skins.Add(strSkin);
            #endregion

            #region FwnSkin
            // Skin def
            SkinDef fwnSkin = Skins.CreateSkinDef(POPPY_PREFIX + "FWN_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            fwnSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "FWN_PopMesh",
                "FWN_PopHam");

            // Materials
            fwnSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("FWN_PopMat");
            fwnSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("FWN_PopMat");

            // Add skin
            skins.Add(fwnSkin);
            #endregion

            #region HexSkin
            // Skin def
            SkinDef hexSkin = Skins.CreateSkinDef(POPPY_PREFIX + "HEX_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            hexSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "HEX_PopMesh",
                "HEX_PopHam");

            // Materials
            hexSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("HEX_PopMat");
            hexSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("HEX_PopMat");

            // Add skin
            skins.Add(hexSkin);
            #endregion

            #region AstSkin
            // Skin def
            SkinDef astSkin = Skins.CreateSkinDef(POPPY_PREFIX + "AST_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            astSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "AST_PopMesh",
                "AST_PopHam");

            // Materials
            astSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("AST_PopMat");
            astSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("AST_PopMat");

            // Add skin
            skins.Add(astSkin);
            #endregion

            #region BewSkin
            // Skin def
            SkinDef bewSkin = Skins.CreateSkinDef(POPPY_PREFIX + "BEW_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            bewSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "BEW_PopMesh",
                "BEW_PopHam");

            // Materials
            bewSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("BEW_PopMat");
            bewSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("BEW_PopMat");

            // Add skin
            skins.Add(bewSkin);
            #endregion

            #region CafSkin
            // Skin def
            SkinDef cafSkin = Skins.CreateSkinDef(POPPY_PREFIX + "CAF_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                PoppyUnlockables.masterySkinUnlockableDef);

            // Mesh
            cafSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "CAF_PopMesh",
                "CAF_PopHam");

            // Materials
            cafSkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("CAF_PopMat");
            cafSkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("CAF_PopMat");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            cafSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("CAF_PopHamEyes"),
                    shouldActivate = true,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            // Add skin
            skins.Add(cafSkin);
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
            On.RoR2.PickupDropletController.OnCollisionEnter += PickupDropletController_OnCollisionEnter;
        }

        private void PickupDropletController_OnCollisionEnter(On.RoR2.PickupDropletController.orig_OnCollisionEnter orig, PickupDropletController self, Collision collision)
        {
            if (self.createPickupInfo.pickupIndex == PickupCatalog.FindPickupIndex(ItemCatalog.FindItemIndex(Items.shieldyDef.name)))
            {
                self.createPickupInfo.artifactFlag = GenericPickupController.PickupArtifactFlag.NONE;
            }
            orig(self, collision);
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