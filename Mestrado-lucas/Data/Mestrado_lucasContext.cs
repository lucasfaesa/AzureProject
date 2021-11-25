using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mestrado_lucas;

namespace Mestrado_lucas.Data
{
    public class Mestrado_lucasContext : DbContext
    {
        public Mestrado_lucasContext (DbContextOptions<Mestrado_lucasContext> options)
            : base(options)
        {
        }

        public DbSet<Mestrado_lucas.Aluno> Aluno { get; set; }
    }
}
