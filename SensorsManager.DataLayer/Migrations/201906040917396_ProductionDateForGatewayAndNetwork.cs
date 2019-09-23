namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionDateForGatewayAndNetwork : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gateways", "ProductionDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Networks", "ProductionDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Networks", "ProductionDate");
            DropColumn("dbo.Gateways", "ProductionDate");
        }
    }
}
