namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveRegistration : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "Registered");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Registered", c => c.Boolean(nullable: false));
        }
    }
}
