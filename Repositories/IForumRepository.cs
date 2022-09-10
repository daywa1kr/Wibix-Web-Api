using wibix_api.Models;

namespace wibix_api.Repositories;

public interface IForumRepository{
    /// <summary>
    /// Gets forum posts in specified
    /// <paramref name="order">order</paramref>
    /// </summary>
    IEnumerable<Post> GetPosts(string order);

    /// <summary>
    /// Searches and returns the post with specified
    /// <paramref name="id">id</paramref>
    /// </summary>
    Task<Post> GetPost(int id);

    /// <summary>
    /// Gets forum posts of a user by 
    /// <paramref name="userId">userId</paramref>
    /// </summary>
    IEnumerable<Post> GetPostsByUserId(string userId);

    /// <summary>
    /// Creates a <see cref="wibix_api.Models.Post"/> object from <paramref name="model"></paramref> and adds it to the database with the specified 
    /// <paramref name="userId">userId</paramref>
    /// </summary>
    Task AddPost(CreatePost model, string userId);

    /// <summary>
    /// Gets all answers of the forum post with specified <paramref name="postId">postId</paramref> sorted by highest rating
    /// </summary>
    IEnumerable<Answer> GetAnswers(int postId);

    /// <summary>
    /// Creates a <see cref="wibix_api.Models.Answer"/> object from <paramref name="model"></paramref> and adds it to the database with the specified 
    /// <paramref name="userId">userId</paramref>
    /// </summary>
    Task AddAnswer(CreateAnswer model, string userId);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.Post"/> with specified
    /// <paramref name="id">id</paramref> and increments its Rating
    /// </summary>
    Task Upvote(int id);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.Post"/> with specified
    /// <paramref name="id">id</paramref> and decrements its Rating
    /// </summary>
    Task Downvote(int id);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.Answer"/> with specified
    /// <paramref name="id">id</paramref> and increments its Rating
    /// </summary>
    Task UpvoteAnswer(int id);

    /// <summary>
    /// Finds the <see cref="wibix_api.Models.Answer"/> with specified
    /// <paramref name="id">id</paramref> and decrements its Rating
    /// </summary>
    Task DownvoteAnswer(int id);
}