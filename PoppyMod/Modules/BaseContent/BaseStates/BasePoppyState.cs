using EntityStates;
using PoppyMod.Survivors.Poppy;
using PoppyMod.Modules.BaseStates;

namespace PoppyMod.Modules.BaseContent.BaseStates
{
    public class BasePoppyState : GenericCharacterMain
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();
            CheckEmote();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void CheckEmote()
        {
            if (isAuthority && isGrounded)
            {
                if (Config.GetKeyPressed(PoppyConfig.jokeConfig))
                {
                    this.outer.SetInterruptState(new JokeState(), InterruptPriority.Any);
                    return;
                }
                if (Config.GetKeyPressed(PoppyConfig.tauntConfig))
                {
                    this.outer.SetInterruptState(new Modules.BaseStates.TauntState(), InterruptPriority.Any);
                    return;
                }
                if (Config.GetKeyPressed(PoppyConfig.danceConfig))
                {
                    this.outer.SetInterruptState(new DanceIntroState(), InterruptPriority.Any);
                    return;
                }
                if (Config.GetKeyPressed(PoppyConfig.laughConfig))
                {
                    this.outer.SetInterruptState(new LaughState(), InterruptPriority.Any);
                    return;
                }
            }
        }
    }
}
