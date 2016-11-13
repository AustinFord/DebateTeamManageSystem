namespace DebateTeamManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeSlots", "RoundStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeSlots", "RoundStatus");
        }
    }
}
