import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ArticleComment } from "../models/article.comment";

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
   * Отправка запроса на добавление комментария
   * @param comment Новый комментарий
   */
  addComment(comment: ArticleComment) {
    this.http.post<ArticleComment>(this.articleUrl, comment, httpOptions);
  }

  /**
   * Отправка запроса на обновление комментария
   * @param comment Обновленные данные комментария
   */
  updateComment(comment: ArticleComment) {
    return this.http.put(this.articleUrl, comment, httpOptions);
  }

  /**
   * Отправка запроса на удаление комментария
   * @param comment Удаляемый комментария
   */
  deleteComment(comment: ArticleComment) {
    this.http.delete<ArticleComment>(this.articleUrl + `/${comment.id}`);
  }
}
