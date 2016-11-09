namespace DebateTeamManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TimeSlots", "isMorning", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TimeSlots", "isMorning");
        }
    }
}
