using Api.DTOs;
using Api.Services;
using AutoMapper;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassDataAccessService _classDataAccessService;
        private readonly IMapper _mapper;

        public ClassesController(IClassDataAccessService classDataAccessService, IMapper mapper)
        {
            _classDataAccessService = classDataAccessService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert(ClassDto classDto)
        {
            var cls = _mapper.Map<Class>(classDto);
            var added = await _classDataAccessService.CreateClassAsync(cls);

            return new CreatedResult($"api/classes/{added.Id}", added);
        }

        [HttpPost]
        [Route("{classId}/students/{studentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EnrollStudent(int classId, int studentId)
        {
            var result = await _classDataAccessService.EnrollStudentAsync(classId, studentId);

            return result.Match<IActionResult>(
                student =>
                {
                    return Ok();
                }, err =>
                {
                    if (err is InvalidOperationException)
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
        public async Task<ActionResult<ClassDto>> Get(int id)
        {
            var result = await _classDataAccessService.GetClassByIdAsync(id);

            return result.Match<ActionResult<ClassDto>>(
                cls =>
                {
                    var classDto = _mapper.Map<ClassDto>(cls);
                    return new OkObjectResult(classDto);
                },
                err =>
                {
                    return NotFound(err.Message);
                });
        }
    }
}