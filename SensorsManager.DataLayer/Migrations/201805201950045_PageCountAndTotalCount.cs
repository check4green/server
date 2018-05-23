namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PageCountAndTotalCount : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sensors", "GatewayAddress", c => c.String(maxLength: 4));
            AlterColumn("dbo.Sensors", "ClientAddress", c => c.String(maxLength: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sensors", "ClientAddress", c => c.String());
            AlterColumn("dbo.Sensors", "GatewayAddress", c => c.String());
        }
    }
}
