using Microsoft.AspNetCore.Mvc;
using Step05_MinimalApiAndControllers.Models;

namespace Step05_MinimalApiAndControllers.Controllers;

/// <summary>Controller-style CRUD for comparison with Minimal API products endpoints.</summary>
[ApiController]
[Route("api/students")]
public sealed class StudentsController(StudentStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Student>> GetAll() => Ok(store.All());

    [HttpGet("{number}")]
    public ActionResult<Student> Get(string number)
        => store.Get(number) is { } s ? Ok(s) : NotFound();

    [HttpPost]
    public ActionResult<Student> Create([FromBody] CreateStudentRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.StudentNumber))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Errors = { ["studentNumber"] = ["Required"] }
            });
        }

        var student = store.Upsert(new Student(req.StudentNumber, req.FullName, req.Major));
        return CreatedAtAction(nameof(Get), new { number = student.StudentNumber }, student);
    }

    [HttpDelete("{number}")]
    public IActionResult Delete(string number)
        => store.Delete(number) ? NoContent() : NotFound();
}
