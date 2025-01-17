using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Logger
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public string Location { get; set; } 
        public string WhatWentWrong { get; set; } 
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow; 
        public string Function { get; set; } 
    }
}
