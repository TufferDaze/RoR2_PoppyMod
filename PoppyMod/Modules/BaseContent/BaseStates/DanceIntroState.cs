namespace PoppyMod.Modules.BaseStates
{
    public class DanceIntroState : BaseEmote
    {
        public override void OnEnter()
        {
            animName = "IdleToDance";
            isLooping = false;
            clipLen = 1.208f;
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= clipLen)
            {
                this.outer.SetNextState(new DanceLoopState());
            }
        }
    }
}