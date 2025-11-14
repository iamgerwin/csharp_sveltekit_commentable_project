namespace CommentableAPI.Application.DTOs.Stats;

public class StatsDto
{
    public int TotalVideos { get; set; }
    public int TotalPosts { get; set; }
    public int TotalComments { get; set; }
    public int TotalReactions { get; set; }
    public int MyVideos { get; set; }
    public int MyPosts { get; set; }
    public int MyComments { get; set; }
    public int PendingReports { get; set; }
    public int FlaggedContent { get; set; }
}
