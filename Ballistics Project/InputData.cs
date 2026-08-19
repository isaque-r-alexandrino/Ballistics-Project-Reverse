// InputData.cs
namespace BallisticSimulator
{
    public class InputData
    {
        public double V0 { get; set; }
        public double Elevation { get; set; }
        public double Azimuth { get; set; }
        public double Mass { get; set; }
        public double Diameter { get; set; }
        public double BC { get; set; }
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }
        public double WindNorth { get; set; }
        public double WindEast { get; set; }
        public double Latitude { get; set; }
        public double CannonNorth { get; set; }
        public double CannonEast { get; set; }
        public double CannonAltitude { get; set; }
        public double TargetAltitude { get; set; }
    }
}