using Champions.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Champions.Context
{
    public class MyContext : DbContext
    {
        public MyContext()
        : base("name=MyContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CustomerModel>().ToTable("CustomerModels");
            modelBuilder.Entity<GameModel>().ToTable("GameModel");
            modelBuilder.Entity<User_Games>().ToTable("User_Games");
            modelBuilder.Entity<Players>().ToTable("Players");
            modelBuilder.Entity<Teams>().ToTable("Teams");
        }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<GameModel> GameDb { get; set; }
        public DbSet<User_Games> UserGamesDB { get; set; }
        public DbSet<Players> PlayersDB { get; set; }
        public DbSet<Teams> TeamsDB { get; set; }
    }
}