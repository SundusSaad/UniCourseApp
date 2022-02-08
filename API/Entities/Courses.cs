using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Courses
    {
        public CourseTitle courseTitle { get; set; }

        public TeachingFormat Teaching_format { get; set; }
        public workload Workload { get; set; }
        public Creditpoints Credit_Points { get; set; }

        public Lecturer Lecturer { get; set; }
        public Semester Semester { get; set; }
        public language Language { get; set; }

        public RecommendedPrerequisite Recommended_prerequisites { get; set; }

        public IList<learningOutcome> Targeted_Learning_Outcome { get; set; }

        public Content Content { get; set; }
    }
}