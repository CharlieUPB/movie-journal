import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule],
  templateUrl: './comment-form.component.html',
  styleUrl: './comment-form.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentFormComponent {
  readonly isSubmitting = input(false);
  readonly commentSubmitted = output<string>();

  readonly form = new FormGroup({
    content: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(500)],
    }),
  });

  protected get content(): FormControl<string> {
    return this.form.controls.content;
  }

  protected submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid || this.isSubmitting()) {
      return;
    }

    this.commentSubmitted.emit(this.content.value.trim());
    this.form.reset();
  }

  protected showError(): boolean {
    return this.content.invalid && (this.content.touched || this.content.dirty);
  }
}
