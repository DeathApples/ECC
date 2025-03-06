using System.Numerics;

namespace ECDH.Models
{
    public static class EllipticCurve
    {
        public static BigInteger A { get; set; }
        public static BigInteger B { get; set; }
        public static BigInteger P { get; set; }

        public static BigInteger Discriminant => 4 * BigInteger.Pow(A, 3) + 27 * BigInteger.Pow(B, 2);

        public static new string? ToString()
        {
            string formula = "y² = x³";

            if (A != 0)
                formula += A == 1 ? " + x" : A == -1 ? " - x" : A < 0 ? $" - {BigInteger.Abs(A)}x" : $" + {A}x";

            if (B != 0)
                formula += B < 0 ? $" - {BigInteger.Abs(B)}" : $" + {B}";

            return formula;
        }
    }
}
