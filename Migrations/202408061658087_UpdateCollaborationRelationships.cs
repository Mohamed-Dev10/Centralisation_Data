namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCollaborationRelationships : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("public.Collaboration", "idClient", "public.Client");
            DropForeignKey("public.Collaboration", "idTypeCollaboration", "public.TypeCollaboration");
            AddColumn("public.Collaboration", "TypeCollaboration_idTypeCollaboration", c => c.Int());
            AddColumn("public.Collaboration", "Client_idClient", c => c.Int());
            CreateIndex("public.Collaboration", "TypeCollaboration_idTypeCollaboration");
            CreateIndex("public.Collaboration", "Client_idClient");
            AddForeignKey("public.Collaboration", "Client_idClient", "public.Client", "idClient");
            AddForeignKey("public.Collaboration", "TypeCollaboration_idTypeCollaboration", "public.TypeCollaboration", "idTypeCollaboration");
        }
        
        public override void Down()
        {
            DropForeignKey("public.Collaboration", "TypeCollaboration_idTypeCollaboration", "public.TypeCollaboration");
            DropForeignKey("public.Collaboration", "Client_idClient", "public.Client");
            DropIndex("public.Collaboration", new[] { "Client_idClient" });
            DropIndex("public.Collaboration", new[] { "TypeCollaboration_idTypeCollaboration" });
            DropColumn("public.Collaboration", "Client_idClient");
            DropColumn("public.Collaboration", "TypeCollaboration_idTypeCollaboration");
            AddForeignKey("public.Collaboration", "idTypeCollaboration", "public.TypeCollaboration", "idTypeCollaboration", cascadeDelete: true);
            AddForeignKey("public.Collaboration", "idClient", "public.Client", "idClient", cascadeDelete: true);
        }
    }
}
