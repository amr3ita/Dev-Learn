using Code_Road.Dto.Account;
using Code_Road.Dto.Lesson;
using Code_Road.Services.LessonService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("GetListOfLessons")]
        public async Task<IActionResult> GetAllLessons()
        {
            List<LessonDetailsDto> lessons = await _lessonService.GetAllLessons();
            if (!lessons[0].State.Flag)
                return Ok(lessons[0].State.Message);
            return Ok(lessons);
        }

        [HttpGet("GetLessonById")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            LessonDto lesson = await _lessonService.GetLessonById(id);
            if (!lesson.State.Flag)
                return Ok(lesson.State.Message);
            return Ok(lesson);
        }

        [HttpGet("GetLessonByName")]
        public async Task<IActionResult> GetLessonByName(string name)
        {
            LessonDto lesson = await _lessonService.GetLessonByName(name);
            if (!lesson.State.Flag)
                return Ok(lesson.State.Message);
            return Ok(lesson);
        }

        [HttpGet("GetLessonsByUser")]
        public async Task<IActionResult> GetLessonByUser(string userId)
        {
            var lesson = await _lessonService.GetLessonAddedByUser(userId);
            if (!lesson[0].State.Flag)
                return Ok(lesson[0].State.Message);
            return Ok(lesson);
        }

        [HttpPost("AddLesson")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLesson([FromForm] AddLessonDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            LessonDto lesson = await _lessonService.AddLesson(model);
            if (!lesson.State.Flag)
                return Ok(lesson.State.Message);
            return Ok(lesson);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("EditLesson/{id:int}")]
        public async Task<IActionResult> EditLesson(int id, [FromForm] EditLessonDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            LessonDto lesson = await _lessonService.UpdateLessonById(id, model);
            if (!lesson.State.Flag)
                return Ok(lesson.State.Message);
            return Ok(lesson);
        }

        [HttpDelete("DeleteLesson/{id:int}")]
        //ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _lessonService.DeleteLesson(id);
            if (!state.Flag)
                return Ok(state.Message);
            return Ok(state.Message);
        }

    }
}
