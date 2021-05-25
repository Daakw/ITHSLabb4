using System;
using System.IO;
using System.Linq;
using WordClassLibrary;


namespace Labb4
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Directory.Exists(Wordlist.folder))
                Directory.CreateDirectory(Wordlist.folder);

            string[] parameters = args;
            if (args.Length == 0)
            {
                Console.WriteLine(new string('-', 100));
                ShowOptions();
            }

            int counter = 0;
            bool flag = true;
            while (flag)
            {
                counter++;
                try
                {
                    if (counter > 1 || args.Length == 0)
                    {
                        parameters = Console.ReadLine()
                            .Split(new char[] { ' ', '.', ',', ';', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    string fileName = string.Empty;
                    if (parameters.Length > 1)
                    {
                        fileName = parameters[1];
                    }
                    parameters = Array.ConvertAll(parameters, x => x.ToLower());
                    Console.WriteLine(new string('-', 100));

                   
                    switch (parameters[0])
                    {
                        case "-list" or "list":
                        case "-lists" or "lists":
                            ShowCurrentLists();
                            break;
                        case "-new" or "new":
                            if (parameters.Length > 2)
                            {
                                if (parameters.Length == 3)
                                {
                                    Console.WriteLine($"You need to add more languages");
                                    Console.Write($"Write one or more languages (separated by a space) to add to your list: ");
                                    string[] moreLanguages = Console.ReadLine()
                                        .Split(new char[] { ' ', '.', ',', ';', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

                                    Array.Resize(ref parameters, moreLanguages.Length + parameters.Length);
                                    for (int i = 3; i < parameters.Length; i++)
                                    {
                                        parameters[i] = moreLanguages[i - 3];
                                    }
                                }
                                var wordList = new Wordlist(fileName, parameters[2..(parameters.Length)]);
                                AddWords(wordList);
                                SaveList(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            else
                            {
                                string name = string.Empty;
                                if (parameters.Length != 2)
                                {
                                    Console.Write($"Choose a name for your list without adding the .dat extension: "); 
                                    name = Console.ReadLine();
                                    
                                }

                                Console.Write($"Write all the languages (separated by a space) on this line: ");
                                string[] languages = Console.ReadLine()
                                    .Split(new char[] { ' ', '.', ',', ';', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                                

                                if (languages.Length == 1)
                                {
                                    Console.WriteLine($"\nYou need to have more than one language to practice with");
                                    Console.Write($"Write one or more languages (separated by a space) to add to your list: ");
                                    string[] moreLanguages = Console.ReadLine()
                                        .Split(new char[] { ' ', '.', ',', ';', '<', '>' }, StringSplitOptions.RemoveEmptyEntries);

                                    Array.Resize(ref languages, moreLanguages.Length + 1);
                                    for (int i = 1; i < languages.Length; i++)
                                    {
                                        languages[i] = moreLanguages[i - 1];
                                    }
                                }

                                languages = Array.ConvertAll(languages, x => x.ToLower());
                                var wordList = new Wordlist(parameters.Length == 2 ? fileName : name, languages);
                                AddWords(wordList);
                                SaveList(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            break;
                        case "-add" or "add":
                            ShowCurrentLists();
                            if (parameters.Length > 1)
                            {
                                var wordList = Wordlist.LoadList(fileName);
                                AddWords(wordList);
                                SaveList(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            else
                            {
                                Console.Write($"Write the name of the list: ");
                                string name = Console.ReadLine();
                                var wordList = Wordlist.LoadList(name);
                                AddWords(wordList);
                                SaveList(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            break;
                        case "-remove" or "remove":
                            ShowCurrentLists();
                            if (parameters.Length > 2)
                            {
                                var wordList = Wordlist.LoadList(fileName);
                                if (parameters.Length == 3)
                                {
                                    RemoveWords(wordList, parameters[2]);
                                    SaveList(wordList);
                                }
                                else
                                {
                                    var test = parameters[2..(parameters.Length)];
                                    RemoveWords(wordList, parameters[2..(parameters.Length)]);
                                    SaveList(wordList);
                                }

                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            else if (parameters.Length == 2)
                            {
                                var wordList = Wordlist.LoadList(fileName);
                                Console.Write($"Write language: ");
                                string language = Console.ReadLine().ToLower();
                                RemoveWords(wordList, language);
                                SaveList(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            else
                            {
                                Console.Write($"Write the name of the list: ");
                                string name = Console.ReadLine();
                                var wordList = Wordlist.LoadList(name);
                                Console.Write($"Write language: ");
                                string language = Console.ReadLine().ToLower();
                                RemoveWords(wordList, language);
                                SaveList(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            break;
                        case "-word" or "word":
                        case "-words" or "words":
                            ShowCurrentLists();
                            if (parameters.Length > 1)
                            {
                                string language = string.Empty;
                                var wordList = Wordlist.LoadList(fileName);
                                if (parameters.Length == 2)
                                {
                                    Console.Write($"Language: ");
                                    language = Console.ReadLine().ToLower();
                                }
                                Console.WriteLine();
                                SortWordList(wordList, language == string.Empty ? parameters[2] : language);
                                Console.Write($"Type \"-options\" to display options: ");
                            }
                            else
                            {
                                Console.Write($"Write the name of the list: ");
                                string name = Console.ReadLine();
                                var wordList = Wordlist.LoadList(name);
                                Console.Write($"Language: ");
                                string language = Console.ReadLine().ToLower();
                                Console.WriteLine();
                                SortWordList(wordList, language);
                                Console.Write($"Type \"-options\" to display options: ");
                            }
                            break;
                        case "-count" or "count":
                            ShowCurrentLists();
                            if (parameters.Length > 1)
                            {
                                var wordList = Wordlist.LoadList(fileName);
                                Console.WriteLine($"There are {wordList.Count()} Words in the {wordList.Name} list");
                                Console.WriteLine(new string('-', 100));
                            }
                            else
                            {
                                Console.Write($"Write the name of the list: ");
                                string name = Console.ReadLine();
                                var wordList = Wordlist.LoadList(name);
                                Console.WriteLine($"\nThere are {wordList.Count()} Words in the {wordList.Name} list");
                                Console.WriteLine(new string('-', 100));
                            }
                            break;
                        case "-practice" or "practice":
                            ShowCurrentLists();
                            if (parameters.Length > 1)
                            {
                                var wordList = Wordlist.LoadList(fileName);
                                PracticeWords(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            else
                            {
                                Console.Write($"Write the name of the list: ");
                                string name = Console.ReadLine();
                                var wordList = Wordlist.LoadList(name);
                                PracticeWords(wordList);
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            break;
                        case "-option" or "option":
                        case "-options" or "options":
                            ShowOptions();
                            break;
                        case "-exit" or "exit":
                            flag = false;
                            break;
                        default:
                            Console.WriteLine($"Invalid input");
                            if (counter == 1)
                            {
                                Console.WriteLine(new string('-', 100));
                                ShowOptions();
                            }
                            break;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nInvalid option, please try again\n");
                    Console.ResetColor();
                    Console.WriteLine(new string('-', 100));
                    ShowOptions();
                }
                catch (ArgumentNullException ane)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + ane.Message);
                    Console.ResetColor();
                    Console.WriteLine(new string('-', 100));
                    ShowOptions();
                }
                catch (ArgumentException ae)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + ae.Message);
                    Console.ResetColor();
                    Console.WriteLine(new string('-', 100));
                    ShowOptions();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + e.Message);
                    Console.ResetColor();
                    Console.WriteLine(new string('-', 100));
                    ShowOptions();
                }
            }
        }
        private static void ShowOptions()
        {
            Console.WriteLine("Type any of the following options:");
            Console.WriteLine(new string('-', 100));
            Console.WriteLine("-lists");
            Console.WriteLine("-new <list name> <language 1> <language 2> .. <language n>");
            Console.WriteLine("-add <list name>");
            Console.WriteLine("-remove <list name> <language> <word 1> <word 2> .. <word n>");
            Console.WriteLine("-words <listname> <sortByLanguage>");
            Console.WriteLine("-count <listname>");
            Console.WriteLine("-practice <listname>");
            Console.WriteLine("-options");
            Console.WriteLine("-exit");
            Console.WriteLine(new string('-', 100));
        }
        private static void SaveList(Wordlist wordList)
        {
            bool flag = true;
            Console.Write($"Do you wish to save (y/n): ");
            while (flag)
            {
                string save = Console.ReadLine().ToLower();
                if (save == "y" || save == "yes")
                {
                    flag = false;
                    wordList.Save();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n{wordList.Name}.dat was saved sucessfully\n");
                    Console.ResetColor();
                }
                else if (save == "n" || save == "no")
                {
                    flag = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{wordList.Name}.dat was not saved\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write($"Invalid input. Please type \"yes\" or \"no\": ");
                }
            }
        }
        private static void AddWords(Wordlist wordList)
        {
            int counter = 0;
            bool condition = true;
            Console.WriteLine($"Press \"enter\" to cancel the prompt below\n");
            while (condition)
            {
                string[] translations = new string[wordList.Languages.Length];
                for (int i = 0; i < translations.Length; i++)
                {
                    Console.Write($"Add a word in {wordList.Languages[i]}: ");
                    translations[i] = Console.ReadLine();
                    if (translations[i] == "")
                    {
                        condition = false;
                        break;
                    }
                }
                if (translations[0] != "")
                {
                    wordList.Add(translations);
                    counter++;
                }
            }
            if (counter == 1)
            {
                Console.WriteLine($"\n{counter} Word added to the list");
            }
            else
            {
                Console.WriteLine($"\n{counter} Words added to the list");
            }
        }
        private static void RemoveWords(Wordlist wordList, params string[] args)
        {
            args = Array.ConvertAll(args, x => x.ToLower());

            for (int i = 0; i < wordList.Languages.Length; i++)
            {
                if (args[0] == wordList.Languages[i])
                {
                    if (args.Length > 1)
                    {
                        for (int j = 1; j < args.Length; j++)
                        {
                            bool removedOrNot = wordList.Remove(i, args[j]);
                            if (removedOrNot == true)
                            {
                                Console.Write($"\nThe word ");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write($"\"{args[j]}\"");
                                Console.ResetColor();
                                Console.WriteLine($" and its associated translations successfully removed");
                            }
                            else
                            {
                                Console.Write($"\nCould not find ");
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write($"\"{args[j]}\"");
                                Console.ResetColor();
                                Console.WriteLine($". Cannot remove word from list");
                            }
                        }
                        Console.WriteLine();
                    }

                    bool condition = true;
                    Console.WriteLine($"Press \"enter\" to cancel the prompt below");
                    while (condition)
                    {
                        Console.Write($"\nWrite the word in {args[0]} that you want to remove: ");
                        string wordToRemove = Console.ReadLine().ToLower();
                        if (wordToRemove == "")
                        {
                            condition = false;
                            break;
                        }
                        bool removedOrNot = wordList.Remove(i, wordToRemove);
                        if (removedOrNot == true)
                        {
                            Console.Write($"The word ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"\"{wordToRemove}\"");
                            Console.ResetColor();
                            Console.WriteLine($" and its associated translations successfully removed");
                        }
                        else
                        {
                            Console.Write($"\nCould not find ");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"\"{wordToRemove}\"");
                            Console.ResetColor();
                            Console.WriteLine($". Cannot remove word from list");
                        }
                    }
                    return;
                }
            }
            throw new ArgumentException($"\nCould not find language");
        }
        private static void SortWordList(Wordlist wordList, string language)
        {
            Console.WriteLine(string.Join(", ", wordList.Languages));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string('-', 100));
            Console.ResetColor();
            for (int i = 0; i < wordList.Languages.Length; i++)
            {
                if (language == wordList.Languages[i])
                {
                    Action<string[]> action = new Action<string[]>(ShowTranslations);
                    wordList.List(i, action);
                    Console.WriteLine(new string('-', 100));
                    return;
                }
            }
            throw new ArgumentException($"Could not find language");
        }
        private static void PracticeWords(Wordlist wordList)
        {
            bool condition = true;
            int numerator = 0, denominator = 0;
            Console.WriteLine($"Press \"enter\" to cancel the prompt below");
            while (condition)
            {
                var word = wordList.GetWordToPractice();
                string toLanguage = wordList.Languages[word.ToLanguage];
                string fromLanguage = word.Translations[word.FromLanguage];
                Console.Write($"\nTranslate \"{fromLanguage}\" to {toLanguage}: ");
                string translation = Console.ReadLine().ToLower();
                if (translation == "")
                {
                    condition = false;
                    break;
                }
                if (translation.Equals(word.Translations[word.ToLanguage]))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Correct");
                    Console.ResetColor();
                    numerator++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"Incorrect. ");
                    Console.ResetColor();
                    Console.Write($"The correct word was: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(word.Translations[word.ToLanguage]);
                    Console.ResetColor();
                }
                denominator++;
            }
            if (numerator != 0 && numerator == denominator)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\nCongratulations! ");
                Console.ResetColor();
                Console.WriteLine($"You got {numerator}/{denominator} correct");
            }
            else if (denominator != 0)
            {
                Console.WriteLine($"\nYou got {numerator}/{denominator} correct");
            }
            else
            {
                Console.WriteLine($"\nNo words were guessed");
            }
        }
        public static void ShowTranslations(string[] words)
        {
            Console.WriteLine(string.Join(", ", words));
        }

        public static void ShowCurrentLists() 
        {
            Console.WriteLine($"Available lists:\n");
            Console.WriteLine(string.Join(Environment.NewLine, Wordlist.GetLists()));
            Console.WriteLine();
            Console.WriteLine(new string('-', 100));
        }   
    }
}
