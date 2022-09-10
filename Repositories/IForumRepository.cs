using wibix_api.Models;

namespace wibix_api.Repositories;

public interface IForumRepository{
    IEnumerable<Post> GetPosts(string order);
    Task<Post> GetPost(int id);
    IEnumerable<Post> GetPostsByUserId(string id);
    Task AddPost(CreatePost model, string userId);
    IEnumerable<Answer> GetAnswers(int postId);
    Task AddAnswer(CreateAnswer model, string userId);
    Task Upvote(int id);
    Task Downvote(int id);
    Task UpvoteAnswer(int id);
    Task DownvoteAnswer(int id);
}