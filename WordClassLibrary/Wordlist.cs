using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClassLibrary
{
    public class Wordlist
    {
 
        public static readonly string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Danny.Labb4");

        public string Name { get; }
        public string[] Languages { get; }
        private List<Word> Words { get; set; }

        public Wordlist(string name, params string[] languages)
        {
            Name = name;
            Languages = new string[languages.Length];
            for (int i = 0; i < languages.Length; i++)
            {
                Languages[i] = languages[i].ToLower();
            }
        }

        public static string[] GetLists()
        {
            string[] files = Directory.GetFiles(folder, "*.dat", SearchOption.AllDirectories);
            string[] nameArray = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                nameArray[i] = Path.GetFileNameWithoutExtension(files[i]);
            }

            return nameArray;
        }
        public static Wordlist LoadList(string name)
        {
            if (File.Exists(Path.Combine(folder, name + ".dat")))
            {
                string[] content = File.ReadAllLines(Path.Combine(folder, name + ".dat"));
                if (content.Length > 0)
                {
                    string[] languages = content[0].Split(';', StringSplitOptions.RemoveEmptyEntries);
                    var wordList = new Wordlist(name, languages);
                    wordList.Words = new List<Word>();

                    for (int i = 1; i < content.Length; i++)
                    {
                        wordList.Words.Add(new Word(content[i].Split(';', StringSplitOptions.RemoveEmptyEntries)));
                    }

                    return wordList;
                }
                else
                {
                    throw new IndexOutOfRangeException("Cannot read file. The file is empty");
                }
            }
            else
            {
                throw new ArgumentException("Could not find path");
            }
        }
        public void Save()
        {
            using (var sw = new StreamWriter(Path.Combine(folder, Name + ".dat")))
            {
                sw.WriteLine(string.Join(";", Languages));
                for (int i = 0; i < Words.Count; i++)
                {
                    sw.WriteLine(string.Join(";", Words[i].Translations));
                }
            }
        }
        public void Add(params string[] translations)
        {
            if (Words == null)
                Words = new List<Word>();

            translations = translations.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            translations = translations.Select(x => x.Trim()).ToArray();
            translations = translations.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            if (translations.Length == Languages.Length)
            {
                Words.Add(new Word(translations.Select(x => x.ToLower()).ToArray()));
            }
            else
            {
                throw new ArgumentException("Invalid number of translations added");
            }
        }
        public bool Remove(int translation, string word)
        {
            if (translation < 0 || translation > Languages.Length - 1)
            {
                throw new ArgumentOutOfRangeException($"Index {translation} is out of range");
            }

            for (int i = 0; i < Words.Count; i++)
            {
                if (word.ToLower() == Words[i].Translations[translation])
                {
                    Words.Remove(Words[i]);
                    return true;
                }
            }
            return false;
        }
        public int Count()
        {
            if (Words != null)
            {
                return Words.Count;
            }
            return 0;
        }
        public void List(int sortByTranslation, Action<string[]> showTranslations)
        {
            if (sortByTranslation < 0 || sortByTranslation > Languages.Count() - 1)
            {
                throw new ArgumentOutOfRangeException($"Index {sortByTranslation} is out of range");
            }

            foreach (var word in Words.OrderBy(w => w.Translations[sortByTranslation]).ToList())
            {
                showTranslations?.Invoke(word.Translations);
            }
        }
        public Word GetWordToPractice()
        {
            Random rng = new Random();
            if (Words != null)
            {
                int fromLanguages = rng.Next(Languages.Length), toLanguages = rng.Next(Languages.Length);
                while (fromLanguages == toLanguages)
                {
                    fromLanguages = rng.Next(Languages.Length);
                    toLanguages = rng.Next(Languages.Length);
                }
                return new Word(fromLanguages, toLanguages, Words[rng.Next(Words.Count)].Translations);
            }
            throw new ArgumentNullException("No Word objects found");
        }

    }
}
