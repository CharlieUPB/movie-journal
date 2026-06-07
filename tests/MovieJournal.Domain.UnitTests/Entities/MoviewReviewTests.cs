using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Enums;
using MovieJournal.Domain.Exceptions;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Domain.UnitTests.Entities;

public class MovieReviewTests
{
    [Fact]
    public void ShouldCreateMovieReviewAsDraft()
    {
        var userId = Guid.NewGuid();

        var review = CreateMovieReview(userId);

        Assert.Equal(userId, review.UserId);
        Assert.Equal(ReviewStatus.Draft, review.Status);
    }

    [Fact]
    public void PublishReview_WhenReviewIsDraft_ShouldChangeStatusToPublished()
    {
        var review = CreateMovieReview();

        review.PublishReview();

        Assert.Equal(ReviewStatus.Published, review.Status);
    }

    [Fact]
    public void PublishReview_WhenReviewIsAlreadyPublished_ShouldThrowDomainException()
    {
        var review = CreateMovieReview();
        review.PublishReview();

        var exception = Assert.Throws<DomainException>(() =>
            review.PublishReview());

        Assert.Equal("Only draft reviews can be published", exception.Message);
    }

    [Fact]
    public void UpdateMovieReview_WhenReviewIsArchived_ShouldThrowDomainException()
    {
        var review = CreateMovieReview();
        review.ArchiveReview();

        var newMovieInformation = CreateMovieInformation("Interstellar", 2014);
        var newReviewInformation = CreateReviewInformation(
            "Amazing movie",
            "One of the best sci-fi movies I have watched, great character development and excellent visuals",
            5);

        var exception = Assert.Throws<DomainException>(() =>
            review.UpdateMovieReview(newMovieInformation, newReviewInformation));

        Assert.Equal("Archived reviews cannot be updated", exception.Message);
    }

    [Fact]
    public void UpdateMovieReview_WhenReviewIsDraft_ShouldUpdateInformation()
    {
        var review = CreateMovieReview();

        var newMovieInformation = CreateMovieInformation("Interstellar", 2014);
        var newReviewInformation = CreateReviewInformation(
            "Amazing movie",
            "One of the best sci-fi movies I have watched, great character development and excellent visuals",
            5);

        review.UpdateMovieReview(newMovieInformation, newReviewInformation);

        Assert.Equal("Interstellar", review.MovieInformation.MovieTitle);
        Assert.Equal(2014, review.MovieInformation.ReleaseYear);
        Assert.Equal("Amazing movie", review.ReviewInformation.ReviewTitle);
        Assert.Equal(5, review.ReviewInformation.Rating);
    }

    [Fact]
    public void UpdateMovieReview_WhenReviewIsPublished_ShouldUpdateInformation()
    {
        var review = CreateMovieReview();

        review.PublishReview();

        var newMovieInformation = CreateMovieInformation("Interstellar", 2014);
        var newReviewInformation = CreateReviewInformation(
            "Amazing movie",
            "One of the best sci-fi movies I have watched, great character development and excellent visuals",
            5);

        review.UpdateMovieReview(newMovieInformation, newReviewInformation);

        Assert.Equal("Interstellar", review.MovieInformation.MovieTitle);
        Assert.Equal(2014, review.MovieInformation.ReleaseYear);
        Assert.Equal("Amazing movie", review.ReviewInformation.ReviewTitle);
        Assert.Equal(5, review.ReviewInformation.Rating);
    }

    [Fact]
    public void ArchiveReview_ShouldChangeStatusToArchived()
    {
        var review = CreateMovieReview();

        review.ArchiveReview();

        Assert.Equal(ReviewStatus.Archived, review.Status);
    }

    [Fact]
    public void DeleteReview_ShouldChangeIsDeleted()
    {
        var review = CreateMovieReview();

        review.Delete();

        Assert.True(review.IsDeleted);
    }

    [Fact]
    public void EnsureCanReceiveComments_WhenReviewIsPublished_ShouldNotThrow()
    {
        var movieReview = CreateMovieReview();
        movieReview.PublishReview();

        var exception = Record.Exception(() =>
            movieReview.EnsureCanReceiveComments());

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureCanReceiveComments_WhenReviewIsDraft_ShouldThrowDomainException()
    {
        var movieReview = CreateMovieReview();

        var exception = Assert.Throws<DomainException>(() =>
            movieReview.EnsureCanReceiveComments());

        Assert.Equal("Comments are allowed only on published reviews", exception.Message);
    }

    [Fact]
    public void EnsureCanReceiveComments_WhenReviewIsArchived_ShouldThrowDomainException()
    {
        var movieReview = CreateMovieReview();
        movieReview.ArchiveReview();

        var exception = Assert.Throws<DomainException>(() =>
            movieReview.EnsureCanReceiveComments());

        Assert.Equal("Comments are allowed only on published reviews", exception.Message);
    }

    private static MovieReview CreateMovieReview(Guid? userId = null)
    {
        return MovieReview.Create(
            userId ?? Guid.NewGuid(),
            CreateMovieInformation(),
            CreateReviewInformation());
    }

    private static MovieInformation CreateMovieInformation(
        string movieTitle = "Project Hail Mary",
        int? releaseYear = 2026)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return MovieInformation.Create(
            movieTitle,
            today,
            releaseYear);
    }

    private static ReviewInformation CreateReviewInformation(
        string reviewTitle = "Great story",
        string reviewContent = "A very enjoyable story with interesting characters and strong emotional moments.",
        int rating = 5)
    {
        return ReviewInformation.Create(
            reviewTitle,
            reviewContent,
            rating);
    }
}