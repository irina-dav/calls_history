﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CallsHistory.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("extension")]
        public string Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
      
    }
}
