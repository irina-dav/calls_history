using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CallsHistory.Models
{
    [Table("freepbx_settings")]
    public class PbxSettings
    {
        [Key]
        [Column("keyword")]
        public string Name { get; set; }

        [Column("value")]
        public string Value { get; set; }
    }
}
