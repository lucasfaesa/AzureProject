using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mestrado_lucas.Models
{
    public class Sessao
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public virtual Aluno Aluno { get;  set; }
        public int FaseId { get; set; }
        public virtual Fase Fase { get; set; }
        public int Concluida { get; set; } // 1 para sim e 0 para não, vai subir quando abrir a fase
        public int Pontuacao { get; set; } //vai subir quando abrir a fase, com pontuação 0
        public DateTime? DtConclusao { get; set; }
        public float TempoDecorrido { get; set; } //vai subir quando abrir a fase como 0
        public DateTime DtJogada { get; set; } //vai subir quando abrir a fase
    }
}
