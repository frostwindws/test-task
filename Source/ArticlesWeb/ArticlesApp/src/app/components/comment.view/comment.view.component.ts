import { Component, Input } from "@angular/core";
import { MatDialog, MatSnackBar } from '@angular/material';

import { ArticleComment } from "../../models/article.comment";
import { Result } from "../../models/result";

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
  constructor(public snackBar: MatSnackBar, public dialog: MatDialog, private commentService: CommentService) { }

  /**
   * Запуск редактирования комментария.
   */
  onEdit() {
    let comment: ArticleComment = Object.assign({}, this.comment);
    this.dialog.open(CommentEditor, {
      width: '600px',
      data: comment
    }).afterClosed().subscribe(result => {
      if (result) {
        this.commentService.updateComment(result)
          .subscribe((result: Result) => {
            let message = result.success ? 'Edit comment request was sent succesfully' : result.message;
            this.snackBar.open(message, null, { duration: 1000 });
          });
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
        this.commentService.deleteComment(this.comment)
          .subscribe((result: Result) => {
            let message = result.success ? 'Delete comment request was sent succesfully' : result.message;
            this.snackBar.open(message, null, { duration: 1000 });
          });
      }
    });
  }
}

