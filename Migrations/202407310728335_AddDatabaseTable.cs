namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDatabaseTable : DbMigration
    {
        public override void Up()
        {

            CreateTable(
                "public.database",
                c => new
                {
                    idDataBase = c.Int(nullable: false, identity: true),
                    DataBaseName = c.String(),
                    Owner = c.String(),
                    createdDate = c.DateTime(nullable: false),
                    description = c.String(),
                    Keywords = c.String(),
                })
                .PrimaryKey(t => t.idDataBase);

            CreateTable(
               "public.DataBaseDatas",
               c => new
               {
                   DataBase_idDataBase = c.Int(nullable: false),
                   Data_IdData = c.Int(nullable: false),
               })
               .PrimaryKey(t => new { t.DataBase_idDataBase, t.Data_IdData })
               .ForeignKey("public.database", t => t.DataBase_idDataBase, cascadeDelete: true)
               .ForeignKey("public.Data", t => t.Data_IdData, cascadeDelete: true)
               .Index(t => t.DataBase_idDataBase)
               .Index(t => t.Data_IdData);
        }

        public override void Down()
        {
            DropForeignKey("public.DataBaseDatas", "Data_IdData", "public.Data");
            DropForeignKey("public.DataBaseDatas", "DataBase_idDataBase", "public.database");

            DropIndex("public.DataBaseDatas", new[] { "Data_IdData" });
            DropIndex("public.DataBaseDatas", new[] { "DataBase_idDataBase" });

            DropTable("public.DataBaseDatas");
            DropTable("public.database");
        }

    }
}
