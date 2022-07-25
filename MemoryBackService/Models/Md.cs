using LiteDB;

namespace MemoryBackService.Models
{
    public class Md
    {
        public ObjectId Id { get; set; }
        public Headline Headline { get; set; }
        public string Text { get; set; }
    }
}