namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCollaborationDatasTable : DbMigration
    {
        public override void Up()
        {
            
            DropPrimaryKey("public.CollaborationDatas");
            AddPrimaryKey("public.CollaborationDatas", new[] { "Collaboration_idCollaborateur", "Data_IdData" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("public.CollaborationDatas");
            AddPrimaryKey("public.CollaborationDatas", new[] { "Data_IdData", "Collaboration_idCollaborateur" });
            RenameTable(name: "public.CollaborationDatas", newName: "DataCollaborations");
        }
    }
}
