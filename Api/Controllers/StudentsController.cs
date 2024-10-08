using System;
using Api.DTOs;
using Api.Services;
using AutoMapper;
using Database;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentDataAccessService _studentDataAccessService;
        private readonly IMapper _mapper;

        public StudentsController(IStudentDataAccessService studentDataAccessService, IMapper mapper)
        {
            _studentDataAccessService = studentDataAccessService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert(StudentDto studentDto)
        {
            var student = _mapper.Map<Student>(studentDto);
            var added = await _studentDataAccessService.CreateStudentAsync(student);

            return new CreatedResult($"api/students/{added.Id}", added);
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDto>> Get(int id)
        {
            var result = await _studentDataAccessService.GetStudentByIdAsync(id);

            return result.Match<ActionResult<StudentDto>>(
                student =>
                {
                    var studentDto = _mapper.Map<StudentDto>(student);
                    return new OkObjectResult(studentDto);
                },
                err =>
                {
                    return NotFound(err.Message);
                });
        }
    }
}