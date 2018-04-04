namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Measurements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UnitOfMeasure = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sensors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SensorTypeId = c.Int(nullable: false),
                        ProductionDate = c.DateTime(nullable: false),
                        UploadInteval = c.Int(nullable: false),
                        BatchSize = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SensorTypes", t => t.SensorTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.SensorTypeId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SensorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 50),
                        MinValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MeasureId = c.Int(nullable: false),
                        Multiplier = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Measurements", t => t.MeasureId, cascadeDelete: true)
                .Index(t => t.MeasureId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sensors", "UserId", "dbo.Users");
            DropForeignKey("dbo.Sensors", "SensorTypeId", "dbo.SensorTypes");
            DropForeignKey("dbo.SensorTypes", "MeasureId", "dbo.Measurements");
            DropIndex("dbo.SensorTypes", new[] { "MeasureId" });
            DropIndex("dbo.Sensors", new[] { "UserId" });
            DropIndex("dbo.Sensors", new[] { "SensorTypeId" });
            DropTable("dbo.Users");
            DropTable("dbo.SensorTypes");
            DropTable("dbo.Sensors");
            DropTable("dbo.Measurements");
        }
    }
}
