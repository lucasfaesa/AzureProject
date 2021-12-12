using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mestrado_lucas.Models
{
    public class SessaoFaseTotal
    {
        public int FaseId { get; set; }
        public int Concluida { get; set; }
        public int Tentativas { get; set; }
        public int Repetidas { get; set; }
        public int Falhas { get; set; }
        public int PontuacaoMax { get; set; }
        public DateTime? DtConclusao { get; set; }
        public float TempoDecorrido { get; set; }
        public int Plays { get; set; }
        public DateTime? DtUltimaJogada { get; set; }
    }
}
