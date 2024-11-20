namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientContactClientArcGisRelations : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("public.ArcgisSolutionClients");
            AddPrimaryKey("public.ArcgisSolutionClients", new[] { "Client_idClient", "ArcgisSolution_idArcgisSolution" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("public.ArcgisSolutionClients");
            AddPrimaryKey("public.ArcgisSolutionClients", new[] { "ArcgisSolution_idArcgisSolution", "Client_idClient" });
        }
    }
}
