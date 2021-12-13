using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mestrado_lucas.Models
{
    public class SessaoAlunoTotal
    {
        public int AlunoId { get; set; }
        public virtual Aluno Aluno { get; set; }
        public int MaxPontos { get; set; }
        public int TotalPontos { get; set; }
        public int FaseMaxPontosId { get; set; }
        public int MaxRepeticoes { get; set; }
        public int TotalRepeticoes { get; set; }
        public int FaseMaxRepeticoesId { get; set; }
        public int MaxTentativas { get; set; }
        public int TotalTentativas { get; set; }
        public int FaseMaxTentativasId { get; set; }
        public int MaxFalhas { get; set; }
        public int TotalFalhas { get; set; }
        public int FaseMaxFalhasId { get; set; }
        public float MaxTempoDecorrido { get; set; }
        public float TotalTempoDecorrido { get; set; }
        public int FaseMaxTempoDecorridoId { get; set; }
        public DateTime? MinDtConclusao { get; set; }
        public DateTime? MaxDtConclusao { get; set; }
        public int FasesConcluidasQuantidade { get; set; }
        public int FasesPendentesQuantidade { get; set; }
        public int MaxPlays { get; set; }
        public int TotalPlays { get; set; }
        public int FaseMaxPlays { get; set; }
        public DateTime MaxDtJogada { get; set; }
        public DateTime DtUltimoLogin { get; set; }
    }
}
