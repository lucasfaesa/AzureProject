using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mestrado_lucas;
using Mestrado_lucas.Data;
using Newtonsoft.Json;

namespace Mestrado_lucas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerBase
    {
        private readonly Mestrado_lucasContext _context;

        public AlunosController(Mestrado_lucasContext context)
        {
            _context = context;
        }

        [HttpGet("babish")]
        public async Task<ActionResult<Aluno>> Fill()
        {
            string[] nome = { "Lucas", "Joao", "Maria", "Paula"};
            string[] login = { "Luke12", "Joo44", "Mar5", "PaulaRanger222" };
            string[] senha = { "abc123", "algo23232", "something22", "chokito"};
            DateTime[] dataCriacao = { new DateTime(2021, 12, 11), new DateTime(2021, 11, 30), new DateTime(2021, 12, 1), new DateTime(2021, 12, 5) };
            DateTime[] dataULogin = { new DateTime(2021, 12, 11), new DateTime(2021, 12, 9), new DateTime(2021, 12, 3), new DateTime(2021, 12, 10) };

            for (int i = 0; i < nome.Length; i++)
            {
                _context.Aluno.Add(new Aluno
                {
                    Nome = nome[i],
                    LoginNome = login[i],
                    Senha = senha[i],
                    DtCriacao = dataCriacao[i],
                    DtUltimoLogin = dataULogin[i]
                });

            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Alunos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Aluno>>> GetAluno()
        {
            return await _context.Aluno.ToListAsync();
        }

        // GET: api/Alunos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Aluno>> GetAluno(int id)
        {
            var aluno = await _context.Aluno.FindAsync(id);

            if (aluno == null)
            {
                return NotFound();
            }

            return aluno;
        }

        //api/alunos/login 
        //verificar algum tipo de hashzação das senhas
        [HttpGet("Login/{loginNome}/{senha}")]
        public ActionResult<Aluno> LoginAluno(string loginNome, string senha)
        {
            var aluno = _context.Aluno.FirstOrDefault(x => x.LoginNome.ToLower() == loginNome.ToLower() && x.Senha == senha);

            if (aluno == null)
            {
                return NotFound();
            }

            return aluno;
        }

        // PUT: api/Alunos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAluno(int id, Aluno aluno)
        {
            if (id != aluno.Id)
            {
                return BadRequest();
            }

            _context.Entry(aluno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlunoExists(id))
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

        //Criar um aluno (conta)
        // POST: api/Alunos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Aluno>> PostAluno(Aluno aluno)
        {
            if(_context.Aluno.Any(x=>x.LoginNome == aluno.LoginNome))
            {
                return NotFound();
            }
            _context.Aluno.Add(aluno);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAluno", new { id = aluno.Id }, aluno);
        }

        // GET: api/Alunos/CheckLoginName/Lukas123
        //if return true exists, if false, doesn't
        [HttpGet("CheckLoginName/{loginNome}")]
        public bool CheckLoginName(string loginNome)
        {
            return _context.Aluno.Any(e => e.LoginNome.ToLower() == loginNome.ToLower());
        }

        // DELETE: api/Alunos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Aluno>> DeleteAluno(int id)
        {
            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno == null)
            {
                return NotFound();
            }

            _context.Aluno.Remove(aluno);
            await _context.SaveChangesAsync();

            return aluno;
        }

        private bool AlunoExists(int id)
        {
            return _context.Aluno.Any(e => e.Id == id);
        }
    }
}
