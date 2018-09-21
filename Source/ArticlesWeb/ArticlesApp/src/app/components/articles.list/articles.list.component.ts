import { Component, OnInit } from "@angular/core";

import { Article } from "../../models/article";
import { ArticleService } from "../../services/article.service";
import { ViewDataService } from "../../services/viewdata.service";

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
  constructor(private articleService: ArticleService, private viewData: ViewDataService) { }

  /**
   * Инициализация компонента.
   */
  ngOnInit() {
    this.viewData.currentArticle.subscribe(article => this.currentArticle = article);
    this.getArticles();
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
    this.viewData.setEditableArticle(new Article());
  }
}

