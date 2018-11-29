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
        MULT,
        DIV,
        RESTO,
        EXP,
        ESCREVA,
        LEIA,
        VIRGULA,
        STRING,
        FIM,
        ERRO,
        EOF
    }

    public class Token
    {
        public TipoToken Tipo { get; }
        public double Valor { get; set; }
        public string ValorString { get; set; }
        public string Nome { get; }

        public Token(TipoToken tipo)
        {
            Tipo = tipo;
        }

        public Token(TipoToken tipo, string nome)
        {
            Tipo = tipo;
            Nome = nome;
        }

        public Token(TipoToken tipo, string nome, double valor)
        {
            Tipo = tipo;
            Nome = nome;
            Valor = valor;
        }

        public Token(TipoToken tipo, string nome, string valor)
        {
            Tipo = tipo;
            Nome = nome;
            ValorString = valor;
        }

        public override string ToString() => $"Tipo: {Tipo.ToString()} | Nome: {Nome ?? "null"} | Valor: {(Valor.ToString()?.Length == 0 ? "null" : Valor.ToString())}";
    }
}