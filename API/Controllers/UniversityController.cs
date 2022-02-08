using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UniversityController : ControllerBase
    {

        private readonly IGraphClient _client;

        public UniversityController(IGraphClient client)
        {
            _client = client;
        }



        //to create overall knowledge graph 
        // All cypher queries are started from the cypher property of IGraphClient
        [HttpPost]

        public async Task<ActionResult> CreateUniCourses()
        {
            await _client.Cypher.Call("apoc.load.json('file:///University.json')").Yield("value")
            .Unwind("value.Courses", "courses")
            .Merge("(course:Courses {name:courses.Course_Title})")
            .Merge("(program:Program {name:value.Study_Program})")
            .Merge("(program)-[:HAS_A_COURSE]->(course)")
            .Merge("(professor:Professor {name:courses.Lecturer})")
            .Merge("(course)-[:HAS_A_PROFESSOR]->(professor)")
            .Merge("(workload:Workload{name:courses.Workload})")
            .Merge("(course)-[:HAS_A_WORKLOAD]->(workload)")
            .Merge("(language:Language {name:courses.Language})")
            .Merge("(course)-[:HAS_A_LANGUAGE]->(language)")
            .Merge("(content:Content{name:courses.Content})")
            .Merge("(course)-[:HAS_A_CONTENT]->(content)")
            .Merge("(semester:Semester {name:courses.Semester})")
            .Merge("(course)-[:HAS_A_SEMESTER]->(semester)")
            .Merge("(tf:Tf {name:courses.Teaching_format})")
            .Merge("(course)-[:HAS_A_TEACHING_FORMAT]->(tf)")
            .Merge("(cp:CP{name:courses.Credit_Points})")
            .Merge("(course)-[:HAS_A_CREDIT_POINT]->(cp)")
            .Merge("(rp:RP{name:courses.Recommended_prerequisites})")
            .Merge("(course)-[:HAS_A_PREREQUISITES]->(rp)")
            .ForEach("(l IN courses.Targeted_learning_outcomes | MERGE (loc:LOC {name:l}) MERGE (course)-[:HAS_A_LEARNINGOUTCOME]->(loc))")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }


        // to retrieve information of a particular course
        [HttpGet("{name}")]
        public async Task<IActionResult> GetByCoursetitle(string name)
        {
            var course = await _client.Cypher
            .Match(@"(c: Courses)-[:HAS_A_PROFESSOR]->(p:Professor),
             (c)-[:HAS_A_LANGUAGE]->(l:Language), 
             (prog:Program)-[:HAS_A_COURSE]->(c),
             (c)-[:HAS_A_WORKLOAD]->(w:Workload),
             (c)-[:HAS_A_CONTENT]->(ct:Content),
             (c)-[:HAS_A_SEMESTER]->(sem:Semester),
             (c)-[:HAS_A_TEACHING_FORMAT]->(tf:Tf),
             (c)-[:HAS_A_CREDIT_POINT]->(cp:CP),
             (c)-[:HAS_A_PREREQUISITES]->(rp:RP),
             (c)-[:HAS_A_LEARNINGOUTCOME]->(loc:LOC)
             ")

            .Where((CourseTitle c) => c.name == name)
            .Return((c, prog, p, l, w, ct, sem, tf, cp, rp, loc) => new
            {

                CourseTitle = c.As<CourseTitle>().name,
                StudyProgram = prog.As<StudyPRO>().name,
                Professor = p.As<Lecturer>().name,
                Language = l.As<language>().name,
                Workload = w.As<workload>().name,
                Content = ct.As<Content>().name,
                Semester = sem.As<Semester>().name,
                TeachingFormat = tf.As<TeachingFormat>().name,
                CreditPoints = cp.As<Creditpoints>().name,
                RecommendedPrerequisites = rp.As<RecommendedPrerequisite>().name,
                TargetedLearningOutcomes = loc.CollectAs<learningOutcome>()



            }).ResultsAsync;

            return Ok(course);
        }

        // delete all nodes and relationships

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            await _client.Cypher.Match("(n)")
            .DetachDelete("n")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }


        // delete a particular course with all its relationships and relational nodes
        [HttpDelete("{title}")]
        public async Task<IActionResult> Delete(string title)
        {
            await _client.Cypher.Match("(c:Courses)-[r]->(b)")
            .Where((CourseTitle c) => c.name == title)
            .DetachDelete("c,r")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }
    }
}

