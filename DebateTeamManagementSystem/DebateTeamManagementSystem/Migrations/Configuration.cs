namespace DebateTeamManagementSystem.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DebateTeamManagementSystem.Models.DebateContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DebateTeamManagementSystem.Models.DebateContext context)
        {
            context.Teams.Add(
                new Team {
                    TeamID = 50, 
                    TeamName = "Test Team 1" }
                );
            context.SaveChanges();
        }
    }
}
