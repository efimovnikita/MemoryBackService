namespace MemoryBackService.Models
{
    public class Headline
    {
        public Title Title { get; set; }
        public Date Date { get; set; }
        public Layout Layout { get; set; }
        public Category Category { get; set; }
        public Tags Tags { get; set; }
        public Book Book { get; set; }
    }
}