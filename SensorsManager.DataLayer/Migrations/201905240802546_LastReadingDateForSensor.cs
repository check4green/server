namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastReadingDateForSensor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sensors", "LastReadingDate", c => c.DateTimeOffset(precision: 7));
            AddColumn("dbo.Sensors", "LastInsertDate", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sensors", "LastInsertDate");
            DropColumn("dbo.Sensors", "LastReadingDate");
        }
    }
}
