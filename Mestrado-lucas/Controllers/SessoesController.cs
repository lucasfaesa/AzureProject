using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mestrado_lucas.Data;
using Mestrado_lucas.Models;

namespace Mestrado_lucas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessoesController : ControllerBase
    {
        private readonly Mestrado_lucasContext _context;

        public SessoesController(Mestrado_lucasContext context)
        {
            _context = context;
        }

        [HttpGet("babish")]
        public async Task<ActionResult<Aluno>> Fill()
        {
            int[] alunoId = { 1,1,1,1, 2,2, 3,3,3,3, 4,4,4 };
            int[] faseId = {  1,1,2,3, 1,2, 1,2,3,4, 1,3,3 };
            int[] concluida ={1,1,0,1, 1,0, 1,0,0,1, 1,1,1 };
            int[] pontuacao ={100,135,0,140, 200,0, 145,0,0,146, 200,250,200 };
            DateTime?[] DtConclusao = { new DateTime(2021, 12, 11), new DateTime(2021, 12, 9), null, new DateTime(2021, 12, 12),
                                        new DateTime(2021, 12, 11), null,
                                        new DateTime(2021, 12, 9),null,null,new DateTime(2021, 12, 12),
                                        new DateTime(2021, 12, 8), new DateTime(2021, 12, 9), new DateTime(2021, 12, 10)};
            int[] tempoDecorr = { 120, 60, 0, 140, 60, 0, 140, 0, 0, 150, 60, 65, 120 };
            DateTime[] DtJogadaa = { new DateTime(2021, 12, 10), new DateTime(2021, 12, 9), new DateTime(2021, 12, 11), new DateTime(2021, 12, 12),
                                        new DateTime(2021, 12, 11), new DateTime(2021, 12, 11),
                                        new DateTime(2021, 12, 9),new DateTime(2021, 12, 9),new DateTime(2021, 12, 10),new DateTime(2021, 12, 12),
                                        new DateTime(2021, 12, 8), new DateTime(2021, 12, 9), new DateTime(2021, 12, 10)};

            for (int i = 0; i < alunoId.Length; i++)
            {
                _context.Sessao.Add(new Sessao
                {
                    AlunoId = alunoId[i],
                    FaseId = faseId[i],
                    Concluida = concluida[i],
                    DtConclusao = DtConclusao[i],
                    DtJogada = DtJogadaa[i],
                    Pontuacao = pontuacao[i],
                    TempoDecorrido = tempoDecorr[i]
                });

            }
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("SessaoFaseTotalByAlunoId/{id}")]
        public async Task<ActionResult<IEnumerable<SessaoFaseTotal>>> SessaoFaseTotalByAlunoId(int id)
        {
            var sessoes = await _context.Sessao.Where(x => x.AlunoId == id).ToListAsync();

            if (sessoes.Count == 0)
                return NotFound();

            var groupedSessions = sessoes.GroupBy(x => x.FaseId).ToList();

            List<SessaoFaseTotal> sessFaseTotalList = new List<SessaoFaseTotal>();

            foreach (var fase in groupedSessions){

                int levelId = 0;
                int finished = 0;
                int tries = 0;
                int repetitions = 0;
                int fails = 0;
                int maxPoints = 0;
                DateTime? minFinishedDate = null;
                float totalElapsedTime = 0;
                int plays = 0;
                DateTime maxPlayedDate = new DateTime();

                foreach (Sessao sess in fase)
                {
                    levelId = sess.FaseId;

                    if(sess.Concluida != 0)
                        finished = sess.Concluida;

                    if (sess.DtConclusao != null)
                    {
                        if (sess.DtConclusao > sess.DtJogada)
                            tries++; //tentativas (antes da conclusao
                        else
                            repetitions++; //repetições (depois da conclusao)
                    }

                    if (sess.Concluida == 0)
                        fails++;

                    if(sess.Pontuacao > maxPoints)
                            maxPoints = sess.Pontuacao;

                    if (sess.DtConclusao != null)
                    {
                        if (minFinishedDate == null)
                            minFinishedDate = sess.DtConclusao.Value;

                        if (sess.DtConclusao.Value < minFinishedDate.Value) //TODO ficar atento a diferenças de datas (americana x brasileira)
                            minFinishedDate = sess.DtConclusao.Value;
                    }

                    totalElapsedTime += sess.TempoDecorrido;

                    plays++; //quantidade de plays é a mesma quantidade de sessoes existentes

                    if (sess.DtJogada > maxPlayedDate)
                        maxPlayedDate = sess.DtJogada;
                }

                sessFaseTotalList.Add(new SessaoFaseTotal
                {
                    FaseId = levelId,
                    Concluida = finished,
                    Tentativas = tries,
                    Repetidas = repetitions,
                    Falhas = fails,
                    PontuacaoMax = maxPoints,
                    DtConclusao = minFinishedDate,
                    TempoDecorrido = totalElapsedTime,
                    Plays = plays,
                    DtUltimaJogada = maxPlayedDate
                });
            }

            return sessFaseTotalList;
        }


        // GET: api/Sessao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sessao>>> GetSessoes()
        {
            return await _context.Sessao.Include(s=>s.Aluno).Include(x=>x.Fase).ToListAsync();
        }

        // GET: api/GetSessaoById/5
        [HttpGet("GetSessaoById/{id}")]
        public async Task<ActionResult<Sessao>> GetSessaoById(int id)
        {
            var sessao = await _context.Sessao.FindAsync(id);

            if (sessao == null)
            {
                return NotFound();
            }

            return sessao;
        }

        [HttpGet("GetSessaoByAlunoId/{id}")]
        public async Task<ActionResult<IEnumerable<Sessao>>> GetSessoesByAlunoId(int id)
        {
            var sessoes = await _context.Sessao.Where(x=>x.AlunoId == id).ToListAsync();

            if (sessoes == null)
            {
                return NotFound();
            }

            return sessoes;
        }

        // PUT: api/Sessao/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSessao(int id, Sessao sessao)
        {
            if (id != sessao.Id)
            {
                return BadRequest();
            }

            _context.Entry(sessao).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessaoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sessao
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sessao>> PostSessao(Sessao sessao)
        {
            _context.Sessao.Add(sessao);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSessao", new { id = sessao.Id }, sessao);
        }

        

        // DELETE: api/Sessao/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sessao>> DeleteSessao(int id)
        {
            var sessao = await _context.Sessao.FindAsync(id);
            if (sessao == null)
            {
                return NotFound();
            }

            _context.Sessao.Remove(sessao);
            await _context.SaveChangesAsync();

            return sessao;
        }

        private bool SessaoExists(int id)
        {
            return _context.Sessao.Any(e => e.Id == id);
        }
    }
}
