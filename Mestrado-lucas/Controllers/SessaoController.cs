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
    public class SessaoController : ControllerBase
    {
        private readonly Mestrado_lucasContext _context;

        public SessaoController(Mestrado_lucasContext context)
        {
            _context = context;
        }

        // GET: api/Sessao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sessao>>> GetSessao()
        {
            return await _context.Sessao.ToListAsync();
        }

        // GET: api/Sessao/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sessao>> GetSessao(int id)
        {
            var sessao = await _context.Sessao.FindAsync(id);

            if (sessao == null)
            {
                return NotFound();
            }

            return sessao;
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
