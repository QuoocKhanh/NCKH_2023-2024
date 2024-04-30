using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("User_Group")]
    public class ItemUserGroup
    {
        [Key]
        public int ID { get; set; }

        public int ID_UserChat { get; set; }

        public int ID_GroupChat { get; set; }
    }
}
