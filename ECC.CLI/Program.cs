namespace ECC.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var delay = 0;
            
            while (true)
            {
                int choice1;

                Console.Clear();

                Console.WriteLine("Действия, доступные для выбора:");
                Console.WriteLine(" 1. Подбор \"правильной\" эллиптической кривой");
                Console.WriteLine(" 2. Выход");

                do
                    Console.Write("\nВыберите действие (укажите номер от 1 до 2): ");
                while (!int.TryParse(Console.ReadLine(), out choice1) || choice1 < 1 || 2 < choice1);

                switch (choice1)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Производится генерация параметров эллиптической кривой");
                        Console.WriteLine("Данная операция может выполняться продолжительное время!");
                        EllipticCurve.Prepare();
                        Console.Clear();
                        ConsoleVisualizer.PrintParameters();
                        Console.Write("Нажмите любую клавишу для продолжения: ");
                        Console.ReadKey(false);
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Завершение работы");
                        return;
                }

                var isBack = false;

                while (!isBack)
                {
                    var choice2 = 0;

                    Console.Clear();

                    Console.WriteLine("Действия, доступные для выбора:");
                    Console.WriteLine(" 1. Алгоритм Диффи-Хеллмана на эллиптической кривой");
                    Console.WriteLine(" 2. Электронная цифровая подпись на эллиптической кривой");
                    Console.WriteLine(" 3. Задать задержку между выполнением шагов");
                    Console.WriteLine(" 4. Назад");

                    do
                        Console.Write("\nВыберите действие (укажите номер от 1 до 4): ");
                    while (!int.TryParse(Console.ReadLine(), out choice2) || choice2 < 1 || 4 < choice2);

                    switch (choice2)
                    {
                        case 1:
                            Console.Clear();
                            DiffieHellman.Execute(delay);
                            Console.Write("Нажмите любую клавишу для продолжения: ");
                            Console.ReadKey(false);
                            break;

                        case 2:
                            var defaultMessage = "Hello World";
                            Console.Write($"\nВведите сообщение для подписания [{defaultMessage}]: ");
                            var message = Console.ReadLine() ?? defaultMessage;
                            
                            Console.Clear();
                            DigitalSignature.Execute(message, delay);
                            Console.Write("Нажмите любую клавишу для продолжения: ");
                            Console.ReadKey(false);
                            break;

                        case 3:
                            do
                                Console.Write("\nУкажите задержку между выполнением шагов (в миллисекундах): ");
                            while (!int.TryParse(Console.ReadLine(), out delay) || delay < 0);
                            break;

                        case 4:
                            isBack = true;
                            break;
                    }
                }
            }
        }
    }
}
