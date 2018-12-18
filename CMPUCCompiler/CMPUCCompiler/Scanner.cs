using System;
using System.Collections.Generic;
using static System.Char;

namespace CMPUCCompiler
{
    public class Scanner
    {
        public string Entrada { get; set; }
        public int Posicao { get; set; }

        Dictionary<string, dynamic> tabelaVariaveis;

        public Scanner()
        {

        }

        public Scanner(Dictionary<string, dynamic> tab)
        {
            tabelaVariaveis = tab;
        }

        public Token ProximoToken()
        {
            Token NovoToken = null;

            if (Posicao == Entrada.Length)
            {
                NovoToken = new Token(TipoToken.EOF);
                return NovoToken;
            }

            while (Entrada[Posicao] == ' '
                   || Entrada[Posicao] == '\r'
                   || Entrada[Posicao] == '\t'
                   || Entrada[Posicao] == '\n')
            {
                Posicao++;
            }

            if (IsLetter(Entrada[Posicao]))
            {
                string lexema = Entrada[Posicao].ToString();
                Posicao++;

                while (Posicao < Entrada.Length && (IsLetter(Entrada[Posicao]) || IsNumber(Entrada[Posicao])))
                {
                    lexema += Entrada[Posicao];
                    Posicao++;
                }

                if (!IsPalavraReservada(lexema))
                {
                    NovoToken = new Token(TipoToken.VARIAVEL, lexema);
                }
                else
                {
                    NovoToken = ObterTokenPalavraReservada(lexema);
                }
            }
            else if (IsDigit(Entrada[Posicao]))
            {
                string lexema = Entrada[Posicao].ToString();
                Posicao++;

                bool isDecimal = false;

                while (Posicao < Entrada.Length && (IsDigit(Entrada[Posicao]) || (Entrada[Posicao] == '.' && !isDecimal)))
                {
                    if (Entrada[Posicao] == '.')
                        isDecimal = true;

                    lexema += Entrada[Posicao];
                    Posicao++;
                }

                try
                {
                    NovoToken = new Token(TipoToken.NUMERO, null, lexema.ToDouble());
                }
                catch (Exception ex)
                {
                    //TODO Considerar FormatException
                    Console.WriteLine("Número Inválido");
                    NovoToken = new Token(TipoToken.ERRO);
                }
            }
            else if (Entrada[Posicao] == '"')
            {
                string lexema = Entrada[Posicao].ToString();
                Posicao++;

                while (Posicao < Entrada.Length && (IsDigit(Entrada[Posicao]) || Entrada[Posicao] != '"'))
                {
                    lexema += Entrada[Posicao];
                    Posicao++;
                }

                if (Entrada[Posicao] == '"')
                {
                    lexema += Entrada[Posicao];
                    Posicao++;
                }

                NovoToken = new Token(TipoToken.STRING, lexema, "");

            }
            else if (Entrada[Posicao] == ',')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.VIRGULA);
            }
            else if (Entrada[Posicao] == '=')
            {
                Posicao++;

                if (Entrada[Posicao] == '=')
                {
                    NovoToken = new Token(TipoToken.IGUAL);
                    Posicao++;
                }
                else
                {
                    NovoToken = new Token(TipoToken.ATRIBUICAO);
                }
            }

            #region Operadores Aritméticos

            else if (Entrada[Posicao] == '+')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.SOMA);
            }
            else if (Entrada[Posicao] == '*')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.MULT);
            }
            else if (Entrada[Posicao] == '/')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.DIV);
            }
            else if (Entrada[Posicao] == '%')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.RESTO);
            }
            else if (Entrada[Posicao] == '^')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.EXP);
            }
            else if (Entrada[Posicao] == '-')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.SUB);
            }

            #endregion

            #region Operadores de Comparação

            else if (Entrada[Posicao] == '<')
            {
                Posicao++;

                if (Entrada[Posicao] == '=')
                {
                    NovoToken = new Token(TipoToken.MENOR_IGUAL);
                    Posicao++;
                }
                else
                {
                    NovoToken = new Token(TipoToken.MENOR);
                }
            }

            else if (Entrada[Posicao] == '>')
            {
                Posicao++;

                if (Entrada[Posicao] == '=')
                {
                    NovoToken = new Token(TipoToken.MAIOR_IGUAL);
                    Posicao++;
                }
                else
                {
                    NovoToken = new Token(TipoToken.MAIOR);
                }
            }

            else if (Entrada[Posicao] == '!')
            {
                Posicao++;

                if (Entrada[Posicao] == '=')
                {
                    NovoToken = new Token(TipoToken.DIFERENTE);
                    Posicao++;
                }

            }

            #endregion

            else if (Entrada[Posicao] == '{')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.ABRE_CHAVE);
            }
            else if (Entrada[Posicao] == '}')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.FECHA_CHAVE);
            }
            else if (Entrada[Posicao] == '(')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.ABRE_PAREN);
            }
            else if (Entrada[Posicao] == ')')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.FECHA_PAREN);
            }
            else if (Entrada[Posicao] == ';')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.PV);
            }
            else
            {
                NovoToken = new Token(TipoToken.ERRO);
            }

            return NovoToken;
        }

        bool IsPalavraReservada(string lexema)
        {
            return lexema == "fim" 
                || lexema == "escreva" || lexema == "leia"
                || lexema == "se" || lexema == "senao" || lexema == "enquanto"
                || lexema == "true" || lexema == "false";
        }

        Token ObterTokenPalavraReservada(string lexema)
        {
            switch (lexema)
            {
                case "escreva":
                    return new Token(TipoToken.ESCREVA);
                case "leia":
                    return new Token(TipoToken.LEIA);
                case "fim":
                    return new Token(TipoToken.FIM);
                case "se":
                    return new Token(TipoToken.SE);
                case "senao":

                    if (Entrada[Posicao] == '_') {
                        lexema += Entrada[Posicao];
                        Posicao++;

                        while (IsLetter(Entrada[Posicao]))
                        {
                            lexema += Entrada[Posicao];
                            Posicao++;
                        }
                        if (lexema == "senao_se")
                            return new Token(TipoToken.SENAO_SE);
                        else
                            return new Token(TipoToken.ERRO);

                    }

                    return new Token(TipoToken.SENAO);
                case "enquanto":
                    return new Token(TipoToken.ENQUANTO);
                case "true":
                    return new Token(TipoToken.BOOLEAN, "", true);
                case "false":
                    return new Token(TipoToken.BOOLEAN, "", false);

                default:
                    return null;
            }
        }
    }
}