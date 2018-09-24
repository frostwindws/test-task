import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { ArticleComment } from "../models/article.comment";
import { Result } from "../models/result";

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

/**
 * Сервис запросов обновления комментариев
 */
@Injectable({
  providedIn: 'root',
})
export class CommentService {
  private articleUrl = 'api/comments';

  constructor(private http: HttpClient) { }

  /**
   * Отправка запроса на добавление комментария.
   * @param comment Новый комментарий
   */
  addComment(comment: ArticleComment): Observable<Result> {
    return this.http.post<Result>(this.articleUrl, comment, httpOptions);
  }

  /**
   * Отправка запроса на обновление комментария
   * @param comment Обновленные данные комментария
   */
  updateComment(comment: ArticleComment): Observable<Result> {
    return this.http.put<Result>(this.articleUrl, comment, httpOptions);
  }

  /**
   * Отправка запроса на удаление комментария
   * @param comment Удаляемый комментария
   */
  deleteComment(comment: ArticleComment): Observable<Result> {
    return this.http.delete<Result>(this.articleUrl + `/${comment.id}`);
  }
}
