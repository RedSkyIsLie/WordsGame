using System;
using System.Linq;
using AllClasses;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace WordsGame
{
    class Program
    {
        static void Main(string[] args)
        {
            byte number = 2;
            string[] commands = new string[3] { "/show-words", "/score", "/total-score"};
            Gamer gamer = new Gamer();
            WorkWithFile dict = new WorkWithFile();
            Dictionary<string, int> currentGamers = new Dictionary<string, int>();
            List<EnterWords> enterWords = new List<EnterWords>(); // список введенных слов
            dict.GamersInDict();
            Reader(dict);
            EnterGamerInfo(currentGamers, gamer, dict);
            Rules(); // правила
            string mainWord = MainWord(); // основное слово
            do
            {
                Player(ref number); // смена игрока
                string playerWord = Entering(commands, number); // ввод слова 
                bool result = Checker(mainWord,playerWord);
                if (result == true)
                    enterWords.Add(new EnterWords() { EnterWord = playerWord });
                else
                {
                    if (CommandCheck(commands, playerWord) == true)
                    {
                        number = CommandWork(playerWord, enterWords, number, dict, currentGamers);
                        Console.WriteLine($"Ключевое слово: {mainWord}");
                    }
                    else
                        number = Solution(number, currentGamers);
                }
            }
            while (number > 0);
        }
        static void Reader(WorkWithFile dict)
        {
            // вывод результатов прошлой игры
            Console.WriteLine("Итог прошлой игры:");
            foreach (KeyValuePair<string, int> keyValue in dict.DictForRead)
            {
                Console.WriteLine("Имя: " + keyValue.Key + " Счет: " + keyValue.Value);
            }
            Console.ReadKey();
            Console.Clear();
        }
        static void EnterGamerInfo(Dictionary<string, int> currentGamers, Gamer gamer, WorkWithFile dict)
        {
            for (byte i = 0; i < 2; i++)
            {
                Console.Write($"Введите имя игрока №{i + 1}: ");
                string playerName = Console.ReadLine();
                gamer.Name = playerName;
                if (dict.ActivDict.ContainsKey(playerName))
                    gamer.Score = dict.ActivDict[playerName];
                else
                    gamer.Score = 0;
                currentGamers.Add(gamer.Name, gamer.Score);
            }
            Console.ReadKey();
            Console.Clear();
        }

        static void Writer(Dictionary<string, int> currentGamers)
        {
            // сохранение данных
            using (StreamWriter sw = new StreamWriter("GamersInfo.json", true, System.Text.Encoding.Default))
            {
                string json = JsonSerializer.Serialize(currentGamers);
                sw.WriteLine(json);
            }
        }

        static void Rules()
        {
            int width = Console.WindowWidth;
            string[] text = {"Игра в слова", // правила
                             "Правила",
                             "Для начала игры необходимо ввести ключевое слово (или набор букв)",
                             "(Ключевое слово должно содержать в себе не менее 8 и не более 30 букв)",
                             "Задача двух игроков по очереди составлять из ключевого слова свои слова",
                             "Кто не смог, тот проиграл!",
                             "Удачи!"};
            for (byte j = 0; j < text.Length; j++) // вывод правил по центру консоли
            {
                string str = text[j].PadLeft((width - text[j].Length) / 2 + text[j].Length);
                Console.WriteLine(str);
            }
            Console.ReadKey();
            Console.Clear();
        }

        static string MainWord()
        {
            bool check = false;
            Console.Write("Введите ключевое слово: ");
            string keyWord = Console.ReadLine();
            while (check == false) // проверка введенного слова
            {
                if (keyWord.Length < 8 | keyWord.Length > 30)
                {
                    Console.WriteLine("Вы ввели недопустимое по длине слово!");
                    Console.WriteLine("Повторите попытку ввода");
                    Console.Write("Введите ключевое слово: ");
                    keyWord = Console.ReadLine();
                }
                else check = true;
            }
            return keyWord;
        }

        static void Player(ref byte number)
        {
            if (number == 2) // смена игрока
                number--;
            else
                number++;
        }

        static Dictionary<char, int> CharCount(string str) => str.ToLower().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        static string Entering(string[] commands, byte number)
        {
            Console.Write($"[Игрок №{number}] Ввод: ");
            string playerWord = Console.ReadLine();
            return playerWord;
        }

        static bool Checker(string mainWord, string playerWord)
        {
            var basic = CharCount(mainWord);
            var active = CharCount(playerWord);
            var result = active.All(x => basic.ContainsKey(x.Key) && basic[x.Key] >= x.Value); // проверка на нахождение в основном слове
            return result;
        }

        static bool CommandCheck(string[] commands, string playerWord)
        {
            for (byte i = 0; i<commands.Length; i++)
            {
                if (playerWord == commands[i])
                    return true;
            }
            return false;
        }

        static byte CommandWork (string playerWord, List<EnterWords> enterWords, byte number, WorkWithFile dict, Dictionary<string, int> currentGamers)
        {
            Console.Clear();
            byte i = 1;
            int playerNum = 0;
            switch (playerWord)
            {
                case "/show-words": // вывод введенных за игру слов
                    Console.WriteLine("Введенный слова за игру:");
                    foreach (EnterWords word in enterWords)
                    {
                        i++;
                        playerNum = i % 2;
                        Console.Write($"Игрок №{playerNum + 1}: ");
                        Console.WriteLine(word.EnterWord);
                    }
                    Console.ReadKey();

                    break;
                case "/score": // вывод счета для текущих игроков
                    Console.WriteLine("Счет:");
                    foreach (KeyValuePair<string, int> keyValue in currentGamers)
                    {
                        Console.WriteLine("Имя: " + keyValue.Key + " Счет: " + keyValue.Value);
                    }
                    Console.ReadKey();
                    break;
                case "/total-score": // вывод счета для всех игроков
                    Console.WriteLine("Глобальный счет: ");
                    foreach (KeyValuePair<string, int> keyValue in dict.ActivDict)
                    {
                        Console.WriteLine("Имя: " + keyValue.Key + " Счет: " + keyValue.Value);
                    }
                    Console.ReadKey();
                    break;
            }
            Player(ref number);
            Console.Clear();
            return number;
        }

        static byte Solution(byte number, Dictionary<string, int> currentGamers)
        {
            int width = Console.WindowWidth;
            Console.WriteLine();
            Console.WriteLine("Вы ввели несоответствующее слово!");
            Console.WriteLine("Если вы хотите продолжить, введите <1>");
            Console.WriteLine("Если вы хотите сдаться, введите любую другую цифру");
            Console.WriteLine();
            Console.Write("Ввод: ");
            byte choose = Convert.ToByte(Console.ReadLine());
            if (choose == 1)
            {
                Player(ref number);
                Console.WriteLine();
            }
            else
            {
                Console.Clear(); //вывод на экран номер проигравшего игрока
                string lose = $"Игрок под номером {number} проиграл";
                Console.WriteLine(lose.PadLeft((width - lose.Length) / 2 + lose.Length));
                Player(ref number);
                string key = currentGamers.Keys.ElementAt(number-1); // получение ключа игрока под номером number-1
                currentGamers[key]++; // изменение счета игрока под номером number-1
                Writer(currentGamers); // запись в файл
                Console.ReadKey();
                number = 0;
            }
            return number;
        }
    }
}
