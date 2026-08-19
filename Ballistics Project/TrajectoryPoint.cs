// TrajectoryPoint.cs
namespace BallisticSimulator
{
    public class TrajectoryPoint
    {
        public double Tempo { get; set; }
        public double Norte { get; set; }
        public double Leste { get; set; }
        public double Altitude { get; set; }
        public double VelocidadeNorte { get; set; }
        public double VelocidadeLeste { get; set; }
        public double VelocidadeVertical { get; set; }
        public double VelocidadeTotal { get; set; }
        public double Angulo { get; set; }
    }
}