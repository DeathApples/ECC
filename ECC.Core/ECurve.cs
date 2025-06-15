using ECC.Core.Algorithms;
using ECC.Core.Models;
using System.Numerics;

namespace ECC.Core
{
    public static class ECurve
    {
        public static event Action? EllipticCurveChanged;

        public static BigInteger A
        {
            get => EllipticCurve.A;
            set => TryGenerateEllipticCurve(value, EllipticCurve.B, EllipticCurve.Prime);
        }

        public static BigInteger B
        {
            get => EllipticCurve.B;
            set => TryGenerateEllipticCurve(EllipticCurve.A, value, EllipticCurve.Prime);
        }

        public static BigInteger Prime
        {
            get => EllipticCurve.Prime;
            set => TryGenerateEllipticCurve(EllipticCurve.A, EllipticCurve.B, value);
        }

        public static BigInteger Order => EllipticCurve.Order;

        public static BigInteger Discriminant => EllipticCurve.Discriminant;

        public static bool TryGenerateEllipticCurve(BigInteger a, BigInteger b, BigInteger p)
        {
            var tempA = EllipticCurve.A; EllipticCurve.A = a;
            var tempB = EllipticCurve.B; EllipticCurve.B = b;
            var tempP = EllipticCurve.Prime; EllipticCurve.Prime = p;

            if (EllipticCurve.Discriminant == 0 || !MillerRabinPrimalityTest.IsPrimeNumber(p))
            {
                EllipticCurve.A = tempA;
                EllipticCurve.B = tempB;
                EllipticCurve.Prime = tempP;

                return false;
            }

            EllipticCurve.Order = SchoofAlgorithm.GetPointsCount();
            EllipticCurveChanged?.Invoke();
            return true;
        }

        public static List<ECPoint> GetPoints()
        {
            var points = new List<ECPoint>();

            for (BigInteger x = 0; x < Prime; x++)
            {
                if (EllipticCurve.TryGetPointByX(x, out EllipticCurvePoint point))
                {
                    points.Add(new(point.X.Value, point.Y.Value));

                    if (point.Y.Value != 0)
                    {
                        var oppositePoint = -point;
                        points.Add(new(oppositePoint.X.Value, oppositePoint.Y.Value));
                    }
                }
            }

            return points;
        }

        public static bool TryGetBasePoint(out ECPoint G)
        {
            var result = EllipticCurve.TryGetBasePoint(out EllipticCurvePoint basePoint);
            G = new(basePoint);
            return result;
        }

        public static string Formula
        {
            get
            {
                string formula = "";

                if (A != 0)
                    formula += A == 1 ? " + x" : A == -1 ? " - x" : A < 0 ? $" - {-A}x" : A > 0 ? $" + {A}x" : "";

                if (B != 0)
                    formula += B < 0 ? $" - {-B}" : B > 0 ? $" + {B}" : "";

                return $"{formula} (mod {Prime})";
            }
        }
    }
}
