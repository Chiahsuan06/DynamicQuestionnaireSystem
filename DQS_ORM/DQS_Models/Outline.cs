namespace DQS_ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Outline")]
    public partial class Outline
    {
        [Key]
        public int QuestionnaireID { get; set; }

        public Guid QuestionnaireNum { get; set; }

        [Required]
        [StringLength(50)]
        public string Heading { get; set; }

        [Required]
        [StringLength(10)]
        public string Vote { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
