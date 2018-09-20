namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRegister : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Registered", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Registered");
        }
    }
}
