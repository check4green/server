namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNameFieldsToSensor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sensors", "Name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sensors", "Name");
        }
    }
}
