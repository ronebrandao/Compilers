using System;
using static System.Char;

namespace CMPUCCompiler
{
    public class Scanner
    {
        public string Entrada { get; set; }
        public int Posicao { get; set; }


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
                    NovoToken = new Token(TipoToken.VARIAVEL, lexema, null);
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

                while (Posicao < Entrada.Length && IsDigit(Entrada[Posicao]))
                {
                    lexema += Entrada[Posicao];
                    Posicao++;
                }

                try
                {
                    NovoToken = new Token(TipoToken.NUMERO, null, Double.Parse(lexema));
                }
                catch (Exception e)
                {
                    //TODO Considerar FormatException
                    Console.WriteLine("Número Inválido");
                    NovoToken = new Token(TipoToken.ERRO);
                }
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
            return lexema == "fim" || lexema == "escreva";
        }

        Token ObterTokenPalavraReservada(string lexema)
        {
            switch (lexema)
            {
                case "escreva":
                    return new Token(TipoToken.ESCREVA);
                case "fim":
                    return new Token(TipoToken.FIM);
                   
                default:
                    return null;
            }
        }
    }
}