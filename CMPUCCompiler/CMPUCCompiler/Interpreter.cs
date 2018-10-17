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
        Dictionary<string, double> tabelaVariaveis;
        Stack pilhaExpressoes;
        double operando1, operando2, resultado;
        string nomeVariavel;

        public bool Status { get; set; }

        public Interpreter(string nomeArquivo)
        {
            tabelaVariaveis = new Dictionary<string, double>();
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
                || tokenAtual.Tipo == TipoToken.ESCREVA
                || tokenAtual.Tipo == TipoToken.LEIA)
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

                double valorExpr = Convert.ToDouble(pilhaExpressoes.Pop());

                AdicionarValorTabela(nomeVariavel, valorExpr);

            }
            else if (tokenAtual.Tipo == TipoToken.ESCREVA)
            {
                VerificarToken(TipoToken.ESCREVA);
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.ABRE_PAREN);
                tokenAtual = scanner.ProximoToken();

                string saida = "";
                ListaArgs();

                VerificarToken(TipoToken.FECHA_PAREN);
                tokenAtual = scanner.ProximoToken();


                void ListaArgs()
                {
                    Argumento();
                    RestoArgs();

                    Console.WriteLine(saida);
                }

                void RestoArgs()
                {
                    if (tokenAtual.Tipo == TipoToken.VIRGULA)
                    {
                        VerificarToken(TipoToken.VIRGULA);
                        tokenAtual = scanner.ProximoToken();

                        Argumento();
                        RestoArgs();
                    }
                    else;
                }

                void Argumento()
                {
                    if (tokenAtual.Tipo == TipoToken.VARIAVEL)
                    {
                        VerificarToken(TipoToken.VARIAVEL);
                        nomeVariavel = tokenAtual.Nome;
                        tokenAtual = scanner.ProximoToken();

                        saida += tabelaVariaveis.FirstOrDefault(variavel => variavel.Key == nomeVariavel).Value + " ";
                    }
                    else if (tokenAtual.Tipo == TipoToken.NUMERO)
                    {
                        VerificarToken(TipoToken.NUMERO);

                        saida += tokenAtual.Valor + " ";

                        tokenAtual = scanner.ProximoToken();

                    }
                    else if (tokenAtual.Tipo == TipoToken.STRING)
                    {
                        VerificarToken(TipoToken.STRING);

                        saida += tokenAtual.Nome + " ";

                        tokenAtual = scanner.ProximoToken();
                    }
                }

            }
            else if (tokenAtual.Tipo == TipoToken.LEIA)
            {
                VerificarToken(TipoToken.LEIA);
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.ABRE_PAREN);
                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.VARIAVEL);
                nomeVariavel = tokenAtual.Nome;
                tokenAtual.Valor = Convert.ToDouble(Console.ReadLine());

                AdicionarValorTabela(nomeVariavel, tokenAtual.Valor);

                tokenAtual = scanner.ProximoToken();

                VerificarToken(TipoToken.FECHA_PAREN);
                tokenAtual = scanner.ProximoToken();

            }
        }

        private void Expressao()
        {
            Termo();
            RestanteExpressao();
        }

        private void RestanteExpressao()
        {
            if (tokenAtual.Tipo == TipoToken.SOMA)
            {
                VerificarToken(TipoToken.SOMA);
                tokenAtual = scanner.ProximoToken();
                Termo();

                //Cálculo   
                operando2 = Convert.ToDouble(pilhaExpressoes.Pop());
                operando1 = Convert.ToDouble(pilhaExpressoes.Pop());
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
                operando2 = Convert.ToDouble(pilhaExpressoes.Pop());
                operando1 = Convert.ToDouble(pilhaExpressoes.Pop());
                resultado = operando1 - operando2;

                pilhaExpressoes.Push(resultado);

                RestanteExpressao();
            }
            else;

        }
        private void Termo()
        {
            Fator();
            RestoTermo();
        }

        private void Fator()
        {
            if (tokenAtual.Tipo == TipoToken.VARIAVEL)
            {
                VerificarToken(TipoToken.VARIAVEL);
                pilhaExpressoes.Push(tabelaVariaveis.FirstOrDefault(variavel => variavel.Key == tokenAtual.Nome).Value);

                tokenAtual = scanner.ProximoToken();

                Potencia();
            }
            else if (tokenAtual.Tipo == TipoToken.NUMERO)
            {
                VerificarToken(TipoToken.NUMERO);
                pilhaExpressoes.Push(tokenAtual.Valor);

                tokenAtual = scanner.ProximoToken();

                Potencia();
            }
            else
            {
                VerificarToken(TipoToken.ABRE_PAREN);
                tokenAtual = scanner.ProximoToken();

                Expressao();

                VerificarToken(TipoToken.FECHA_PAREN);
                tokenAtual = scanner.ProximoToken();

                Potencia();
            }
        }

        private void RestoTermo()
        {
            if (tokenAtual.Tipo == TipoToken.MULT)
            {
                VerificarToken(TipoToken.MULT);
                tokenAtual = scanner.ProximoToken();
                Fator();

                operando2 = Convert.ToDouble(pilhaExpressoes.Pop());
                operando1 = Convert.ToDouble(pilhaExpressoes.Pop());
                resultado = operando1 * operando2;

                pilhaExpressoes.Push(resultado);

                RestoTermo();
            }
            else if (tokenAtual.Tipo == TipoToken.DIV)
            {
                VerificarToken(TipoToken.DIV);
                tokenAtual = scanner.ProximoToken();
                Fator();

                operando2 = Convert.ToDouble(pilhaExpressoes.Pop());

                if (operando2 == 0)
                {
                    Console.WriteLine("Divisão por 0 inválida.");
                    tokenAtual = new Token(TipoToken.ERRO);
                    ListaInstrucoes();
                }

                operando1 = Convert.ToDouble(pilhaExpressoes.Pop());
                resultado = operando1 / operando2;

                pilhaExpressoes.Push(resultado);

                RestoTermo();
            }
            else if (tokenAtual.Tipo == TipoToken.RESTO)
            {
                VerificarToken(TipoToken.RESTO);
                tokenAtual = scanner.ProximoToken();
                Fator();

                operando2 = Convert.ToDouble(pilhaExpressoes.Pop());

                if (operando2 == 0)
                {
                    Console.WriteLine("Divisão por 0 inválida.");
                    tokenAtual = new Token(TipoToken.ERRO);
                    ListaInstrucoes();
                }

                operando1 = Convert.ToDouble(pilhaExpressoes.Pop());
                resultado = operando1 % operando2;

                pilhaExpressoes.Push(resultado);

                RestoTermo();
            }
            else;
        }

        private void Potencia()
        {
            if (tokenAtual.Tipo == TipoToken.EXP)
            {
                VerificarToken(TipoToken.EXP);
                tokenAtual = scanner.ProximoToken();

                Expressao();

                operando2 = Convert.ToDouble(pilhaExpressoes.Pop());
                operando1 = Convert.ToDouble(pilhaExpressoes.Pop());
                resultado = Math.Pow(operando1, operando2);

                pilhaExpressoes.Push(resultado);
            }
            
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
            }

        }

        private void AdicionarValorTabela(string nomeVariavel, double valor)
        {
            if (tabelaVariaveis.ContainsKey(nomeVariavel))
            {
                tabelaVariaveis[nomeVariavel] = valor;
            }
            else
            {
                tabelaVariaveis.Add(nomeVariavel, valor);
            }
        }
    }
}
