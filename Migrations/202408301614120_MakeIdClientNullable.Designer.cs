﻿// <auto-generated />
namespace CentralisationV0.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.4.0")]
    public sealed partial class MakeIdClientNullable : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(MakeIdClientNullable));
        
        string IMigrationMetadata.Id
        {
            get { return "202408301614120_MakeIdClientNullable"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}