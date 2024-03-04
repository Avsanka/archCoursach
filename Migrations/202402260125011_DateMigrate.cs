namespace course.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateMigrate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.parts", "DateAdded", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.parts", "DateAdded");
        }
    }
}
