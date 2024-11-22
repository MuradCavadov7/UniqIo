namespace UniqIo.Models
{
    public class Company : BaseEntity
    {
        public  string Name  { get; set; }
        public ICollection <Product> Products { get; set; }
    }
}
