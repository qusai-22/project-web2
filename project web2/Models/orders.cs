﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace project_web2.Models
{
    public class orders
    {
        public int Id { get; set; }
        public string custname { get; set; }
        [BindProperty, DataType(DataType.Date)]
        public DateTime orderdate { get; set; }
        public int total { get; set; }
       
        
    }
}
