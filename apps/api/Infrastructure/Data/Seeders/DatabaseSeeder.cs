using CommentableAPI.Domain.Entities;
using CommentableAPI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CommentableAPI.Infrastructure.Data.Seeders;

/// <summary>
/// Main database seeder that orchestrates all entity seeders
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly Random _random = new();

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed all data in the correct order (respecting foreign key constraints)
    /// </summary>
    public async Task SeedAllAsync()
    {
        Console.WriteLine("🌱 Starting database seeding...");

        // Check if data already exists
        if (await _context.Users.AnyAsync())
        {
            Console.WriteLine("⚠️  Database already contains data. Skipping seed.");
            return;
        }

        try
        {
            // Seed in dependency order
            var users = await SeedUsersAsync();
            Console.WriteLine($"✓ Seeded {users.Count} users");

            var videos = await SeedVideosAsync(users);
            Console.WriteLine($"✓ Seeded {videos.Count} videos");

            var posts = await SeedPostsAsync(users);
            Console.WriteLine($"✓ Seeded {posts.Count} posts");

            var comments = await SeedCommentsAsync(users, videos, posts);
            Console.WriteLine($"✓ Seeded {comments.Count} comments");

            var reactions = await SeedReactionsAsync(users, comments);
            Console.WriteLine($"✓ Seeded {reactions.Count} reactions");

            var reports = await SeedReportsAsync(users, comments);
            Console.WriteLine($"✓ Seeded {reports.Count} reports");

            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during seeding: {ex.Message}");
            throw;
        }
    }

    private async Task<List<User>> SeedUsersAsync()
    {
        var users = new List<User>
        {
            // Admin user
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@commentable.com",
                DisplayName = "Admin User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                Status = EntityStatus.Active,
                Bio = "System administrator",
                AvatarUrl = "https://i.pravatar.cc/150?img=1",
                CreatedAt = DateTime.UtcNow.AddMonths(-6)
            },
            // Moderator users
            new User
            {
                Id = Guid.NewGuid(),
                Username = "moderator",
                Email = "moderator@commentable.com",
                DisplayName = "Sarah Moderator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Mod123!"),
                Role = UserRole.Moderator,
                Status = EntityStatus.Active,
                Bio = "Keeping the community safe",
                AvatarUrl = "https://i.pravatar.cc/150?img=2",
                CreatedAt = DateTime.UtcNow.AddMonths(-5)
            },
            // Regular users
            new User
            {
                Id = Guid.NewGuid(),
                Username = "john_doe",
                Email = "john@example.com",
                DisplayName = "John Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                Status = EntityStatus.Active,
                Bio = "Tech enthusiast and content creator",
                AvatarUrl = "https://i.pravatar.cc/150?img=3",
                CreatedAt = DateTime.UtcNow.AddMonths(-4)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "jane_smith",
                Email = "jane@example.com",
                DisplayName = "Jane Smith",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                Status = EntityStatus.Active,
                Bio = "Developer | Coffee lover ☕",
                AvatarUrl = "https://i.pravatar.cc/150?img=4",
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "mike_wilson",
                Email = "mike@example.com",
                DisplayName = "Mike Wilson",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                Status = EntityStatus.Active,
                Bio = "Gaming and tech reviews",
                AvatarUrl = "https://i.pravatar.cc/150?img=5",
                CreatedAt = DateTime.UtcNow.AddMonths(-2)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "emily_chen",
                Email = "emily@example.com",
                DisplayName = "Emily Chen",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                Status = EntityStatus.Active,
                Bio = "UX Designer & Frontend Developer",
                AvatarUrl = "https://i.pravatar.cc/150?img=6",
                CreatedAt = DateTime.UtcNow.AddMonths(-1)
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "alex_kumar",
                Email = "alex@example.com",
                DisplayName = "Alex Kumar",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.User,
                Status = EntityStatus.Active,
                Bio = "Full-stack developer exploring new technologies",
                AvatarUrl = "https://i.pravatar.cc/150?img=7",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            // Guest user (for testing)
            new User
            {
                Id = Guid.NewGuid(),
                Username = "guest",
                Email = "guest@example.com",
                DisplayName = "Guest User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Guest123!"),
                Role = UserRole.Guest,
                Status = EntityStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        await _context.Users.AddRangeAsync(users);
        return users;
    }

    private async Task<List<Video>> SeedVideosAsync(List<User> users)
    {
        var contentCreators = users.Where(u => u.Role == UserRole.User).Take(4).ToList();
        var videos = new List<Video>();

        var videoTitles = new[]
        {
            "Getting Started with C# .NET 9",
            "Building Modern Web Apps with SvelteKit",
            "Database Design Best Practices",
            "Introduction to Clean Architecture",
            "API Development with Swagger/OpenAPI",
            "PostgreSQL Performance Tuning",
            "Redis Caching Strategies",
            "Entity Framework Core Deep Dive",
            "TypeScript Tips and Tricks",
            "Microservices Architecture Explained"
        };

        for (int i = 0; i < videoTitles.Length; i++)
        {
            var creator = contentCreators[i % contentCreators.Count];
            var video = new Video
            {
                Id = Guid.NewGuid(),
                Title = videoTitles[i],
                Description = $"A comprehensive guide to {videoTitles[i].ToLower()}. Perfect for beginners and experienced developers alike.",
                VideoUrl = $"https://example.com/videos/{i + 1}.mp4",
                ThumbnailUrl = $"https://picsum.photos/seed/{i}/640/360",
                Duration = _random.Next(300, 3600), // 5-60 minutes
                ViewCount = _random.Next(100, 50000),
                CommentCount = 0, // Will be updated after comments are seeded
                Status = EntityStatus.Active,
                UserId = creator.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 90))
            };
            videos.Add(video);
        }

        await _context.Videos.AddRangeAsync(videos);
        return videos;
    }

    private async Task<List<Post>> SeedPostsAsync(List<User> users)
    {
        var authors = users.Where(u => u.Role == UserRole.User).Take(4).ToList();
        var posts = new List<Post>();

        var postData = new[]
        {
            ("why-clean-architecture-matters", "Why Clean Architecture Matters in Modern Development", "Clean architecture is more than just a buzzword..."),
            ("monorepo-best-practices", "MonoRepo Best Practices for Large Teams", "Managing multiple projects in a single repository..."),
            ("api-versioning-strategies", "API Versioning Strategies That Actually Work", "When building APIs, versioning is crucial..."),
            ("typescript-vs-javascript", "TypeScript vs JavaScript: When to Use Each", "The eternal debate in the JavaScript ecosystem..."),
            ("database-indexing-guide", "The Complete Guide to Database Indexing", "Proper indexing can make or break your application..."),
            ("svelte-vs-react", "SvelteKit vs React: A Practical Comparison", "Comparing two popular frontend frameworks..."),
            ("microservices-pitfalls", "Common Microservices Pitfalls and How to Avoid Them", "Microservices aren't always the answer..."),
            ("csharp-performance-tips", "C# Performance Optimization Tips", "Writing performant C# code requires attention..."),
        };

        for (int i = 0; i < postData.Length; i++)
        {
            var (slug, title, content) = postData[i];
            var author = authors[i % authors.Count];
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = title,
                Slug = slug,
                Content = content + " Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                FeaturedImageUrl = $"https://picsum.photos/seed/post{i}/1200/630",
                CommentCount = 0, // Will be updated after comments are seeded
                ViewCount = _random.Next(500, 10000),
                Status = EntityStatus.Active,
                PublishedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 60)),
                UserId = author.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 60))
            };
            posts.Add(post);
        }

        await _context.Posts.AddRangeAsync(posts);
        return posts;
    }

    private async Task<List<Comment>> SeedCommentsAsync(List<User> users, List<Video> videos, List<Post> posts)
    {
        var commenters = users.Where(u => u.Role != UserRole.Guest).ToList();
        var comments = new List<Comment>();

        var commentTemplates = new[]
        {
            "Great content! Really helpful.",
            "Thanks for sharing this. Learned a lot!",
            "This is exactly what I was looking for.",
            "Could you explain more about {topic}?",
            "Interesting perspective on this.",
            "I disagree with some points, but overall good.",
            "Brilliant explanation! Subscribed.",
            "This helped me solve my problem. Thanks!",
            "Would love to see more content like this.",
            "Not sure I understand the part about {topic}.",
        };

        // Comments on videos
        foreach (var video in videos.Take(8))
        {
            var numComments = _random.Next(3, 10);
            for (int i = 0; i < numComments; i++)
            {
                var commenter = commenters[_random.Next(commenters.Count)];
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = commentTemplates[_random.Next(commentTemplates.Length)].Replace("{topic}", "this"),
                    UserId = commenter.Id,
                    CommentableType = CommentableType.Video,
                    CommentableId = video.Id,
                    Status = EntityStatus.Active,
                    ReactionCount = 0,
                    ReplyCount = 0,
                    CreatedAt = video.CreatedAt.AddHours(_random.Next(1, 24))
                };
                comments.Add(comment);
            }
        }

        // Comments on posts
        foreach (var post in posts.Take(6))
        {
            var numComments = _random.Next(5, 15);
            for (int i = 0; i < numComments; i++)
            {
                var commenter = commenters[_random.Next(commenters.Count)];
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = commentTemplates[_random.Next(commentTemplates.Length)].Replace("{topic}", post.Title),
                    UserId = commenter.Id,
                    CommentableType = CommentableType.Post,
                    CommentableId = post.Id,
                    Status = EntityStatus.Active,
                    ReactionCount = 0,
                    ReplyCount = 0,
                    CreatedAt = post.CreatedAt.AddHours(_random.Next(1, 48))
                };
                comments.Add(comment);
            }
        }

        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync(); // Save to get IDs for replies

        // Add some nested comments (replies)
        var topLevelComments = comments.Take(20).ToList();
        var replies = new List<Comment>();

        foreach (var parentComment in topLevelComments.Take(10))
        {
            var numReplies = _random.Next(1, 4);
            for (int i = 0; i < numReplies; i++)
            {
                var commenter = commenters[_random.Next(commenters.Count)];
                var reply = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = $"@{users.First(u => u.Id == parentComment.UserId).Username} I agree with your point!",
                    UserId = commenter.Id,
                    CommentableType = parentComment.CommentableType,
                    CommentableId = parentComment.CommentableId,
                    ParentCommentId = parentComment.Id,
                    Status = EntityStatus.Active,
                    ReactionCount = 0,
                    ReplyCount = 0,
                    CreatedAt = parentComment.CreatedAt.AddMinutes(_random.Next(30, 300))
                };
                replies.Add(reply);
            }
        }

        await _context.Comments.AddRangeAsync(replies);
        comments.AddRange(replies);

        return comments;
    }

    private async Task<List<Reaction>> SeedReactionsAsync(List<User> users, List<Comment> comments)
    {
        var reactors = users.Where(u => u.Role != UserRole.Guest).ToList();
        var reactions = new List<Reaction>();
        var reactionTypes = Enum.GetValues<ReactionType>();

        // Add reactions to random comments
        foreach (var comment in comments.Take(50))
        {
            var numReactions = _random.Next(0, Math.Min(reactors.Count, 8));
            var selectedReactors = reactors.OrderBy(_ => _random.Next()).Take(numReactions).ToList();

            foreach (var reactor in selectedReactors)
            {
                var reaction = new Reaction
                {
                    Id = Guid.NewGuid(),
                    UserId = reactor.Id,
                    CommentId = comment.Id,
                    ReactionType = reactionTypes[_random.Next(reactionTypes.Length)],
                    CreatedAt = comment.CreatedAt.AddMinutes(_random.Next(5, 120))
                };
                reactions.Add(reaction);
            }
        }

        await _context.Reactions.AddRangeAsync(reactions);
        return reactions;
    }

    private async Task<List<Report>> SeedReportsAsync(List<User> users, List<Comment> comments)
    {
        var reporters = users.Where(u => u.Role == UserRole.User || u.Role == UserRole.Moderator).Take(3).ToList();
        var moderator = users.First(u => u.Role == UserRole.Moderator);
        var reports = new List<Report>();

        // Flag a few comments as inappropriate
        var flaggedComments = comments.OrderBy(_ => _random.Next()).Take(5).ToList();

        foreach (var comment in flaggedComments)
        {
            var reporter = reporters[_random.Next(reporters.Count)];
            var isReviewed = _random.Next(2) == 0;

            var report = new Report
            {
                Id = Guid.NewGuid(),
                UserId = reporter.Id,
                CommentId = comment.Id,
                Reason = (ReportCategory)_random.Next(1, 8), // Random category
                Description = "This comment violates community guidelines.",
                Status = isReviewed ? (ReportStatus)_random.Next(2, 5) : ReportStatus.Pending,
                ReviewedAt = isReviewed ? DateTime.UtcNow.AddHours(-_random.Next(1, 24)) : null,
                ReviewedBy = isReviewed ? moderator.Id : null,
                ReviewNotes = isReviewed ? "Reviewed and appropriate action taken." : null,
                CreatedAt = comment.CreatedAt.AddHours(_random.Next(1, 48))
            };
            reports.Add(report);

            // Update comment status if flagged
            if (!isReviewed)
            {
                comment.Status = EntityStatus.Flagged;
            }
        }

        await _context.Reports.AddRangeAsync(reports);
        return reports;
    }
}
