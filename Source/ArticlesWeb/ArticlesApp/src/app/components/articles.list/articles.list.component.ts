import { Component, OnInit } from "@angular/core";
import { MatDialog, MatSnackBar } from '@angular/material';

import { Article } from "../../models/article";
import { Result } from "../../models/result";

import { ArticleService } from "../../services/article.service";
import { SignalRService } from "../../services/signalr.service";
import { ViewDataService } from "../../services/viewdata.service";

import { ArticleEditor } from '../article.editor/article.editor.component';

/**
 * Компонент списка статей.
 */
@Component({
  selector: "articles-list",
  templateUrl: "./articles.list.component.html",
  styleUrls: ["./articles.list.component.less"]
})
export class ArticlesList implements OnInit {
  private articles: Article[];
  private currentArticle: Article;
  private isLoading = false;

  /**
   * Конструктор компонента.
   * @param articleService Сервис получения/обновления данных о статьях.
   * @param viewData Сервис разделяемых данных для отображения.
   */
  constructor(public snackBar: MatSnackBar, public dialog: MatDialog, private signalr: SignalRService, private articleService: ArticleService, private viewData: ViewDataService) { }

  /**
   * Инициализация компонента.
   */
  ngOnInit() {
    this.viewData.currentArticle.subscribe(article => this.currentArticle = article);
    this.getArticles();

    // Подписка на обновления данных
    this.signalr.subscribe('article-create', this, this.onArticleCreated);
    this.signalr.subscribe('article-update', this, this.onArticleUpdated);
    this.signalr.subscribe('article-delete', this, this.onArticleDeleted);
  }

  /**
   * Запрос на получение списка статей.
   */
  getArticles() {
    this.isLoading = true;
    this.articleService.getArticles()
      .subscribe(articles => {
        this.articles = articles;
        this.isLoading = false;
      });
  }

  /**
   * Обработчик выбора статьи.
   * @param article Выбранная статья.
   */
  onSelect(article: Article) {
    // Предварительно отображается уже имеющаяся информация,
    // полная информация о статье обновляется по окончании загрузки.
    this.isLoading = true;
    this.viewData.setCurrentArticle(article);
    this.articleService.getArticle(article.id)
      .subscribe(a => {
        this.viewData.setCurrentArticle(a);
        this.isLoading = false;
      });
  }

  /**
   * Добавление новой статьи.
  */
  onAdd() {
    this.dialog.open(ArticleEditor, {
      width: '600px',
      data: new Article()
    }).afterClosed().subscribe(result => {
      if (result) {
        this.articleService.addArticle(result)
          .subscribe((result: Result) => {
            let message = result.success ? 'Add article request was sent succesfully' : result.message;
            this.snackBar.open(message, null, { duration: 1000 });
          });
      }
    });
  }

  /**
   * Обработчик оповещения о создании новой статьи.
   * @param article Созданная статья.
   */
  private onArticleCreated(article: Article) {
    if (!this.isLoading) {
      this.articles.unshift(article);
    }
  }

  /**
   * Обработчик оповещения об обновлении статьи.
   * @param article Обновленная статья.
   */
  private onArticleUpdated(article: Article) {
    if (!this.isLoading) {
      let index = this.getIndexById(this.articles, article.id);
      if (index >= 0) {
        this.articles[index].title = article.title;
      }
    }
  }

  /**
   * Обработчик оповещения о удалении статьи.
   * @param article Удаленная статья.
   */
  private onArticleDeleted(article: Article) {

    if (!this.isLoading) {
      let index = this.getIndexById(this.articles, article.id);
      if (index >= 0) {
        this.articles.splice(index, 1);
      }
    }
  }

  /**
   * Поиск статьи в массиве по ее илентификатору.
   * @param id Идентификатор статьи.
   */
  private getIndexById(array: Article[], id: number): number {
    for (var i = 0; i < array.length; i++) {
      if (array[i].id === id) return i;
    }

    return -1;
  }
}

