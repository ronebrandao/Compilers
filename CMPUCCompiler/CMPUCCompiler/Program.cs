using System;

namespace CMPUCCompiler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Scanner scanner = new Scanner();
            scanner.Entrada = Console.ReadLine();

            Token token = scanner.ProximoToken();

            while (token.Tipo != TipoToken.EOF)
            {
                Console.WriteLine(token.ToString());
                token = scanner.ProximoToken();
            }
        }
    }
}