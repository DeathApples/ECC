using ECDH.Services;
using System.Numerics;

namespace ECDH.Models
{
    public class Polynomial
    {
        public static BigInteger P { get; set; }
        public Dictionary<BigInteger, BigInteger> MonomialDictionary { get; private set; }

        public BigInteger Degree => MonomialDictionary.Count > 0 ? MonomialDictionary.Keys.Max() : 0;

        private static BigInteger NormilizeInField(BigInteger a) => MathService.NormilizeInField(a, P);
        private static BigInteger ModularInverse(BigInteger a) => ExtendedEuclideanAlgorithm.ModularInverse(a, P);

        public Polynomial()
        {
            MonomialDictionary = [];
        }

        public Polynomial(BigInteger degree)
        {
            MonomialDictionary = [];
            MonomialDictionary[degree] = 0;
        }

        public Polynomial(Polynomial polynomial)
        {
            MonomialDictionary = new(polynomial.MonomialDictionary);
        }

        public Polynomial(params (BigInteger power, BigInteger coefficient)[] numbers)
        {
            MonomialDictionary = [];

            for (int i = 0; i < numbers.Length; i++)
                MonomialDictionary[numbers[i].power] = NormilizeInField(numbers[i].coefficient);
        }

        public BigInteger this[BigInteger index]
        {
            get => MonomialDictionary.TryGetValue(index, out BigInteger value) ? value : 0;
            set
            {
                if (value == 0)
                    MonomialDictionary.Remove(index);
                else
                    MonomialDictionary[index] = value;
            }
        }

        public static Polynomial operator +(Polynomial left, Polynomial right)
        {
            Polynomial result = new(BigInteger.Max(left.Degree, right.Degree));

            for (int i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = NormilizeInField(left[i] + right[i]);

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

            for (int i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = NormilizeInField(left[i] - right[i]);

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = P - right[i];
            }

            return result;
        }

        public static Polynomial operator *(Polynomial left, Polynomial right)
        {
            Polynomial result = new(left.Degree + right.Degree);

            for (int i = 0; i <= left.Degree; i++)
            {
                for (int k = 0; k <= right.Degree; k++)
                {
                    result[i + k] = NormilizeInField(result[i + k] + left[i] * right[k]);
                }
            }

            return result;
        }

        public static Polynomial operator /(Polynomial left, Polynomial right)
        {
            var newDegree = left.Degree - right.Degree;
            Polynomial result = new(newDegree);
            Polynomial temp = new(left);

            for (BigInteger i = newDegree; i >= 0; i--)
            {
                result[i] = NormilizeInField(temp[right.Degree + i] * ModularInverse(right[right.Degree]));

                for (BigInteger k = right.Degree + i - 1; k >= i; k--)
                {
                    temp[k] = NormilizeInField(temp[k] - result[i] * right[k - i]);
                }
            }

            return result;
        }
    }
}
