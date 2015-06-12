namespace EdmxStuff.Context
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class MyDatabaseContext : DbContext
	{
		public MyDatabaseContext() : base("name=MyDatabaseContext")
		{
			this.Configuration.LazyLoadingEnabled = false;
		}

		public virtual DbSet<Person> Persons { get; set; }
		public virtual DbSet<Ragdoll> Ragdolls { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Person>()
				.HasMany(e => e.Ragdolls)
				.WithRequired(e => e.Person)
				.HasForeignKey(e => e.Person_Id)
				.WillCascadeOnDelete(false);
		}
	}
}
