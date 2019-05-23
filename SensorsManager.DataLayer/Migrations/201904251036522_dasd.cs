namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dasd : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Gateways", "LastSignalDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Gateways", "LastSignalDate", c => c.DateTime());
        }
    }
}
