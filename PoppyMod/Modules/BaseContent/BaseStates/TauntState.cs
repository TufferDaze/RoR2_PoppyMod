namespace PoppyMod.Modules.BaseStates
{
    public class TauntState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "Taunt";
            soundString = "PlayPoppyTaunt";
            //soundFXString = "PlayPoppyTauntSFX";
            isLooping = false;
            clipLen = 5.333f;
            base.OnEnter();
        }
    }
}