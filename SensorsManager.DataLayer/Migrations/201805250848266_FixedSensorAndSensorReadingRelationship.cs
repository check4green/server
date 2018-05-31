namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedSensorAndSensorReadingRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" }, "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" });
            DropPrimaryKey("dbo.Sensors");
            AlterColumn("dbo.Sensors", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Sensors", "Id");
            CreateIndex("dbo.SensorReadings", "SensorId");
            AddForeignKey("dbo.SensorReadings", "SensorId", "dbo.Sensors", "Id", cascadeDelete: true);
            DropColumn("dbo.SensorReadings", "SensorGatewayAddress");
            DropColumn("dbo.SensorReadings", "SensorClientAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SensorReadings", "SensorClientAddress", c => c.String(nullable: false, maxLength: 4));
            AddColumn("dbo.SensorReadings", "SensorGatewayAddress", c => c.String(nullable: false, maxLength: 4));
            DropForeignKey("dbo.SensorReadings", "SensorId", "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId" });
            DropPrimaryKey("dbo.Sensors");
            AlterColumn("dbo.Sensors", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Sensors", new[] { "Id", "GatewayAddress", "ClientAddress" });
            CreateIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" });
            AddForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAddress", "SensorClientAddress" }, "dbo.Sensors", new[] { "Id", "GatewayAddress", "ClientAddress" }, cascadeDelete: true);
        }
    }
}
