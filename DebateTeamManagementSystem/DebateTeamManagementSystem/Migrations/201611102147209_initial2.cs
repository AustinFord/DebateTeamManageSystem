namespace DebateTeamManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeSlots", "time", c => c.String(nullable: false));
            DropColumn("dbo.TimeSlots", "isMorning");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TimeSlots", "isMorning", c => c.Boolean(nullable: false));
            DropColumn("dbo.TimeSlots", "time");
        }
    }
}
