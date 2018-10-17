using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMPUCCompiler
{
    class Interpreter
    {
        Scanner scanner;
        Token tokenAtual;
        Dictionary<string, int> tabelaVariaveis;
        Stack pilhaExpressoes;
        int operando1, operando2, resultado;
        string nomeVariavel;

        public bool Status { get; set; }

        public Interpreter(string nomeArquivo)
        {
            tabelaVariaveis = new Dictionary<string, int>();
            pilhaExpressoes = new Stack();

            scanner = new Scanner(tabelaVariaveis);
            scanner.Entrada = Helpers.LerArquivo(nomeArquivo);
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
                tokenAtual = scanner.ProximoToken();
                ListaInstrucoes();
            }
            else if (tokenAtual.Tipo == TipoToken.ERRO)
            {
                return;
            }
            else
            {
                VerificarToken(TipoToken.FIM);
                tokenAtual = scanner.ProximoToken();
            }
        }

        private void Instrucao()
        {
            if (tokenAtual.Tipo == TipoToken.VARIAVEL)
            {
                VerificarToken(TipoToken.VARIAVEL);
                nomeVariavel = tokenAtual.Nome;
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.ATRIBUICAO);
                tokenAtual = scanner.ProximoToken();
                Expressao();

                int valorExpr = pilhaExpressoes.Pop().ToInt();

                if (tabelaVariaveis.ContainsKey(nomeVariavel))
                {
                    tabelaVariaveis[nomeVariavel] = valorExpr;
                }
                else
                {
                    tabelaVariaveis.Add(nomeVariavel, valorExpr);
                }

            }
            else if (tokenAtual.Tipo == TipoToken.ESCREVA)
            {
                VerificarToken(TipoToken.ESCREVA);
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.ABRE_PAREN);
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.VARIAVEL);
                nomeVariavel = tokenAtual.Nome;
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.FECHA_PAREN);
                tokenAtual = scanner.ProximoToken();

                Console.WriteLine(tabelaVariaveis.FirstOrDefault(variavel => variavel.Key == nomeVariavel).Value);

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
                pilhaExpressoes.Push(tabelaVariaveis.FirstOrDefault(variavel => variavel.Key == tokenAtual.Nome).Value);

                tokenAtual = scanner.ProximoToken();
            }
            else if (tokenAtual.Tipo == TipoToken.NUMERO)
            {
                VerificarToken(TipoToken.NUMERO);
                pilhaExpressoes.Push(tokenAtual.Valor);

                tokenAtual = scanner.ProximoToken();
            }
            else
            {
                VerificarToken(TipoToken.ABRE_PAREN);
                tokenAtual = scanner.ProximoToken();

                Expressao();

                VerificarToken(TipoToken.FECHA_PAREN);
                tokenAtual = scanner.ProximoToken();
            }
        }

        private void RestanteExpressao()
        {
            if (tokenAtual.Tipo == TipoToken.SOMA)
            {
                VerificarToken(TipoToken.SOMA);
                tokenAtual = scanner.ProximoToken();
                Termo();

                //Cálculo   
                operando2 = pilhaExpressoes.Pop().ToInt();
                operando1 = pilhaExpressoes.Pop().ToInt();
                resultado = operando1 + operando2;

                pilhaExpressoes.Push(resultado);

                RestanteExpressao();
            }
            else if (tokenAtual.Tipo == TipoToken.SUB)
            {
                VerificarToken(TipoToken.SUB);
                tokenAtual = scanner.ProximoToken();
                Termo();

                //Cálculo
                operando2 = pilhaExpressoes.Pop().ToInt();
                operando1 = pilhaExpressoes.Pop().ToInt();
                resultado = operando1 - operando2;

                pilhaExpressoes.Push(resultado);

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

        }
    }
}
