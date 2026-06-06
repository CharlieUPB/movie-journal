# Movie Review Journal - Use Case README

## 1. Product Summary

**Movie Review Journal** is a small web application where users can write personal reviews for movies they have watched, publish selected reviews, and comment on reviews published by other users.

The app is **not** a movie database. It does not manage full movie information such as cast, trailers, posters, runtime, production companies, or recommendations. The system focuses on **user-generated movie reviews**.

A movie is represented only through minimal reference information inside a review, such as the movie title and an optional external movie identifier. This keeps the domain focused and leaves room for a future integration with IMDb, TMDb, or another movie data provider.

## 2. Main Product Goal

Allow users to keep a simple personal journal of movie reviews and share selected reviews with others.

## 3. Main User Story

As a movie lover, I want to create and manage reviews for movies I have watched, so I can remember my opinions, share recommendations, and discuss movies with other users.

## 4. Main Actors

### Anonymous Visitor

A person who has not logged in.

They can:

- Browse published movie reviews.
- View the details of a published review.
- See comments on published reviews.
- Register or log in.

They cannot:

- Create reviews.
- Edit reviews.
- Publish reviews.
- Archive reviews.
- Delete reviews.
- Add comments.

### Registered User

A user who has created an account and logged in.

They can:

- Create movie reviews.
- View their own private and published reviews.
- Edit their own active reviews.
- Publish their own reviews.
- Archive their own reviews.
- Delete their own reviews when allowed.
- Comment on published reviews.

They cannot:

- Edit another user's review.
- Delete another user's review.
- Archive another user's review.
- Comment on private, archived, or deleted reviews.

### Review Owner

The user who created a specific movie review.

They have full control over their own review while the review is active.

## 5. Entities

The application has three main entities:

1. User
2. MovieReview
3. ReviewComment

## 6. Entity: User

Represents an account in the system.

### Suggested Fields for Mockups

- Display name
- Email
- Password

### Expected Behavior

- A user can register with a display name, email, and password.
- A user can log in with email and password.
- A user owns the movie reviews they create.
- A user can add comments to published reviews.

### Business Rules

- Email is required.
- Email must be unique.
- Display name is required.
- Password is required.
- Password must meet minimum security requirements.

## 7. Entity: MovieReview

Represents a user's review of a movie they have watched.

This is the main entity of the application.

The system does not store a full Movie entity. Instead, the movie is referenced inside the review using minimal movie information.

### Suggested Fields for Mockups

- Movie title
- Release year, optional
- External movie ID, optional
- External source, optional. Example: IMDb, TMDb, Manual
- Review title
- Review content
- Rating, from 1 to 5
- Watched date
- Visibility: Private or Published
- Status: Active, Archived, or Deleted
- Created by
- Created date
- Updated date

### Field Notes

#### Movie title

The display name of the movie being reviewed.

Example: `Inception`

#### Release year

Optional field used only to help users distinguish remakes or movies with the same title.

Example: `2010`

#### External movie ID

Optional field that can be used later to connect the review with an external movie provider.

Example: `tt1375666`

#### External source

Optional field that identifies where the external movie ID comes from.

Example values:

- IMDb
- TMDb
- Manual

#### Review title

A short headline for the review.

Example: `A clever sci-fi thriller that still feels fresh`

#### Review content

The user's opinion about the movie.

#### Rating

A numeric score from 1 to 5.

#### Watched date

The date when the user watched the movie.

#### Visibility

Controls who can see the review.

Allowed values:

- Private
- Published

#### Status

Controls the lifecycle of the review.

Allowed values:

- Active
- Archived
- Deleted

## 8. Entity: ReviewComment

Represents a comment added by a registered user to a published movie review.

### Suggested Fields for Mockups

- Comment content
- Author
- Created date

### Expected Behavior

- A logged-in user can comment on a published review.
- Comments appear under the review details.
- Anonymous visitors can read comments, but cannot add comments.

### Business Rules

- Comment content is required.
- Comment content must not exceed a reasonable length, for example 500 characters.
- Comments can only be added to published reviews.
- Comments cannot be added to private reviews.
- Comments cannot be added to archived reviews.
- Comments cannot be added to deleted reviews.

## 9. Review Visibility

Movie reviews have two visibility states:

### Private

A private review is visible only to the owner.

Private reviews are useful when the user wants to save a review without sharing it publicly yet.

### Published

A published review is visible to everyone.

Published reviews can receive comments from logged-in users.

## 10. Review Status

Movie reviews have three lifecycle statuses:

### Active

The review is available to the owner and can be edited.

If it is published, it can be seen by other users.

### Archived

The review is no longer public and becomes read-only.

The owner can still see it in their archived reviews.

