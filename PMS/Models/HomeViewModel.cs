﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMS.Models
{
    public class SearchViewModel
    {
        [Required]
        [DisplayName("keyword")]
        public string keyword { get; set; }
    }

    
}