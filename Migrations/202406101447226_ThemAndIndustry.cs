namespace CentralisationV0.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ThemAndIndustry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "public.industry",
                c => new
                    {
                        idIndustry = c.Int(nullable: false, identity: true),
                        nomIndustry = c.String(),
                        Keywords = c.String(),
                        ThemeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idIndustry)
                .ForeignKey("public.theme", t => t.ThemeId, cascadeDelete: true)
                .Index(t => t.ThemeId);
            
            CreateTable(
                "public.theme",
                c => new
                    {
                        IdTheme = c.Int(nullable: false, identity: true),
                        nom = c.String(),
                        description = c.String(),
                        Keywords = c.String(),
                    })
                .PrimaryKey(t => t.IdTheme);
            
            CreateTable(
                "public.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "public.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("public.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("public.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "public.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nom = c.String(),
                        Prenom = c.String(),
                        DateNaissance = c.DateTime(nullable: false),
                        Address = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "public.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("public.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "public.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("public.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("public.AspNetUserRoles", "UserId", "public.AspNetUsers");
            DropForeignKey("public.AspNetUserLogins", "UserId", "public.AspNetUsers");
            DropForeignKey("public.AspNetUserClaims", "UserId", "public.AspNetUsers");
            DropForeignKey("public.AspNetUserRoles", "RoleId", "public.AspNetRoles");
            DropForeignKey("public.industry", "ThemeId", "public.theme");
            DropIndex("public.AspNetUserLogins", new[] { "UserId" });
            DropIndex("public.AspNetUserClaims", new[] { "UserId" });
            DropIndex("public.AspNetUsers", "UserNameIndex");
            DropIndex("public.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("public.AspNetUserRoles", new[] { "UserId" });
            DropIndex("public.AspNetRoles", "RoleNameIndex");
            DropIndex("public.industry", new[] { "ThemeId" });
            DropTable("public.AspNetUserLogins");
            DropTable("public.AspNetUserClaims");
            DropTable("public.AspNetUsers");
            DropTable("public.AspNetUserRoles");
            DropTable("public.AspNetRoles");
            DropTable("public.theme");
            DropTable("public.industry");
        }
    }
}
