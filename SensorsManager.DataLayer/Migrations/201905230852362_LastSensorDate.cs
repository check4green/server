namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastSensorDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Gateways", "LastSensorDate", c => c.DateTimeOffset(precision: 7));
            AlterColumn("dbo.Gateways", "LastSignalDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Gateways", "LastSignalDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Gateways", "LastSensorDate");
        }
    }
}
