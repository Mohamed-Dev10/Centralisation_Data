namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCoordinateSystemRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("public.Data", "CoordinateSystemId", "public.CoordinateSystems");
            AddColumn("public.Data", "CoordinateSystem_IdCoordinateSystem", c => c.Int());
            CreateIndex("public.Data", "CoordinateSystem_IdCoordinateSystem");
            AddForeignKey("public.Data", "CoordinateSystem_IdCoordinateSystem", "public.CoordinateSystems", "IdCoordinateSystem");
        }
        
        public override void Down()
        {
            DropForeignKey("public.Data", "CoordinateSystem_IdCoordinateSystem", "public.CoordinateSystems");
            DropIndex("public.Data", new[] { "CoordinateSystem_IdCoordinateSystem" });
            DropColumn("public.Data", "CoordinateSystem_IdCoordinateSystem");
            AddForeignKey("public.Data", "CoordinateSystemId", "public.CoordinateSystems", "IdCoordinateSystem", cascadeDelete: true);
        }
    }
}
