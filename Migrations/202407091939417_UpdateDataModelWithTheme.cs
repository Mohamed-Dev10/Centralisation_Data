namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateDataModelWithTheme : DbMigration
    {
        public override void Up()
        {
            // Ajouter une valeur par défaut à la colonne avant de la modifier
            AlterColumn("public.Data", "Theme_IdUseConstraint", c => c.Int(nullable: false, defaultValue: 1));

            // Mettre à jour toutes les valeurs NULL de Theme_IdUseConstraint à une valeur par défaut (par exemple 1)
            Sql("UPDATE \"public\".\"Data\" SET \"Theme_IdUseConstraint\" = 1 WHERE \"Theme_IdUseConstraint\" IS NULL");

            // Ensuite, procéder avec la migration
            DropForeignKey("public.Data", "Theme_IdUseConstraint", "public.theme");
            DropIndex("public.Data", new[] { "Theme_IdUseConstraint" });
            DropColumn("public.Data", "ThemeId");
            RenameColumn(table: "public.Data", name: "Theme_IdUseConstraint", newName: "ThemeId");
            AlterColumn("public.Data", "ThemeId", c => c.Int(nullable: false));
            CreateIndex("public.Data", "ThemeId");
            AddForeignKey("public.Data", "ThemeId", "public.theme", "IdTheme", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("public.Data", "ThemeId", "public.theme");
            DropIndex("public.Data", new[] { "ThemeId" });
            AlterColumn("public.Data", "ThemeId", c => c.Int());
            RenameColumn(table: "public.Data", name: "ThemeId", newName: "Theme_IdUseConstraint");
            AddColumn("public.Data", "ThemeId", c => c.Int(nullable: false));
            CreateIndex("public.Data", "Theme_IdUseConstraint");
            AddForeignKey("public.Data", "Theme_IdUseConstraint", "public.theme", "IdTheme");
        }
    }
}
