namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedTypo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sensors", "Active", c => c.Boolean(nullable: false));
            DropColumn("dbo.Sensors", "Activ");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sensors", "Activ", c => c.Boolean(nullable: false));
            DropColumn("dbo.Sensors", "Active");
        }
    }
}
