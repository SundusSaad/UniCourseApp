using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class StudyDTO
    {
        public string StudyProgram { get; set; }
        public IList<string> Courses { get; set; }
    }
}