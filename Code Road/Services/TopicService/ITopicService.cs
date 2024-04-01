using Code_Road.Dto.Account;
using Code_Road.Dto.Topic;

namespace Code_Road.Services.TopicService
{
    public interface ITopicService
    {
        Task<List<TopicDto>> getAllTopics();
        Task<TopicDto> getTopicById(int id);
        Task<TopicDto> getTopicByName(string topicName);
        Task<TopicDto> AddTopic(string topicName);
        Task<TopicDto> EditTopic(int id, string name);
        Task<StateDto> DeleteTopic(int id);
    }
}
