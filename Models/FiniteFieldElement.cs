using System.Numerics;

namespace ECDH.Models
{
    public class FiniteFieldElement
    {
        public BigInteger Value { get; }
        public BigInteger Prime { get; }

        public FiniteFieldElement(BigInteger value, BigInteger prime)
        {
            Prime = prime;
            Value = Mod(value);
        }

        private BigInteger Mod(BigInteger value)
        {
            value %= Prime;
            return value < 0 ? value + Prime : value;
        }

        private static BigInteger ExtendedEuclideanAlgoritm(BigInteger a, BigInteger b, out BigInteger x, out BigInteger y)
        {
            (x, BigInteger xx, y, BigInteger yy) = (1, 0, 0, 1);
            BigInteger q;

            while (b != 0)
            {
                q = a / b;
                (a, b) = (b, a % b);
                (x, xx) = (xx, x - xx * q);
                (y, yy) = (yy, y - yy * q);
            }

            return a;
        }

        public FiniteFieldElement Copy() => new(Value, Prime);

        public FiniteFieldElement Inverse()
        {
            BigInteger gcd = ExtendedEuclideanAlgoritm(Value, Prime, out BigInteger x, out BigInteger y);

            if (gcd != 1)
                throw new InvalidOperationException("Element not invertible");

            return new(x, Prime);
        }

        public static FiniteFieldElement Pow(FiniteFieldElement value, BigInteger exponent)
        {
            FiniteFieldElement result = new(1, value.Prime);
            FiniteFieldElement addend = value.Copy();

            int sign = exponent.Sign;
            exponent *= sign;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                    result *= addend;

                addend *= addend;
                exponent >>= 1;
            }

            return sign < 0 ? result.Inverse() : result;
        }

        public static FiniteFieldElement operator +(FiniteFieldElement a, FiniteFieldElement b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return new(a.Value + b.Value, a.Prime);
        }

        public static FiniteFieldElement operator +(BigInteger a, FiniteFieldElement b) => new(a + b.Value, b.Prime);

        public static FiniteFieldElement operator +(FiniteFieldElement a, BigInteger b) => new(a.Value + b, a.Prime);

        public static FiniteFieldElement operator -(FiniteFieldElement a, FiniteFieldElement b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return new(a.Value - b.Value, a.Prime);
        }

        public static FiniteFieldElement operator -(BigInteger a, FiniteFieldElement b) => new(a - b.Value, b.Prime);

        public static FiniteFieldElement operator -(FiniteFieldElement a, BigInteger b) => new(a.Value - b, a.Prime);

        public static FiniteFieldElement operator -(FiniteFieldElement a) => new(-a.Value, a.Prime);

        public static FiniteFieldElement operator *(FiniteFieldElement a, FiniteFieldElement b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return new(a.Value * b.Value, a.Prime);
        }

        public static FiniteFieldElement operator *(BigInteger a, FiniteFieldElement b) => new(a * b.Value, b.Prime);

        public static FiniteFieldElement operator *(FiniteFieldElement a, BigInteger b) => new(a.Value * b, a.Prime);

        public static FiniteFieldElement operator /(FiniteFieldElement a, FiniteFieldElement b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return a * b.Inverse();
        }

        public static FiniteFieldElement operator /(BigInteger a, FiniteFieldElement b) => a * b.Inverse();

        public static FiniteFieldElement operator /(FiniteFieldElement a, BigInteger b) => a * new FiniteFieldElement(b, a.Prime).Inverse();

        public static bool operator ==(FiniteFieldElement a, FiniteFieldElement b) => a.Equals(b);

        public static bool operator !=(FiniteFieldElement a, FiniteFieldElement b) => !a.Equals(b);

        public override bool Equals(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            if (obj is FiniteFieldElement a)
                return Value == a.Value && Prime == a.Prime;

            else
                throw new ArgumentException("Argument not a finite field element");
        }

        public override int GetHashCode() => HashCode.Combine(Value, Prime);

        public override string? ToString() => $"{Value} (mod {Prime})";
    }
}
