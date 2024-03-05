using Code_Road.Dto;
using Code_Road.Dto.Account;
using Code_Road.Dto.Lesson;
using Code_Road.Services.LessonService;
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
        [HttpGet("Get List of Lessons")]
        public async Task<IActionResult> GetAllLessons()
        {
            List<LessonDetailsDto> lessons = await _lessonService.GetAllLessons();
            if (!lessons[0].State.Flag)
                return BadRequest(lessons[0].State.Message);
            return Ok(lessons);
        }
        [HttpGet("Get Lesson By Id")]
        public async Task<IActionResult> GetLessonById(int id)
        {
            LessonDto lesson = await _lessonService.GetLessonById(id);
            if (!lesson.State.Flag)
                return BadRequest(lesson.State.Message);
            return Ok(lesson);
        }
        [HttpGet("Get Lesson By Name")]
        public async Task<IActionResult> GetLessonByName(string name)
        {
            LessonDto lesson = await _lessonService.GetLessonByName(name);
            if (!lesson.State.Flag)
                return BadRequest(lesson.State.Message);
            return Ok(lesson);
        }
        [HttpPost("Add Lesson")]
        //[Authorize(Roles = "Admin")]
        //ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLesson([FromForm] AddLessonDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            LessonDto lesson = await _lessonService.AddLesson(model);
            if (!lesson.State.Flag)
                return BadRequest(lesson.State.Message);
            return Ok(lesson);
        }
        [HttpPatch("edit Lesson")]
        //ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int id, [FromForm] EditLessonDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            LessonDto lesson = await _lessonService.UpdateLessonById(id, model);
            if (!lesson.State.Flag)
                return BadRequest(lesson.State.Message);
            return Ok(lesson);
        }
        [HttpDelete("Delete Lesson")]
        //ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto state = await _lessonService.DeleteLesson(id);
            if (!state.Flag)
                return BadRequest(state.Message);
            return Ok(state.Message);
        }

    }
}
