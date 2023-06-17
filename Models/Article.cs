using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{

    public class Article
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        [Column(TypeName = "nvarchar")]
        public string Title { get; set; }

        [Column(TypeName = "ntext")]
        public string? Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
    }
}