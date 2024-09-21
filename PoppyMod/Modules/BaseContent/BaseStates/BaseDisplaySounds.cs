using RoR2;
using UnityEngine;

namespace PoppyMod.Modules.BaseContent.BaseStates
{
	public class BaseDisplaySounds : MonoBehaviour
	{
		private uint soundID;

		private void OnEnable()
		{
			soundID = Util.PlaySound("PlayPoppyMenu", gameObject);
		}

		private void OnDestroy()
		{
			AkSoundEngine.StopPlayingID(soundID);
		}
	}
}