namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeysForReadings : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SensorReadings", "SensorId", "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId" });
            DropPrimaryKey("dbo.Sensors");
            AddColumn("dbo.SensorReadings", "SensorGatewayAdress", c => c.String(maxLength: 4));
            AddColumn("dbo.SensorReadings", "SensorClientAdress", c => c.String(maxLength: 4));
            AlterColumn("dbo.Sensors", "Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Sensors", "GatewayAddress", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.Sensors", "ClientAddress", c => c.String(nullable: false, maxLength: 4));
            AddPrimaryKey("dbo.Sensors", new[] { "Id", "GatewayAddress", "ClientAddress" });
            CreateIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" });
            AddForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" }, "dbo.Sensors", new[] { "Id", "GatewayAddress", "ClientAddress" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" }, "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId", "SensorGatewayAdress", "SensorClientAdress" });
            DropPrimaryKey("dbo.Sensors");
            AlterColumn("dbo.Sensors", "ClientAddress", c => c.String(maxLength: 4));
            AlterColumn("dbo.Sensors", "GatewayAddress", c => c.String(maxLength: 4));
            AlterColumn("dbo.Sensors", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.SensorReadings", "SensorClientAdress");
            DropColumn("dbo.SensorReadings", "SensorGatewayAdress");
            AddPrimaryKey("dbo.Sensors", "Id");
            CreateIndex("dbo.SensorReadings", "SensorId");
            AddForeignKey("dbo.SensorReadings", "SensorId", "dbo.Sensors", "Id", cascadeDelete: true);
        }
    }
}
