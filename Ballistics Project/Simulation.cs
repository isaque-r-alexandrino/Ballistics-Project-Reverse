// Simulation.cs
using System;

namespace BallisticSimulator
{
    public class Simulation
    {
        private const double G0 = 9.80665;
        private const double R = 287.05;
        private const double OMEGA = 7.292115e-5;
        private const double DT = 0.01;
        private const double MAX_TEMPO = 600.0;

        public TrajectoryResult Run(InputData input)
        {
            TrajectoryResult result = new TrajectoryResult
            {
                CannonNorth = input.CannonNorth,
                CannonEast = input.CannonEast,
                TargetAltitude = input.TargetAltitude
            };

            double elevRad = input.Elevation * Math.PI / 180.0;
            double azimRad = input.Azimuth * Math.PI / 180.0;

            double vx = input.V0 * Math.Cos(elevRad) * Math.Sin(azimRad);
            double vy = input.V0 * Math.Sin(elevRad);
            double vz = input.V0 * Math.Cos(elevRad) * Math.Cos(azimRad);

            double x = input.CannonEast;
            double y = input.CannonAltitude;
            double z = input.CannonNorth;

            double tempo = 0.0;
            bool impactou = false;

            result.MaxAltitude = y;
            result.MaxVelocity = input.V0;

            while (tempo < MAX_TEMPO && !impactou)
            {
                double x1 = x, y1 = y, z1 = z;
                double vx1 = vx, vy1 = vy, vz1 = vz;

                RK4Step(input, ref x1, ref y1, ref z1, ref vx1, ref vy1, ref vz1, ref tempo);

                x = x1; y = y1; z = z1;
                vx = vx1; vy = vy1; vz = vz1;

                double vTotal = Math.Sqrt(vx * vx + vy * vy + vz * vz);
                
                if (y > result.MaxAltitude) result.MaxAltitude = y;
                if (vTotal > result.MaxVelocity) result.MaxVelocity = vTotal;

                TrajectoryPoint ponto = new TrajectoryPoint
                {
                    Tempo = tempo,
                    Norte = z,
                    Leste = x,
                    Altitude = y,
                    VelocidadeNorte = vz,
                    VelocidadeLeste = vx,
                    VelocidadeVertical = vy,
                    VelocidadeTotal = vTotal,
                    Angulo = Math.Atan2(vy, Math.Sqrt(vx * vx + vz * vz)) * 180.0 / Math.PI
                };
                result.Trajectory.Add(ponto);

                if (y <= input.TargetAltitude)
                {
                    impactou = true;
                    result.ImpactNorth = z;
                    result.ImpactEast = x;
                    result.ImpactAltitude = y;
                    result.ImpactVelocity = vTotal;
                    result.ImpactAngle = Math.Atan2(vy, Math.Sqrt(vx * vx + vz * vz)) * 180.0 / Math.PI;
                    result.TimeOfFlight = tempo;
                    result.TotalRange = Math.Sqrt((z - input.CannonNorth) * (z - input.CannonNorth) +
                                                 (x - input.CannonEast) * (x - input.CannonEast));
                    
                    double densidade = CalcularDensidadeAr(y, input.Temperature, input.Pressure, input.Humidity);
                    double area = Math.PI * (input.Diameter / 2) * (input.Diameter / 2);
                    double mach = vTotal / 340.0;
                    double cd = CalcularCd(mach, input.BC);
                    result.FinalDragForce = 0.5 * densidade * cd * area * vTotal * vTotal;
                }
            }

            if (!impactou)
            {
                result.ImpactNorth = z;
                result.ImpactEast = x;
                result.ImpactAltitude = y;
                result.ImpactVelocity = Math.Sqrt(vx * vx + vy * vy + vz * vz);
                result.ImpactAngle = Math.Atan2(vy, Math.Sqrt(vx * vx + vz * vz)) * 180.0 / Math.PI;
                result.TimeOfFlight = tempo;
                result.TotalRange = Math.Sqrt((z - input.CannonNorth) * (z - input.CannonNorth) +
                                             (x - input.CannonEast) * (x - input.CannonEast));
            }

            return result;
        }

