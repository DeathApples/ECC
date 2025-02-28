using System;
using System.Numerics;

namespace ECDH.Models
{
    class MillerRabinPrimeGenerator
    {
        private readonly double _precision;
        public double Precision => _precision;

        public MillerRabinPrimeGenerator(double precision)
        {
            _precision = precision;
        }

        public BigInteger GeneratePrimeNumber()
        {
            // ToDo: Реализовать основную функцию для генерации простого числа на основе Теста Миллера-Рабина
            // Для генерации числа простого числа P использовать алгоритм хэширования SHA-256

            return 0;
        }

        private bool IsPrimeNumber(BigInteger number)
        {
            // ToDo: Реализовать проверку числа на простоту с указанной точностью 'precision'

            return false;
        }
    }
}
