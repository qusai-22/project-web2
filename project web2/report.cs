using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;




namespace project_web2
{
    [Keyless]
    public class report
    {
        public String custname {  get; set; }
        public int total { get; set; }
    }
}
