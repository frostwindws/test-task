import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'prompt',
  templateUrl: './prompt.component.html',
})
export class Prompt {

  /**
   * Конструктор компонента
   * @param promptText текст вопроса для подтверждения.
   */
  constructor(@Inject(MAT_DIALOG_DATA) public promptText: string) {}
}
