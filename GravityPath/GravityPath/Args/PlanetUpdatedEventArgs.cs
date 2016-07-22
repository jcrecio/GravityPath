namespace GravityPath.Args
{
    using System;
    using System.Collections.Generic;
    using GravityPath.EntityGame;
    using GravityPath.Services;

    public class PlanetUpdatedEventArgs: EventArgs
    {
        public List<Planet> PlanetsUpdated { get; private set; }
        public List<DangerSignal> DangersUpdated { get; private set; }

        public PlanetUpdatedEventArgs(List<Planet> planet/*, List<DangerSignal> dangerSignals*/)
        {
            this.PlanetsUpdated = planet;
            // this.DangersUpdated = dangerSignals;
        }
    }
}