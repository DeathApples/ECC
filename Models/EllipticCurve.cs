using System.Numerics;

namespace ECDH.Models
{
    public class EllipticCurve
    {
        public BigInteger a { get; set; }
        public BigInteger b { get; set; }
        public BigInteger p { get; set; }

        public List<Point>? Points { get; set; }

        public int Discriminant => 4 * (int)a * (int)a * (int)a + 27 * (int)b * (int)b;

        public override string? ToString()
        {
            string formula = "y² = x³";

            if (a != 0)
                formula += a == 1 ? " + x" : a == -1 ? " - x" : a < 0 ? $" - {Math.Abs((int)a)}x" : $" + {a}x";

            if (b != 0)
                formula += b < 0 ? $" - {Math.Abs((int)b)}" : $" + {b}";

            return formula;
        }
    }
}
