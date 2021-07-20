using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] words = { "The", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog." };

            var unreadablePhrase = string.Concat(words);
            System.Console.WriteLine(unreadablePhrase);

            var readablePhrase = string.Join(" ", words);
            System.Console.WriteLine(readablePhrase);
        }
    }
}
