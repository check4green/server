namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelError : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sensors", "UploadInteval", c => c.Int(nullable: false));
            DropColumn("dbo.Sensors", "UploadInterval");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sensors", "UploadInterval", c => c.Int(nullable: false));
            DropColumn("dbo.Sensors", "UploadInteval");
        }
    }
}
