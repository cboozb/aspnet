namespace ODataVersioningSample.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DbProducts",
                c => new
                {
                    ID = c.Int(nullable: false),
                    Name = c.String(),
                    ReleaseDate = c.DateTime(),
                    SupportedUntil = c.DateTime(),
                })
                .PrimaryKey(t => t.ID);

        }

        public override void Down()
        {
            DropTable("dbo.DbProducts");
        }
    }
}
