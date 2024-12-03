using AnimalCrossingAPI.Auth;
using AnimalCrossingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace AnimalCrossingAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly YelpReviewDbContext _db;
        private readonly IAuthorizationService _authorizationService;
        public ReviewController(YelpReviewDbContext dbContext, IAuthorizationService authorizationService)
        {
            _db = dbContext;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Retrieves a list of all reviews.
        /// </summary>
        /// <returns>A list of reviews with detailed information.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> ListReviews()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var reviews = await _db.Reviews.Include(r => r.User).ToListAsync();
            var reviewDtoList = reviews.Select(r => new ReviewDto(r)).ToList();
            return Ok(reviewDtoList);
        }

        /// <summary>
        /// Creates a new review entry in the database.
        /// </summary>
        /// <param name="createReviewDto">The data containing details about the new review.</param>
        /// <returns>The created review with its generated identifier.</returns>
        [HttpPost]
        [Authorize(Policy = "RequireEditReviewScope")]
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Map the CreateReviewDto to the Review entity
            var review = new Review
            {
                Stars = createReviewDto.Stars,
                Text = createReviewDto.Text,
                CreatedBy = userId,
                BusinessName = createReviewDto.BusinessName
            };

            review.User = new User
            {
                Name = createReviewDto.User.Name
            };

            // Add the new fish to the database
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            // Map the saved fish to FishDto to return in the response
            var reviewDto = new ReviewDto(review);

            return CreatedAtAction(nameof(CreateReview), new { id = review.Id }, reviewDto);
        }



        /// <summary>
        /// Updates details of an existing review (excluding user information).
        /// </summary>
        /// <param name="id">The unique identifier of the review to update.</param>
        /// <param name="updateDto">The data containing updated review details.</param>
        /// <returns>A confirmation of successful update or an error if the review is not found.</returns>
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDto reviewDto)
        {
            var review = await _db.Reviews.FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound($"Review with Id {id} not found.");
            }

            // Manually check authorization with the Review resource
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, review, "RequireEditReviewScope");

            if (!authorizationResult.Succeeded)
            {
                return Forbid(); // Or return a custom forbidden response
            }

            // Update properties only if they are provided in the DTO
            if (reviewDto.Stars != null) review.Stars = reviewDto.Stars;
            if (reviewDto.BusinessName != null) review.BusinessName = reviewDto.BusinessName;
            if (reviewDto.Text != null) review.Text = reviewDto.Text;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes an existing review.
        /// </summary>
        /// <param name="id">The unique identifier of the review to delete.</param>
        /// <returns>A confirmation of successful delete or an error if the review is not found.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "RequireEditReviewScope")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            var review = await _db.Reviews
                .Include(r => r.User)  // Optional: Include the user for any user-related logic
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                return NotFound(); // Return 404 if the review does not exist
            }

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();

            // Optionally, return the deleted review data
            return Ok(new ReviewDto(review));  // Return the deleted review as a DTO (optional)
        }

     


    }
}