import { Component, OnInit } from "@angular/core";
import { MatDialog } from '@angular/material';

import { Article } from "../../models/article";

import { ArticleService } from "../../services/article.service";
import { CommentService } from "../../services/comment.service";
import { ViewDataService } from "../../services/viewdata.service";

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
  constructor(public dialog: MatDialog, private articleService: ArticleService, private commentService: CommentService, private viewData: ViewDataService) { }

  /**
   * Инициализация компонента.
   */
  ngOnInit() {
    this.viewData.currentArticle.subscribe(article => this.article = article);
  }

  /**
   * Запуск редактирования статьи.
   */
  onEdit() {
    this.dialog.open(ArticleEditor, {
      width: '600px',
      data: this.article
    }).afterClosed().subscribe(result => {
      if (result) {
        this.articleService.updateArticle(result);
      }
    });
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
   * Запрос удаления статьи.
   */
  onDelete() {
    this.dialog.open(Prompt, {
      width: '600px',
      data: `Do you really want to delete article "${this.article.title}"?`
    }).afterClosed().subscribe(result => {
      if (result) {
        this.articleService.deleteArticle(this.article);
      }
    });
  }

  /**
   * Запрос добавления комментария.
   */
  onAddComment() {
    this.dialog.open(CommentEditor, {
      width: '600px',
      data: { articleId: this.article.id }
    }).afterClosed().subscribe(result => {
      if (result) {
        this.commentService.addComment(result);
      }
    });
  }
}

