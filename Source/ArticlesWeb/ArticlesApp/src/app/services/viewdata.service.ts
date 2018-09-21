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
   * Источник данных о редактирумой статье.
   */
  private editableArticleSource = new BehaviorSubject<Article>(null);

  /**
   * Текущая статья.
   */
  currentArticle = this.currentArticleSource.asObservable();

  /**
   * Редактируемая статья.
   */
  editableArticle = this.editableArticleSource.asObservable();

  /**
   * Установка текущей статьи.
   * @param article Устанавливаемое значение.
   */
  setCurrentArticle(article: Article) {
    this.editableArticleSource.next(null);
    this.currentArticleSource.next(article);
  }

  /**
   * Установка редатируемой статьи.
   * @param article Устанавливаемое значение.
   */
  setEditableArticle(article: Article) {
    this.editableArticleSource.next(article);
  }
}
