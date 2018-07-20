namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixEveryThing : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sensors", "User_Id", "dbo.Users");
            DropIndex("dbo.Sensors", new[] { "User_Id" });
            RenameColumn(table: "dbo.Sensors", name: "User_Id", newName: "UserId");
            AlterColumn("dbo.Sensors", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Sensors", "UserId");
            AddForeignKey("dbo.Sensors", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sensors", "UserId", "dbo.Users");
            DropIndex("dbo.Sensors", new[] { "UserId" });
            AlterColumn("dbo.Sensors", "UserId", c => c.Int());
            RenameColumn(table: "dbo.Sensors", name: "UserId", newName: "User_Id");
            CreateIndex("dbo.Sensors", "User_Id");
            AddForeignKey("dbo.Sensors", "User_Id", "dbo.Users", "Id");
        }
    }
}
