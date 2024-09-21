namespace PoppyMod.Modules.BaseStates
{
    public class LaughState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "Laugh";
            soundString = "PlayPoppyLaugh";
            //soundFXString = "PlayPoppyLaughSFX";
            isLooping = false;
            clipLen = 2.5f;
            base.OnEnter();
        }
    }
}