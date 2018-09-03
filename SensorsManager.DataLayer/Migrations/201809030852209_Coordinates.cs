namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Coordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sensors", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Sensors", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sensors", "Longitude");
            DropColumn("dbo.Sensors", "Latitude");
        }
    }
}
