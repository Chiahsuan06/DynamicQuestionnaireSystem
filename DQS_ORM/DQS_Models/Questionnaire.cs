namespace DQS_ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Questionnaire
    {
        public int QuestionnaireID { get; set; }

        [Key]
        public int TopicNum { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string TopicDescription { get; set; }
    }
}
