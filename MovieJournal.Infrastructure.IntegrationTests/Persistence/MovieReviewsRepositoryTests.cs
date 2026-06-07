using MovieJournal.Application.MovieReviews;
using MovieJournal.Domain.Enums;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Infrastructure.IntegrationTests.Persistence;

public class MovieReviewsRepositoryTests
{
    [Fact]
    public async Task CreateAsync_ShouldInsertMovieReview()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview(
            movieTitle: "Arrival",
            releaseYear: 2016,
            reviewTitle: "Thoughtful sci-fi",
            reviewContent: TestDatabase.ValidReviewContent,
            rating: 5);

        await database.Repository.CreateAsync(movieReview);

        var savedReview = await database.Repository.GetByIdAsync(movieReview.Id);

        Assert.NotNull(savedReview);
        Assert.Equal(movieReview.Id, savedReview.Id);
        Assert.Equal(TestDatabase.DemoUserId, savedReview.UserId);
        Assert.Equal("Arrival", savedReview.MovieInformation.MovieTitle);
        Assert.Equal(2016, savedReview.MovieInformation.ReleaseYear);
        Assert.Equal("Thoughtful sci-fi", savedReview.ReviewInformation.ReviewTitle);
        Assert.Equal(TestDatabase.ValidReviewContent, savedReview.ReviewInformation.ReviewContent);
        Assert.Equal(5, savedReview.ReviewInformation.Rating);
        Assert.Equal(ReviewStatus.Draft, savedReview.Status);
    }

    [Fact]
    public async Task GetByIdAsync_WhenReviewDoesNotExist_ShouldReturnNull()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);

        var savedReview = await database.Repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(savedReview);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldRebuildMovieReviewWithNullableReleaseYear()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview(releaseYear: null);

        await database.Repository.CreateAsync(movieReview);

        var savedReview = await database.Repository.GetByIdAsync(movieReview.Id);

        Assert.NotNull(savedReview);
        Assert.Null(savedReview.MovieInformation.ReleaseYear);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistUpdatedMovieReviewFields()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);
        var movieInformation = MovieInformation.Create(
            "Dune: Part Two",
            DateOnly.FromDateTime(DateTime.Today),
            2024);
        var reviewInformation = ReviewInformation.Create(
            "Epic follow-up",
            TestDatabase.UpdatedReviewContent,
            4);

        movieReview.UpdateMovieReview(movieInformation, reviewInformation);
        await database.Repository.UpdateAsync(movieReview);

        var savedReview = await database.Repository.GetByIdAsync(movieReview.Id);

        Assert.NotNull(savedReview);
        Assert.Equal("Dune: Part Two", savedReview.MovieInformation.MovieTitle);
        Assert.Equal(2024, savedReview.MovieInformation.ReleaseYear);
        Assert.Equal("Epic follow-up", savedReview.ReviewInformation.ReviewTitle);
        Assert.Equal(TestDatabase.UpdatedReviewContent, savedReview.ReviewInformation.ReviewContent);
        Assert.Equal(4, savedReview.ReviewInformation.Rating);
        Assert.NotNull(savedReview.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistPublishedStatus()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);

        movieReview.PublishReview();
        await database.Repository.UpdateAsync(movieReview);

        var savedReview = await database.Repository.GetByIdAsync(movieReview.Id);

        Assert.NotNull(savedReview);
        Assert.Equal(ReviewStatus.Published, savedReview.Status);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistArchivedStatus()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);

        movieReview.ArchiveReview();
        await database.Repository.UpdateAsync(movieReview);

        var savedReview = await database.Repository.GetByIdAsync(movieReview.Id);

        Assert.NotNull(savedReview);
        Assert.Equal(ReviewStatus.Archived, savedReview.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteMovieReview()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);

        movieReview.Delete();
        await database.Repository.DeleteAsync(movieReview);

        var savedReview = await database.Repository.GetByIdAsync(movieReview.Id);

        Assert.Null(savedReview);
    }

    [Fact]
    public async Task ListAsync_ShouldExcludeDeletedReviewsByDefault()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var activeReview = TestDatabase.CreateValidMovieReview(movieTitle: "Active Movie");
        var deletedReview = TestDatabase.CreateValidMovieReview(movieTitle: "Deleted Movie");
        await database.Repository.CreateAsync(activeReview);
        await database.Repository.CreateAsync(deletedReview);
        deletedReview.Delete();
        await database.Repository.DeleteAsync(deletedReview);

        var reviews = await database.Repository.ListAsync(new MovieReviewQueryCriteria());

        Assert.Contains(reviews, review => review.Id == activeReview.Id);
        Assert.DoesNotContain(reviews, review => review.Id == deletedReview.Id);
    }

    [Fact]
    public async Task ListAsync_WhenIncludeDeletedIsTrue_ShouldIncludeDeletedReviews()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);
        movieReview.Delete();
        await database.Repository.DeleteAsync(movieReview);

        var reviews = await database.Repository.ListAsync(new MovieReviewQueryCriteria(IncludeDeleted: true));

        var deletedReview = Assert.Single(reviews, review => review.Id == movieReview.Id);
        Assert.True(deletedReview.IsDeleted);
    }

    [Fact]
    public async Task ListAsync_WithEmptyCriteria_ShouldReturnAllNonDeletedReviews()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var firstReview = TestDatabase.CreateValidMovieReview(movieTitle: "First Movie");
        var secondReview = TestDatabase.CreateValidMovieReview(movieTitle: "Second Movie");
        await database.Repository.CreateAsync(firstReview);
        await database.Repository.CreateAsync(secondReview);

        var reviews = await database.Repository.ListAsync(new MovieReviewQueryCriteria());

        Assert.Equal(2, reviews.Count);
        Assert.Contains(reviews, review => review.Id == firstReview.Id);
        Assert.Contains(reviews, review => review.Id == secondReview.Id);
    }

    [Fact]
    public async Task ListAsync_WithUserId_ShouldReturnOnlyReviewsForThatUser()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var secondUserId = Guid.NewGuid();
        database.InsertUser(secondUserId);
        await database.Repository.CreateAsync(TestDatabase.CreateValidMovieReview(userId: TestDatabase.DemoUserId));
        await database.Repository.CreateAsync(TestDatabase.CreateValidMovieReview(userId: secondUserId));

        var reviews = await database.Repository.ListAsync(
            new MovieReviewQueryCriteria(UserId: TestDatabase.DemoUserId));

        Assert.NotEmpty(reviews);
        Assert.All(reviews, review => Assert.Equal(TestDatabase.DemoUserId, review.UserId));
    }

    [Fact]
    public async Task ListAsync_WithStatus_ShouldReturnOnlyReviewsWithThatStatus()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var draftReview = TestDatabase.CreateValidMovieReview(movieTitle: "Draft Movie");
        var publishedReview = TestDatabase.CreateValidMovieReview(movieTitle: "Published Movie");
        publishedReview.PublishReview();
        await database.Repository.CreateAsync(draftReview);
        await database.Repository.CreateAsync(publishedReview);

        var reviews = await database.Repository.ListAsync(
            new MovieReviewQueryCriteria(Status: ReviewStatus.Published));

        Assert.NotEmpty(reviews);
        Assert.All(reviews, review => Assert.Equal(ReviewStatus.Published, review.Status));
    }

    [Fact]
    public async Task ListAsync_WithUserIdAndStatus_ShouldReturnOnlyReviewsForThatUserAndStatus()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var secondUserId = Guid.NewGuid();
        database.InsertUser(secondUserId);
        var matchingReview = TestDatabase.CreateValidMovieReview(
            userId: TestDatabase.DemoUserId,
            movieTitle: "Matching Movie");
        var sameUserDraftReview = TestDatabase.CreateValidMovieReview(
            userId: TestDatabase.DemoUserId,
            movieTitle: "Same User Draft Movie");
        var otherUserPublishedReview = TestDatabase.CreateValidMovieReview(
            userId: secondUserId,
            movieTitle: "Other User Published Movie");
        matchingReview.PublishReview();
        otherUserPublishedReview.PublishReview();
        await database.Repository.CreateAsync(matchingReview);
        await database.Repository.CreateAsync(sameUserDraftReview);
        await database.Repository.CreateAsync(otherUserPublishedReview);

        var reviews = await database.Repository.ListAsync(
            new MovieReviewQueryCriteria(TestDatabase.DemoUserId, ReviewStatus.Published));

        var review = Assert.Single(reviews);
        Assert.Equal(matchingReview.Id, review.Id);
        Assert.Equal(TestDatabase.DemoUserId, review.UserId);
        Assert.Equal(ReviewStatus.Published, review.Status);
    }
}
