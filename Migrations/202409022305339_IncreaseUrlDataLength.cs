namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreaseUrlDataLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("public.Data", "UrlData", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("public.Data", "UrlData", c => c.String(maxLength: 255));
        }
    }
}
