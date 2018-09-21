import { Component, OnInit } from "@angular/core";

import { Article } from "../../models/article";
import { ArticleService } from "../../services/article.service";
import { ViewDataService } from "../../services/viewdata.service";

/**
 * Компонент отображения статьи.
 */
@Component({
  selector: "article-view",
  templateUrl: "./article.view.component.html",
  styleUrls: ["./article.view.component.less"]
})
export class ArticleView implements OnInit {
  currentArticle: Article;
  comments : Article[];
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
  }

  /**
   * Запуск редактирования статьи.
   */
  onEdit() {

  }

  /**
   * Обновление статьи.
   */
  onRefresh() {
    this.isLoading = true;
    this.articleService.getArticle(this.currentArticle.id)
      .subscribe(article => {
        this.viewData.setCurrentArticle(article);
        this.isLoading = false;
      });
  }

  /**
   * Запрос удаления статьи.
   */
  onDelete() {
    
  }

  /**
   * Запрос добавления комментария.
   */
  onAddComment() {

  }
}

