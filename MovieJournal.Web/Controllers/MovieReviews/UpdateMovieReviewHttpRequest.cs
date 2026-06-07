namespace MovieJournal.Web.Controllers.MovieReviews;

public record UpdateMovieReviewHttpRequest(
string MovieTitle,
string ReviewTitle,
string ReviewContent,
int ReviewRating,
int? MovieReleaseYear);
