namespace GravityPath.Args
{
    using System;
    using GravityPath.Enumeration;

    public class StateGameEventArgs: EventArgs
    {
        public GameState GameState { get; private set; }

        public StateGameEventArgs(GameState gameState)
        {
            this.GameState = gameState;
        }
    }
}
