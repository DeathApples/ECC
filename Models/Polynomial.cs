using System.Numerics;

namespace ECDH.Models
{
    public class Polynomial
    {
        public static BigInteger Prime { get; set; }
        public Dictionary<BigInteger, FiniteFieldElement> MonomialDictionary { get; private set; }

        public BigInteger Degree => MonomialDictionary.Count > 0 ? MonomialDictionary.Keys.Max() : 0;

        public Polynomial()
        {
            MonomialDictionary = [];
        }

        public Polynomial(BigInteger degree)
        {
            MonomialDictionary = [];
            MonomialDictionary[degree] = new(0, Prime);
        }

        public Polynomial(Polynomial polynomial)
        {
            MonomialDictionary = new(polynomial.MonomialDictionary);
        }

        public Polynomial(params (BigInteger power, FiniteFieldElement coefficient)[] numbers)
        {
            MonomialDictionary = [];

            for (int i = 0; i < numbers.Length; i++)
                MonomialDictionary[numbers[i].power] = numbers[i].coefficient;
        }

        public FiniteFieldElement this[BigInteger index]
        {
            get => MonomialDictionary.TryGetValue(index, out FiniteFieldElement? value) ? value : new(0, Prime);
            
            set
            {
                if (value.Value == 0)
                    MonomialDictionary.Remove(index);
                else
                    MonomialDictionary[index] = value;
            }
        }

        public static Polynomial operator +(Polynomial left, Polynomial right)
        {
            Polynomial result = new(BigInteger.Max(left.Degree, right.Degree));

            for (BigInteger i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = left[i] + right[i];

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = right[i];
            }

            return result;
        }

        public static Polynomial operator -(Polynomial left, Polynomial right)
        {
            Polynomial result = new(BigInteger.Max(left.Degree, right.Degree));

            for (BigInteger i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = left[i] - right[i];

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = -right[i];
            }

            return result;
        }

        public static Polynomial operator *(Polynomial left, Polynomial right)
        {
            Polynomial result = new(left.Degree + right.Degree);

            for (BigInteger i = 0; i <= left.Degree; i++)
                for (BigInteger k = 0; k <= right.Degree; k++)
                    result[i + k] = result[i + k] + left[i] * right[k];

            return result;
        }

        public static Polynomial operator /(Polynomial left, Polynomial right)
        {
            Polynomial result = new(left.Degree - right.Degree);
            Polynomial temp = new(left);

            for (BigInteger i = result.Degree; i >= 0; i--)
            {
                result[i] = temp[right.Degree + i] / right[right.Degree];

                for (BigInteger k = right.Degree + i - 1; k >= i; k--)
                    temp[k] -= result[i] * right[k - i];
            }

            return result;
        }

        public static bool operator ==(Polynomial left, Polynomial right) => left.Equals(right);

        public static bool operator !=(Polynomial left, Polynomial right) => !left.Equals(right);

        public override bool Equals(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            if (obj is Polynomial polynomial)
            {
                if (MonomialDictionary.Count != polynomial.MonomialDictionary.Count) return false;
                return MonomialDictionary.All(kvp => polynomial.MonomialDictionary.TryGetValue(kvp.Key, out var value) && value.Equals(kvp.Value));
            }
            else
                throw new ArgumentException("Argument “obj” must be type of Class “Polynomial”", nameof(obj));
        }

        public override int GetHashCode() => MonomialDictionary.GetHashCode();
    }
}
