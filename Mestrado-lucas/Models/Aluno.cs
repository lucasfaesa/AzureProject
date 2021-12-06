using System;

namespace Mestrado_lucas
{
    public class Aluno
    {
        public int Id { get; set; }
        public string LoginNome { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        public DateTime DtCriacao { get; set; }
        public DateTime DtUltimoLogin { get; set; }
    }
}
