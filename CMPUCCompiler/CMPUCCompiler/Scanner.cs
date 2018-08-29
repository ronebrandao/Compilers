using System;
using static System.Char;

namespace CMPUCCompiler
{
    public class Scanner
    {
        public string Entrada { get; set; }
        private int posicao;


        public Token ProximoToken()
        {
            Token NovoToken = null;

            if (posicao == Entrada.Length)
            {
                NovoToken = new Token(TipoToken.EOF);
                return NovoToken;
            }

            while (Entrada[posicao] == ' ' ||
                   Entrada[posicao] == '\t' ||
                   Entrada[posicao] == '\n')
                posicao++;


            if (IsLetter(Entrada[posicao]))
            {
                string lexema = Entrada[posicao].ToString();
                posicao++;

                while (posicao < Entrada.Length && (IsLetter(Entrada[posicao]) || IsNumber(Entrada[posicao])))
                {
                    lexema += Entrada[posicao];
                    posicao++;
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
            else if (IsDigit(Entrada[posicao]))
            {
                string lexema = Entrada[posicao].ToString();
                posicao++;

                while (posicao < Entrada.Length && IsDigit(Entrada[posicao]))
                {
                    lexema += Entrada[posicao];
                    posicao++;
                }

                try
                {
                    NovoToken = new Token(TipoToken.NUMERO, Double.Parse(lexema));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Número Inválido");
                    NovoToken = new Token(TipoToken.ERRO);
                }
            }
            else if (Entrada[posicao] == '=')
            {
                posicao++;
                NovoToken = new Token(TipoToken.ATRIBUICAO);
            }
            else if (Entrada[posicao] == '+')
            {
                posicao++;
                NovoToken = new Token(TipoToken.SOMA);
            }
            else if (Entrada[posicao] == '-')
            {
                posicao++;
                NovoToken = new Token(TipoToken.SUB);
            }
            else if (Entrada[posicao] == '(')
            {
                posicao++;
                NovoToken = new Token(TipoToken.ABRE_PAREN);
            }
            else if (Entrada[posicao] == ')')
            {
                posicao++;
                NovoToken = new Token(TipoToken.FECHA_PAREN);
            }
            else if (Entrada[posicao] == ';')
            {
                posicao++;
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