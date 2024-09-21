using UnityEngine;

namespace PoppyMod.Modules.BaseStates
{
	public class JokeState : BaseEmote
	{
		private int jokeIndex;

		public override void OnEnter()
		{
			jokeIndex = Random.Range(1, 4);
			animName = "Joke"+jokeIndex;
			//soundString = "PlayPoppyJoke";
			isLooping = false;
			switch (jokeIndex)
			{
				case 1:
					clipLen = 4f;
                    break;
				case 2:
					clipLen = 2.625f;
					break;
				case 3:
					clipLen = 5.917f;
                    break;
			}
            base.OnEnter();
		}
	}
}