using Code_Road.Dto.Account;
using Code_Road.Dto.Lesson;
using Code_Road.Models;
using Code_Road.Services.QuizService;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.LessonService
{
    public class LessonService : ILessonService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQuizService _quizService;

        public LessonService(AppDbContext context, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, IQuizService quizService)
        {
            _context = context;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _quizService = quizService;
        }

        #region Public Section
        public async Task<List<LessonDetailsDto>> GetAllLessons()
        {
            List<Lesson> lessons = await _context.Lessons.Include(t => t.topic).ToListAsync();
            List<LessonDetailsDto> LessonsDetails = new List<LessonDetailsDto>();
            StateDto state = new StateDto();
            state.Flag = true;
            state.Message = "added successfully";
            if (lessons.Count > 0)
            {
                foreach (var lesson in lessons)
                {
                    LessonsDetails.Add(new LessonDetailsDto() { State = state, Id = lesson.Id, Name = lesson.Name, Level = lesson.Level, Topic = lesson.topic.Name });
                }
                return LessonsDetails;
            }
            state.Flag = false;
            state.Message = "there is no Lessons to represint";
            LessonsDetails.Add(new LessonDetailsDto() { State = state });
            return LessonsDetails;
        }
        public async Task<LessonDto> GetLessonById(int id)
        {
            Lesson? lesson = await _context.Lessons.Include(t => t.topic).Include(q => q.Quiz).FirstOrDefaultAsync(t => t.Id == id);
            StateDto state = new StateDto() { Flag = true, Message = "every thing go well" };
            if (lesson is null)
            {
                state.Flag = false;
                state.Message = "there is no Lesson with this Id";
                return new LessonDto() { State = state };
            }
            return new LessonDto()
            {
                Explanation = lesson.Explanation,
                Name = lesson.Name,
                Level = lesson.Level,
                Topic = lesson.topic.Name,
                Img = await _context.Image.Where(l => l.LessonId == id).Select(i => i.ImageUrl).ToListAsync(),
                //QuizId = (await _context.Quizzes.FirstOrDefaultAsync(l => l.LessonId == lesson.Id)).Id,
                State = state
            };


        }
        public async Task<LessonDto> GetLessonByName(string name)
        {
            Lesson? lesson = await _context.Lessons.Include(t => t.topic).FirstOrDefaultAsync(t => t.Name == name);
            StateDto state = new StateDto() { Flag = true, Message = "every thing go well" };
            if (lesson is null)
            {
                state.Flag = false;
                state.Message = "there is no Lesson with this Name";
                return new LessonDto() { State = state };
            }
            return new LessonDto()
            {
                Explanation = lesson.Explanation,
                Name = lesson.Name,
                Level = lesson.Level,
                Topic = lesson.topic.Name,
                Img = await _context.Image.Where(l => l.LessonId == lesson.Id).Select(i => i.ImageUrl).ToListAsync(),
                //QuizId = lesson.Quiz.Id,
                State = state
            };
        }
        public async Task<LessonDto> AddLesson(AddLessonDto model)
        {
            StateDto state = new StateDto() { Flag = false };
            state.Message = "Lesson is null here";
            if (model is null)
                return new LessonDto() { State = state };
            Lesson lesson = new Lesson();
            state.Message = "This Name already exist";
            if (await _context.Lessons.FirstOrDefaultAsync(l => l.Name == model.Name) is not null)
                return new LessonDto() { State = state };
            state.Message = "This Topic Name is not found";
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Name == model.TopicName);
            if (topic is null)
                return new LessonDto() { State = state };

            state.Flag = true;
            state.Message = "done";
            lesson.Name = model.Name;
            lesson.Explanation = model.Explanation;
            lesson.Level = model.Level;
            lesson.TopicId = topic.Id;

            state.Message = "This user is not found";

            await _context.Lessons.AddAsync(lesson);
            await _context.SaveChangesAsync();
            if (model.Images is not null && model.Images.Count > 0)
                lesson.Images = await UploadImage(model.Images, lesson.Id, model.TopicName, model.Name);



            await _context.SaveChangesAsync();
            return await GetLessonById(lesson.Id);
        }
        public async Task<LessonDto> UpdateLessonById(int id, EditLessonDto model)
        {
            Lesson? oldLesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
            StateDto state = new StateDto() { Flag = false };
            state.Message = "There is no Lesson with this id";
            if (oldLesson is null)
                return new LessonDto() { State = state };
            state.Message = "This Name already exist";
            List<Lesson> lessons = await _context.Lessons.Where(l => l.Name == model.Name).ToListAsync();
            if (lessons.Count > 1)
                return new LessonDto() { State = state };
            state.Message = "This Topic Name is not found";
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Name == model.TopicName);
            if (topic is null)
                return new LessonDto() { State = state };
            List<Image> images = await ChangeDirctoryName(oldLesson.Id, oldLesson.Name, oldLesson.topic.Name, model.Name, model.TopicName);
            oldLesson.Name = model.Name;
            oldLesson.Level = model.Level;
            oldLesson.Explanation = model.Explanation;
            oldLesson.TopicId = topic.Id;
            if (images is not null)
                oldLesson.Images = images;

            await _context.SaveChangesAsync();
            return await GetLessonById(oldLesson.Id);
        }
        public async Task<StateDto> DeleteLesson(int id)
        {
            Lesson? oldLesson = await _context.Lessons.Include(l => l.topic).FirstOrDefaultAsync(l => l.Id == id);
            StateDto state = new StateDto() { Flag = false };
            state.Message = "There is no Lesson with this id";
            if (oldLesson is null)
                return state;


            List<Image> images = await _context.Image.Where(l => l.LessonId == id).ToListAsync();
            if (images.Count > 0)
            {
                _context.Image.RemoveRange(images);
                await DeleteImageFile(oldLesson.topic.Name, oldLesson.Name);
            }
            Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == q.LessonId);
            if (quiz is not null)
            {
                await _quizService.DeleteQuiz(quiz.Id);
                _context.Lessons.Remove(oldLesson);

            }
            else
            {
                _context.Lessons.Remove(oldLesson);

            }

            _context.SaveChanges();
            state.Flag = true;
            state.Message = "Deleted Successfully";
            return state;
        }
        #endregion
        #region private Section
        private async Task<List<Image>> UploadImage(IFormFileCollection files, int lesson_id, string topicName, string lessonName)
        {
            //string url = $"{environment.EnvironmentName} - {environment.ContentRootPath}";
            var httpContext = _httpContextAccessor.HttpContext;
            int counter = 1;
            List<Image> images = new List<Image>();
            // Get the base URL
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            string filePath = await GetGilePath(topicName, lessonName);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            foreach (var file in files)
            {
                Image image = new Image();
                string imagePath = filePath + "\\" + "(" + counter + ").png";
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
                using (FileStream stream = File.Create(imagePath))
                {
                    await file.CopyToAsync(stream);
                }
                string imageUrl = baseUrl + "/Upload/Lessons/" + topicName + "/" + lessonName + "/" + "(" + counter + ").png";
                image.ImageUrl = imageUrl;
                image.LessonId = lesson_id;
                images.Add(image);
                await _context.Image.AddAsync(image);
                await _context.SaveChangesAsync();
                counter++;
            }


            return images;
        }
        private async Task<List<Image>> ChangeDirctoryName(int id, string oldName, string oldTopicName, string newName, string newTopicName)
        {
            string filePath = await GetGilePath(oldTopicName, oldName);
            string newFilePath = await GetGilePath(newTopicName, newName);
            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            int counter = 1;
            List<Image> images = _context.Image.Where(i => i.LessonId == id).ToList();
            if (images.Count > 0)
            {
                if (Directory.Exists(filePath))
                {
                    Directory.Move(filePath, newFilePath);
                }
                foreach (Image image in images)
                {
                    image.ImageUrl = baseUrl + "/Upload/Lessons/" + newTopicName + "/" + newName + "/" + "(" + counter + ").png";
                    counter++;
                }
                await _context.SaveChangesAsync();
                return images;
            }
            return null;

        }
        private async Task<string> GetGilePath(string topicName, string lessonName)
        {
            return _environment.WebRootPath + "\\Upload\\Lessons\\" + topicName + "\\" + lessonName;
        }
        private async Task<StateDto> DeleteImageFile(string topicName, string lessonName)
        {
            string filePath = await GetGilePath(topicName, lessonName);
            if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, true);
                return new StateDto { Flag = true, Message = "Folder Is deleted" };
                //Directory.Delete(filePath);
            }
            return new StateDto { Flag = false, Message = "There is no folder" };

        }
        #endregion{
    }
}