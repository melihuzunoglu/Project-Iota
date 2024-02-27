using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectIota.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}

		/*Standart Models*/
		public DbSet<Models.User> User { get; set; }
		public DbSet<Models.WebMenu> WebMenu { get; set; }
		public DbSet<Models.Category> Category { get; set; }
		public DbSet<Models.RabbitMq> RabbitMq { get; set; }

		/*Private Models*/
		public virtual DbSet<PrivateModels.CategoryList> CategoryList { get; set; }
	}
}
