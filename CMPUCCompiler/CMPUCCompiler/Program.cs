using System;
using System.IO;
using System.Text;

namespace CMPUCCompiler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Interpreter parser = new Interpreter("teste.txt");
            parser.Analisar();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".data");

            sb.AppendLine(parser.AssemblyVariaveis.ToString());
            
            sb.AppendLine(".text");
            sb.AppendLine(".globl main");
            sb.AppendLine("main:");
            sb.AppendLine();

            sb.AppendLine(parser.AssemblyInstrucoes.ToString());

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