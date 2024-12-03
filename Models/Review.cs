using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AnimalCrossingAPI.Models
{

    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public User User { get; set; }
        [Required]
        public int Stars { get; set; }

        [Required]
        public string BusinessName { get; set; }

        [Required]
        public string Text { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Review")]
        public int ReviewId { get; set; }

        public string Name { get; set; }

        public List<Review> Reviews { get; set; }
    }

}