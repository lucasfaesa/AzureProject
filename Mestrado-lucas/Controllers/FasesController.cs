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
    public class FasesController : ControllerBase
    {
        private readonly Mestrado_lucasContext _context;

        public FasesController(Mestrado_lucasContext context)
        {
            _context = context;
        }

        [HttpGet("babish")]
        public async Task<ActionResult<Fase>> Fill()
        {
            string[] nome = { "Encontre os números", "Procure o sujeito", "Arranje a frase", "Encontre o valor de x",
                               "Resolva a equação de segundo grau", "Encontre os números"};

            string[] desc = { " Jogador deve encontrar os números primos no tempo permitido", 
                                "Jogador deve procurar o sujeito da frase com o max de 3 tentativas",
                                "Jogador deve corrigir a frase no tempo permitido",
                                "Jogador deve encontrar o valor de X no permitido",
                                "Jogador deve resolver a equação de segundo grau com o max de 3 chances",
                                "Jogador deve encontrar os números primos no tempo permitido"};

            for(int i=0; i<nome.Length; i++)
            {
                _context.Fase.Add(new Fase
                {
                    Nome = nome[i],
                    Descricao = desc[i]
                });
                
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/Fase
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fase>>> GetFase()
        {
            return await _context.Fase.ToListAsync();
        }

        // GET: api/Fase/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fase>> GetFase(int id)
        {
            var fase = await _context.Fase.FindAsync(id);

            if (fase == null)
            {
                return NotFound();
            }

            return fase;
        }

        // PUT: api/Fase/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFase(int id, Fase fase)
        {
            if (id != fase.Id)
            {
                return BadRequest();
            }

            _context.Entry(fase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FaseExists(id))
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

        // POST: api/Fase
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Fase>> PostFase(Fase fase)
        {
            _context.Fase.Add(fase);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFase", new { id = fase.Id }, fase);
        }

        // DELETE: api/Fase/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Fase>> DeleteFase(int id)
        {
            var fase = await _context.Fase.FindAsync(id);
            if (fase == null)
            {
                return NotFound();
            }

            _context.Fase.Remove(fase);
            await _context.SaveChangesAsync();

            return fase;
        }

        private bool FaseExists(int id)
        {
            return _context.Fase.Any(e => e.Id == id);
        }
    }
}
