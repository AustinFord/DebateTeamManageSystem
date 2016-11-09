namespace DebateTeamManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TimeSlots", "time");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimeSlots", "time", c => c.String(nullable: false));
        }
    }
}
