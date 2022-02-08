using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class University
    {
        public IList<Courses> Courses { get; set; }
        public  StudyPRO Study_Program { get; set; }
        

    }
}