using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Domain.Enums;
using MovieJournal.Web.Authentication;

namespace MovieJournal.Web.Controllers.MovieReviews;

[Route("api/movie-reviews")]
[ApiController]
public class MovieReviewsController : ControllerBase
{
    private readonly CreateMovieReviewCmd _createMovieReviewCmd;
    private readonly UpdateMovieReviewCmd _updateMovieReviewCmd;
    private readonly DeleteMovieReviewCmd _deleteMovieReviewCmd;
    private readonly PublishMovieReviewCmd _publishMovieReviewCmd;
    private readonly ArchiveMovieReviewCmd _archiveMovieReviewCmd;

    private readonly GetMovieReviewQuery _getMovieReviewQuery;
    private readonly ListMovieReviewsQuery _listMovieReviewsQuery;
    private readonly ListPublishedMovieReviewsQuery _listPublishedMovieReviewsQuery;
    private readonly ListMovieReviewsByUserIdQuery _listMovieReviewsByUserIdQuery;
    private readonly ListMovieReviewsByUserIdAndStatusQuery _listMovieReviewsByUserIdAndStatusQuery;

    public MovieReviewsController(
        CreateMovieReviewCmd createMovieReviewCmd,
        UpdateMovieReviewCmd updateMovieReviewCmd,
        DeleteMovieReviewCmd deleteMovieReviewCmd,
        PublishMovieReviewCmd publishMovieReviewCmd,
        ArchiveMovieReviewCmd archiveMovieReviewCmd,
        GetMovieReviewQuery getMovieReviewQuery,
        ListMovieReviewsQuery listMovieReviewsQuery,
        ListPublishedMovieReviewsQuery listPublishedMovieReviewsQuery,
        ListMovieReviewsByUserIdQuery listMovieReviewsByUserIdQuery,
        ListMovieReviewsByUserIdAndStatusQuery listMovieReviewsByUserIdAndStatusQuery)
    {
        _createMovieReviewCmd = createMovieReviewCmd;
        _updateMovieReviewCmd = updateMovieReviewCmd;
        _deleteMovieReviewCmd = deleteMovieReviewCmd;
        _publishMovieReviewCmd = publishMovieReviewCmd;
        _archiveMovieReviewCmd = archiveMovieReviewCmd;
        _getMovieReviewQuery = getMovieReviewQuery;
        _listMovieReviewsQuery = listMovieReviewsQuery;
        _listPublishedMovieReviewsQuery = listPublishedMovieReviewsQuery;
        _listMovieReviewsByUserIdQuery = listMovieReviewsByUserIdQuery;
        _listMovieReviewsByUserIdAndStatusQuery = listMovieReviewsByUserIdAndStatusQuery;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMovieReviews()
    {
        var result = await _listMovieReviewsQuery.Execute(new ListMovieReviewsRequest());

        return Ok(result);
    }

    [HttpGet("published")]
    public async Task<IActionResult> GetPublishedMovieReviews()
    {
        var result = await _listPublishedMovieReviewsQuery.Execute(new ListPublishedMovieReviewsRequest());

        return Ok(result);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyMovieReviews([FromQuery] ReviewStatus? status)
    {
        var userId = User.GetRequiredUserId();

        if (status.HasValue)
        {
            var resultByStatus = await _listMovieReviewsByUserIdAndStatusQuery.Execute(
                new ListMovieReviewsByUserIdAndStatusRequest(userId, status.Value));

            return Ok(resultByStatus);
        }

        var result = await _listMovieReviewsByUserIdQuery.Execute(new ListMovieReviewsByUserIdRequest(userId));

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetMovieReviewById(Guid id)
    {
        var userId = User.GetOptionalUserId();

        var result = await _getMovieReviewQuery.Execute(new GetMovieReviewRequest(id, userId));

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateMovieReview([FromBody] CreateMovieReviewHttpRequest body)
    {
        var userId = User.GetRequiredUserId();

        var request = new CreateMovieRequest(
            userId,
            body.MovieTitle,
            body.ReviewTitle,
            body.ReviewContent,
            body.ReviewRating,
            body.MovieReleaseYear);

        var result = await _createMovieReviewCmd.Execute(request);

        return CreatedAtAction(
            nameof(GetMovieReviewById),
            new { id = result.Id },
            result);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateMovieReview(
        Guid id,
        [FromBody] UpdateMovieReviewHttpRequest body)
    {
        var userId = User.GetRequiredUserId();

        var request = new UpdateMovieReviewRequest(
            id,
            userId,
            body.MovieTitle,
            body.ReviewTitle,
            body.ReviewContent,
            body.ReviewRating,
            body.MovieReleaseYear);

        var result = await _updateMovieReviewCmd.Execute(request);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> PublishMovieReview(Guid id)
    {
        var userId = User.GetRequiredUserId();

        var result = await _publishMovieReviewCmd.Execute(new PublishMovieReviewRequest(id, userId));

        return Ok(result);
    }

    [Authorize]
    [HttpPost("{id:guid}/archive")]
    public async Task<IActionResult> ArchiveMovieReview(Guid id)
    {
        var userId = User.GetRequiredUserId();

        var result = await _archiveMovieReviewCmd.Execute(new ArchiveMovieReviewRequest(id, userId));

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMovieReview(Guid id)
    {
        var userId = User.GetRequiredUserId();

        await _deleteMovieReviewCmd.Execute(new DeleteMovieReviewRequest(id, userId));

        return NoContent();
    }
}
