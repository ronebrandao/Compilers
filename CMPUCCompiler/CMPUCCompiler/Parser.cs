using System;
using System.IO;

namespace CMPUCCompiler
{
    public class Parser
    {
        Scanner scanner;
        Token tokenAtual;
        public bool Status { get; set; }

        public Parser(string nomeArquivo)
        {
            scanner = new Scanner();
            scanner.Entrada = LerArquivo(nomeArquivo);
        }

        private string LerArquivo(string nomeArquivo)
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

        public void Analisar()
        {
            tokenAtual = scanner.ProximoToken();

            ListaInstrucoes();
        }

        private void ListaInstrucoes()
        {
            if (tokenAtual.Tipo == TipoToken.VARIAVEL
                || tokenAtual.Tipo == TipoToken.ESCREVA)
            {
                Instrucao();
                VerificarToken(TipoToken.PV);
                ListaInstrucoes();
            }
            else if (tokenAtual.Tipo == TipoToken.ERRO)
            {
                return;
            }
            else
            {
                VerificarToken(TipoToken.FIM);
            }
        }

        private void Instrucao()
        {
            if (tokenAtual.Tipo == TipoToken.VARIAVEL)
            {
                VerificarToken(TipoToken.VARIAVEL);
                VerificarToken(TipoToken.ATRIBUICAO);
                Expressao();
            }
            else if (tokenAtual.Tipo == TipoToken.ESCREVA)
            {
                VerificarToken(TipoToken.ESCREVA);
                VerificarToken(TipoToken.ABRE_PAREN);
                VerificarToken(TipoToken.VARIAVEL);
                VerificarToken(TipoToken.FECHA_PAREN);
            }
        }

        private void Expressao()
        {
            Termo();
            RestanteExpressao();
        }
        private void Termo()
        {
            if (tokenAtual.Tipo == TipoToken.VARIAVEL)
            {
                VerificarToken(TipoToken.VARIAVEL);
            }
            else if (tokenAtual.Tipo == TipoToken.NUMERO)
            {
                VerificarToken(TipoToken.NUMERO);
            }
            else
            {
                VerificarToken(TipoToken.ABRE_PAREN);
                Expressao();
                VerificarToken(TipoToken.FECHA_PAREN);
            }
        }

        private void RestanteExpressao()
        {
            if (tokenAtual.Tipo == TipoToken.SOMA)
            {
                VerificarToken(TipoToken.SOMA);
                Termo();
                RestanteExpressao();
            }
            else if (tokenAtual.Tipo == TipoToken.SUB)
            {
                VerificarToken(TipoToken.SUB);
                Termo();
                RestanteExpressao();
            }
            else;

        }

        private void VerificarToken(TipoToken tipo)
        {
            if (tokenAtual.Tipo == tipo)
            {
                Status = true;
            }
            else
            {
                Status = false;
                Console.WriteLine($"Token {tipo} esperado. Erro na posição {scanner.Posicao}");
                tokenAtual = new Token(TipoToken.ERRO);
                return;
            }

            tokenAtual = scanner.ProximoToken();
        }

    }

}
