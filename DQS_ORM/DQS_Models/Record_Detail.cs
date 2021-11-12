namespace DQS_ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Record Details")]
    public partial class Record_Detail
    {
        public int RecordNum { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ReplicationNum { get; set; }

        public int TopicNum { get; set; }

        public int? OptionsNum { get; set; }
    }
}
