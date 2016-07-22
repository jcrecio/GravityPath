namespace GravityPath.EntityGame
{
    public class PlanetFiller
    {
        public PlanetFiller(int dimensionX, int dimensionY, float mass, float positionX, float positionY)
        {
            this.DimensionX = dimensionX;
            this.DimensionY = dimensionY;
            this.Mass = mass;
            this.PositionX = positionX;
            this.PositionY = positionY;
        }

        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public float Mass { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
    }
}
