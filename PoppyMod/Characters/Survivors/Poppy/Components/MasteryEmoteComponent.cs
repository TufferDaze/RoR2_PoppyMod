using EntityStates;
using PoppyMod.Modules.BaseStates;
using PoppyMod.Modules;
using RoR2;
using System;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.Components
{
	public class MasteryEmoteComponent : MonoBehaviour
	{
        private static GameObject masteryEmotePrefab;
        private static GameObject masteryEmoteInstance;
        private Transform masteryTransform;
        private Animator masteryAnimator;
        private float masteryCoolDown = 1.7f;
        private float masteryStopwatch;
        private ChildLocator childLocator;

        public void Awake()
        {
            childLocator = gameObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>().GetComponent<ChildLocator>();
            masteryTransform = childLocator.FindChild("MasteryEmoteLocation");
            if (!masteryEmotePrefab)
            {
                masteryEmotePrefab = PoppyAssets.masteryEmote;
            }
            if (!masteryEmoteInstance)
            {
                masteryEmoteInstance = UnityEngine.Object.Instantiate(masteryEmotePrefab, masteryTransform);
            }
            if (!masteryAnimator)
            {
                masteryAnimator = masteryEmoteInstance.GetComponent<Animator>();
            }
        }

        public void Update()
        {
            CheckEmote();
            if (masteryEmoteInstance)
            {
                masteryEmoteInstance.transform.LookAt(Camera.main.transform);
            }
        }

        public void FixedUpdate()
        {
            masteryStopwatch += Time.fixedDeltaTime;
        }

        private void CheckEmote()
        {
            if (Config.GetKeyPressed(PoppyConfig.masteryConfig) && masteryStopwatch >= masteryCoolDown)
            {
                masteryStopwatch = 0f;
                masteryAnimator.Update(0f);
                int layerIndex = masteryAnimator.GetLayerIndex("Base Layer");
                masteryAnimator.PlayInFixedTime("MasteryEmote", layerIndex, 0f);
                masteryAnimator.Update(0f);
                Util.PlaySound("PlayPoppyMastery", gameObject);
            }
        }
    }
}