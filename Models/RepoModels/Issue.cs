namespace PlaywrightProject.Models.RepoModels
{
    public class Issue
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Author? Author { get; set; }
        public string? State { get; set; }
    }
}
