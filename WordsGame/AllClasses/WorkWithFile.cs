using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace AllClasses
{
    public class WorkWithFile
    {
        public Dictionary<string, int> DictForRead { get; set; } 
        public Dictionary<string, int> ActivDict = new Dictionary<string, int>(); // словарь со всеми игроками
        public void GamersInDict()
        {
            DictForRead = new Dictionary<string, int>();
            using (StreamReader fs = new StreamReader("GamersInfo.json", System.Text.Encoding.Default))
            {
                string str;
                while (fs.EndOfStream != true)
                {
                    str = fs.ReadLine();
                    DictForRead = JsonConvert.DeserializeObject<Dictionary<string, int>>(str); // считывание во временный словарь
                    foreach (KeyValuePair<string, int> keyValue in DictForRead) // добавление в активный словарь
                    {
                        ActivDict.Remove(keyValue.Key);
                        ActivDict.Add(keyValue.Key, keyValue.Value);
                    }
                }
            }
        }
    }
}
