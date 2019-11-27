namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Razvan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GatewayConnections",
                c => new
                    {
                        Gateway_Id = c.Int(nullable: false),
                        Sensor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Gateway_Id, t.Sensor_Id })
                .ForeignKey("dbo.Gateways", t => t.Gateway_Id, cascadeDelete: true)
                .Index(t => t.Gateway_Id);
            
            CreateTable(
                "dbo.Gateways",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Network_Id = c.Int(nullable: false),
                        Address = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 50),
                        ProductionDate = c.DateTime(nullable: false),
                        LastSignalDate = c.DateTime(),
                        LastSensorDate = c.DateTimeOffset(precision: 7),
                        UploadInterval = c.Int(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Networks", t => t.Network_Id, cascadeDelete: true)
                .Index(t => t.Network_Id);
            
            CreateTable(
                "dbo.Networks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(nullable: false, maxLength: 10, fixedLength: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        User_Id = c.Int(nullable: false),
                        ProductionDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Sensors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Network_Id = c.Int(nullable: false),
                        SensorType_Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 10),
                        ProductionDate = c.DateTime(nullable: false),
                        LastReadingDate = c.DateTimeOffset(precision: 7),
                        LastInsertDate = c.DateTimeOffset(precision: 7),
                        UploadInterval = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Networks", t => t.Network_Id, cascadeDelete: true)
                .ForeignKey("dbo.SensorTypes", t => t.SensorType_Id, cascadeDelete: true)
                .Index(t => t.Network_Id)
                .Index(t => t.SensorType_Id);
            
            CreateTable(
                "dbo.SensorReadings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sensor_Id = c.Int(nullable: false),
                        GatewayAddress = c.String(maxLength: 10, fixedLength: true),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReadingDate = c.DateTimeOffset(nullable: false, precision: 7),
                        InsertDate = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sensors", t => t.Sensor_Id, cascadeDelete: true)
                .Index(t => t.Sensor_Id);
            
            CreateTable(
                "dbo.SensorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Measure_Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 100),
                        MinValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Measurements", t => t.Measure_Id, cascadeDelete: true)
                .Index(t => t.Measure_Id);
            
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
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false),
                        CompanyName = c.String(maxLength: 100),
                        Country = c.String(nullable: false, maxLength: 50),
                        PhoneNumber = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GatewayConnections", "Gateway_Id", "dbo.Gateways");
            DropForeignKey("dbo.Gateways", "Network_Id", "dbo.Networks");
            DropForeignKey("dbo.Networks", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Sensors", "SensorType_Id", "dbo.SensorTypes");
            DropForeignKey("dbo.SensorTypes", "Measure_Id", "dbo.Measurements");
            DropForeignKey("dbo.SensorReadings", "Sensor_Id", "dbo.Sensors");
            DropForeignKey("dbo.Sensors", "Network_Id", "dbo.Networks");
            DropIndex("dbo.SensorTypes", new[] { "Measure_Id" });
            DropIndex("dbo.SensorReadings", new[] { "Sensor_Id" });
            DropIndex("dbo.Sensors", new[] { "SensorType_Id" });
            DropIndex("dbo.Sensors", new[] { "Network_Id" });
            DropIndex("dbo.Networks", new[] { "User_Id" });
            DropIndex("dbo.Gateways", new[] { "Network_Id" });
            DropIndex("dbo.GatewayConnections", new[] { "Gateway_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.Measurements");
            DropTable("dbo.SensorTypes");
            DropTable("dbo.SensorReadings");
            DropTable("dbo.Sensors");
            DropTable("dbo.Networks");
            DropTable("dbo.Gateways");
            DropTable("dbo.GatewayConnections");
        }
    }
}
