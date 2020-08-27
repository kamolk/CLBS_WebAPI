namespace CLBS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CLBSMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CLBSData",
                c => new
                    {
                        OPC = c.String(nullable: false, maxLength: 50),
                        Eye = c.String(),
                        SurfaceID = c.String(nullable: false, maxLength: 30),
                        CreateBy = c.String(),
                        CreateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OPC)
                .ForeignKey("dbo.STANDARDData", t => t.SurfaceID, cascadeDelete: true)
                .Index(t => t.SurfaceID);
            
            CreateTable(
                "dbo.PrismCol",
                c => new
                    {
                        cceCode = c.String(nullable: false, maxLength: 50),
                        slopeRx = c.String(nullable: false),
                        slopeRy = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.cceCode)
                .ForeignKey("dbo.CLBSData", t => t.cceCode, cascadeDelete: true)
                .Index(t => t.cceCode);
            
            CreateTable(
                "dbo.STANDARDData",
                c => new
                    {
                        SurfaceID = c.String(nullable: false, maxLength: 30),
                        SlopeX = c.String(),
                        SlopeY = c.String(),
                        constantX = c.String(),
                        constantY = c.String(),
                        Base = c.Double(nullable: false),
                        Addition = c.Double(nullable: false),
                        Material = c.String(),
                        Design = c.String(),
                        CreateBy = c.String(),
                    })
                .PrimaryKey(t => t.SurfaceID);
            
            CreateTable(
                "dbo.sysdiagrams",
                c => new
                    {
                        diagram_id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 128),
                        principal_id = c.Int(nullable: false),
                        version = c.Int(),
                        definition = c.Binary(),
                    })
                .PrimaryKey(t => t.diagram_id);
            
            CreateTable(
                "dbo.Users1",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserEmail = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserEmail = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CLBSData", "SurfaceID", "dbo.STANDARDData");
            DropForeignKey("dbo.PrismCol", "cceCode", "dbo.CLBSData");
            DropIndex("dbo.PrismCol", new[] { "cceCode" });
            DropIndex("dbo.CLBSData", new[] { "SurfaceID" });
            DropTable("dbo.Users");
            DropTable("dbo.Users1");
            DropTable("dbo.sysdiagrams");
            DropTable("dbo.STANDARDData");
            DropTable("dbo.PrismCol");
            DropTable("dbo.CLBSData");
        }
    }
}
