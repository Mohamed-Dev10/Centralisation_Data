namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnBooleanIsReset : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.AspNetUsers", "IsReseted", c => c.Boolean(nullable: false, defaultValue: false)) ;
        }
        
        public override void Down()
        {
            DropColumn("public.AspNetUsers", "IsReseted");
        }
    }
}
