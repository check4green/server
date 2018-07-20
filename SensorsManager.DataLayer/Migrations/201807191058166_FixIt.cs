namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixIt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SensorTypes", "User_Id", "dbo.Users");
            DropForeignKey("dbo.SensorTypes", "UserId_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "SensorType_Id", "dbo.SensorTypes");
            DropIndex("dbo.SensorTypes", new[] { "User_Id" });
            DropIndex("dbo.SensorTypes", new[] { "UserId_Id" });
            DropIndex("dbo.Users", new[] { "SensorType_Id" });
            AlterColumn("dbo.Users", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Users", "CompanyName", c => c.String());
            AlterColumn("dbo.Users", "Country", c => c.String(nullable: false));
            DropColumn("dbo.SensorTypes", "User_Id");
            DropColumn("dbo.SensorTypes", "UserId_Id");
            DropColumn("dbo.Users", "SensorTypeId");
            DropColumn("dbo.Users", "SensorType_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "SensorType_Id", c => c.Int());
            AddColumn("dbo.Users", "SensorTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.SensorTypes", "UserId_Id", c => c.Int());
            AddColumn("dbo.SensorTypes", "User_Id", c => c.Int());
            AlterColumn("dbo.Users", "Country", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "CompanyName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Users", "Email", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Users", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Users", "FirstName", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.Users", "SensorType_Id");
            CreateIndex("dbo.SensorTypes", "UserId_Id");
            CreateIndex("dbo.SensorTypes", "User_Id");
            AddForeignKey("dbo.Users", "SensorType_Id", "dbo.SensorTypes", "Id");
            AddForeignKey("dbo.SensorTypes", "UserId_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.SensorTypes", "User_Id", "dbo.Users", "Id");
        }
    }
}
