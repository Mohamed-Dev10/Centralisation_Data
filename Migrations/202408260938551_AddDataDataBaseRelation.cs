namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataDataBaseRelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("public.Data", "DataBase_idDataBase", "public.database");
            DropIndex("public.Data", new[] { "DataBase_idDataBase" });
            CreateTable(
                "public.DataDataBases",
                c => new
                    {
                        Data_IdData = c.Int(nullable: false),
                        DataBase_idDataBase = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Data_IdData, t.DataBase_idDataBase })
                .ForeignKey("public.Data", t => t.Data_IdData, cascadeDelete: true)
                .ForeignKey("public.database", t => t.DataBase_idDataBase, cascadeDelete: true)
                .Index(t => t.Data_IdData)
                .Index(t => t.DataBase_idDataBase);
            
            DropColumn("public.Data", "DataBase_idDataBase");
        }
        
        public override void Down()
        {
            AddColumn("public.Data", "DataBase_idDataBase", c => c.Int(nullable: false));
            DropForeignKey("public.DataDataBases", "DataBase_idDataBase", "public.database");
            DropForeignKey("public.DataDataBases", "Data_IdData", "public.Data");
            DropIndex("public.DataDataBases", new[] { "DataBase_idDataBase" });
            DropIndex("public.DataDataBases", new[] { "Data_IdData" });
            DropTable("public.DataDataBases");
            CreateIndex("public.Data", "DataBase_idDataBase");
            AddForeignKey("public.Data", "DataBase_idDataBase", "public.database", "idDataBase", cascadeDelete: true);
        }
    }
}
