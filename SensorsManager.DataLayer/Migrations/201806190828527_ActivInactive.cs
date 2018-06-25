namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ActivInactive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sensors", "Activ", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sensors", "Activ");
        }
    }
}
