using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class CourseDTO
    {
        public string StudyProgram { get; set; }
        public string courseTitle { get; set; }

        public string Teaching_format { get; set; }
        public string Workload { get; set; }
        public string Credit_Points { get; set; }

        public string Lecturer { get; set; }
        public string Semester { get; set; }
        public string Language { get; set; }

        public string Recommended_prerequisites { get; set; }

        public IList<string> Targeted_Learning_Outcome { get; set; }

        public string Content { get; set; }
    }
}