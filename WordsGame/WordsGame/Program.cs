using System;
using System.Linq;
using System.Collections.Generic;

namespace WordsGame
{
    class Program
    {
        static Dictionary<char, int> CharCount(string str) => str.ToLower().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        static void Main(string[] args)
        {          
            int width = Console.WindowWidth;
            string[] text = {"Игра в слова", // правила
                             "Правила",
                             "Для начала игры необходимо ввести ключевое слово (или набор букв)",
                             "(Ключевое слово должно содержать в себе не менее 8 и не более 30 букв)",
                             "Задача двух игроков по очереди составлять из ключевого слова свои слова",
                             "Кто не смог, тот проиграл!",
                             "Удачи!"};
            for(byte j=0;j<text.Length;j++) // вывод правил по центру консоли
            {
                string str = text[j].PadLeft((width - text[j].Length) / 2 + text[j].Length);
                Console.WriteLine(str);
            }
            Console.ReadKey();
            Console.Clear();
            bool Check = false;
            Console.Write("Введите ключевое слово: ");
            string KeyWord = Console.ReadLine();
            while (Check == false) // проверка введенного слова
            {
                if (8 > KeyWord.Length | KeyWord.Length > 30)
                {
                    Console.WriteLine("Вы ввели недопустимое по длине слово!");
                    Console.WriteLine("Повторите попытку ввода");
                    Console.Write("Введите ключевое слово: ");
                    KeyWord = Console.ReadLine();
                }
                else Check = true;
            }
            byte i = 2;
            do
            {
                if (i == 2) // смена игрока
                    i--;
                else
                    i++;
                Console.Write($"Игрок под номером {i} введите слово: ");
                string Player = Console.ReadLine();
                var basic = CharCount(KeyWord);
                var active = CharCount(Player);
                var result = active.All(x => basic.ContainsKey(x.Key) && basic[x.Key] >= x.Value); // проверка на нахождение в основном слове
                if (result == false) // проверка на проигрыш
                {
                    Console.WriteLine();
                    Console.WriteLine("Вы ввели несоответствующее слово!");
                    Console.WriteLine("Если вы хотите продолжить, введите <1>");
                    Console.WriteLine("Если вы хотите сдаться, введите любую другую цифру");
                    Console.WriteLine();
                    Console.Write("Ввод: ");
                    byte b = Convert.ToByte(Console.ReadLine());
                    if (b == 1)
                    {
                        if (i == 2) // возвращение к текущему игроку
                            i--;
                        else
                            i++;
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.Clear(); //вывод на экран номер проигравшего игрока
                        string lose = $"Игрок под номером {i} проиграл";
                        Console.WriteLine(lose.PadLeft((width - lose.Length) / 2 + lose.Length));
                        Console.ReadKey();
                        return;
                    }
                }

            }
            while (i>0);

        }
    }
}
