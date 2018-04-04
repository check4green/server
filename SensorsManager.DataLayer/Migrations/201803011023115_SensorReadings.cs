namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SensorReadings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SensorReadings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SensorId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReadingDate = c.DateTimeOffset(nullable: false, precision: 7),
                        InsertDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sensors", t => t.SensorId, cascadeDelete: true)
                .Index(t => t.SensorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SensorReadings", "SensorId", "dbo.Sensors");
            DropIndex("dbo.SensorReadings", new[] { "SensorId" });
            DropTable("dbo.SensorReadings");
        }
    }
}
