namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedSpellingErrors : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" }, "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" });
            RenameColumn(table: "dbo.SensorReadings", name: "SensorGatewayAdress", newName: "SensorGatewayAddress");
            RenameColumn(table: "dbo.SensorReadings", name: "SensorClientAdress", newName: "SensorClientAddress");
            AlterColumn("dbo.SensorReadings", "SensorGatewayAddress", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.SensorReadings", "SensorClientAddress", c => c.String(nullable: false, maxLength: 4));
            CreateIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" });
            AddForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" }, "dbo.Sensors", new[] { "Id", "GatewayAddress", "ClientAddress" }, cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" }, "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" });
            AlterColumn("dbo.SensorReadings", "SensorClientAddress", c => c.String(maxLength: 4));
            AlterColumn("dbo.SensorReadings", "SensorGatewayAddress", c => c.String(maxLength: 4));
            RenameColumn(table: "dbo.SensorReadings", name: "SensorClientAddress", newName: "SensorClientAdress");
            RenameColumn(table: "dbo.SensorReadings", name: "SensorGatewayAddress", newName: "SensorGatewayAdress");
            CreateIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" });
            AddForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" }, "dbo.Sensors", new[] { "Id", "GatewayAddress", "ClientAddress" });
        }
    }
}
