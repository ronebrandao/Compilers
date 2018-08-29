namespace CMPUCCompiler
{
    public enum TipoToken
    {
        VARIAVEL,
        NUMERO,
        PV, 
        ABRE_PAREN, 
        FECHA_PAREN, 
        ATRIBUICAO, 
        SOMA, 
        SUB, 
        ESCREVA,
        FIM,
        ERRO,
        EOF
    }

    public class Token
    {
        public TipoToken Tipo { get; }
        public dynamic Valor { get; }

        public Token(TipoToken tipo)
        {
            Tipo = tipo;
        }
        
        public Token(TipoToken tipo, string valor)
        {
            Tipo = tipo;
            Valor = valor;
        }
        
        
        public Token(TipoToken tipo, double valor)
        {
            Tipo = tipo;
            Valor = valor;
        }

        public override string ToString()
        {
            return Valor != null ? $"Tipo: {Tipo.ToString()} e Valor: {Valor.ToString()}" : $"Tipo: {Tipo.ToString()}";
        }
    }
}