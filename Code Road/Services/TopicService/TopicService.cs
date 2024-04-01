using Code_Road.Dto.Account;
using Code_Road.Dto.Topic;
using Code_Road.Models;
using Code_Road.Services.LessonService;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.TopicService
{
    public class TopicService : ITopicService
    {
        private readonly AppDbContext _context;
        private readonly ILessonService _lessonService;

        public TopicService(AppDbContext context, ILessonService lessonService)
        {
            _context = context;
            _lessonService = lessonService;
        }
        public async Task<List<TopicDto>> getAllTopics()
        {
            List<Topic> topics = await _context.Topics.ToListAsync();
            List<string> lessons = new List<string>();
            List<TopicDto> topicDtos = new List<TopicDto>();
            StateDto state = new StateDto();
            state.Flag = false;
            state.Message = "There is no topics to Represent";
            if (topics.Count == 0)
            {
                topicDtos.Add(new TopicDto() { State = state });
                return topicDtos;
            }
            state.Flag = true;
            state.Message = "every thing go well";
            foreach (Topic topic in topics)
            {
                topicDtos.Add(
                    new TopicDto()
                    {
                        State = state,
                        Id = topic.Id,
                        Name = topic.Name,
                        Lessons = await _context.Lessons.Where(t => t.TopicId == topic.Id).Select(l => l.Name).ToListAsync()
                    }
                    );
            }
            return topicDtos;
        }

        public async Task<TopicDto> getTopicById(int id)
        {
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
            List<string> lessons = new List<string>();
            StateDto state = new StateDto();
            state.Flag = false;
            state.Message = "There is no topics with this id";
            if (topic is null)
                return new TopicDto() { State = state };
            state.Flag = true;
            state.Message = "every thing go well";
            return new TopicDto()
            {
                State = state,
                Id = topic.Id,
                Name = topic.Name,
                Lessons = await _context.Lessons.Where(t => t.TopicId == topic.Id).Select(l => l.Name).ToListAsync()
            };

        }
        public async Task<TopicDto> getTopicByName(string name)
        {
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Name == name);
            List<string> lessons = new List<string>();
            StateDto state = new StateDto();
            state.Flag = false;
            state.Message = "There is no topics with this name";
            if (topic is null)
                return new TopicDto() { State = state };
            state.Flag = true;
            state.Message = "every thing go well";
            return new TopicDto()
            {
                State = state,
                Id = topic.Id,
                Name = topic.Name,
                Lessons = await _context.Lessons.Where(t => t.TopicId == topic.Id).Select(l => l.Name).ToListAsync()
            };

        }
        public async Task<TopicDto> AddTopic(string name)
        {
            Topic? topic = new Topic();
            Topic? topic1 = await _context.Topics.FirstOrDefaultAsync(t => t.Name == name);

            List<string> lessons = new List<string>();
            StateDto state = new StateDto() { Flag = false, Message = "failed to add" };
            if (topic1 is not null) return new TopicDto() { State = state };
            string topicName = name;
            char c = char.ToUpper(topicName[0]);
            topic.Name = c + topicName.Substring(1);
            await _context.Topics.AddAsync(topic);
            await _context.SaveChangesAsync();
            state.Flag = true;
            state.Message = "Added Successfully";
            return new TopicDto()
            {
                State = state,
                Id = topic.Id,
                Name = topic.Name,
                Lessons = await _context.Lessons.Where(t => t.TopicId == topic.Id).Select(l => l.Name).ToListAsync()
            };
        }
        public async Task<TopicDto> EditTopic(int id, string name)
        {

            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);

            List<string> lessons = new List<string>();
            StateDto state = new StateDto() { Flag = false, Message = "not found" };
            if (topic is null) return new TopicDto() { State = state };
            string topicName = name;
            char c = char.ToUpper(topicName[0]);
            topic.Name = c + topicName.Substring(1);

            await _context.SaveChangesAsync();
            state.Flag = true;
            state.Message = "Updated Successfully";
            return new TopicDto()
            {
                State = state,
                Id = topic.Id,
                Name = topic.Name,
                Lessons = await _context.Lessons.Where(t => t.TopicId == topic.Id).Select(l => l.Name).ToListAsync()
            };
        }
        public async Task<StateDto> DeleteTopic(int id)
        {
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Id == id);
            StateDto state = new StateDto() { Flag = false, Message = "not found" };
            if (topic is null) return state;
            List<Lesson> lessons = await _context.Lessons.Where(l => l.TopicId == id).ToListAsync();
            if (lessons.Count > 0)
            {
                foreach (Lesson lesson in lessons)
                {
                    await _lessonService.DeleteLesson(lesson.Id);
                }
            }
            _context.Topics.Remove(topic);
            _context.SaveChanges();
            state.Flag = true;
            state.Message = "Deleted Sucessfully";

            return state;

        }

    }
}
