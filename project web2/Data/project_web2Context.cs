using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using project_web2.Models;

namespace project_web2.Data
{
    public class project_web2Context : DbContext
    {
        public project_web2Context (DbContextOptions<project_web2Context> options)
            : base(options)
        {
        }

        public DbSet<project_web2.Models.usersaccounts> usersaccounts { get; set; } = default!;
        public DbSet<project_web2.Models.customer> customer { get; set; } = default!;
        public DbSet<project_web2.Models.orders> orders { get; set; } = default!;
        public DbSet<project_web2.Models.items> items { get; set; } = default!;
        public DbSet<project_web2.report> report { get; set; } = default!;
        public DbSet<project_web2.Models.orderline> orderline { get; set; } = default!;


    }
}
