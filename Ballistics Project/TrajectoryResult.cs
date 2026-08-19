// TrajectoryResult.cs
using System.Collections.Generic;

namespace BallisticSimulator
{
    public class TrajectoryResult
    {
        public double CannonNorth { get; set; }
        public double CannonEast { get; set; }
        public double ImpactNorth { get; set; }
        public double ImpactEast { get; set; }
        public double ImpactAltitude { get; set; }
        public double ImpactVelocity { get; set; }
        public double ImpactAngle { get; set; }
        public double TimeOfFlight { get; set; }
        public double TotalRange { get; set; }
        public double MaxAltitude { get; set; }
        public double MaxVelocity { get; set; }
        public double FinalDragForce { get; set; }
        public double TargetAltitude { get; set; }
        public List<TrajectoryPoint> Trajectory { get; set; } = new List<TrajectoryPoint>();
    }
}