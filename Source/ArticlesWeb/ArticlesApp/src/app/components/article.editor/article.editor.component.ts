import { Component, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormControl, FormGroup } from '@angular/forms';

import { Article } from "../../models/article";

/**
 * Компонент отображения статьи.
 */
@Component({
  selector: "article-editor",
  templateUrl: "./article.editor.component.html",
  styleUrls: ["./article.editor.component.less"]
})
export class ArticleEditor {

  /**
   * Данные формы.
   */
  formData: FormGroup;

  /**
   * Идентификатор редактируемой статьи.
   */
  articleId: number;

  /**
   * Флаг редактирования новой статьи.
   */
  isNew: boolean;

  /**
   * Заголовок окна редактирования.
   */
  title: string;

  /**
   * Конструктор компонента.
   * @param articleService Сервис получения/обновления данных о статьях.
   * @param viewData Сервис разделяемых данных для отображения.
   */
  constructor(public dialogRef: MatDialogRef<ArticleEditor>, @Inject(MAT_DIALOG_DATA) public article: Article) {
    this.isNew = article.id == 0;

    if (!this.isNew) {
      this.articleId = article.id;
      this.title = 'Edit article ' + article.title;
    } else {
      this.title = 'Add new article';
    }

    this.formData = new FormGroup({
      title: new FormControl(article.title),
      author: new FormControl(article.author),
      content: new FormControl(article.content),
    });
  }

  /**
   * Обработчик кнопки сохранения.
   */
  onSave(): void {
    if (this.formData.valid) {
      this.dialogRef.close(Object.assign({ id: this.articleId || 0 }, this.formData.value));
    }
  }
}

