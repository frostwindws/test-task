import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, FormGroup } from '@angular/forms';

import { ArticleComment } from "../../models/article.comment";

/**
 * Компонент отображения статьи.
 */
@Component({
  selector: "comment-editor",
  templateUrl: "./comment.editor.component.html",
  styleUrls: ["./comment.editor.component.less"]
})
export class CommentEditor {

  /**
   * Данные формы.
   */
  formData: FormGroup;

  /**
   * Заголовок окна редактирования.
   */
  title: string;

  /**
   * Конструктор компонента.
   * @param dialogRef Ссылка на диалог-контейнер компонента.
   * @param comment Редактируемый комментарий.
   */
  constructor(public dialogRef: MatDialogRef<CommentEditor>, @Inject(MAT_DIALOG_DATA) public comment: ArticleComment) {
    let isNew = comment.id === 0;
    this.title = !isNew ? 'Edit comment' : 'Add new comment';

    this.formData = new FormGroup({
      author: new FormControl({ value: comment.author, disabled: !isNew }),
      content: new FormControl(comment.content),
    });
  }

  /**
   * Обработчик кнопки сохранения.
   */
  onSave(): void {
    if (this.formData.valid) {
      this.dialogRef.close(Object.assign(this.comment, this.formData.value));
    }
  }
}

