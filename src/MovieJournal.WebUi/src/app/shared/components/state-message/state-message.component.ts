import { NgClass } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export type StateMessageKind = 'loading' | 'empty' | 'error';

@Component({
  selector: 'app-state-message',
  imports: [NgClass],
  templateUrl: './state-message.component.html',
  styleUrl: './state-message.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StateMessageComponent {
  readonly kind = input.required<StateMessageKind>();
  readonly title = input.required<string>();
  readonly message = input<string>('');
}
