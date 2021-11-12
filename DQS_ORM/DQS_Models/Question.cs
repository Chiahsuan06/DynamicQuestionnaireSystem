namespace DQS_ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Question")]
    public partial class Question
    {
        public int TopicNum { get; set; }

        [Key]
        public int OptionsNum { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string OptionsDescription { get; set; }
    }
}
