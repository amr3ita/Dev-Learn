using Code_Road.Dto.Account;
using Code_Road.Dto.Post;
using Code_Road.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Code_Road.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostService(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _userManager = userManager;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<PostDto>> GetAllAsync()
        {
            var today = DateTime.Today;

            var posts = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.User)
                .Select(p => new PostDto
                {
                    Status = new StateDto { Flag = true, Message = "Success" },
                    PostId = p.Id,
                    UserName = p.User.FirstName + " " + p.User.LastName,
                    Content = p.Content,
                    Up = p.Up,
                    Down = p.Down,
                    Date = p.Date,
                    Image_url = p.Images.Where(i => i.UserId == p.User.Id).Select(i => i.ImageUrl).ToList()
                })
                .OrderByDescending(p => p.Date.Date == today ? 1 : 0) // Today's posts first
                .ThenByDescending(p => p.Date.Date) // Then by date
                .ThenByDescending(p => p.Up) // Then by Up property
            .ToListAsync();

            //Posts Not Found
            if (posts is null)
            {
                return null;
            }

            return posts;
        }


        /// <summary>
        /// get All posts for  one user 
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<List<PostDto>> GetAllByUserIdAsync(string user_id)
        {
            var today = DateTime.Today;
            var posts = await _context.Posts
               .Include(p => p.Images)
               .Include(p => p.User)
               .Where(p => p.UserId == user_id)
               .Select(p => new PostDto
               {
                   Status = new StateDto { Flag = true, Message = "Success" },
                   PostId = p.Id,
                   UserName = p.User.FirstName + " " + p.User.LastName,
                   Content = p.Content,
                   Up = p.Up,
                   Down = p.Down,
                   Date = p.Date,
                   Image_url = p.Images.Where(i => i.UserId == p.User.Id).Select(i => i.ImageUrl).ToList()
               })
               .OrderByDescending(p => p.Date.Date == today ? 1 : 0) // Today's posts first
               .ThenByDescending(p => p.Date.Date) // Then by date 
              .ToListAsync();
            //Posts Not Found
            if (posts is null)
            {
                return null;
            }

            return posts;
        }

        public async Task<PostDto> GetByIdAsync(int post_id)
        {
            StateDto state = new StateDto();
            var post = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == post_id);

            if (post == null)
            {
                state.Flag = false;
                state.Message = "Post Not Found ! ";
                return new PostDto { Status = state };
            }
            var postDto = new PostDto
            {
                Status = new StateDto
                {
                    Flag = true,
                    Message = "Sucsses"
                },
                PostId = post.Id,
                UserName = post.User.FirstName + " " + post.User.LastName,
                Content = post.Content,
                Up = post.Up,
                Down = post.Down,
                Date = post.Date,
                Image_url = post.Images.Where(i => i.UserId == post.User.Id).Select(i => i.ImageUrl).ToList()
            };
            return postDto;
        }



        public async Task<PostDto> AddPostAsync(AddPostDto postModel)
        {

            StateDto state = new StateDto();
            var validationResult = await ValidatePostModel(postModel);

            if (!validationResult.Flag)
                return new PostDto { Status = validationResult };

            var user = await _userManager.FindByIdAsync(postModel.UserId);
            if (user == null)
            {
                state.Flag = false;
                state.Message = "Invalid user 'user not Found!' ";
                return new PostDto { Status = state };
            }

            var post = new Post
            {
                Content = postModel.Content,
                Date = DateTime.Now,
                UserId = user.Id
            };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            if (postModel.Images != null)
            {
                string user_name = user.UserName;
                post.Images = await GetImagePath(postModel.Images, user_name, post.Id, user.Id);
            }

            return await GetByIdAsync(post.Id);

        }

        public async Task<PostDto> UpdatePostAsync(int post_id, UpdatePostDto postModel)
        {
            StateDto state = new StateDto();
            var old_post = await _context.Posts
              .FirstOrDefaultAsync(p => p.Id == post_id);
            //check user access 
            Console.WriteLine($"beFore operation {old_post.Content}");
            if (old_post.UserId != postModel.UserId)
            {
                state.Flag = false;
                state.Message = "'Access Denied'or inValid UserId ";
                return new PostDto { Status = state };
            }
            //Check If post exisest or not
            if (old_post == null)
            {
                state.Flag = false;
                state.Message = "Pst Not Found! ";
                return new PostDto { Status = state };
            }

            var user = await _userManager.FindByIdAsync(postModel.UserId);
            if (user == null)
            {
                state.Flag = false;
                state.Message = "Invalid user 'user not Found!' ";
                return new PostDto { Status = state };
            }

            old_post.Content = postModel.Content;
            //    await _context.Posts.ExecuteUpdateAsync(old_post);
            if (postModel.Images != null)
            {
                List<Image> images = await _context.Image.Where(p => p.PostId == post_id).ToListAsync();

                foreach (var item in postModel.Images)
                {
                    await DeletImage(post_id);
                }
                _context.Image.RemoveRange(images);
                old_post.Images = await GetImagePath(postModel.Images, user.UserName, old_post.Id, old_post.UserId);
            }

            //old_post.Content =postModel.Content;
            await _context.SaveChangesAsync();
            Console.WriteLine($"After operation {old_post.Content}");

            return await GetByIdAsync(old_post.Id);
        }

        public async Task<StateDto> DeletePostAsync(int post_id)
        {
            var del_post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == post_id);
            List<Image> images = await _context.Image.Where(p => p.PostId == post_id).ToListAsync();
            if (del_post != null)
            {
                if (images.Count > 0)
                {

                    foreach (var image in images)
                    {

                        await DeletImage(del_post.Id);

                    }
                    _context.Image.RemoveRange(images);

                }
                _context.Posts.Remove(del_post);

                await _context.SaveChangesAsync();

                return new StateDto { Flag = true, Message = "deleted Successfully" };
            }
            return new StateDto { Flag = false, Message = "not found" };
        }


        private async Task<bool> DeletImage(int post_id)
        {
            try
            {
                string file_path = await getFilePath(post_id);
                if (Directory.Exists(file_path))
                {
                    Directory.Delete(file_path, true);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task<List<Image>> GetImagePath(IFormFileCollection formFileCollection, string userName, int postId, string user_id)
        {
            int counter = 0;
            var httpContext = _httpContextAccessor.HttpContext;
            string hosturl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            List<Image> imageReturnd = new List<Image>();
            string filepath = await getFilePath(postId);
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            foreach (var file in formFileCollection)
            {
                Image img = new Image();
                string imgpath = filepath + "\\" + userName + "_" + counter++ + ".png";
                if (File.Exists(imgpath))
                {
                    File.Delete(imgpath);
                }
                using (FileStream stream = File.Create(imgpath))
                {
                    await file.CopyToAsync(stream);
                }
                counter--;
                string imgUrl = hosturl + "/Upload/Post/" + postId + "/" + userName + "_" + counter++ + ".png";
                img.ImageUrl = imgUrl;
                img.PostId = postId;
                img.UserId = user_id;
                imageReturnd.Add(img);
                await _context.Image.AddAsync(img);
                await _context.SaveChangesAsync();

            }
            return imageReturnd;
        }
        private async Task<string> getFilePath(int post_id)
        {
            return _environment.WebRootPath + "\\Upload\\Post\\" + post_id;
        }


        private Task<StateDto> ValidatePostModel(AddPostDto postModel)
        {
            StateDto state = new StateDto();
            state.Flag = true;
            state.Message = "Success!";

            if (postModel == null)
            {
                state.Flag = false;
                state.Message = "Post Is null Enter Data!";
                return Task.FromResult(state);
            }

            if (string.IsNullOrEmpty(postModel.UserId))
            {
                state.Flag = false;
                state.Message = "User Id Is null Enter User Id!";
                return Task.FromResult(state);
            }

            if (string.IsNullOrEmpty(postModel.Content) || postModel.Content.Length < 5 || postModel.Content.Length > 500)
            {
                state.Flag = false;
                state.Message = "Content must be greater than 5 char and less Than 500 char!";
                return Task.FromResult(state);
            }

            return Task.FromResult(state);
        }

        #region Up and Down aactions

        public async Task<StateDto> IncreaseUpvoteAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return new StateDto { Flag = false, Message = "Post not found." };
            }

            post.Up++;
            await _context.SaveChangesAsync();

            return new StateDto { Flag = true, Message = "Upvote increased successfully." };
        }

        public async Task<StateDto> IncreaseDownvoteAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return new StateDto { Flag = false, Message = "Post not found." };
            }

            post.Down++;
            await _context.SaveChangesAsync();

            return new StateDto { Flag = true, Message = "Downvote increased successfully." };
        }

        public async Task<StateDto> DecreaseUpvoteAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return new StateDto { Flag = false, Message = "Post not found." };
            }

            if (post.Up > 0)
            {
                post.Up--;
                await _context.SaveChangesAsync();
                return new StateDto { Flag = true, Message = "Upvote decreased successfully." };
            }

            return new StateDto { Flag = false, Message = "Upvote is already at minimum." };
        }

        public async Task<StateDto> DecreaseDownvoteAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return new StateDto { Flag = false, Message = "Post not found." };
            }

            if (post.Down > 0)
            {
                post.Down--;
                await _context.SaveChangesAsync();
                return new StateDto { Flag = true, Message = "Downvote decreased successfully." };
            }

            return new StateDto { Flag = false, Message = "Downvote is already at minimum." };
        }

        #endregion


    }
}
