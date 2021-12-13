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
            int[] concluida ={1,1,0,1, 0,0, 1,0,0,1, 1,1,1 };
            int[] pontuacao ={100,135,0,140, 0,0, 145,0,0,146, 200,250,200 };
            DateTime?[] DtConclusao = { new DateTime(2021, 12, 11), new DateTime(2021, 12, 9), null, new DateTime(2021, 12, 12),
                                        null, null,
                                        new DateTime(2021, 12, 9),null,null,new DateTime(2021, 12, 12),
                                        new DateTime(2021, 12, 8), new DateTime(2021, 12, 9), new DateTime(2021, 12, 10)};
            int[] tempoDecorr = { 120,60,0,140, 0,0, 140,0,0, 150,60,65,120 };
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

                levelId = fase.First().FaseId;

                finished = fase.Any(x => x.Concluida == 1) == true ? 1 : 0;
                fails = fase.Where(x => x.Concluida == 0).Count();
                maxPoints = fase.Max(x => x.Pontuacao);

                var dtConclusaoList = fase.Where(x => x.DtConclusao != null);
                if (dtConclusaoList.Count() > 0)
                {
                    minFinishedDate = dtConclusaoList.Min(x => x.DtConclusao);
                }

                foreach (var session in fase)
                    totalElapsedTime += session.TempoDecorrido;

                plays = fase.Count();

                maxPlayedDate = fase.Max(x => x.DtJogada);

                var levelTriesList = fase.Where(x => x.DtJogada < minFinishedDate || x.DtConclusao == null).ToList();
                var levelRepetitionsList = fase.Where(x => x.DtJogada > minFinishedDate).ToList();

                tries += levelTriesList.Count();
                repetitions += levelRepetitionsList.Count();

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

        [HttpGet("SessaoAlunoTotalCompleta")]
        public async Task<ActionResult<IEnumerable<SessaoAlunoTotal>>> SessaoAlunoTotalCompleta()
        {
            var sessoes = await _context.Sessao.Include(s => s.Aluno).Include(f=>f.Fase).OrderBy(x=>x.FaseId).ToListAsync();

            if (sessoes.Count == 0)
                return NotFound();

            var groupedSessions = sessoes.GroupBy(x => x.AlunoId).ToList();

            List<SessaoAlunoTotal> sessAlunoTotalList = new List<SessaoAlunoTotal>();

            foreach (var aluno in groupedSessions)
            {
                Aluno student = null;
                int maxPoints = 0; 
                int totalPoints = 0;
                int levelMaxPointsId = 0; 
                int maxRepetitions = 0;
                int totalRepetitions = 0;
                int levelMaxRepetitionsId = 0;
                float totalTimeElapsed = 0; 
                float maxTimeElapsed = 0;  
                int levelMaxTimeElapsedId = 0; 
                int maxTries = 0;
                int totalTries = 0;
                int levelMaxTriesId = 0;
                int maxFails = 0; 
                int totalFails = 0; 
                int levelMaxFailsId = 0; 
                DateTime? minFinishedDate = null; 
                DateTime? maxFinishedDate = null; 
                int finishedLevelsQuantity = 0; 
                int pendingLevelsQuantity = 0;
                int maxPlays = 0; 
                int totalPlays = 0; 
                int levelMaxPlaysId = 0; 
                DateTime? maxPlayedDate = null; 

                List<Tuple<int, int>> triesByLevelId = new List<Tuple<int, int>>(); //int FaseId, int Tries
                List<Tuple<int, int>> repetitionsByLevelId = new List<Tuple<int, int>>(); //int FaseId, int repetitions

                var maxPointsSession = aluno.OrderByDescending(x => x.Pontuacao);
                maxPoints = maxPointsSession.First().Pontuacao;
                levelMaxPointsId = maxPoints > 0 ? maxPointsSession.First().FaseId : 0;
                foreach (var session in maxPointsSession)
                    totalPoints += session.Pontuacao;

                var maxElapsedTimeSession = aluno.OrderByDescending(x => x.TempoDecorrido);
                maxTimeElapsed = maxElapsedTimeSession.First().TempoDecorrido;
                levelMaxTimeElapsedId = maxElapsedTimeSession.First().FaseId;
                foreach (var session in maxElapsedTimeSession)
                    totalTimeElapsed += session.TempoDecorrido;

                var maxPlaysSession = aluno.GroupBy(x => x.FaseId).OrderByDescending(x => x.Count()).ThenByDescending(x => x.Key).First();
                if(maxPlaysSession.Count() > 0)
                {
                    maxPlays = maxPlaysSession.Count();
                    levelMaxPlaysId = maxPlaysSession.First().FaseId;
                    totalPlays = aluno.Count();
                }

                var finishedDates = aluno.Where(x=>x.DtConclusao != null).OrderByDescending(x => x.DtConclusao).ToList();
                if(finishedDates.Count > 0)
                {
                    maxFinishedDate = finishedDates.First().DtConclusao;
                    minFinishedDate = finishedDates.Last().DtConclusao;
                }

                maxPlayedDate = aluno.Max(x => x.DtJogada);

                var maxLevelFails = aluno.Where(x => x.Concluida == 0).GroupBy(x => x.FaseId).
                                            OrderByDescending(x => x.Count()).ThenByDescending(x => x.Key); //https://stackoverflow.com/questions/41835981/sort-by-frequency-and-value-using-linq
                if(maxLevelFails.Count() > 0)
                {
                    maxFails = maxLevelFails.First().Count();
                    levelMaxFailsId = maxLevelFails.First().First().FaseId;
                    totalFails = maxLevelFails.SelectMany(x => x).Count();
                }

                var maxLevelTries = aluno.GroupBy(x => x.FaseId).OrderByDescending(x => x.Count()).ThenByDescending(x => x.Key);

                List<Sessao> levelTriesList = new List<Sessao>();
                List<Sessao> levelRepetitionsList = new List<Sessao>();
                List<Sessao> totalTriesList = new List<Sessao>();
                List<Sessao> totalRepetitionsList = new List<Sessao>();

                foreach (var levels in maxLevelTries)
                {
                    var minDataConclusao = levels.Min(x => x.DtConclusao);

                    levelTriesList = levels.Where(x => x.DtJogada < minDataConclusao || x.DtConclusao == null).ToList();
                    levelRepetitionsList = levels.Where(x => x.DtJogada > minDataConclusao).ToList();

                    totalTriesList.AddRange(levelTriesList);
                    totalRepetitionsList.AddRange(levelRepetitionsList);

                    totalTries += levelTriesList.Count();
                    totalRepetitions += levelRepetitionsList.Count();
                }

                if(totalTriesList.Count > 0)
                {
                    var totalTriesListByLevel = totalTriesList.GroupBy(x => x.FaseId).OrderByDescending(x=>x.Count()).ThenByDescending(x=>x.Key);
                    maxTries = totalTriesListByLevel.First().Count();
                    levelMaxTriesId = totalTriesListByLevel.First().First().FaseId;
                }
                if(totalRepetitionsList.Count > 0){
                    var totalRepetitionsListByLevel = totalRepetitionsList.GroupBy(x => x.FaseId).OrderByDescending(x => x.Count()).ThenByDescending(x => x.Key);
                    maxRepetitions = totalRepetitionsListByLevel.First().Count();
                    levelMaxRepetitionsId = totalRepetitionsListByLevel.First().First().FaseId;
                }


                var levelsList = await _context.Fase.ToListAsync();
                finishedLevelsQuantity = aluno.GroupBy(x=>x.FaseId).Select(g=>g.First()).ToList().Where(x => x.Concluida == 1).Count();
                pendingLevelsQuantity = levelsList.Count - finishedLevelsQuantity;

                student = aluno.First().Aluno;

                sessAlunoTotalList.Add(new SessaoAlunoTotal
                {
                    Aluno = student,
                    AlunoId = student.Id,
                    MaxPontos = maxPoints,
                    TotalPontos = totalPoints,
                    FaseMaxPontosId = levelMaxPointsId,
                    MaxRepeticoes = maxRepetitions,
                    TotalRepeticoes = totalRepetitions,
                    FaseMaxRepeticoesId = levelMaxRepetitionsId,
                    MaxTentativas = maxTries,
                    TotalTentativas = totalTries,
                    FaseMaxTentativasId = levelMaxTriesId,
                    MaxFalhas = maxFails,
                    TotalFalhas = totalFails,
                    FaseMaxFalhasId = levelMaxFailsId,
                    MaxTempoDecorrido = maxTimeElapsed,
                    TotalTempoDecorrido = totalTimeElapsed,
                    FaseMaxTempoDecorridoId = levelMaxTimeElapsedId,
                    MinDtConclusao = minFinishedDate,
                    MaxDtConclusao = maxFinishedDate,
                    FasesConcluidasQuantidade = finishedLevelsQuantity,
                    FasesPendentesQuantidade = pendingLevelsQuantity,
                    MaxPlays = maxPlays,
                    TotalPlays = totalPlays,
                    FaseMaxPlays = levelMaxPlaysId,
                    MaxDtJogada = maxPlayedDate.Value,
                    DtUltimoLogin = student.DtUltimoLogin

                }); ;
            }

            return sessAlunoTotalList;
        }

        [HttpGet("SessaoAlunoTotalCompleta2")]
        public async Task<ActionResult<IEnumerable<SessaoAlunoTotal>>> SessaoAlunoTotalCompletaDois()
        {
            var sessoes = await _context.Sessao.Include(s => s.Aluno).Include(f => f.Fase).OrderBy(x => x.FaseId).ToListAsync();

            if (sessoes.Count == 0)
                return NotFound();

            var groupedSessionsByLevel = sessoes.GroupBy(x => x.FaseId).ToList();
            var groupedSessionsByStudent = sessoes.GroupBy(x => x.AlunoId).ToList();

            List<SessaoAlunoTotal> sessAlunoTotalList = new List<SessaoAlunoTotal>();

            foreach (var aluno in groupedSessionsByStudent)
            {
                int studentId = 0;
                Aluno student = null;
                int maxPoints = 0;
                int totalPoints = 0;
                int levelMaxPointsId = 0;
                int maxRepetitions = 0;
                int totalRepetitions = 0;
                int levelMaxRepetitionsId = 0;
                int totalFails = 0;
                float totalTimeElapsed = 0;
                int maxTries = 0;
                int totalTries = 0;
                int levelMaxTriesId = 0;
                int maxFails = 0;
                int levelMaxFailsId = 0;
                float maxTimeElapsed = 0;
                int levelMaxTimeElapsedId = 0;
                DateTime? minFinishedDate = null;
                DateTime? maxFinishedDate = null;
                int finishedLevelsQuantity = 0;
                int pendingLevelsQuantity = 0;
                int maxPlays = 0;
                int totalPlays = 0;
                int levelMaxPlaysId = 0;
                DateTime? maxPlayedDate = null;

                //Helpers
                int previousLevelId = 0;
                int currentLevelId = 0;
                int totalTriesByLevel = 0;
                int totalRepetitionsByLevel = 0;
                int totalFailsByLevel = 0;
                float totalTimeElapsedByLevel = 0;
                int totalPlaysByLevel = 0;

                List<Tuple<int, int>> triesByLevelId = new List<Tuple<int, int>>(); //int FaseId, int Tries
                List<Tuple<int, int>> repetitionsByLevelId = new List<Tuple<int, int>>(); //int FaseId, int repetitions
                List<Tuple<int, int>> failsByLevelId = new List<Tuple<int, int>>(); //int FaseId, int fails
                List<Tuple<int, float>> timeElapsedByLevelId = new List<Tuple<int, float>>(); //int FaseId, int timeElapsed
                List<Tuple<int, int>> playsByLevelId = new List<Tuple<int, int>>(); // int FaseId, int plays

                foreach (Sessao sess in aluno)
                {
                    currentLevelId = sess.FaseId; //helper
                    if (previousLevelId == 0)
                        previousLevelId = sess.FaseId;

                    student = sess.Aluno;

                    if (maxPlayedDate == null)
                        maxPlayedDate = sess.DtJogada;

                    if (sess.DtJogada > maxPlayedDate)
                        maxPlayedDate = sess.DtJogada;

                    if (sess.Pontuacao > maxPoints)
                    {
                        maxPoints = sess.Pontuacao;
                        levelMaxPointsId = sess.FaseId;
                    }
                    totalPoints += sess.Pontuacao;

                    if (sess.Concluida == 0)
                    {
                        totalFails++;
                    }

                    totalTimeElapsed += sess.TempoDecorrido;
                    totalPlays++;

                    if (currentLevelId == previousLevelId)
                    {
                        if (sess.Concluida == 1)
                            finishedLevelsQuantity++;

                        if (sess.Concluida == 0)
                            totalFailsByLevel++;

                        totalPlaysByLevel++;
                        totalTimeElapsedByLevel += sess.TempoDecorrido;
                    }
                    else
                    {
                        playsByLevelId.Add(new Tuple<int, int>(sess.FaseId, totalPlaysByLevel));
                        failsByLevelId.Add(new Tuple<int, int>(sess.FaseId, totalFailsByLevel));
                        timeElapsedByLevelId.Add(new Tuple<int, float>(sess.FaseId, totalTimeElapsedByLevel));

                        totalPlaysByLevel = 0;
                        totalFailsByLevel = 0;
                        totalTimeElapsedByLevel = 0;

                        previousLevelId = currentLevelId;

                        if (sess.Concluida == 0)
                            totalFailsByLevel++;
                        totalTimeElapsedByLevel += sess.TempoDecorrido;
                    }

                    if (sess.DtConclusao != null)
                    {
                        if (minFinishedDate == null)
                            minFinishedDate = sess.DtConclusao.Value;
                        if (maxFinishedDate == null)
                            maxFinishedDate = sess.DtConclusao.Value;

                        if (sess.DtConclusao.Value < minFinishedDate.Value) //TODO ficar atento a diferenças de datas (americana x brasileira)
                            minFinishedDate = sess.DtConclusao;
                        if (sess.DtConclusao.Value > maxFinishedDate) //TODO ficar atento a diferenças de datas (americana x brasileira)
                            maxFinishedDate = sess.DtConclusao;


                        if (sess.DtConclusao > sess.DtJogada)
                        {
                            totalTries++;
                        }
                        else
                        {
                            totalRepetitions++;
                        }

                        if (currentLevelId == previousLevelId)
                        {
                            if (sess.DtConclusao > sess.DtJogada)
                            {
                                totalTriesByLevel++; //tentativas (antes da conclusao
                                totalTries++;
                            }
                            else
                            {
                                totalRepetitionsByLevel++; //repetições (depois da conclusao)
                                totalRepetitions++;
                            }
                        }
                        else
                        {
                            triesByLevelId.Add(new Tuple<int, int>(sess.FaseId, totalTriesByLevel));
                            repetitionsByLevelId.Add(new Tuple<int, int>(sess.FaseId, totalRepetitionsByLevel));

                            previousLevelId = currentLevelId;
                            totalTriesByLevel = 0;
                            totalRepetitionsByLevel = 0;

                            if (sess.DtConclusao > sess.DtJogada)
                                totalTriesByLevel++; //tentativas (antes da conclusao
                            else
                                totalRepetitionsByLevel++; //repetições (depois da conclusao)
                        }
                    }
                }
                if (triesByLevelId.Count > 0)
                {
                    levelMaxTriesId = triesByLevelId.OrderBy(x => x.Item2).First().Item1; //pegando a fase que possui mais tentativas
                    maxTries = triesByLevelId.OrderBy(x => x.Item2).First().Item2; //pegando o max de tentativas
                }
                if (repetitionsByLevelId.Count > 0)
                {
                    levelMaxRepetitionsId = repetitionsByLevelId.OrderBy(x => x.Item2).First().Item1; //pegando a fase que possui mais repeticoes
                    maxRepetitions = repetitionsByLevelId.OrderBy(x => x.Item2).First().Item2; //pegando o max de repeticoes
                }
                if (failsByLevelId.Count > 0)
                {
                    levelMaxFailsId = failsByLevelId.OrderBy(x => x.Item2).First().Item1; // pegando a fase que possui mais falhas
                    maxFails = failsByLevelId.OrderBy(x => x.Item2).First().Item2; //pegando o max de falhas
                }
                if (timeElapsedByLevelId.Count > 0)
                {
                    levelMaxTimeElapsedId = timeElapsedByLevelId.OrderBy(x => x.Item2).First().Item1; //pegando a fase que possui mais tempo jogado
                    maxTimeElapsed = timeElapsedByLevelId.OrderBy(x => x.Item2).First().Item2; //pegando o max de tempo jogado
                }
                if (playsByLevelId.Count > 0)
                {
                    levelMaxPlaysId = playsByLevelId.OrderBy(x => x.Item2).First().Item1; //pegando a fase que possui mais plays
                    maxPlays = playsByLevelId.OrderBy(x => x.Item2).First().Item2; //pegando o max de plays
                }

                var levelsList = await _context.Fase.ToListAsync();

                pendingLevelsQuantity = levelsList.Count - finishedLevelsQuantity;

                sessAlunoTotalList.Add(new SessaoAlunoTotal
                {
                    Aluno = student,
                    AlunoId = student.Id,
                    MaxPontos = maxPoints,
                    TotalPontos = totalPoints,
                    FaseMaxPontosId = levelMaxPointsId,
                    MaxRepeticoes = maxRepetitions,
                    TotalRepeticoes = totalRepetitions,
                    FaseMaxRepeticoesId = levelMaxRepetitionsId,
                    MaxTentativas = maxTries,
                    TotalTentativas = totalTries,
                    FaseMaxTentativasId = levelMaxTriesId,
                    MaxFalhas = maxFails,
                    TotalFalhas = totalFails,
                    FaseMaxFalhasId = levelMaxFailsId,
                    MaxTempoDecorrido = maxTimeElapsed,
                    TotalTempoDecorrido = totalTimeElapsed,
                    FaseMaxTempoDecorridoId = levelMaxTimeElapsedId,
                    MinDtConclusao = minFinishedDate.Value,
                    MaxDtConclusao = maxFinishedDate.Value,
                    FasesConcluidasQuantidade = finishedLevelsQuantity,
                    FasesPendentesQuantidade = pendingLevelsQuantity,
                    MaxPlays = maxPlays,
                    TotalPlays = totalPlays,
                    FaseMaxPlays = levelMaxPlaysId,
                    MaxDtJogada = maxPlayedDate.Value,
                    DtUltimoLogin = student.DtUltimoLogin

                }); ;
            }

            return sessAlunoTotalList;
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

        [HttpGet("GetSessoesByAlunoId/{id}")]
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
