using Microsoft.AspNetCore.Mvc;
using MovieJournal.Application.ReviewComments.Commands;
using MovieJournal.Application.ReviewComments.Queries;
using MovieJournal.Application.ReviewComments.Requests;

namespace MovieJournal.Web.Controllers.ReviewComments;

[Route("api")]
[ApiController]
public class ReviewCommentsController : ControllerBase
{
    private readonly AddReviewCommentCmd _addReviewCommentCmd;
    private readonly UpdateReviewCommentCmd _updateReviewCommentCmd;
    private readonly DeleteReviewCommentCmd _deleteReviewCommentCmd;
    private readonly ListReviewCommentsQuery _listReviewCommentsQuery;

    public ReviewCommentsController(
        AddReviewCommentCmd addReviewCommentCmd,
        UpdateReviewCommentCmd updateReviewCommentCmd,
        DeleteReviewCommentCmd deleteReviewCommentCmd,
        ListReviewCommentsQuery listReviewCommentsQuery)
    {
        _addReviewCommentCmd = addReviewCommentCmd;
        _updateReviewCommentCmd = updateReviewCommentCmd;
        _deleteReviewCommentCmd = deleteReviewCommentCmd;
        _listReviewCommentsQuery = listReviewCommentsQuery;
    }

    [HttpGet("movie-reviews/{movieReviewId:guid}/comments")]
    public async Task<IActionResult> GetReviewComments(Guid movieReviewId)
    {
        var userId = GetCurrentUserIdOrNull();

        var result = await _listReviewCommentsQuery.Execute(
            new ListReviewCommentsRequest(movieReviewId, userId));

        return Ok(result);
    }

    [HttpPost("movie-reviews/{movieReviewId:guid}/comments")]
    public async Task<IActionResult> AddReviewComment(
        Guid movieReviewId,
        [FromBody] AddReviewCommentHttpRequest body)
    {
        var userId = GetCurrentUserId();

        var result = await _addReviewCommentCmd.Execute(
            new AddReviewCommentRequest(movieReviewId, userId, body.Content));

        return Created($"/api/review-comments/{result.Id}", result);
    }

    [HttpPut("review-comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateReviewComment(
        Guid commentId,
        [FromBody] UpdateReviewCommentHttpRequest body)
    {
        var userId = GetCurrentUserId();

        var result = await _updateReviewCommentCmd.Execute(
            new UpdateReviewCommentRequest(commentId, userId, body.Content));

        return Ok(result);
    }

    [HttpDelete("review-comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteReviewComment(Guid commentId)
    {
        var userId = GetCurrentUserId();

        await _deleteReviewCommentCmd.Execute(new DeleteReviewCommentRequest(commentId, userId));

        return NoContent();
    }

    private static Guid GetCurrentUserId()
    {
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    private static Guid? GetCurrentUserIdOrNull()
    {
        // For now, return the demo user.
        // Later, return null when the user is anonymous.
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }
}
