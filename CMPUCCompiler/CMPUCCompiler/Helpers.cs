using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMPUCCompiler
{
    public static class Helpers
    {

        public static string LerArquivo(string nomeArquivo)
        {
            string entrada = "";

            try
            {
                using (StreamReader sr = new StreamReader(nomeArquivo))
                {
                    entrada = sr.ReadToEnd();
                    Console.WriteLine(entrada + "\n");
                    Console.WriteLine(" -------- Resultado -------- \n");
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Não foi possível ler o arquivo.");
                Console.WriteLine(e.Message);
            }

            return entrada;
        }

        public static int ToInt(this object num)
        {
            return Convert.ToInt32(num);
        }

    }
}
