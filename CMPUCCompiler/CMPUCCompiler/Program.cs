using System;
using System.IO;

namespace CMPUCCompiler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Interpreter parser = new Interpreter("main.txt");
            parser.Analisar();

            if (parser.Status)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nPrograma compilado com sucesso.\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nErro sintático.\n");
                Console.ForegroundColor = ConsoleColor.White;
            }

        }
    }
}