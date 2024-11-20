namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDataBaseForeignKeyToData : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("public.Data", "DataBase_idDataBase", "public.database");
            DropIndex("public.Data", new[] { "DataBase_idDataBase" });
            AlterColumn("public.Data", "DataBase_idDataBase", c => c.Int(nullable: false));
            CreateIndex("public.Data", "DataBase_idDataBase");
            AddForeignKey("public.Data", "DataBase_idDataBase", "public.database", "idDataBase", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("public.Data", "DataBase_idDataBase", "public.database");
            DropIndex("public.Data", new[] { "DataBase_idDataBase" });
            AlterColumn("public.Data", "DataBase_idDataBase", c => c.Int());
            CreateIndex("public.Data", "DataBase_idDataBase");
            AddForeignKey("public.Data", "DataBase_idDataBase", "public.database", "idDataBase");
        }
    }
}
