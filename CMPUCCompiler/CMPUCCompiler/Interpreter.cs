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
        Dictionary<string, dynamic> tabelaVariaveis;
        Stack pilhaExpressoes;

        double operando1, operando2, resultado;
        string nomeVariavel;

        public StringBuilder AssemblyVariaveis { get; set; }
        public StringBuilder AssemblyInstrucoes { get; set; }
        public bool Status { get; set; }

        public Interpreter(string nomeArquivo)
        {
            tabelaVariaveis = new Dictionary<string, dynamic>();
            pilhaExpressoes = new Stack();

            scanner = new Scanner(tabelaVariaveis);
            scanner.Entrada = Helpers.LerArquivo(nomeArquivo);
        }

        public void Analisar()
        {
            AssemblyVariaveis = new StringBuilder();
            AssemblyInstrucoes = new StringBuilder();

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

                #region Declaração de Variáveis

                AssemblyVariaveis.Append(nomeVariavel);
                AssemblyVariaveis.AppendLine(": .word 0");

                #endregion

                VerificarToken(TipoToken.ATRIBUICAO);
                tokenAtual = scanner.ProximoToken();
                Expressao();

                double valorExpr = Convert.ToDouble(pilhaExpressoes.Pop());

                AdicionarValorTabela(nomeVariavel, valorExpr);

                #region Desempilhamento e Atribuição
                AssemblyInstrucoes.AppendLine("#DESEMPILHANDO E FAZENDO ATRIBUICAO");
                AssemblyInstrucoes.AppendLine("lw $t4, ($sp)");
                AssemblyInstrucoes.AppendLine($"sw $t4, {nomeVariavel}");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine();

                #endregion

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

                        #region Imprimir Variavel

                        AssemblyInstrucoes.AppendLine("#IMPRIMIR VARIAEL INTERIA");
                        AssemblyInstrucoes.AppendLine($"lw $a0, {nomeVariavel}");
                        AssemblyInstrucoes.AppendLine("li $v0, 1");
                        AssemblyInstrucoes.AppendLine("syscall");

                        #endregion

                    }
                    else if (tokenAtual.Tipo == TipoToken.NUMERO)
                    {
                        VerificarToken(TipoToken.NUMERO);

                        saida += tokenAtual.Valor + " ";

                        #region Imprimir Float
                        string NomeVar = "txt" + AssemblyVariaveis.Length;
                        AssemblyVariaveis.AppendLine($"{NomeVar}: .float {tokenAtual.Valor.ToString().Replace(",",".")}");


                        AssemblyInstrucoes.AppendLine("#IMPRIMIR FLOAT");

                        AssemblyInstrucoes.AppendLine("li $v0, 2");
                        AssemblyInstrucoes.AppendLine($"l.s $f12, {NomeVar}");
                        AssemblyInstrucoes.AppendLine("syscall");

                        #endregion

                        tokenAtual = scanner.ProximoToken();

                    }
                    else if (tokenAtual.Tipo == TipoToken.STRING)
                    {
                        VerificarToken(TipoToken.STRING);
                        string str = tokenAtual.Nome.Replace("\"", "");
                        saida += str + " ";

                        #region Imprimir String
                        
                        string NomeVar = "txt" + AssemblyVariaveis.Length;
                        AssemblyVariaveis.AppendLine($"{NomeVar}: .asciiz \"{str}\"");

                        AssemblyInstrucoes.AppendLine("#IMPRIMIR STRING");
                        AssemblyInstrucoes.AppendLine("li $v0, 4");
                        AssemblyInstrucoes.AppendLine($"la $a0, {NomeVar}");
                        AssemblyInstrucoes.AppendLine("syscall");

                        #endregion

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

                string input = Console.ReadLine();

                if (!tabelaVariaveis.ContainsKey(tokenAtual.Nome))
                {
                    AssemblyVariaveis.AppendLine($"{tokenAtual.Nome}: .word 0");
                }

                if (input.IsNumber())
                {
                    tokenAtual.Valor = Convert.ToDouble(input);

                    AdicionarValorTabela(nomeVariavel, tokenAtual.Valor);
                }
                else
                {
                    tokenAtual.ValorString = input;
                    AdicionarValorTabela(nomeVariavel, tokenAtual.ValorString);
                }

                #region Ler Entrada

                AssemblyInstrucoes.AppendLine("li $v0, 5");
                AssemblyInstrucoes.AppendLine("syscall");

                AssemblyInstrucoes.AppendLine($"sw $v0, {nomeVariavel}");

                #endregion

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

                #region Soma e Empilhamento
                AssemblyInstrucoes.AppendLine("#DESEMPILHANDO E SOMANDO");
                AssemblyInstrucoes.AppendLine("lw $t1, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("lw $t0, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("add $t2, $t0, $t1");

                AssemblyInstrucoes.AppendLine("#EMPILHANDO O RESULTADO");
                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t2, ($sp)");
                AssemblyInstrucoes.AppendLine();

                #endregion

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

                #region Subtração e Empilhamento

                AssemblyInstrucoes.AppendLine("lw $t1, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("lw $t0, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("sub $t2, $t0, $t1");

                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t2, ($sp)");
                AssemblyInstrucoes.AppendLine();

                #endregion

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
                string nomeVar = tokenAtual.Nome;

                tokenAtual = scanner.ProximoToken();

                AssemblyInstrucoes.AppendLine("#ARMAZENANDO A VARIAVEL BASE NA PILHA");
                AssemblyInstrucoes.AppendLine($"lw $t5, {nomeVar}");
                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t5, ($sp)");
                AssemblyInstrucoes.AppendLine();

                Potencia();
            }
            else if (tokenAtual.Tipo == TipoToken.NUMERO)
            {
                VerificarToken(TipoToken.NUMERO);
                pilhaExpressoes.Push(tokenAtual.Valor);
                double valor = tokenAtual.Valor;

                tokenAtual = scanner.ProximoToken();

                AssemblyInstrucoes.AppendLine("#ARMAZENANDO O NUMERO NA PILHA");
                AssemblyInstrucoes.AppendLine($"li $t5, {valor.ToString()}");
                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t5, ($sp)");
                AssemblyInstrucoes.AppendLine();

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

                #region Multiplicação e Empilhamento

                AssemblyInstrucoes.AppendLine("lw $t1, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("lw $t0, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("mul $t2, $t0, $t1");

                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t2, ($sp)");

                #endregion

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

                #region Divisão e Empilhamento

                AssemblyInstrucoes.AppendLine("lw $t1, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("lw $t0, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("div $t2, $t0, $t1");

                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t2, ($sp)");

                #endregion

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

                #region Resto e Empilhamento

                AssemblyInstrucoes.AppendLine("lw $t1, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("lw $t0, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("rem $t2, $t0, $t1");

                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t2, ($sp)");

                #endregion

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

                #region Potencia 

                AssemblyInstrucoes.AppendLine("lw $t1, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");

                AssemblyInstrucoes.AppendLine("lw $t0, ($sp)");
                AssemblyInstrucoes.AppendLine("addu $sp, $sp, 4");
                
                AssemblyInstrucoes.AppendLine("li $t2, 1");
                AssemblyInstrucoes.AppendLine("li $t5, 1");

                AssemblyInstrucoes.AppendLine("for:");
                AssemblyInstrucoes.AppendLine("beq $t1, $zero, fim");
                AssemblyInstrucoes.AppendLine("mul $t2, $t2, $t0");
                AssemblyInstrucoes.AppendLine("sub $t1, $t1, $t5");

                AssemblyInstrucoes.AppendLine("j for");

                AssemblyInstrucoes.AppendLine("fim:");
                AssemblyInstrucoes.AppendLine("subu $sp, $sp, 4");
                AssemblyInstrucoes.AppendLine("sw $t2, ($sp)");

                #endregion

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

        private void AdicionarValorTabela(string nomeVariavel, dynamic valor)
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
