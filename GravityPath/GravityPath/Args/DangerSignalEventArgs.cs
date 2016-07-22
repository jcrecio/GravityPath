namespace GravityPath.Args
{
    using System;
    using System.Collections.Generic;
    using GravityPath.EntityGame;
    using GravityPath.Services;

    public class DangerSignalEventArgs: EventArgs
    {
        public List<DangerSignal> DangerSignals { get; private set; }

        public DangerSignalEventArgs(List<DangerSignal> dangerSignals)
        {
            this.DangerSignals = dangerSignals;
        }
    }
}