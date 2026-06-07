using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieJournal.Application.ReviewComments.Commands;
using MovieJournal.Application.ReviewComments.Queries;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Web.Authentication;

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
        var userId = User.GetOptionalUserId();

        var result = await _listReviewCommentsQuery.Execute(
            new ListReviewCommentsRequest(movieReviewId, userId));

        return Ok(result);
    }

    [Authorize]
    [HttpPost("movie-reviews/{movieReviewId:guid}/comments")]
    public async Task<IActionResult> AddReviewComment(
        Guid movieReviewId,
        [FromBody] AddReviewCommentHttpRequest body)
    {
        var userId = User.GetRequiredUserId();

        var result = await _addReviewCommentCmd.Execute(
            new AddReviewCommentRequest(movieReviewId, userId, body.Content));

        return Created($"/api/review-comments/{result.Id}", result);
    }

    [Authorize]
    [HttpPut("review-comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateReviewComment(
        Guid commentId,
        [FromBody] UpdateReviewCommentHttpRequest body)
    {
        var userId = User.GetRequiredUserId();

        var result = await _updateReviewCommentCmd.Execute(
            new UpdateReviewCommentRequest(commentId, userId, body.Content));

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("review-comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteReviewComment(Guid commentId)
    {
        var userId = User.GetRequiredUserId();

        await _deleteReviewCommentCmd.Execute(new DeleteReviewCommentRequest(commentId, userId));

        return NoContent();
    }
}
