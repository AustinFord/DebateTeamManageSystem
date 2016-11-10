namespace DebateTeamManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "isActive", c => c.Boolean(nullable: false));
            DropColumn("dbo.Teams", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "Status", c => c.Boolean(nullable: false));
            DropColumn("dbo.Teams", "isActive");
        }
    }
}
