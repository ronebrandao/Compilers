﻿using System;
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
                Console.WriteLine("Programa compilado com sucesso.\n");
            }
            else
            {
                Console.WriteLine("Erro sintático.\n");
            }

        }
    }
}