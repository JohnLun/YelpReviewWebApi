using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnimalCrossingAPI.Models
{

    public class ReviewDto
    {
        public ReviewDto(Review review)
        {
            Id = review.Id;
            Stars = review.Stars;
            Text = review.Text;
            BusinessName = review.BusinessName;
            User = new UserDto(review.User);
        }

        public int Id { get; set; }
        public int Stars { get; set; }
        public string Text { get; set; }

        public string BusinessName { get; set; }
        
        public UserDto User { get; set; }
        
    }

    public class UserDto
    {
        public UserDto(User user)
        {
            Id = user.Id;
            ReviewId = user.ReviewId;
            Name = user.Name;   
        }
        public int Id { get; set; }
        public int ReviewId { get; set; }

        public string Name { get; set; }

        //public List<Review> Reviews { get; set; }
    }

    public class CreateReviewDto
    {
        public int Stars { get; set; }
        public string Text { get; set; }

        public string BusinessName { get; set;}
        public CreateUserDto User { get; set; }
       
    }

    public class CreateUserDto
    {

        public string Name { get; set; }
    }

    public class UpdateUserDto
    {
       
    }

    public class UpdateReviewDto
    {
        public int Stars { get; set; }

        public string Text { get; set; }

        public string BusinessName { get; set;}

    }

}