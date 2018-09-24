import { Component, OnInit } from "@angular/core";
import { MatDialog } from '@angular/material';

import { Article } from "../../models/article";

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
  articles: Article[];
  currentArticle: Article;
  isLoading = false;

  /**
   * Конструктор компонента.
   * @param articleService Сервис получения/обновления данных о статьях.
   * @param viewData Сервис разделяемых данных для отображения.
   */
  constructor(public dialog: MatDialog, private signalr: SignalRService, private articleService: ArticleService, private viewData: ViewDataService) { }

  /**
   * Инициализация компонента.
   */
  ngOnInit() {
    this.viewData.currentArticle.subscribe(article => this.currentArticle = article);
    this.getArticles();

    // Подписка на обновления данных
    this.signalr.subscribe('article-create', this.onArticleCreate);
    this.signalr.subscribe('article-update', this.onArticleUpdate);
    this.signalr.subscribe('article-delete', this.onArticleDelete);
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
        this.articleService.addArticle(result);
      }
    });
  }

  /**
   * Обработчик оповещения о создании новой статьи.
   * @param article Созданная статья.
   */
  private onArticleCreate(article: Article) {
    if (!this.isLoading) {
      let clients = this.articles.slice();
      clients.push(article);
      this.articles = clients;
    }
  }

  /**
   * Обработчик оповещения об обновлении статьи
   * @param article Обновленная статья
   */
  private onArticleUpdate(article: Article) {
    if (!this.isLoading) {
      let index = this.getIndexById(article.id);
      this.articles[index].title = article.title;
    }
  }

  /**
   * Обработчик оповещения о удалении статьи
   * @param article Удаленная статья
   */
  private onArticleDelete(article: Article) {

    if (!this.isLoading) {
      let index = this.getIndexById(article.id);
      let articles = this.articles.slice(index, 1);
      this.articles = articles;
    }
  }

  /**
   * Get client index in clients list by his id
   * @param id Client id
   */
  private getIndexById(id: number): number {
    for (var i = 0; i < this.articles.length; i++) {
      if (this.articles[i].id === id) return i;
    }

    return -1;
  }
}

