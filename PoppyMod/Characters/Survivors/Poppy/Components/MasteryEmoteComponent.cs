using PoppyMod.Modules;
using RoR2;
using UnityEngine;

namespace PoppyMod.Survivors.Poppy.Components
{
	public class MasteryEmoteComponent : MonoBehaviour
	{
        private ChildLocator childLocator;
        private Transform masteryEmote;
        private Animator masteryAnimator;
        private float masteryCoolDown = 1.7f;
        private float masteryStopwatch;
        private bool isLocal;

        public void Awake()
        {
            childLocator = GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>().GetComponent<ChildLocator>();
            masteryEmote = childLocator.FindChild("MasteryEmote");
            if (masteryEmote)
            {
                masteryAnimator = masteryEmote.GetComponent<Animator>();
            }
            else
            {
                Debug.LogError("MasteryEmoteComponent: masteryEmote NULL. Cannot get animator component.");
            }
            isLocal = ReferenceEquals(LocalUserManager.GetFirstLocalUser().cachedBodyObject, gameObject);
        }

        public void Update()
        {
            if (masteryEmote)
            {
                CheckEmote();
                masteryEmote.transform.LookAt(Camera.main.transform);
            }
        }

        public void FixedUpdate()
        {
            isLocal = ReferenceEquals(LocalUserManager.GetFirstLocalUser().cachedBodyObject, gameObject);
            masteryStopwatch += Time.fixedDeltaTime;
        }

        private void CheckEmote()
        {
            if (isLocal && Config.GetKeyPressed(PoppyConfig.masteryConfig) && masteryStopwatch >= masteryCoolDown)
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