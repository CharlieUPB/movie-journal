namespace MovieJournal.Web.Controllers.MovieReviews;

public record CreateMovieReviewHttpRequest(
string MovieTitle,
string ReviewTitle,
string ReviewContent,
int ReviewRating,
int? MovieReleaseYear
);
