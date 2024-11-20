using CentralisationdeDonnee.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CentralisationV0.Models.Entities
{
    public class CentralisationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Data> Datas { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<CoordinateSystem> CoordinateSystems { get; set; }
        public DbSet<UseConstraint> UseConstraints { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Collaboration> Collaborations { get; set; }

        public DbSet<TypeCollaboration> typeCollaborations { get; set; }
        public DbSet<DataTyp> DataTyps { get; set; }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ArcgisSolution> ArcgisSolutions { get; set; }


        public DbSet<ContactClient> ContactClients { get; set; }

        public DbSet<DataBase> DataBases { get; set; }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }

        public CentralisationContext() : base(nameOrConnectionString: "PostgresConnection")
        {
            this.Database.CommandTimeout = 120;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            // Relation un-à-plusieurs entre Client et ContactClient
            modelBuilder.Entity<Client>()
                .HasMany(c => c.ContactClients)
                .WithRequired(cc => cc.Client)
                .HasForeignKey(cc => cc.idClient)
                .WillCascadeOnDelete(true); // Active la suppression en cascade

            // Relation plusieurs-à-plusieurs entre Client et ArcGisSolution
            modelBuilder.Entity<Client>()
                .HasMany(c => c.ArcgisSolutions)
                .WithMany(ags => ags.Clients)
                .Map(m =>
                {
                    m.ToTable("ArcgisSolutionClients"); // Nom de la table de jonction doit être "ArcgisSolutionClients"
        m.MapLeftKey("Client_idClient");
                    m.MapRightKey("ArcgisSolution_idArcgisSolution");
                });


            // Autres configurations déjà présentes
            modelBuilder.Entity<Theme>()
                .HasKey(t => t.IdTheme);

            modelBuilder.Entity<Data>()
                .HasRequired(d => d.Theme)
                .WithMany(t => t.Datas)
                .HasForeignKey(d => d.ThemeId);

            modelBuilder.Entity<Industry>()
                .HasRequired(i => i.Theme)
                .WithMany(t => t.Industries)
                .HasForeignKey(i => i.ThemeId);
            // Nouvelle configuration de la relation plusieurs-à-plusieurs entre Data et DataBase
            modelBuilder.Entity<Data>()
                .HasMany(d => d.DataBases)
                .WithMany(db => db.Datas)
                .Map(m =>
                {
                    m.ToTable("DataDataBases"); // Nom de la table de jonction
            m.MapLeftKey("Data_IdData"); // Clé étrangère vers Data
            m.MapRightKey("DataBase_idDataBase"); // Clé étrangère vers DataBase
        });


            modelBuilder.Entity<Collaboration>()
                .HasMany(c => c.Datas)
                .WithMany(d => d.Collaborations)
                .Map(m =>
                {
                    m.ToTable("CollaborationDatas");
                    m.MapLeftKey("Collaboration_idCollaborateur");
                    m.MapRightKey("Data_IdData");
                });

            // Configuration de la clé primaire
            modelBuilder.Entity<Client>()
                .HasKey(c => c.idClient); // Assurez-vous que la clé est bien définie

            // Configuration de la relation un-à-plusieurs entre Client et Collaboration
            modelBuilder.Entity<Collaboration>()
                .HasRequired(c => c.Client)
                .WithMany(client => client.Collaborations)
                .HasForeignKey(c => c.idClient);
            modelBuilder.Entity<Collaboration>()
                .HasRequired(c => c.TypeCollaboration)
                .WithMany()
                .HasForeignKey(c => c.idTypeCollaboration);

            modelBuilder.Entity<Data>()
                .HasRequired(d => d.CoordinateSystem)
                .WithMany()
                .HasForeignKey(d => d.CoordinateSystemId);

            base.OnModelCreating(modelBuilder);
        }

        public static CentralisationContext Create()
        {
            return new CentralisationContext();
        }
    }
}