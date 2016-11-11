﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.BL.DTOs.Addresses
{
   public class AddressCreateDTO
    {
        [Required]
        [MaxLength(512)]
        public string Building { get; set; }
        [Required]
        [MaxLength(256)]
        public string Street { get; set; }
        [Required]
        [MaxLength(16)]
        public string StreetNumber { get; set; }
        [Required]
        [MaxLength(128)]
        public string City { get; set; }
    }
}
