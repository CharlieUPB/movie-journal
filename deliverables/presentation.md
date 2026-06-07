# Use Case Definition - Movie Journal

## 1. Basic Description

As a user who loves watching movies, I want to create and manage reviews for movies I have watched, so I can remember my opinions, share recommendations, and discuss movies with other users.

The application is not a movie database. It stores user-created movie reviews with minimal movie reference information.

## 2. Main Entities

- User
- MovieReview
- ReviewComment

## 3. Movie Review Lifecycle

A movie review has one status:

- Draft
- Published
- Archived

### Draft

A draft review is private and only visible to the owner.

### Published

A published review is visible to everyone and can receive comments from logged-in users.

### Archived

An archived review is hidden from public lists, visible only to the owner, read-only, and cannot receive new comments.

Deletion is an action, not a review status.

## 4. Actors

### Anonymous User

A user who is not logged into the application.

### Registered User

A user who has created an account and is logged into the application.

### Movie Review Owner

A user who created a specific movie review. The owner can manage their own review while it is not archived.

## 5. Use Cases

## Use Case 1: Register User

### Actor

Anonymous User

### Expected Flow

1. The user opens the registration page.
2. The user enters display name, email, and password.
3. The system validates the information.
4. The account is created.
5. The user can log in.

### Business Rules

- Display name is required.
- Email is required and must be unique.
- Password is required.

## Use Case 2: Login User

### Actor

Registered User

### Expected Flow

1. The user opens the login page.
2. The user enters email and password.
3. The system validates the credentials.
4. If valid, the user is logged in.
5. The user is redirected to their reviews or the published reviews page.

### Business Rules

- Email is required.
- Password is required.
- Invalid credentials should return an error.

## Use Case 3: Browse Published Reviews

### Actors

- Anonymous User
- Registered User

### Expected Flow

1. The actor opens the public reviews page.
2. The system displays published reviews.
3. The actor can open a review details page.

### Business Rules

- Only published reviews are visible publicly.
- Draft reviews are not visible publicly.
- Archived reviews are not visible publicly.

## Use Case 4: View Review Details

### Actors

- Anonymous User
- Registered User
- Movie Review Owner

### Expected Flow

1. The actor selects a review.
2. The system opens the review details page.
3. The system displays the full review and existing comments.
4. If the actor is logged in and the review is published, the system shows the comment form.
5. If the actor is the owner, the system shows owner actions.

### Business Rules

- Published reviews can be viewed by anyone.
- Draft reviews can only be viewed by the owner.
- Archived reviews can only be viewed by the owner.

## Use Case 5: Create Movie Review

### Actor

Registered User

### Expected Flow

1. The user opens the create review page.
2. The user enters movie reference information.
3. The user enters review information.
4. The system validates the information.
5. The review is created as Draft by default.
6. The user can later publish the review.

### Business Rules

- Only logged-in users can create reviews.
- Movie title is required.
- Release year is optional, but cannot be in the future.
- Review title is required.
- Review content is required.
- Rating is required and must be between 1 and 5.
- A user cannot create two non-archived reviews for the same movie title and release year.
- A new review is Draft by default.

## Use Case 6: View My Reviews

### Actor

Registered User

### Expected Flow

1. The user opens the My Reviews page.
2. The system displays the user's reviews.
3. The user can filter reviews by status.
4. The user can open a review details page.
5. The user can edit, publish, archive, or delete allowed reviews.

### Business Rules

- Only logged-in users can access My Reviews.
- Users can only see their own draft reviews.
- Users can see their own published and archived reviews.

## Use Case 7: Update Movie Review

### Actor

Movie Review Owner

### Expected Flow

1. The owner opens one of their reviews.
2. The owner selects edit.
3. The owner changes review information.
4. The system validates the changes.
5. The system saves the updated review.

### Business Rules

