namespace Mentorea.Entities
{
    public class Specialization
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;

        public List<Field> Fields { get; set; } = [];
    }
}
