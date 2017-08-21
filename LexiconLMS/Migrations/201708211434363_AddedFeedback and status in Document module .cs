namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFeedbackandstatusinDocumentmodule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "FeedBack", c => c.String());
            AddColumn("dbo.Documents", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "Status");
            DropColumn("dbo.Documents", "FeedBack");
        }
    }
}
