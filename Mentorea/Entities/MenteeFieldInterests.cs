using System.ComponentModel.DataAnnotations.Schema;

namespace Mentorea.Entities
{
    public class MenteeFieldInterests
    {
        
        public string MenteeId { get; set; } = null!;
        public string FieldId { get; set; } = null!;
        public Field Field { get; set; } = null!;
        public ApplicationUser? User { get; set; } 
    }
}
