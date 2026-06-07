import {
  ChangeDetectionStrategy,
  Component,
  effect,
  input,
  output,
} from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { CreateMovieReviewRequest } from '../../../domain/movie-reviews/movie-review.requests';
import { RatingStarsInputComponent } from '../rating-stars-input/rating-stars-input.component';

export interface ReviewFormValue extends CreateMovieReviewRequest {}

export type ReviewFormMode = 'create' | 'edit';

@Component({
  selector: 'app-review-form',
  imports: [ReactiveFormsModule, RatingStarsInputComponent],
  templateUrl: './review-form.component.html',
  styleUrl: './review-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReviewFormComponent {
  readonly mode = input<ReviewFormMode>('create');
  readonly initialValue = input<ReviewFormValue | null>(null);
  readonly readOnly = input(false);
  readonly isSubmitting = input(false);
  readonly submitLabel = input('Save changes');

  readonly saveDraft = output<ReviewFormValue>();
  readonly saveAndPublish = output<ReviewFormValue>();
  readonly submitReview = output<ReviewFormValue>();

  protected readonly currentYear = new Date().getFullYear();

  readonly form = new FormGroup({
    movieTitle: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    movieReleaseYear: new FormControl<number | null>(null, {
      validators: [releaseYearValidator],
    }),
    reviewTitle: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    reviewContent: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(50)],
    }),
    reviewRating: new FormControl<number | null>(null, {
      validators: [Validators.required, Validators.min(1), Validators.max(5)],
    }),
  });

  constructor() {
    effect(() => {
      const initialValue = this.initialValue();

      if (initialValue) {
        this.form.patchValue(initialValue, { emitEvent: false });
      }
    });

    effect(() => {
      if (this.readOnly() || this.isSubmitting()) {
        this.form.disable({ emitEvent: false });
      } else {
        this.form.enable({ emitEvent: false });
      }
    });
  }

  protected get movieTitle(): FormControl<string> {
    return this.form.controls.movieTitle;
  }

  protected get movieReleaseYear(): FormControl<number | null> {
    return this.form.controls.movieReleaseYear;
  }

  protected get reviewTitle(): FormControl<string> {
    return this.form.controls.reviewTitle;
  }

  protected get reviewContent(): FormControl<string> {
    return this.form.controls.reviewContent;
  }

  protected get reviewRating(): FormControl<number | null> {
    return this.form.controls.reviewRating;
  }

  protected setRating(rating: number): void {
    this.reviewRating.setValue(rating);
    this.reviewRating.markAsTouched();
  }

  protected submit(action: 'draft' | 'publish' | 'save'): void {
    this.form.markAllAsTouched();

    if (this.form.invalid || this.readOnly() || this.isSubmitting()) {
      return;
    }

    const value = this.toRequest();

    if (action === 'draft') {
      this.saveDraft.emit(value);
      return;
    }

    if (action === 'publish') {
      this.saveAndPublish.emit(value);
      return;
    }

    this.submitReview.emit(value);
  }

  protected showError(control: AbstractControl): boolean {
    return control.invalid && (control.touched || control.dirty);
  }

  protected releaseYearError(): string {
    if (this.movieReleaseYear.hasError('min')) {
      return 'Release year looks too old.';
    }

    if (this.movieReleaseYear.hasError('releaseYear')) {
      return 'Enter a valid release year.';
    }

    return 'Release year cannot be in the future.';
  }

  private toRequest(): ReviewFormValue {
    const releaseYear = this.movieReleaseYear.value;

    return {
      movieTitle: this.movieTitle.value.trim(),
      movieReleaseYear: typeof releaseYear === 'number' ? releaseYear : null,
      reviewTitle: this.reviewTitle.value.trim(),
      reviewContent: this.reviewContent.value.trim(),
      reviewRating: this.reviewRating.value ?? 0,
    };
  }
}

function releaseYearValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;

  if (value === null || value === '') {
    return null;
  }

  if (typeof value !== 'number' || Number.isNaN(value)) {
    return { releaseYear: true };
  }

  const currentYear = new Date().getFullYear();

  if (value < 1888) {
    return { min: true };
  }

  if (value > currentYear) {
    return { futureYear: true };
  }

  return null;
}
