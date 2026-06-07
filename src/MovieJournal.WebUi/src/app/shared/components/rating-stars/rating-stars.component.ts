import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-rating-stars',
  imports: [NgClass],
  templateUrl: './rating-stars.component.html',
  styleUrl: './rating-stars.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RatingStarsComponent {
  readonly rating = input.required<number>();
  protected readonly stars = [1, 2, 3, 4, 5];

  protected isFilled(star: number): boolean {
    return star <= Math.round(this.rating());
  }
}
