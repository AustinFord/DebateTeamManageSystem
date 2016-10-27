namespace DebateTeamManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class p : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamID = c.Int(nullable: false, identity: true),
                        TeamName = c.String(nullable: false, maxLength: 40),
                    })
                .PrimaryKey(t => t.TeamID);
            
            CreateTable(
                "dbo.TimeSlots",
                c => new
                    {
                        TimeSlotID = c.Int(nullable: false, identity: true),
                        Team1Name = c.String(),
                        Team2Name = c.String(),
                        Team1Score = c.Int(nullable: false),
                        Team2Score = c.Int(nullable: false),
                        date = c.String(),
                        time = c.String(),
                    })
                .PrimaryKey(t => t.TimeSlotID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TimeSlots");
            DropTable("dbo.Teams");
        }
    }
}