### Deleted

The review is removed from normal user views.

Deleted reviews should not appear in public lists or the owner's active list.

## 11. Main Use Cases

## Use Case 1: Register User

### Goal

Allow a visitor to create an account.

### Actor

Anonymous Visitor

### Expected Flow

1. The visitor opens the registration page.
2. The visitor enters display name, email, and password.
3. The system validates the information.
4. The account is created.
5. The user can log in.

### Business Rules

- Display name is required.
- Email is required.
- Email must be unique.
- Password is required.
- Password must meet minimum security requirements.

### Expected UI

- Registration form.
- Validation messages.
- Success message after registration.
- Link to login page.

## Use Case 2: Login User

### Goal

Allow a registered user to access protected features.

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
- Invalid credentials should show a clear error message.

### Expected UI

- Login form.
- Validation messages.
- Error message for invalid credentials.
- Link to registration page.

## Use Case 3: Browse Published Reviews

### Goal

Allow visitors and users to discover public movie reviews.

### Actors

- Anonymous Visitor
- Registered User

### Expected Flow

1. The actor opens the public reviews page.
2. The system displays published active reviews.
3. The actor can open a review details page.

### Business Rules

- Only published reviews are visible publicly.
- Private reviews are not visible publicly.
- Archived reviews are not visible publicly.
- Deleted reviews are not visible publicly.

### Expected UI

A public reviews page with review cards showing:

- Movie title
- Release year, if available
- Review title
- Rating
- Short preview of the review content
- Author display name
- Watched date
- Number of comments

## Use Case 4: View Review Details

### Goal

Allow users and visitors to read a published review and its comments.

### Actors

- Anonymous Visitor
- Registered User
- Review Owner

### Expected Flow

1. The actor selects a review from the public list.
2. The system opens the review details page.
3. The system displays the full review and existing comments.
4. If the actor is logged in, the system shows the comment form.
5. If the actor is the owner, the system shows owner actions.

### Business Rules

- Published reviews can be viewed by anyone.
- Private reviews can only be viewed by the owner.
- Archived reviews can only be viewed by the owner.
- Deleted reviews cannot be viewed from normal screens.

### Expected UI

Review details page showing:

- Movie title
- Release year, if available
- External source badge, if available
- Review title
- Rating
- Watched date
- Full review content
- Author display name
- Comments section
- Add comment form for logged-in users
- Edit, archive, delete actions for the owner

## Use Case 5: Create Movie Review

### Goal

Allow a logged-in user to create a movie review.

### Actor

Registered User

### Expected Flow

1. The user opens the create review page.
2. The user enters movie reference information.
3. The user enters review information.
4. The system validates the information.
5. The review is created as private by default.
6. The user can later publish the review.

### Business Rules

- Only logged-in users can create reviews.
- Movie title is required.
- Release year is optional, but if provided, it cannot be in the future.
- External movie ID is optional.
- If external movie ID is provided, external source must also be provided.
- Review title is required.
- Review content is required.
- Rating is required.
- Rating must be between 1 and 5.
- Watched date is required.
- Watched date cannot be in the future.
- A user cannot create two active reviews for the same movie title and release year.
- A new review is private by default.
- A new review is active by default.

### Expected UI

Create review form with:

- Movie title input
- Release year input, optional
- External movie ID input, optional
- External source select, optional
- Review title input
- Review content textarea
- Rating selector
- Watched date picker
- Save as private button
- Save and publish button, optional

## Use Case 6: View My Reviews

### Goal

Allow a logged-in user to manage their own reviews.

### Actor

Registered User

### Expected Flow

1. The user opens the My Reviews page.
2. The system displays the user's reviews.
3. The user can filter reviews by visibility or status.
4. The user can open a review details page.
5. The user can edit, publish, archive, or delete allowed reviews.

### Business Rules

- Only logged-in users can access My Reviews.
- Users can only see their own private reviews.
- Users can see their own active and archived reviews.
- Deleted reviews should not appear in the normal My Reviews list.

### Expected UI

My Reviews page with:

- Review cards or table rows
- Visibility filter: All, Private, Published
- Status filter: Active, Archived
- Search by movie title
- Create review button
- Edit button for active reviews
- Publish button for private active reviews
- Archive button for active reviews
- Delete button when allowed

## Use Case 7: Update Movie Review

### Goal

Allow the owner to update an active movie review.

### Actor

Review Owner

### Expected Flow

1. The owner opens one of their active reviews.
2. The owner selects edit.
3. The owner changes review information.
4. The system validates the changes.
5. The system saves the updated review.

### Business Rules

