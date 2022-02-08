using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : ControllerBase
    {

        private readonly IGraphClient _client;

        public CoursesController(IGraphClient client)
        {
            _client = client;
        }

        //Create a New Course with new specs
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CourseDTO course)
        {
            await _client.Cypher.Create("(c: Courses {name:$course.courseTitle})")
            .Merge("(program:Program {name:$course.StudyProgram})")
            .Merge("(program)-[:HAS_A_COURSE]->(c)")
            .Merge("(p: Professor {name:$course.Lecturer})")
            .Merge("(c)-[:HAS_A_PROFESSOR]->(p)")
            .Merge("(workload:Workload{name:$course.Workload})")
            .Merge("(c)-[:HAS_A_WORKLOAD]->(workload)")
            .Merge("(language:Language {name:$course.Language})")
            .Merge("(c)-[:HAS_A_LANGUAGE]->(language)")
            .Merge("(content:Content{name:$course.Content})")
            .Merge("(c)-[:HAS_A_CONTENT]->(content)")
            .Merge("(semester:Semester {name:$course.Semester})")
            .Merge("(c)-[:HAS_A_SEMESTER]->(semester)")
            .Merge("(tf:Tf {name:$course.Teaching_format})")
            .Merge("(c)-[:HAS_A_TEACHING_FORMAT]->(tf)")
            .Merge("(cp:CP{name:$course.Credit_Points})")
            .Merge("(c)-[:HAS_A_CREDIT_POINT]->(cp)")
            .Merge("(rp:RP{name:$course.Recommended_prerequisites})")
            .Merge("(c)-[:HAS_A_PREREQUISITES]->(rp)")
            .ForEach("(l IN $course.Targeted_Learning_Outcome | MERGE (loc:LOC {name:l}) MERGE (c)-[:HAS_A_LEARNINGOUTCOME]->(loc))")
            .WithParam("course", course)
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        
        [HttpGet("{c_title}/offersIn/{p_title}/")]
        public async Task<IActionResult> CourseOffered(string c_title, string p_title)
        {

            await _client.Cypher.Match("(p:StudyProgram), (c:Course)")
                                .Where((StudyPRO p, CourseTitle c) => p.name == p_title && c.name == c_title)
                                .Create("(p)-[r:hasCourse]->(c)")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }

        [HttpDelete("{c_title}/notOffered/{p_title}/")]
        public async Task<IActionResult> CourseDelete(string c_title, string p_title)
        {

            await _client.Cypher.Match("(p:StudyProgram), (c:Course)")
                                .Where((StudyPRO p, CourseTitle c) => p.name == p_title && c.name == c_title)
                                .Match("(p)-[r:hasCourse]->(c)")
                                .Delete("r")
                                .ExecuteWithoutResultsAsync();

            return Ok();
        }

    }
}