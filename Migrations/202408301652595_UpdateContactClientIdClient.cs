namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateContactClientIdClient : DbMigration
    {
        public override void Up()
        {
            // Permettre les valeurs NULL pour la colonne idClient
            AlterColumn("public.ContactClient", "idClient", c => c.Int(nullable: true));
        }

        public override void Down()
        {
            // Revenir à la configuration précédente où idClient ne permet pas les valeurs NULL
            AlterColumn("public.ContactClient", "idClient", c => c.Int(nullable: false));
        }
    }
}
