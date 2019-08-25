using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    [Table("cdr")]
    public class Call
    {
        //[Key]
        [Column("sequence")]
        public int Id { get; set; }

        //[Key]
        [Column("linkedid")]
        public string LinkedId { get; set; }        

        [Column("cnum")]
        public string Src { get; set; }

        [Column("dst")]
        public string Dst { get; set; }

        [Column("calldate")]
        public DateTime CallDate { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("disposition")]
        public string Disposition { get; set; }

        [Column("cnam")]
        public string SrcName { get; set; }

        [NotMapped]
        public DateTime CallDateUTC { get; set; }
    }
}
