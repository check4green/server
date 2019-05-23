namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GatewayLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gateways", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Gateways", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Gateways", "Longitude");
            DropColumn("dbo.Gateways", "Latitude");
        }
    }
}
