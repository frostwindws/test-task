import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Article } from "../models/article";
import { Result } from "../models/result";

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

/**
 * Сервис получения данных статей
 */
@Injectable({
  providedIn: 'root',
})
export class ArticleService {
  private articleUrl = 'api/articles';

  constructor(private http: HttpClient) { }

  /**
   * Запрос списка всех статей
   */
  getArticles(): Observable<Article[]> {
    return this.http.get<Article[]>(this.articleUrl);
  }

  /**
   * Запрос на получение данных статьи
   */
  getArticle(id: number): Observable<Article> {
    return this.http.get<Article>(this.articleUrl + `/${id}`);
  }

  /**
   * Отправка запроса на добавление статьи
   * @param article Новая статья
   */
  addArticle(article: Article): Observable<Result> {
    return this.http.post<Result>(this.articleUrl, article, httpOptions);
  }

  /**
   * Отправка запроса на обновление статьи
   * @param article Обновленные данные статьи
   */
  updateArticle(article: Article): Observable<Result> {
    return this.http.put<Result>(this.articleUrl, article, httpOptions);
  }

  /**
   * Отправка запроса на удаление стаьти
   * @param article Удаляемая статья
   */
  deleteArticle(article: Article): Observable<Result> {
    return this.http.delete<Result>(this.articleUrl + `/${article.id}`);
  }
}
