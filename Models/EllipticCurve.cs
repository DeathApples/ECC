using System.Numerics;

namespace ECDH.Models
{
    public sealed class EllipticCurve
    {
        private static EllipticCurve? _instance;

        public static EllipticCurve Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }

        private EllipticCurve() {  }

        public static BigInteger a { get; set; }
        public static BigInteger b { get; set; }
        public static BigInteger p { get; set; }

        public static BigInteger Discriminant => 4 * BigInteger.Pow(a, 3) + 27 * BigInteger.Pow(b, 2);

        public static (BigInteger, BigInteger) O => (0, 0);

        public override string? ToString()
        {
            string formula = "y² = x³";

            if (a != 0)
                formula += a == 1 ? " + x" : a == -1 ? " - x" : a < 0 ? $" - {BigInteger.Abs(a)}x" : $" + {a}x";

            if (b != 0)
                formula += b < 0 ? $" - {BigInteger.Abs(b)}" : $" + {b}";

            return formula;
        }
    }
}
