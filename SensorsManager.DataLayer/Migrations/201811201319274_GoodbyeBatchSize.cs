namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoodbyeBatchSize : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Sensors", "BatchSize");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sensors", "BatchSize", c => c.Int(nullable: false));
        }
    }
}
