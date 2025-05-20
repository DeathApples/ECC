using System.Numerics;

namespace ECDH.Models
{
    public class FiniteFieldNumber
    {
        public BigInteger Value { get; }
        
        public BigInteger Prime { get; }

        public FiniteFieldNumber(BigInteger value, BigInteger prime)
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
            (x, var xx, y, var yy) = (BigInteger.One, BigInteger.Zero, BigInteger.Zero, BigInteger.One);
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

        public FiniteFieldNumber Copy() => new(Value, Prime);

        public FiniteFieldNumber Inverse()
        {
            var gcd = ExtendedEuclideanAlgoritm(Value, Prime, out BigInteger x, out BigInteger _);

            if (gcd != 1)
                throw new InvalidOperationException("Element not invertible");

            return new(x, Prime);
        }

        public static FiniteFieldNumber Pow(FiniteFieldNumber value, BigInteger exponent)
        {
            var result = new FiniteFieldNumber(1, value.Prime);
            var addend = value.Copy();

            var sign = exponent.Sign;
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

        public static FiniteFieldNumber operator +(FiniteFieldNumber a, FiniteFieldNumber b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return new(a.Value + b.Value, a.Prime);
        }

        public static FiniteFieldNumber operator +(BigInteger a, FiniteFieldNumber b) => new(a + b.Value, b.Prime);

        public static FiniteFieldNumber operator +(FiniteFieldNumber a, BigInteger b) => new(a.Value + b, a.Prime);

        public static FiniteFieldNumber operator -(FiniteFieldNumber a, FiniteFieldNumber b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return new(a.Value - b.Value, a.Prime);
        }

        public static FiniteFieldNumber operator -(BigInteger a, FiniteFieldNumber b) => new(a - b.Value, b.Prime);

        public static FiniteFieldNumber operator -(FiniteFieldNumber a, BigInteger b) => new(a.Value - b, a.Prime);

        public static FiniteFieldNumber operator -(FiniteFieldNumber a) => new(-a.Value, a.Prime);

        public static FiniteFieldNumber operator *(FiniteFieldNumber a, FiniteFieldNumber b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return new(a.Value * b.Value, a.Prime);
        }

        public static FiniteFieldNumber operator *(BigInteger a, FiniteFieldNumber b) => new(a * b.Value, b.Prime);

        public static FiniteFieldNumber operator *(FiniteFieldNumber a, BigInteger b) => new(a.Value * b, a.Prime);

        public static FiniteFieldNumber operator /(FiniteFieldNumber a, FiniteFieldNumber b)
        {
            if (a.Prime != b.Prime)
                throw new ArgumentException("Primes must match");

            return a * b.Inverse();
        }

        public static FiniteFieldNumber operator /(BigInteger a, FiniteFieldNumber b) => a * b.Inverse();

        public static FiniteFieldNumber operator /(FiniteFieldNumber a, BigInteger b) => a * new FiniteFieldNumber(b, a.Prime).Inverse();

        public static bool operator ==(FiniteFieldNumber a, FiniteFieldNumber b) => a.Equals(b);

        public static bool operator !=(FiniteFieldNumber a, FiniteFieldNumber b) => !a.Equals(b);

        public override bool Equals(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            if (obj is FiniteFieldNumber a)
                return Value == a.Value && Prime == a.Prime;

            else
                throw new ArgumentException("Argument not a finite field number");
        }

        public override int GetHashCode() => HashCode.Combine(Value, Prime);

        public override string? ToString() => $"{Value} (mod {Prime})";
    }
}
