namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPasswordResetCodeToApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.AspNetUsers", "PasswordResetCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("public.AspNetUsers", "PasswordResetCode");
        }
    }
}
