using System;
using System.IO;
using System.Text;

namespace CMPUCCompiler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".data");
            sb.AppendLine(".text");
            sb.AppendLine(".global main");
            sb.AppendLine("main:");

            Interpreter parser = new Interpreter("teste.txt");
            parser.Analisar(sb);

            sb.AppendLine("li $v0, 10");
            sb.AppendLine("syscall");
            sb.AppendLine(".end main");

            Helpers.CriarEEscreverArquivo(sb.ToString());

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