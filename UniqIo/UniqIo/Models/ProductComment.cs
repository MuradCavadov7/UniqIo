namespace UniqIo.Models
{
    public class ProductComment: BaseEntity
    {
        public string? CommitComment {  get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
