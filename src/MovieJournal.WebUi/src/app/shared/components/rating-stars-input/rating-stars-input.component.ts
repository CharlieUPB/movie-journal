import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({
  selector: 'app-rating-stars-input',
  templateUrl: './rating-stars-input.component.html',
  styleUrl: './rating-stars-input.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RatingStarsInputComponent {
  readonly rating = input<number | null>(null);
  readonly disabled = input(false);
  readonly ratingChange = output<number>();

  protected readonly stars = [1, 2, 3, 4, 5];

  protected selectRating(rating: number): void {
    if (this.disabled()) {
      return;
    }

    this.ratingChange.emit(rating);
  }

  protected isFilled(star: number): boolean {
    return star <= (this.rating() ?? 0);
  }
}
