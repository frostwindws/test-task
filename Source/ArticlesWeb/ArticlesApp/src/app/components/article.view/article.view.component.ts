import { Component, OnInit } from "@angular/core";
import { MatDialog, MatSnackBar } from '@angular/material';

import { Article } from "../../models/article";
import { ArticleComment } from "../../models/article.comment";
import { Result } from "../../models/result";

import { ArticleService } from "../../services/article.service";
import { CommentService } from "../../services/comment.service";
import { ViewDataService } from "../../services/viewdata.service";
import { SignalRService } from "../../services/signalr.service";

import { ArticleEditor } from '../article.editor/article.editor.component';
import { CommentEditor } from '../comment.editor/comment.editor.component';
import { Prompt } from '../prompt/prompt.component';

/**
 * Компонент отображения статьи.
 */
@Component({
  selector: "article-view",
  templateUrl: "./article.view.component.html",
  styleUrls: ["./article.view.component.less"]
})
export class ArticleView implements OnInit {

  /**
   * Текущая статья
   */
  article: Article;

  /**
   * Фла процесса зарузки компонента
   */
  isLoading = false;

  /**
   * Конструктор компонента.
   * @param dialog Компонент диалога.
   * @param articleService Сервис получения/обновления данных о статьях.
   * @param commentService Сервис обновления данных о комментариях.
   * @param viewData Сервис разделяемых данных для отображения.
   */
  constructor(public snackBar: MatSnackBar, public dialog: MatDialog, private signalr: SignalRService, private articleService: ArticleService, private commentService: CommentService, private viewData: ViewDataService) { }

  /**
   * Инициализация компонента.
   */
  ngOnInit() {
    this.viewData.currentArticle.subscribe(article => this.article = article);

    // Подписка на обновления данных
    this.signalr.subscribe('article-update', this, this.onArticleUpdated);
    this.signalr.subscribe('article-delete', this, this.onArticleDeleted);

    this.signalr.subscribe('comment-create', this, this.onCommentCreated);
    this.signalr.subscribe('comment-update', this, this.onCommentUpdated);
    this.signalr.subscribe('comment-delete', this, this.onCommentDeleted);
  }

  /**
   * Обновление статьи.
   */
  onRefresh() {
    this.isLoading = true;
    this.articleService.getArticle(this.article.id)
      .subscribe(article => {
        this.viewData.setCurrentArticle(article);
        this.isLoading = false;
      });
  }

  /**
   * Запуск редактирования статьи.
   */
  onEdit() {
    let article: Article = Object.assign({}, this.article);
    article.comments = null;
    this.dialog.open(ArticleEditor, {
      width: '600px',
      data: article
    }).afterClosed().subscribe(result => {
      if (result) {
        this.articleService.updateArticle(result).subscribe((result) => {
          let message = result.success ? 'Edit article request was sent succesfully' : result.message;
          this.snackBar.open(message, null, { duration: 1000 });
        });
      }
    });
  }

  /**
   * Запрос удаления статьи.
   */
  onDelete() {
    this.dialog.open(Prompt, {
      width: '600px',
      data: `Do you really want to delete article "${this.article.title}"?`
    }).afterClosed().subscribe(result => {
      if (result) {
        this.articleService.deleteArticle(this.article)
          .subscribe((result: Result) => {
            let message = result.success ? 'Delete article request was sent succesfully' : result.message;
            this.snackBar.open(message, null, { duration: 1000 });
          });
      }
    });
  }

  /**
   * Запрос добавления комментария.
   */
  onAddComment() {
    this.dialog.open(CommentEditor, {
      width: '600px',
      data: { id: 0, articleId: this.article.id }
    }).afterClosed().subscribe(result => {
      if (result) {
        this.commentService.addComment(result)
          .subscribe((result: Result) => {
            let message = result.success ? 'Add comemnt request was sent succesfully' : result.message;
            this.snackBar.open(message, null, { duration: 1000 });
          });
      }
    });
  }

  /**
   * Обработчик оповещения об обновлении статьи.
   * @param article Обновленная статья.
   */
  onArticleUpdated(article: Article) {
    if (this.article && !this.isLoading && article.id === this.article.id) {
      this.onRefresh();
    }
  }

  /**
   * Обработчик оповещения об удалении статьи.
   * @param article Удаленная статья.
   */
  onArticleDeleted(article: Article) {
    if (this.article && article.id === this.article.id) {
      this.viewData.setCurrentArticle(null);
    }
  }

  /**
   * Обработчик оповещения о добавлении комментария.
   * @param comment Добавленный комментарий.
   */
  onCommentCreated(comment: ArticleComment) {
    if (this.article && !this.isLoading && comment.articleId === this.article.id) {
      this.article.comments.unshift(comment);
    }
  }

  /**
   * Обработчик оповещения об обновлении комментария.
   * @param comment Обновленный комментарий.
   */
  onCommentUpdated(comment: ArticleComment) {
    if (this.article && !this.isLoading && comment.articleId === this.article.id) {
      let index = this.getIndexById(this.article.comments, comment.id);
      if (index >= 0) {
        this.article.comments[index].content = comment.content;
      }
    }
  }

  /**
   * Обработчик оповещения об удалении комментария.
   * @param comment Удаленный комментарий.
   */
  onCommentDeleted(comment: ArticleComment) {
    if (this.article && !this.isLoading) {
      let index = this.getIndexById(this.article.comments, comment.id);
      if (index >= 0) {
        this.article.comments.splice(index, 1);
      }
    }
  }

  /**
   * Поиск коммеентария в массиве по его илентификатору.
   * @param id Идентификатор комментария.
   */
  private getIndexById(array: ArticleComment[], id: number): number {
    for (var i = 0; i < array.length; i++) {
      if (array[i].id === id) return i;
    }

    return -1;
  }
}

