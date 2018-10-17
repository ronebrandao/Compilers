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
        public int Valor { get; }
        public string Nome { get; }

        public Token(TipoToken tipo)
        {
            Tipo = tipo;
        }

        public Token (TipoToken tipo, string nome)
        {
            Tipo = tipo;
            Nome = nome;
        }
        
        public Token(TipoToken tipo, string nome, int valor)
        {
            Tipo = tipo;
            Nome = nome;
            Valor = valor;
        }
        
        public override string ToString() => $"Tipo: {Tipo.ToString()} | Nome: {Nome ?? "null"} | Valor: {(Valor.ToString()?.Length == 0 ? "null" : Valor.ToString())}";
    }
}