- Only the owner can update a review.
- Archived reviews cannot be edited.
- Deleted reviews cannot be edited.
- Movie title is required.
- Release year cannot be in the future.
- If external movie ID is provided, external source must also be provided.
- Review title is required.
- Review content is required.
- Rating must be between 1 and 5.
- Watched date cannot be in the future.

### Expected UI

Edit review form with the same fields as the create review form.

The UI should clearly show whether the review is private or published.

## Use Case 8: Publish Movie Review

### Goal

Allow the owner to make a private review publicly visible.

### Actor

Review Owner

### Expected Flow

1. The owner opens a private active review.
2. The owner selects publish.
3. The system validates that the review is complete.
4. The review becomes published.
5. The review appears in the public reviews page.

### Business Rules

- Only the owner can publish a review.
- Only active reviews can be published.
- Archived reviews cannot be published.
- Deleted reviews cannot be published.
- Movie title is required.
- Review title is required.
- Review content is required.
- Review content must have a minimum length, for example 50 characters.
- Rating is required.
- Watched date is required.
- Watched date cannot be in the future.

### Expected UI

- Publish button on private active reviews.
- Confirmation dialog before publishing.
- Validation messages if the review is incomplete.
- Published badge after successful publishing.

## Use Case 9: Archive Movie Review

### Goal

Allow the owner to remove a review from public visibility without permanently deleting it.

### Actor

Review Owner

### Expected Flow

1. The owner opens an active review.
2. The owner selects archive.
3. The system asks for confirmation.
4. The review becomes archived.
5. The review disappears from public lists.
6. The review remains visible to the owner in the archived section.

### Business Rules

- Only the owner can archive a review.
- Archived reviews are read-only.
- Archived reviews are not visible publicly.
- Archived reviews cannot receive new comments.

### Expected UI

- Archive button for active reviews.
- Confirmation dialog.
- Archived badge.
- Archived section or status filter in My Reviews.

## Use Case 10: Delete Movie Review

### Goal

Allow the owner to remove a review when deletion is allowed.

### Actor

Review Owner

### Expected Flow

1. The owner opens one of their reviews.
2. The owner selects delete.
3. The system asks for confirmation.
4. The system validates whether the review can be deleted.
5. If deletion is allowed, the review is removed from normal views.
6. If deletion is not allowed, the system suggests archiving instead.

### Business Rules

- Only the owner can delete a review.
- Deleted reviews do not appear in public lists.
- Deleted reviews do not appear in normal owner lists.
- A private review with no comments can be deleted.
- A published review with comments should be archived instead of deleted.

### Expected UI

- Delete button for allowed reviews.
- Confirmation dialog.
- Friendly message when a review cannot be deleted and should be archived instead.

## Use Case 11: Add Comment to Review

### Goal

Allow logged-in users to discuss published reviews.

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
- Comments are allowed only on published active reviews.
- Comments are not allowed on private reviews.
- Comments are not allowed on archived reviews.
- Comments are not allowed on deleted reviews.
- Comment content is required.
- Comment content must not exceed a reasonable length, for example 500 characters.

### Expected UI

- Comments section under review details.
- Comment textarea for logged-in users.
- Login prompt for anonymous visitors.
- Submit comment button.
- Validation message for empty or too long comments.

## 12. Main Screens for Mockups

## Screen 1: Landing Page / Published Reviews

Purpose:

Show public movie reviews and invite users to explore or sign in.

Main elements:

- App name and navigation
- Public review cards
- Search by movie title
- Filter by rating, optional
- Login button
- Register button

Review card should show:

- Movie title
- Release year, if available
- Review title
- Rating
- Short review preview
- Author
- Watched date
- Comment count

## Screen 2: Review Details Page

Purpose:

Show the full content of a review and its comments.

Main elements:

- Movie title
- Release year, if available
- External source badge, if available
- Review title
- Rating
- Watched date
- Full review content
- Author
- Comments section
- Add comment area for logged-in users
- Login prompt for anonymous visitors
- Owner actions when applicable

## Screen 3: Login Page

Purpose:

Allow users to access their account.

Main elements:

- Email input
- Password input
- Login button
- Error message area
- Link to register

## Screen 4: Register Page

Purpose:

Allow new users to create an account.

Main elements:

- Display name input
- Email input
- Password input
- Confirm password input, optional
- Register button
- Validation messages
- Link to login

## Screen 5: My Reviews Page

Purpose:

Allow a logged-in user to manage their own reviews.

Main elements:

- Create review button
- Review list
- Visibility badges: Private, Published
- Status badges: Active, Archived
- Filters by visibility and status
- Search by movie title
- Edit action
- Publish action
- Archive action
- Delete action

## Screen 6: Create Review Page

Purpose:

Allow a logged-in user to create a new movie review.