- Only the owner can update a review.
- Draft and published reviews can be edited by the owner.
- Archived reviews cannot be edited.
- Movie title is required.
- Release year cannot be in the future.
- Review title is required.
- Review content is required.
- Rating must be between 1 and 5.

## Use Case 8: Publish Movie Review

### Actor

Movie Review Owner

### Expected Flow

1. The owner opens a draft review.
2. The owner selects publish.
3. The system validates that the review is complete.
4. The review becomes published.
5. The review appears in the public reviews page.

### Business Rules

- Only the owner can publish a review.
- Only draft reviews can be published.
- Archived reviews cannot be published.
- Movie title is required.
- Review title is required.
- Review content is required.
- Review content must have a minimum length, for example 50 characters.
- Rating is required.

## Use Case 9: Archive Movie Review

### Actor

Movie Review Owner

### Expected Flow

1. The owner opens one of their reviews.
2. The owner selects archive.
3. The system asks for confirmation.
4. The review becomes archived.
5. The review disappears from public lists.
6. The review remains visible to the owner as archived.

### Business Rules

- Only the owner can archive a review.
- Draft and published reviews can be archived.
- Archived reviews are read-only.
- Archived reviews are not visible publicly.
- Archived reviews cannot receive new comments.

## Use Case 10: Delete Movie Review

### Actor

Movie Review Owner

### Expected Flow

1. The owner opens one of their reviews.
2. The owner selects delete.
3. The system asks for confirmation.
4. The system validates whether the review can be deleted.
5. If deletion is allowed, the review is removed from normal views.
6. If deletion is not allowed, the system suggests archiving instead.

### Business Rules

- Only the owner can delete a review.
- A draft review can be deleted.
- A published review without comments can be deleted.
- A published review with comments should be archived instead of deleted.
- Archived reviews should not be deleted from normal screens.

## Use Case 11: Add Comment to Review

### Actor

Registered User

### Expected Flow

1. The user opens a published review.
2. The user writes a comment.
3. The system validates the comment.
4. The comment is added to the review.
5. The comment appears in the comments section.

### Business Rules

- Only logged-in users can comment.
- Comments are allowed only on published reviews.
- Comments are not allowed on draft reviews.
- Comments are not allowed on archived reviews.
- Comment content is required.
- Comment content must not exceed a reasonable length, for example 500 characters.

# Development Process and guidelines of my approach to Clean Architecture:

The domain layer, contains the entitites, value objects, value objects avoid primtiive obsession by representing a domain valid unit of work. Inside the domain layer we have business rules validations, belonging to the specific use case of the movie journal. 

In the application layer we have specific use cases for our domain, like CreateMovieReview, the use cases are represented by Commands and Queries (Based in CQRS) the application layer uses a repository interface but is not aware of the exact implementation, the application uses the domain exposed methods (business logic) and interacts
 with the repository, it is also expert on knowing what request receiv and what response a domain use case needs as input and what ouputs produces. The applicaiton layer makes use of Extension methods to provide The respective commands and queries.
The application layer uses domain objects business logics. and expose them as self contained commands and queries. 

In the infrastruture layer we have the implementation of the repository interface required by the application layer, the repository for now it is built by using SQL Lite and ADO.NET, the repository could be any kind of persisintce, file system, in memory, Entity framework, etc. The infra layer  also takes care of the DB initialzation for data seed, as well as providing in the DI container the implementation for the repository interfaces, all of this is done via Extension Methods of the ServiceCollection. The infra layer provides repository implemtantions, database connections and a databaseinitialization logic.

The Web layer (AKA Presentation Layer) is the outermost layer and is the Container root that glues things together, this layer uses services extensions to register ApplicationServices, InfrastructureServices, Initializes the DB, is aware of external configurations, and the shape of this presentation layer is a REST API that will expose Application Use Cases (Command, Queries) through rest endpoints.  IT also handles exception handler since the domain layer can throw business domain exceptions. 


