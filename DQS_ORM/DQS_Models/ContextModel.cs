using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DQS_ORM
{
    public partial class ContextModel : DbContext
    {
        public ContextModel()
            : base("name=DataContextConnectionString")
        {
        }

        public virtual DbSet<Information> Information { get; set; }
        public virtual DbSet<Outline> Outlines { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Questionnaire> Questionnaires { get; set; }
        public virtual DbSet<Record> Records { get; set; }
        public virtual DbSet<Record_Detail> Record_Details { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>()
                .Property(e => e.OptionsDescription)
                .IsUnicode(false);

            modelBuilder.Entity<Questionnaire>()
                .Property(e => e.TopicDescription)
                .IsUnicode(false);
        }
    }
}
