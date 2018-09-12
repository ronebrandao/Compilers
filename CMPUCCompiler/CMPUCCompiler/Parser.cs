using CMPUCCompiler.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMPUCCompiler
{
    public class Parser
    {
        Scanner scanner;
        Token tokenAtual;

        public Parser(string entrada)
        {
            scanner = new Scanner();
            scanner.Entrada = entrada;
            ListaInstrucoes();
        }

        public void ListaInstrucoes()
        {
            tokenAtual = scanner.ProximoToken();

            if (tokenAtual.Tipo != TipoToken.FIM)
            {
                Instrucao(tokenAtual.Tipo);
                VerificarTokenAtual(TipoToken.PV);
                ListaInstrucoes();
            }
            else
            {
                Console.WriteLine("Programa executado com sucesso!");

            }

        }

        public void Instrucao(TipoToken tipo)
        {
            if (tipo == TipoToken.VARIAVEL)
            {
                VerificarProximoToken(TipoToken.ATRIBUICAO);
                Expressao();
            }
            else if (tipo == TipoToken.ESCREVA)
            {
                VerificarProximoToken(TipoToken.VARIAVEL);
            }
            else if (tipo == TipoToken.EOF)
            {
                throw new ProgramNotFinishedExcepetion("A cláusula 'fim' não foi encontrada");
            }
            else
            {
                throw new InstructionBeginningException("Início de instrução inválido.");
            }

        }

        public void Expressao()
        {
            Termo();
            RestanteExpressao();
        }
        public void Termo()
        {
            tokenAtual = scanner.ProximoToken();

            if (tokenAtual.Tipo == TipoToken.ABRE_PAREN)
            {
                Termo();
                RestanteExpressao();
                VerificarTokenAtual(TipoToken.FECHA_PAREN);
            }

            else if (tokenAtual.Tipo != TipoToken.VARIAVEL && tokenAtual.Tipo != TipoToken.NUMERO)
            {
                throw new TokenException("Somente variváveis e números são válidos");
            }

        }

        public void RestanteExpressao()
        {
            tokenAtual = scanner.ProximoToken();

            if (tokenAtual.Tipo == TipoToken.SOMA)
            {
                Termo();
                RestanteExpressao();
            }
            else if (tokenAtual.Tipo == TipoToken.SUB)
            {
                Termo();
                RestanteExpressao();
            }
            else;

        }

        public void VerificarProximoToken(TipoToken tipo)
        {
            Token tk = scanner.ProximoToken();

            if (tk.Tipo != tipo)
                throw new TokenException($"Token {tipo} esperado.");

        }

        public void VerificarTokenAtual(TipoToken tipo)
        {
            if (tokenAtual.Tipo != tipo)
                throw new TokenException();
        }

    }

}
