namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.AspNetUsers", "Proffession", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("public.AspNetUsers", "Proffession");
        }
    }
}
