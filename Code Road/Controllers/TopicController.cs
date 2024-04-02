using Code_Road.Dto.Account;
using Code_Road.Dto.Topic;
using Code_Road.Services.TopicService;
using Microsoft.AspNetCore.Mvc;

namespace Code_Road.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTopics()
        {
            List<TopicDto> topicDtos = await _topicService.getAllTopics();
            if (!topicDtos[0].State.Flag)
                return BadRequest(topicDtos[0].State.Message);
            return Ok(topicDtos);
        }
        [HttpGet("GetTopicById")]
        public async Task<IActionResult> GetTopicById(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            TopicDto topic = await _topicService.getTopicById(id);
            if (!topic.State.Flag)
                return BadRequest(topic.State.Message);
            return Ok(topic);
        }
        [HttpGet("GetTopicByName")]
        public async Task<IActionResult> GetTopicByName(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            TopicDto topic = await _topicService.getTopicByName(name);
            if (!topic.State.Flag)
                return BadRequest(topic.State.Message);
            return Ok(topic);
        }
        [HttpPost("AddTopic/{name:alpha}")]
        public async Task<IActionResult> AddTopic(string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            TopicDto topic = await _topicService.AddTopic(name);
            if (!topic.State.Flag)
                return BadRequest(topic.State.Message);
            return Ok(topic);
        }
        [HttpPut("EditTopic/{id:int}")]
        public async Task<IActionResult> EditTopic(int id, [FromBody] string name)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            TopicDto topic = await _topicService.EditTopic(id, name);
            if (!topic.State.Flag)
                return BadRequest(topic.State.Message);
            return Ok(topic);
        }
        [HttpDelete("DeleteTopic/{id:int}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            StateDto State = await _topicService.DeleteTopic(id);
            if (!State.Flag)
                return BadRequest(State.Message);
            return Ok(State.Message);
        }


    }
}
