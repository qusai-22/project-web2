﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace project_web2.Models
{
    public class items
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public string discount { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime makedate { get; set; }
        public int category { get; set; }
        public int quantity { get; set; }
        public string imgfile { get; set; }
    }
}
