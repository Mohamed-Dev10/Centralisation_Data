namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsActivatedColumnToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.AspNetUsers", "isActivated", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("public.AspNetUsers", "isActivated");
        }
    }
}
