namespace ODataVersioningSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class upgradetov2models : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.DbProducts");
            CreateTable(
                "dbo.DbProducts",
                c => new
                {
                    ID = c.Long(nullable: false),
                    Name = c.String(),
                    ReleaseDate = c.DateTime(),
                    SupportedUntil = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);
            CreateTable(
                "dbo.DbProductFamilies",
                c => new
                {
                    ID = c.Int(nullable: false),
                    Name = c.String(),
                    Description = c.String(),
                    DbSupplier_ID = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DbSuppliers", t => t.DbSupplier_ID)
                .Index(t => t.DbSupplier_ID);

            CreateTable(
                "dbo.DbSuppliers",
                c => new
                {
                    ID = c.Int(nullable: false),
                    Name = c.String(),
                    Address_Street = c.String(),
                    Address_City = c.String(),
                    Address_State = c.String(),
                    Address_ZipCode = c.String(),
                    Address_Country = c.String(),
                })
                .PrimaryKey(t => t.ID);

            AddColumn("dbo.DbProducts", "Family_ID", c => c.Int());
            AddForeignKey("dbo.DbProducts", "Family_ID", "dbo.DbProductFamilies", "ID");
            CreateIndex("dbo.DbProducts", "Family_ID");
        }

        public override void Down()
        {
            DropIndex("dbo.DbProductFamilies", new[] { "DbSupplier_ID" });
            DropIndex("dbo.DbProducts", new[] { "Family_ID" });
            DropForeignKey("dbo.DbProductFamilies", "DbSupplier_ID", "dbo.DbSuppliers");
            DropForeignKey("dbo.DbProducts", "Family_ID", "dbo.DbProductFamilies");
            AlterColumn("dbo.DbProducts", "ID", c => c.Int(nullable: false));
            DropColumn("dbo.DbProducts", "Family_ID");
            DropTable("dbo.DbSuppliers");
            DropTable("dbo.DbProductFamilies");
        }
    }
}
