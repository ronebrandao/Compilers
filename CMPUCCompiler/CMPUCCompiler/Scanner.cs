using System;
using System.Collections.Generic;
using static System.Char;

namespace CMPUCCompiler
{
    public class Scanner
    {
        public string Entrada { get; set; }
        public int Posicao { get; set; }

        Dictionary<string, double> tabelaVariaveis;

        public Scanner()
        {

        }

        public Scanner(Dictionary<string, double> tab)
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

                NovoToken = new Token(TipoToken.STRING, lexema);

            }
            else if (Entrada[Posicao] == ',')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.VIRGULA);
            }
            else if (Entrada[Posicao] == '=')
            {
                Posicao++;
                NovoToken = new Token(TipoToken.ATRIBUICAO);
            }
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
            return lexema == "fim" || lexema == "escreva" || lexema == "leia";
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

                default:
                    return null;
            }
        }
    }
}