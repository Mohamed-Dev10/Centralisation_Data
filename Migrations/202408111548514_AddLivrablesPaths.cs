namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLivrablesPaths : DbMigration
    {
        public override void Up()
        {
            AddColumn("public.Collaboration", "LivrablesPaths", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("public.Collaboration", "LivrablesPaths");
        }
    }
}
