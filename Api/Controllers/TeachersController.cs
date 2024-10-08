using System;
using System.Net;
using Api.DTOs;
using Api.Services;
using AutoMapper;
using Database;
using Domain.Model;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherDataAccessService _teacherDataAccessService;
        private readonly IMapper _mapper;

        public TeachersController(ITeacherDataAccessService teacherDataAccessService, IMapper mapper)
        {
            _teacherDataAccessService = teacherDataAccessService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert(TeacherDto teacherDto)
        {
            var teacher = _mapper.Map<Teacher>(teacherDto);
            var added = await _teacherDataAccessService.CreateTeacherAsync(teacher);

            return new CreatedResult($"api/teachers/{added.Id}", added);
        }

        [HttpPost]
        [Route("{teacherId}/classes/{classId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignClass(int teacherId, int classId)
        {
            var result = await _teacherDataAccessService.AssignClassAsync(teacherId, classId);

            return result.Match<IActionResult>(
                teacher =>
                {
                    return Ok();
                },
                err =>
                {
                    var vErr = err as ValidationException;
                    if(vErr.Errors.Any(e=>e.CustomState is HttpStatusCode.NotFound))
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
            var result = await _teacherDataAccessService.GetTeacherByIdAsync(id);

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