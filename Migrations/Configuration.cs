namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CentralisationV0.Models.Entities.CentralisationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true; // Activer les migrations automatiques
            AutomaticMigrationDataLossAllowed = true; // Permettre la perte de données
        }

        protected override void Seed(CentralisationV0.Models.Entities.CentralisationContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
