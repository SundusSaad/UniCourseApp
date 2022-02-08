using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neo4jClient;
using API.Entities;
using API.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudyProgramController : ControllerBase
    {
        private readonly IGraphClient _client;

        public StudyProgramController(IGraphClient client)
        {
            _client = client;
        }

        // Get all studyprograms
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var studyPrograms = await _client.Cypher
            .Match("(n:Program)")
            .Return(n => n.As<StudyPRO>().name).ResultsAsync;

            return Ok(studyPrograms);
        }


        // Get a particular study program and its courses
        [HttpGet("{title}")]
        public async Task<IActionResult> GetBytitle(string title)
        {
            var courses = await _client.Cypher.Match("((p:Program)-[:HAS_A_COURSE]->(c:Courses))")
            .Where((StudyPRO p) => p.name == title)
           .Return(c => c.As<CourseTitle>().name).ResultsAsync;

           // return name of studyprogram and its courses as individual objects
            //.Return((c, p) => new{
            // CourseTitle = c.As<CourseTitle>().name,
            // StudyProgram = p.As<StudyPRO>().name,
            //}

            return Ok(courses);
        }

        //Create a New StudyProgram and assign courses
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] StudyDTO study)
        {
            await _client.Cypher
            .Create("(p: Program {name:$study.StudyProgram})")
            .ForEach("(c IN $study.Courses | MERGE (course:Courses {name:c}) MERGE (p)-[:HAS_A_COURSE]->(course))")
            .WithParam("study", study)
            .ExecuteWithoutResultsAsync();

            return Ok();
        }


        //Update a Studyprogram
        [HttpPut("{title}")]
        public async Task<IActionResult> Update(string title, [FromBody] StudyPRO study)
        {
            await _client.Cypher.Match("(p: StudyProgram)")
            .Where((StudyPRO p) => p.name == title)
            .Set("p = $study")
            .WithParam("study", study)
            .ExecuteWithoutResultsAsync();

            return Ok();
        }

        //Delete a StudyProgram
        [HttpDelete("{title}")]
        public async Task<IActionResult> Delete(string title)
        {
            await _client.Cypher.Match("(p: StudyProgram)")
            .Where((StudyPRO p) => p.name == title)
            .Delete("p")
            .ExecuteWithoutResultsAsync();

            return Ok();
        }
    }
}