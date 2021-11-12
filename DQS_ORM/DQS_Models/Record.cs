namespace DQS_ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Record")]
    public partial class Record
    {
        [Key]
        public int RecordNum { get; set; }

        public int QuestionnaireID { get; set; }

        public Guid AnswererID { get; set; }

        [Required]
        [StringLength(50)]
        public string AnsName { get; set; }

        public int AnsPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string AnsEmail { get; set; }

        public int AnsAge { get; set; }

        public DateTime AnsTime { get; set; }
    }
}
