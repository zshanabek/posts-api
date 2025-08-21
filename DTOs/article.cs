namespace PostsBlogApi.DTOs
{
    public class CreateArticle
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class ArticleShort
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }
}
