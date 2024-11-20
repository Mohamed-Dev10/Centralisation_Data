namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.CoordinateSystems",
                c => new
                    {
                        IdCoordinateSystem = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.IdCoordinateSystem);
            
            CreateTable(
                "public.Data",
                c => new
                    {
                        IdData = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        AcquisitionDate = c.DateTime(nullable: false),
                        PublicationDate = c.DateTime(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 255),
                        Category = c.String(maxLength: 100),
                        Telecommunication = c.String(maxLength: 255),
                        ThemeId = c.Int(nullable: false),
                        Coverage = c.String(maxLength: 255),
                        SpatialResolution = c.Double(nullable: false),
                        Summary = c.String(maxLength: 500),
                        UrlData = c.String(maxLength: 255),
                        Keywords = c.String(maxLength: 255),
                        DataSize = c.Int(nullable: false),
                        Theme_IdUseConstraint = c.Int(),
                    })
                .PrimaryKey(t => t.IdData)
                .ForeignKey("public.theme", t => t.Theme_IdUseConstraint)
                .Index(t => t.Theme_IdUseConstraint);
            
            CreateTable(
                "public.Locations",
                c => new
                    {
                        IdLocation = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.IdLocation);
            
            CreateTable(
                "public.UseConstraints",
                c => new
                    {
                        IdUseConstraint = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.IdUseConstraint);
            
            CreateTable(
                "public.DataCoordinateSystems",
                c => new
                    {
                        Data_IdData = c.Int(nullable: false),
                        CoordinateSystem_IdCoordinateSystem = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Data_IdData, t.CoordinateSystem_IdCoordinateSystem })
                .ForeignKey("public.Data", t => t.Data_IdData, cascadeDelete: true)
                .ForeignKey("public.CoordinateSystems", t => t.CoordinateSystem_IdCoordinateSystem, cascadeDelete: true)
                .Index(t => t.Data_IdData)
                .Index(t => t.CoordinateSystem_IdCoordinateSystem);
            
            CreateTable(
                "public.LocationDatas",
                c => new
                    {
                        Location_IdLocation = c.Int(nullable: false),
                        Data_IdData = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Location_IdLocation, t.Data_IdData })
                .ForeignKey("public.Locations", t => t.Location_IdLocation, cascadeDelete: true)
                .ForeignKey("public.Data", t => t.Data_IdData, cascadeDelete: true)
                .Index(t => t.Location_IdLocation)
                .Index(t => t.Data_IdData);
            
            CreateTable(
                "public.UseConstraintDatas",
                c => new
                    {
                        UseConstraint_IdUseConstraint = c.Int(nullable: false),
                        Data_IdData = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UseConstraint_IdUseConstraint, t.Data_IdData })
                .ForeignKey("public.UseConstraints", t => t.UseConstraint_IdUseConstraint, cascadeDelete: true)
                .ForeignKey("public.Data", t => t.Data_IdData, cascadeDelete: true)
                .Index(t => t.UseConstraint_IdUseConstraint)
                .Index(t => t.Data_IdData);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.UseConstraintDatas", "Data_IdData", "public.Data");
            DropForeignKey("public.UseConstraintDatas", "UseConstraint_IdUseConstraint", "public.UseConstraints");
            DropForeignKey("public.Data", "Theme_IdUseConstraint", "public.theme");
            DropForeignKey("public.LocationDatas", "Data_IdData", "public.Data");
            DropForeignKey("public.LocationDatas", "Location_IdLocation", "public.Locations");
            DropForeignKey("public.DataCoordinateSystems", "CoordinateSystem_IdCoordinateSystem", "public.CoordinateSystems");
            DropForeignKey("public.DataCoordinateSystems", "Data_IdData", "public.Data");
            DropIndex("public.UseConstraintDatas", new[] { "Data_IdData" });
            DropIndex("public.UseConstraintDatas", new[] { "UseConstraint_IdUseConstraint" });
            DropIndex("public.LocationDatas", new[] { "Data_IdData" });
            DropIndex("public.LocationDatas", new[] { "Location_IdLocation" });
            DropIndex("public.DataCoordinateSystems", new[] { "CoordinateSystem_IdCoordinateSystem" });
            DropIndex("public.DataCoordinateSystems", new[] { "Data_IdData" });
            DropIndex("public.Data", new[] { "Theme_IdUseConstraint" });
            DropTable("public.UseConstraintDatas");
            DropTable("public.LocationDatas");
            DropTable("public.DataCoordinateSystems");
            DropTable("public.UseConstraints");
            DropTable("public.Locations");
            DropTable("public.Data");
            DropTable("public.CoordinateSystems");
        }
    }
}
