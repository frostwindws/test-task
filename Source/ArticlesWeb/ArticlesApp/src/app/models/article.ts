import { ArticleComment } from "./article.comment";

/**
 * Модель статьи.
 */
export class Article {
  /**
   * Идентификатор.
   */
  id: number = 0;

  /**
   * Заголовок.
   */
  title: string;

  /**
   * Автор.
   */
  author: string;

  /**
   * Текст.
   */
  content: string;

  /**
   * Дата создания.
   */
  created: Date = new Date();

  /**
   * Комментарии к статье
   */
  comments: ArticleComment[] = [];
}
