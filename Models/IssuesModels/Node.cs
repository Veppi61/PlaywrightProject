namespace PlaywrightProject.Models.IssuesModel
{
    public class Node
    {
        public string? Name { get; set; }
        public Owner? Owner { get; set; }
        public int? StargazerCount { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
    }
}
