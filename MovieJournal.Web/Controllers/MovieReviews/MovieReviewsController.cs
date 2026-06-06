using Microsoft.AspNetCore.Mvc;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Requests;

namespace MovieJournal.Web.Controllers.MovieReviews;

[Route("api/[controller]")]
[ApiController]
public class MovieReviewsController : ControllerBase
{

    private readonly CreateMovieReviewCmd _createMovieReviewCmd;

    public MovieReviewsController(CreateMovieReviewCmd createMovieReviewCmd)
    {
        _createMovieReviewCmd = createMovieReviewCmd;
    }

    [HttpGet("{id:Guid}")]
    public IActionResult GetMovieReviewById(Guid id)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovieReview(CreateMovieReviewHttpRequest body)
    {

        string input = "11111111-1111-1111-1111-111111111111";

        var userGuid = Guid.Parse(input);

        var request = new CreateMovieRequest(
            userGuid,
            body.MovieTitle,
            body.ReviewTitle,
            body.ReviewContent,
            body.ReviewRating,
            body.MovieReleaseYear);

        var result = await _createMovieReviewCmd.Execute(request);

        return CreatedAtAction(nameof(GetMovieReviewById), new { id = result.Id }, result);
    }

    [HttpPut("{id:Guid}")]
    public IActionResult UpdateMovieReview(Guid id)
    {
        return Ok();
    }

    [HttpDelete("{id:Guid}")]
    public IActionResult DeleteMovieReview(Guid id)
    {
        return Ok();
    }
}
