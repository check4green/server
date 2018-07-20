namespace SensorsManager.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalInfoForUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CompanyName", c => c.String());
            AddColumn("dbo.Users", "Country", c => c.String(nullable: false));
            AddColumn("dbo.Users", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "PhoneNumber");
            DropColumn("dbo.Users", "Country");
            DropColumn("dbo.Users", "CompanyName");
        }
    }
}
