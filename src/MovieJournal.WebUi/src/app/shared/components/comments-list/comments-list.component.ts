import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ReviewCommentResponse } from '../../../domain/review-comments/review-comment.responses';

@Component({
  selector: 'app-comments-list',
  imports: [DatePipe],
  templateUrl: './comments-list.component.html',
  styleUrl: './comments-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsListComponent {
  readonly comments = input.required<ReviewCommentResponse[]>();

  protected userInitial(): string {
    return 'U';
  }

  protected userLabel(userId: string): string {
    return `User ${userId.slice(0, 8)}`;
  }
}
