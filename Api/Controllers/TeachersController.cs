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
    public class TeachersController : ControllerBase
    {
        private readonly IClassDataAccessService _classDataAccessService;
        private readonly IMapper _mapper;

        public TeachersController(IClassDataAccessService classDataAccessService, IMapper mapper)
        {
            _classDataAccessService = classDataAccessService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert(TeacherDto teacherDto)
        {
            var teacher = _mapper.Map<Teacher>(teacherDto);
            var added = await _classDataAccessService.CreateTeacherAsync(teacher);

            return new CreatedResult($"api/teachers/{added.Id}", added);
        }

        [HttpPost]
        [Route("{teacherId}/classes/{classId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignClass(int teacherId, int classId)
        {
            var result = await _classDataAccessService.AssignClassAsync(teacherId, classId);

            return result.Match<IActionResult>(
                teacher =>
                {
                    return Ok();
                },
                err =>
                {
                    if(err is InvalidOperationException)
                    {
                        return NotFound(err.Message);
                    }
                    else
                    {
                        return BadRequest(err.Message);
                    }
                });
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDto>> Get(int id)
        {
            var result = await _classDataAccessService.GetTeacherByIdAsync(id);

            return result.Match<ActionResult<TeacherDto>>(
                teacher =>
                {
                    var teacherDto = _mapper.Map<TeacherDto>(teacher);
                    return new OkObjectResult(teacherDto);
                },
                err =>
                {
                    return NotFound(err.Message);
                }
            );
        }
    }
}