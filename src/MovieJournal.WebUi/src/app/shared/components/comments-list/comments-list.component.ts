import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, input, output, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReviewCommentResponse } from '../../../domain/review-comments/review-comment.responses';

export interface ReviewCommentEditSubmitted {
  commentId: string;
  content: string;
}

@Component({
  selector: 'app-comments-list',
  imports: [DatePipe, ReactiveFormsModule],
  templateUrl: './comments-list.component.html',
  styleUrl: './comments-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentsListComponent {
  readonly comments = input.required<ReviewCommentResponse[]>();
  readonly busyCommentId = input<string | null>(null);

  readonly commentUpdated = output<ReviewCommentEditSubmitted>();
  readonly commentDeleted = output<string>();

  protected readonly editingCommentId = signal<string | null>(null);
  protected readonly editContent = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(500)],
  });

  protected canManage(comment: ReviewCommentResponse): boolean {
    return comment.isOwner;
  }

  protected isBusy(comment: ReviewCommentResponse): boolean {
    return this.busyCommentId() === comment.id;
  }

  protected startEditing(comment: ReviewCommentResponse): void {
    this.editingCommentId.set(comment.id);
    this.editContent.setValue(comment.content);
    this.editContent.markAsPristine();
    this.editContent.markAsUntouched();
  }

  protected cancelEditing(): void {
    this.editingCommentId.set(null);
    this.editContent.reset('');
  }

  protected submitEdit(event: SubmitEvent, comment: ReviewCommentResponse): void {
    event.preventDefault();
    event.stopPropagation();

    const content = this.editContent.value.trim();
    this.editContent.markAsTouched();

    if (!content) {
      this.editContent.setErrors({ required: true });
      return;
    }

    if (this.editContent.invalid || this.isBusy(comment)) {
      return;
    }

    this.commentUpdated.emit({ commentId: comment.id, content });
    this.editingCommentId.set(null);
  }

  protected deleteComment(comment: ReviewCommentResponse): void {
    if (this.isBusy(comment)) {
      return;
    }

    this.commentDeleted.emit(comment.id);
  }

  protected showEditError(): boolean {
    return this.editContent.invalid && (this.editContent.touched || this.editContent.dirty);
  }

  protected userInitial(ownerName: string): string {
    return ownerName.trim().slice(0, 1).toUpperCase() || 'U';
  }
}
