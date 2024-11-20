namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationDataTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("public.DataCoordinateSystems", "Data_IdData", "public.Data");
            DropForeignKey("public.DataCoordinateSystems", "CoordinateSystem_IdCoordinateSystem", "public.CoordinateSystems");
            DropIndex("public.DataCoordinateSystems", new[] { "Data_IdData" });
            DropIndex("public.DataCoordinateSystems", new[] { "CoordinateSystem_IdCoordinateSystem" });
            CreateTable(
                "public.Collaboration",
                c => new
                    {
                        idCollaborateur = c.Int(nullable: false, identity: true),
                        Titre = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        description = c.String(),
                        Duration = c.Int(nullable: false),
                        MarketNumber = c.Int(nullable: false),
                        Keywords = c.Int(nullable: false),
                        idClient = c.Int(nullable: false),
                        idTypeCollaboration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idCollaborateur)
                .ForeignKey("public.Client", t => t.idClient, cascadeDelete: true)
                .ForeignKey("public.TypeCollaboration", t => t.idTypeCollaboration, cascadeDelete: true)
                .Index(t => t.idClient)
                .Index(t => t.idTypeCollaboration);
            
            CreateTable(
                "public.Client",
                c => new
                    {
                        idClient = c.Int(nullable: false, identity: true),
                        clientName = c.String(),
                        clientAddress = c.String(),
                        clientEmail = c.String(),
                        clientIndustry = c.String(),
                        clientSize = c.Int(nullable: false),
                        clientType = c.String(),
                        Keywords = c.String(),
                    })
                .PrimaryKey(t => t.idClient);
            
            CreateTable(
                "public.ArcgisSolution",
                c => new
                    {
                        idArcgisSolution = c.Int(nullable: false, identity: true),
                        TitredArcgisSolution = c.String(),
                        DescriptionArcgisSolution = c.String(),
                        KeywordsArcgisSolution = c.String(),
                    })
                .PrimaryKey(t => t.idArcgisSolution);
            
            CreateTable(
                "public.ContactClient",
                c => new
                    {
                        idContactClient = c.Int(nullable: false, identity: true),
                        ContactName = c.String(),
                        clientAddress = c.String(),
                        clientEmail = c.String(),
                        clientIndustry = c.String(),
                        idClient = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idContactClient)
                .ForeignKey("public.Client", t => t.idClient, cascadeDelete: true)
                .Index(t => t.idClient);
            
            CreateTable(
                "public.TypeCollaboration",
                c => new
                    {
                        idTypeCollaboration = c.Int(nullable: false, identity: true),
                        NomType = c.String(),
                        Description = c.String(),
                        Keywords = c.String(),
                    })
                .PrimaryKey(t => t.idTypeCollaboration);
            
            CreateTable(
                "public.datatype",
                c => new
                    {
                        iddatatype = c.Int(nullable: false, identity: true),
                        format = c.String(),
                        description = c.String(),
                    })
                .PrimaryKey(t => t.iddatatype);
            
            CreateTable(
                "public.HistoriqueData",
                c => new
                    {
                        idHistoriqueData = c.Int(nullable: false, identity: true),
                        UrlData = c.String(),
                        DateMise_a_Jours = c.DateTime(nullable: false),
                        idData = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idHistoriqueData)
                .ForeignKey("public.Data", t => t.idData, cascadeDelete: true)
                .Index(t => t.idData);
            
            CreateTable(
                "public.ArcgisSolutionClients",
                c => new
                    {
                        ArcgisSolution_idArcgisSolution = c.Int(nullable: false),
                        Client_idClient = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArcgisSolution_idArcgisSolution, t.Client_idClient })
                .ForeignKey("public.ArcgisSolution", t => t.ArcgisSolution_idArcgisSolution, cascadeDelete: true)
                .ForeignKey("public.Client", t => t.Client_idClient, cascadeDelete: true)
                .Index(t => t.ArcgisSolution_idArcgisSolution)
                .Index(t => t.Client_idClient);
            
            CreateTable(
                "public.CollaborationDatas",
                c => new
                    {
                        Collaboration_idCollaborateur = c.Int(nullable: false),
                        Data_IdData = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Collaboration_idCollaborateur, t.Data_IdData })
                .ForeignKey("public.Collaboration", t => t.Collaboration_idCollaborateur, cascadeDelete: true)
                .ForeignKey("public.Data", t => t.Data_IdData, cascadeDelete: true)
                .Index(t => t.Collaboration_idCollaborateur)
                .Index(t => t.Data_IdData);
            
            AddColumn("public.Data", "IndustryId", c => c.Int(nullable: false));
            AddColumn("public.Data", "CoordinateSystemId", c => c.Int(nullable: false));
            AddColumn("public.Data", "DataTypeId", c => c.Int(nullable: false));
            AddColumn("public.AspNetUsers", "Collaboration_idCollaborateur", c => c.Int());
            CreateIndex("public.Data", "IndustryId");
            CreateIndex("public.Data", "CoordinateSystemId");
            CreateIndex("public.Data", "DataTypeId");
            CreateIndex("public.AspNetUsers", "Collaboration_idCollaborateur");
            AddForeignKey("public.AspNetUsers", "Collaboration_idCollaborateur", "public.Collaboration", "idCollaborateur");
            AddForeignKey("public.Data", "CoordinateSystemId", "public.CoordinateSystems", "IdCoordinateSystem", cascadeDelete: true);
            AddForeignKey("public.Data", "DataTypeId", "public.datatype", "iddatatype", cascadeDelete: true);
            AddForeignKey("public.Data", "IndustryId", "public.industry", "idIndustry", cascadeDelete: true);
            DropTable("public.DataCoordinateSystems");
        }
        
        public override void Down()
        {
            CreateTable(
                "public.DataCoordinateSystems",
                c => new
                    {
                        Data_IdData = c.Int(nullable: false),
                        CoordinateSystem_IdCoordinateSystem = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Data_IdData, t.CoordinateSystem_IdCoordinateSystem });
            
            DropForeignKey("public.Data", "IndustryId", "public.industry");
            DropForeignKey("public.HistoriqueData", "idData", "public.Data");
            DropForeignKey("public.Data", "DataTypeId", "public.datatype");
            DropForeignKey("public.Data", "CoordinateSystemId", "public.CoordinateSystems");
            DropForeignKey("public.Collaboration", "idTypeCollaboration", "public.TypeCollaboration");
            DropForeignKey("public.CollaborationDatas", "Data_IdData", "public.Data");
            DropForeignKey("public.CollaborationDatas", "Collaboration_idCollaborateur", "public.Collaboration");
            DropForeignKey("public.Collaboration", "idClient", "public.Client");
            DropForeignKey("public.ContactClient", "idClient", "public.Client");
            DropForeignKey("public.ArcgisSolutionClients", "Client_idClient", "public.Client");
            DropForeignKey("public.ArcgisSolutionClients", "ArcgisSolution_idArcgisSolution", "public.ArcgisSolution");
            DropForeignKey("public.AspNetUsers", "Collaboration_idCollaborateur", "public.Collaboration");
            DropIndex("public.CollaborationDatas", new[] { "Data_IdData" });
            DropIndex("public.CollaborationDatas", new[] { "Collaboration_idCollaborateur" });
            DropIndex("public.ArcgisSolutionClients", new[] { "Client_idClient" });
            DropIndex("public.ArcgisSolutionClients", new[] { "ArcgisSolution_idArcgisSolution" });
            DropIndex("public.HistoriqueData", new[] { "idData" });
            DropIndex("public.ContactClient", new[] { "idClient" });
            DropIndex("public.AspNetUsers", new[] { "Collaboration_idCollaborateur" });
            DropIndex("public.Collaboration", new[] { "idTypeCollaboration" });
            DropIndex("public.Collaboration", new[] { "idClient" });
            DropIndex("public.Data", new[] { "DataTypeId" });
            DropIndex("public.Data", new[] { "CoordinateSystemId" });
            DropIndex("public.Data", new[] { "IndustryId" });
            DropColumn("public.AspNetUsers", "Collaboration_idCollaborateur");
            DropColumn("public.Data", "DataTypeId");
            DropColumn("public.Data", "CoordinateSystemId");
            DropColumn("public.Data", "IndustryId");
            DropTable("public.CollaborationDatas");
            DropTable("public.ArcgisSolutionClients");
            DropTable("public.HistoriqueData");
            DropTable("public.datatype");
            DropTable("public.TypeCollaboration");
            DropTable("public.ContactClient");
            DropTable("public.ArcgisSolution");
            DropTable("public.Client");
            DropTable("public.Collaboration");
            CreateIndex("public.DataCoordinateSystems", "CoordinateSystem_IdCoordinateSystem");
            CreateIndex("public.DataCoordinateSystems", "Data_IdData");
            AddForeignKey("public.DataCoordinateSystems", "CoordinateSystem_IdCoordinateSystem", "public.CoordinateSystems", "IdCoordinateSystem", cascadeDelete: true);
            AddForeignKey("public.DataCoordinateSystems", "Data_IdData", "public.Data", "IdData", cascadeDelete: true);
        }
    }
}