        private void RK4Step(InputData input, ref double x, ref double y, ref double z,
                             ref double vx, ref double vy, ref double vz, ref double tempo)
        {
            double dt = DT;

            double k1vx, k1vy, k1vz, k1x, k1y, k1z;
            double k2vx, k2vy, k2vz, k2x, k2y, k2z;
            double k3vx, k3vy, k3vz, k3x, k3y, k3z;
            double k4vx, k4vy, k4vz, k4x, k4y, k4z;

            double ax = 0, ay = 0, az = 0;
            CalcularAceleracoes(input, x, y, z, vx, vy, vz, out ax, out ay, out az);

            k1vx = ax * dt;
            k1vy = ay * dt;
            k1vz = az * dt;
            k1x = vx * dt;
            k1y = vy * dt;
            k1z = vz * dt;

            double x2 = x + k1x / 2;
            double y2 = y + k1y / 2;
            double z2 = z + k1z / 2;
            double vx2 = vx + k1vx / 2;
            double vy2 = vy + k1vy / 2;
            double vz2 = vz + k1vz / 2;

            CalcularAceleracoes(input, x2, y2, z2, vx2, vy2, vz2, out ax, out ay, out az);

            k2vx = ax * dt;
            k2vy = ay * dt;
            k2vz = az * dt;
            k2x = vx2 * dt;
            k2y = vy2 * dt;
            k2z = vz2 * dt;

            double x3 = x + k2x / 2;
            double y3 = y + k2y / 2;
            double z3 = z + k2z / 2;
            double vx3 = vx + k2vx / 2;
            double vy3 = vy + k2vy / 2;
            double vz3 = vz + k2vz / 2;

            CalcularAceleracoes(input, x3, y3, z3, vx3, vy3, vz3, out ax, out ay, out az);

            k3vx = ax * dt;
            k3vy = ay * dt;
            k3vz = az * dt;
            k3x = vx3 * dt;
            k3y = vy3 * dt;
            k3z = vz3 * dt;

            double x4 = x + k3x;
            double y4 = y + k3y;
            double z4 = z + k3z;
            double vx4 = vx + k3vx;
            double vy4 = vy + k3vy;
            double vz4 = vz + k3vz;

            CalcularAceleracoes(input, x4, y4, z4, vx4, vy4, vz4, out ax, out ay, out az);

            k4vx = ax * dt;
            k4vy = ay * dt;
            k4vz = az * dt;
            k4x = vx4 * dt;
            k4y = vy4 * dt;
            k4z = vz4 * dt;

            vx = vx + (k1vx + 2 * k2vx + 2 * k3vx + k4vx) / 6;
            vy = vy + (k1vy + 2 * k2vy + 2 * k3vy + k4vy) / 6;
            vz = vz + (k1vz + 2 * k2vz + 2 * k3vz + k4vz) / 6;
            x = x + (k1x + 2 * k2x + 2 * k3x + k4x) / 6;
            y = y + (k1y + 2 * k2y + 2 * k3y + k4y) / 6;
            z = z + (k1z + 2 * k2z + 2 * k3z + k4z) / 6;

            tempo += dt;
        }

        private void CalcularAceleracoes(InputData input, double x, double y, double z,
                                         double vx, double vy, double vz,
                                         out double ax, out double ay, out double az)
        {
            double vRelNorth = vz - input.WindNorth;
            double vRelEast = vx - input.WindEast;
            double vRelVertical = vy;
            double vRel = Math.Sqrt(vRelNorth * vRelNorth + vRelEast * vRelEast + vRelVertical * vRelVertical);

            if (vRel < 1e-10)
            {
                ax = 0; ay = 0; az = 0;
                return;
            }

            double density = CalcularDensidadeAr(y, input.Temperature, input.Pressure, input.Humidity);
            double area = Math.PI * (input.Diameter / 2) * (input.Diameter / 2);
            double mach = vRel / 340.0;
            double cd = CalcularCd(mach, input.BC);

            double dragFactor = 0.5 * density * cd * area / input.Mass;
            double axDrag = -dragFactor * vRel * vRelEast;
            double ayDrag = -dragFactor * vRel * vRelVertical;
            double azDrag = -dragFactor * vRel * vRelNorth;

            double gravity = CalcularGravidade(y);

            double latRad = input.Latitude * Math.PI / 180.0;
            double omegaNorth = 2 * OMEGA * Math.Cos(latRad);
            double omegaVertical = 2 * OMEGA * Math.Sin(latRad);

            double axCoriolis = omegaVertical * vz - omegaNorth * vy;
            double ayCoriolis = omegaNorth * vx;
            double azCoriolis = -omegaVertical * vx;

            ax = axDrag + axCoriolis;
            ay = ayDrag - gravity + ayCoriolis;
            az = azDrag + azCoriolis;
        }

        private double CalcularDensidadeAr(double altitude, double temperature, double pressure, double humidity)
        {
            double T = temperature - 0.0065 * altitude;
            double P = pressure * Math.Pow((T / temperature), 5.2561);

            double pv = humidity * 611.0 * Math.Exp(17.27 * (T - 273.15) / (T - 36.0));
            double P_ar = P - pv;
            double rho = P_ar / (R * T) + pv / (461.5 * T);

            return rho;
        }

        private double CalcularGravidade(double altitude)
        {
            double r = 6371000.0;
            return G0 * (r / (r + altitude)) * (r / (r + altitude));
        }

        private double CalcularCd(double mach, double bc)
        {
            if (mach < 0.8)
            {
                return 0.15;
            }
            else if (mach < 1.0)
            {
                return 0.15 + (0.35 - 0.15) * (mach - 0.8) / 0.2;
            }
            else if (mach < 1.2)
            {
                return 0.35 + (0.45 - 0.35) * (mach - 1.0) / 0.2;
            }
            else if (mach < 2.0)
            {
                return 0.45 + (0.55 - 0.45) * (mach - 1.2) / 0.8;
            }
            else
            {
                return 0.55;
            }
        }
    }
}