Main elements:

- Movie title input
- Release year input, optional
- External movie ID input, optional
- External source select, optional
- Review title input
- Review content textarea
- Rating selector
- Watched date picker
- Save as private button
- Save and publish button, optional

## Screen 7: Edit Review Page

Purpose:

Allow the owner to update an active review.

Main elements:

- Same fields as Create Review
- Current visibility badge
- Current status badge
- Save changes button
- Publish button if private
- Archive button
- Delete button if allowed

## Screen 8: Archived Reviews Section

Purpose:

Allow the owner to see read-only archived reviews.

Main elements:

- List of archived reviews
- Archived badge
- Read-only review details
- No edit button
- No comment form

## 13. Navigation Expectations

The application should have simple navigation:

For anonymous visitors:

- Published Reviews
- Login
- Register

For logged-in users:

- Published Reviews
- My Reviews
- Create Review
- Logout

## 14. Important UX Rules

- The app should clearly differentiate private and published reviews.
- The app should clearly show archived reviews as read-only.
- The app should not show edit actions to users who do not own the review.
- The app should not show comment forms to anonymous visitors.
- The app should show clear validation messages near the fields.
- The app should show friendly empty states.

## 15. Empty States

### No published reviews

Message:

`No published reviews yet. Be the first to share one.`

### No personal reviews

Message:

`You have not created any movie reviews yet.`

Action:

`Create your first review`

### No comments

Message:

`No comments yet. Start the conversation.`

### No archived reviews

Message:

`You do not have archived reviews.`

## 16. Validation Messages

Examples:

- `Movie title is required.`
- `Release year cannot be in the future.`
- `Review title is required.`
- `Review content is required.`
- `Published reviews must have at least 50 characters.`
- `Rating must be between 1 and 5.`
- `Watched date cannot be in the future.`
- `External source is required when external movie ID is provided.`
- `Comment cannot be empty.`
- `Comment cannot exceed 500 characters.`
- `You can only edit your own reviews.`
- `Archived reviews cannot be edited.`
- `This review cannot be deleted because it already has comments. You can archive it instead.`

## 17. Suggested Demo Data

### Users

User 1:

- Display name: Carlos
- Email: demo@moviejournal.com
- Password: Demo123!

User 2:

- Display name: Maria
- Email: maria@moviejournal.com
- Password: Demo123!

### Movie Reviews

Review 1:

- Movie title: Inception
- Release year: 2010
- External source: IMDb
- External movie ID: tt1375666
- Review title: A clever sci-fi thriller that still feels fresh
- Rating: 5
- Visibility: Published
- Status: Active
- Author: Carlos

Review 2:

- Movie title: About Time
- Release year: 2013
- External source: Manual
- Review title: A romantic story with more heart than expected
- Rating: 4
- Visibility: Private
- Status: Active
- Author: Carlos

Review 3:

- Movie title: The Menu
- Release year: 2022
- External source: Manual
- Review title: Dark, stylish, and uncomfortable in a good way
- Rating: 4
- Visibility: Published
- Status: Active
- Author: Maria

Review 4:

- Movie title: Interstellar
- Release year: 2014
- External source: IMDb
- External movie ID: tt0816692
- Review title: Big emotions, big ideas, and a beautiful soundtrack
- Rating: 5
- Visibility: Private
- Status: Archived
- Author: Maria

### Comments

Comment 1:

- Review: Inception
- Author: Maria
- Content: I agree, the concept still feels original even years later.

Comment 2:

- Review: The Menu
- Author: Carlos
- Content: This one surprised me. The tone was weird but memorable.

## 18. Non-Goals

The following features are intentionally out of scope:

- Full movie database management
- Cast and crew management
- Movie posters
- Trailer playback
- Movie recommendations
- External API integration
- Likes or reactions
- Followers
- User profiles beyond basic account data
- Admin dashboard
- Multiple watchlists
- Shared couple accounts
- Advanced statistics

These can be future improvements, but they should not be part of the initial technical test scope.

## 19. Summary for Mockup Generation

Create mockups for a responsive web app called **Movie Review Journal**.

The app allows users to:

- Browse published movie reviews.
- Register and log in.
- Create private movie reviews.
- Publish completed reviews.
- View their own reviews.
- Edit their own active reviews.
- Archive their own reviews.
- Delete reviews when allowed.
- Comment on published reviews.

The app has three entities:

- User
- MovieReview
- ReviewComment

The main focus is not movie data. The main focus is the user's review.

A MovieReview should contain only minimal movie reference information:

- Movie title
- Optional release year
- Optional external movie ID
- Optional external source

The UI should feel like a clean, modern personal journal combined with a simple public review board.
