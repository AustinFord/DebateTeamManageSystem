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
            context.Teams.AddOrUpdate(
                new Team
                {
                    TeamName = "TestyTeam"
                },
                new Team
                {
                    TeamName = "TestyTeam2"
                },
                new Team
                {
                    TeamName = "TestyTeam3"
                },
                new Team
                {
                    TeamName = "TestyTeam4"
                }
                );

            context.SaveChanges();

        }
    }
}
