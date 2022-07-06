namespace Champions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Champions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        Email = c.String(),
                        AdminCode = c.Int(nullable: false),
                        ResetPasswordCode = c.String(),
                        Image = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CustomerModels");
        }
    }
}
