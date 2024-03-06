using Code_Road.Dto.Account;
using Code_Road.Dto.Lesson;
using Code_Road.Models;
<<<<<<< HEAD
using Code_Road.Services.PostService.AuthService;
=======
using Code_Road.Services.QuizService;
>>>>>>> master
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.LessonService
{
    public class LessonService : ILessonService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
<<<<<<< HEAD
        private readonly IAuthService _authService;

        public LessonService(AppDbContext context, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, IAuthService authService)
=======
        private readonly IQuizService _quizService;

        public LessonService(AppDbContext context, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, IQuizService quizService)
>>>>>>> master
        {
            _context = context;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
<<<<<<< HEAD
            _authService = authService;
=======
            _quizService = quizService;
>>>>>>> master
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
<<<<<<< HEAD
<<<<<<< Updated upstream
                QuizId = lesson.Quiz.Id,//(await _context.Quizzes.FirstOrDefaultAsync(l => l.Id == lesson.Quiz.Id)).Id,
=======
                //QuizId = (await _context.Quizzes.FirstOrDefaultAsync(l => l.LessonId == lesson.Id)).Id,
>>>>>>> Stashed changes
=======
                QuizId = (await _context.Quizzes.FirstOrDefaultAsync(l => l.LessonId == lesson.Id)).Id,
>>>>>>> master
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
<<<<<<< HEAD
            //state.Message = "This Quiz is not found";
            //Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == model.Quiz);
            //if (quiz is null)
            //    return new LessonDto() { State = state };
=======

>>>>>>> master
            state.Flag = true;
            state.Message = "done";
            lesson.Name = model.Name;
            lesson.Explanation = model.Explanation;
            lesson.Level = model.Level;
            lesson.TopicId = topic.Id;
<<<<<<< HEAD
            //lesson.Quiz = quiz;
            state.Message = "This user is not found";
            //if ((await _authService.GetSignedInUserAsync()) is null)
            //    return new LessonDto() { State = state };
            //lesson.ApplicationUserId = (await _authService.GetSignedInUserAsync()).ToString();
=======

            state.Message = "This user is not found";

>>>>>>> master
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
<<<<<<< HEAD
            List<Lesson> lessons = await _context.Lessons.Where(l => l.Name == model.Name).ToListAsync();
            if (lessons.Count > 1)
=======
            if (await _context.Lessons.FirstOrDefaultAsync(l => l.Name == model.Name) is not null)
>>>>>>> master
                return new LessonDto() { State = state };
            state.Message = "This Topic Name is not found";
            Topic? topic = await _context.Topics.FirstOrDefaultAsync(t => t.Name == model.TopicName);
            if (topic is null)
                return new LessonDto() { State = state };
<<<<<<< HEAD
            //state.Message = "This Quiz is not found";
            //Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == model.Quiz);
            //if (quiz is null)
            //    return new LessonDto() { State = state };
            oldLesson.Name = model.Name;
            oldLesson.Level = model.Level;
            oldLesson.Explanation = model.Explanation;
            oldLesson.TopicId = topic.Id;
            //oldLesson.Quiz = quiz;
=======

            oldLesson.Name = model.Name;
            oldLesson.Explanation = model.Explanation;
            oldLesson.TopicId = topic.Id;

>>>>>>> master
            await _context.SaveChangesAsync();
            return await GetLessonById(oldLesson.Id);
        }
        public async Task<StateDto> DeleteLesson(int id)
        {
<<<<<<< HEAD
            Lesson? oldLesson = await _context.Lessons.Include(l => l.topic).FirstOrDefaultAsync(l => l.Id == id);
=======
            Lesson? oldLesson = await _context.Lessons.Include(l => l.topic).Include(l => l.Quiz).FirstOrDefaultAsync(l => l.Id == id);
>>>>>>> master
            StateDto state = new StateDto() { Flag = false };
            state.Message = "There is no Lesson with this id";
            if (oldLesson is null)
                return state;
<<<<<<< HEAD

=======
            state.Message = "This Quiz is not found";
            Quiz? quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == oldLesson.Quiz.Id);
            if (quiz is null)
                return state;
>>>>>>> master
            List<Image> images = await _context.Image.Where(l => l.LessonId == id).ToListAsync();
            if (images.Count > 0)
            {
                _context.Image.RemoveRange(images);
                await DeleteImageFile(oldLesson.topic.Name, oldLesson.Name);
            }
<<<<<<< HEAD
<<<<<<< Updated upstream
            _context.Quizzes.Remove(quiz);
            _context.Lessons.Remove(oldLesson);
=======
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

>>>>>>> Stashed changes
=======
            await _quizService.DeleteQuiz(quiz.Id);
            _context.Lessons.Remove(oldLesson);
>>>>>>> master
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
                string imagePath = filePath + "\\" + lessonName + "(" + counter + ").png";
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
                using (FileStream stream = File.Create(imagePath))
                {
                    await file.CopyToAsync(stream);
                }
                string imageUrl = baseUrl + "/Upload/Lessons/" + topicName + "/" + lessonName + "/" + lessonName + "(" + counter + ").png";
                image.ImageUrl = imageUrl;
                image.LessonId = lesson_id;
                images.Add(image);
                await _context.Image.AddAsync(image);
                await _context.SaveChangesAsync();
                counter++;
            }




            return images;
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
