using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsStore.Models
{
    [Table("DynamicMenuItem")]
    public class DynamicMenuItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? MID { get; set; }
        public string MenuName { get; set; }
        public string MenuURL { get; set; }
        public int? MenuParentID { get; set; }

        [ForeignKey("MenuParentID")]
        public virtual DynamicMenuItem ParentMenuItem { get; set; }
    }
}
