import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Article } from '../models/article';

/**
 * Сервис разделяемых данных для отображения.
 */
@Injectable()
export class ViewDataService {

  /**
   * Источник данных о текущей статье.
   */
  private currentArticleSource = new BehaviorSubject<Article>(null);

  /**
   * Текущая статья.
   */
  currentArticle = this.currentArticleSource.asObservable();

  /**
   * Установка текущей статьи.
   * @param article Устанавливаемое значение.
   */
  setCurrentArticle(article: Article) {
    this.currentArticleSource.next(article);
  }
}
