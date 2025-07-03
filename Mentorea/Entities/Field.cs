using System.ComponentModel.DataAnnotations.Schema;

namespace Mentorea.Entities
{
    public class Field
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FieldName { get; set; } = null!;
        public string SpecializationId { get; set; } = null!;
        public Specialization Specialization { get; set; } = null!;
        public List<MenteeFieldInterests> MenteeFieldInterests { get; set; } = [];
        public List<ApplicationUser> Users { get; set; } = [];

    }
}
