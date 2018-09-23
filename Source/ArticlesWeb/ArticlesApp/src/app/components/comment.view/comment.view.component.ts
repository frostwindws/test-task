import { Component, Input } from "@angular/core";
import { MatDialog } from '@angular/material';

import { ArticleComment } from "../../models/article.comment";

import { CommentService } from "../../services/comment.service";

import { CommentEditor } from '../comment.editor/comment.editor.component';
import { Prompt } from '../prompt/prompt.component';

/**
 * Компонент отображения комментария.
 */
@Component({
  selector: "comment-view",
  templateUrl: "./comment.view.component.html",
  styleUrls: ["./comment.view.component.less"]
})
export class CommentView {
  /**
   * Отображаемый комментарий
   */
  @Input() comment: ArticleComment;

  /**
   * Конструктор компонента.
   */
  constructor(public dialog: MatDialog, private commentService: CommentService) { }

  /**
   * Запуск редактирования комментария.
   */
  onEdit() {
    this.dialog.open(CommentEditor, {
      width: '600px',
      data: this.comment
    }).afterClosed().subscribe(result => {
      if (result) {
        this.commentService.updateComment(result);
      }
    });
  }

  /**
   * Запрос удаления комментария.
   */
  onDelete() {
    this.dialog.open(Prompt, {
      width: '600px',
      data: `Do you really want to delete comment by "${this.comment.author}"?`
    }).afterClosed().subscribe(result => {
      if (result) {
        this.commentService.deleteComment(this.comment);
      }
    });
  }
}

