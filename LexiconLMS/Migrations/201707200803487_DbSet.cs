namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DbSet : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        ActivityType = c.Int(nullable: false),
                        ActivityName = c.String(),
                        ActivityStartDate = c.DateTime(nullable: false),
                        ActivityEndDate = c.DateTime(nullable: false),
                        ActivityDescription = c.String(),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        DocId = c.Int(nullable: false, identity: true),
                        DocName = c.String(),
                        DocDescription = c.String(),
                        DocTimestamp = c.DateTime(nullable: false),
                        DocDeadline = c.DateTime(),
                        UserId = c.Int(nullable: false),
                        CourseId = c.Int(),
                        ModuleId = c.Int(),
                        ActivityId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.DocId)
                .ForeignKey("dbo.Activities", t => t.ActivityId)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.CourseId)
                .Index(t => t.ModuleId)
                .Index(t => t.ActivityId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        CourseStartDate = c.DateTime(nullable: false),
                        CourseEndDate = c.DateTime(nullable: false),
                        CourseDescription = c.String(),
                    })
                .PrimaryKey(t => t.CourseId);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        ModuleName = c.String(),
                        ModuleDescription = c.String(),
                        ModuleStartDate = c.DateTime(nullable: false),
                        ModuleEndDate = c.DateTime(nullable: false),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            AddColumn("dbo.AspNetUsers", "UserFirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "UserLastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "UserStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "CourseId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "CourseId");
            AddForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Courses", "CourseId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Documents", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Modules", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Documents", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Documents", "ActivityId", "dbo.Activities");
            DropIndex("dbo.AspNetUsers", new[] { "CourseId" });
            DropIndex("dbo.Modules", new[] { "CourseId" });
            DropIndex("dbo.Documents", new[] { "User_Id" });
            DropIndex("dbo.Documents", new[] { "ActivityId" });
            DropIndex("dbo.Documents", new[] { "ModuleId" });
            DropIndex("dbo.Documents", new[] { "CourseId" });
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            DropColumn("dbo.AspNetUsers", "CourseId");
            DropColumn("dbo.AspNetUsers", "UserStartDate");
            DropColumn("dbo.AspNetUsers", "UserLastName");
            DropColumn("dbo.AspNetUsers", "UserFirstName");
            DropTable("dbo.Modules");
            DropTable("dbo.Courses");
            DropTable("dbo.Documents");
            DropTable("dbo.Activities");
        }
    }
}
