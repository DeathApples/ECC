using ECDH.Services;
using System.Numerics;

namespace ECDH.Models
{
    public class Polynomial
    {
        private BigInteger[] coefficients;
        public static BigInteger P { get; set; }

        public int Degree => coefficients.Length - 1;

        private static BigInteger NormilizeInField(BigInteger a) => MathService.NormilizeInField(a, P);
        private static BigInteger ModularInverse(BigInteger a) => ExtendedEuclideanAlgorithm.ModularInverse(a, P);

        public Polynomial()
        {
            coefficients = [];
        }

        public Polynomial(int degree)
        {
            coefficients = new BigInteger[degree + 1];
        }

        public Polynomial(params BigInteger[] numbers)
        {
            coefficients = new BigInteger[numbers.Length];
            Array.Copy(numbers, coefficients, numbers.Length);

            NormilizeCoefficients();
        }

        public BigInteger this[int index]
        {
            get => coefficients[index];
            set => coefficients[index] = value;
        }

        public static Polynomial operator +(Polynomial left, Polynomial right)
        {
            Polynomial result = new (Math.Max(left.Degree, right.Degree));

            for (int i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = NormilizeInField(left[i] + right[i]);

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = right[i];
            }

            result.NormilizeDegree();
            return result;
        }

        public static Polynomial operator -(Polynomial left, Polynomial right)
        {
            Polynomial result = new(Math.Max(left.Degree, right.Degree));

            for (int i = 0; i <= result.Degree; i++)
            {
                if (i <= left.Degree && i <= right.Degree)
                    result[i] = NormilizeInField(left[i] - right[i]);

                else if (i <= left.Degree)
                    result[i] = left[i];

                else if (i <= right.Degree)
                    result[i] = P - right[i];
            }

            result.NormilizeDegree();
            return result;
        }

        public static Polynomial operator *(Polynomial left, Polynomial right)
        {
            Polynomial result = new(left.Degree + right.Degree);

            for (int i = 0; i <= left.Degree; i++)
            {
                for (int j = 0; j <= right.Degree; j++)
                {
                    result[i + j] = NormilizeInField(result[i + j] + left[i] * right[j]);
                }
            }

            result.NormilizeDegree();
            return result;
        }

        public void NormilizeDegree()
        {
            while (coefficients.Length > 1)
            {
                if (coefficients[^1] == 0)
                {
                    BigInteger[] temp = new BigInteger[coefficients.Length - 1];
                    Array.Copy(coefficients, temp, coefficients.Length - 1);
                    coefficients = temp;
                }
                else
                {
                    break;
                }
            }
        }

        public void NormilizeCoefficients()
        {
            for (int i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] = NormilizeInField(coefficients[i]);
            }
        }
    }
}
