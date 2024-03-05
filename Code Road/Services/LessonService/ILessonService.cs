using Code_Road.Dto.Account;
using Code_Road.Dto.Lesson;

namespace Code_Road.Services.LessonService
{
    public interface ILessonService
    {
        Task<List<LessonDetailsDto>> GetAllLessons();
        Task<LessonDto> GetLessonById(int id);
        Task<LessonDto> GetLessonByName(string name);
        Task<LessonDto> AddLesson(AddLessonDto model);
        Task<LessonDto> UpdateLessonById(int id, EditLessonDto model);
        Task<StateDto> DeleteLesson(int id);
    }
}
