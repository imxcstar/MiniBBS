using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MiniBBS.DB
{
    public class ForumDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
        {
        }

        public DbSet<Forum> Forums { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Forum>().ToTable("Forums");
            modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Comment>().ToTable("Comments");
        }
    }

    public static class DbInitializer
    {
        public static async Task InitializeAsync(ForumDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            context.Database.EnsureCreated();

            // 如果有用户数据，则说明数据库已经被初始化过
            if (context.Users.Any())
            {
                return;
            }

            // 添加角色数据
            var roles = new IdentityRole<int>[]
            {
            new IdentityRole<int> { Name = "Admin" },
            new IdentityRole<int> { Name = "User" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // 添加用户数据
            var users = new User[]
            {
            new User { UserName = "admin", Email = "admin@example.com", RegistrationDate = DateTime.Now },
            new User { UserName = "user1", Email = "user1@example.com", RegistrationDate = DateTime.Now },
            new User { UserName = "user2", Email = "user2@example.com", RegistrationDate = DateTime.Now },
            };

            foreach (User user in users)
            {
                await userManager.CreateAsync(user, "123456");
                if (user.UserName == "admin")
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            context.SaveChanges();

            // 添加板块数据
            var forums = new Forum[]
            {
                new Forum { ForumName = "综合讨论", Description = "一般性的讨论主题" },
                new Forum { ForumName = "科技", Description = "与科技相关的讨论" },
                new Forum { ForumName = "娱乐", Description = "娱乐相关的讨论" },
            };

            foreach (Forum forum in forums)
            {
                context.Forums.Add(forum);
            }

            context.SaveChanges();

            // 添加帖子数据
            var posts = new Post[]
            {
                new Post { Title = "欢迎来到论坛", Content = "欢迎大家！", PostedTime = DateTime.Now, UserID = users[0].Id, ForumID = forums[0].ForumID },
                new Post { Title = "你最喜欢的编程语言是什么？", Content = "分享你最喜欢的编程语言。", PostedTime = DateTime.Now, UserID = users[1].Id, ForumID = forums[1].ForumID },
                new Post { Title = "最近看过什么好电影？", Content = "大家最近看过什么好电影？一起来分享一下！", PostedTime = DateTime.Now, UserID = users[2].Id, ForumID = forums[2].ForumID },
                new Post { Title = "大家最喜欢的一款手机是什么？", Content = "分享一下你最喜欢的手机型号。", PostedTime = DateTime.Now, UserID = users[0].Id, ForumID = forums[1].ForumID },
                new Post { Title = "疫情期间，你是如何度过的？", Content = "大家在疫情期间是如何度过的？", PostedTime = DateTime.Now, UserID = users[1].Id, ForumID = forums[0].ForumID },
                new Post { Title = "最近有什么好看的电视剧推荐？", Content = "想找一些好看的电视剧，有推荐吗？", PostedTime = DateTime.Now, UserID = users[2].Id, ForumID = forums[2].ForumID },
                new Post { Title = "你们平时喜欢做什么运动？", Content = "大家平时喜欢做什么运动？", PostedTime = DateTime.Now, UserID = users[0].Id, ForumID = forums[0].ForumID },
            };

            foreach (Post post in posts)
            {
                context.Posts.Add(post);
            }

            context.SaveChanges();

            // 添加评论数据
            var comments = new Comment[]
            {
                new Comment { Content = "你好，我是user1！", PostedTime = DateTime.Now, UserID = users[0].Id, PostID = posts[0].PostID },
                new Comment { Content = "我喜欢C#!", PostedTime = DateTime.Now, UserID = users[1].Id, PostID = posts[1].PostID },
                new Comment { Content = "最近看了《流浪地球》，觉得很不错！", PostedTime = DateTime.Now, UserID = users[2].Id, PostID = posts[2].PostID },
                new Comment { Content = "我最喜欢的手机是iPhone！", PostedTime = DateTime.Now, UserID = users[0].Id, PostID = posts[3].PostID },
                new Comment { Content = "疫情期间我学会了烹饪。", PostedTime = DateTime.Now, UserID = users[1].Id, PostID = posts[4].PostID },
                new Comment { Content = "《长安十二时辰》是一部好看的电视剧。", PostedTime = DateTime.Now, UserID = users[2].Id, PostID = posts[5].PostID },
                new Comment { Content = "我喜欢打篮球。", PostedTime = DateTime.Now, UserID = users[0].Id, PostID = posts[6].PostID },
            };

            foreach (Comment comment in comments)
            {
                context.Comments.Add(comment);
            }

            context.SaveChanges();
        }
    }
}
