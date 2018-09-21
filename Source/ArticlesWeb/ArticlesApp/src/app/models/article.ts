/**
 * Модель статьи.
 */
export class Article {
  /**
   * Идентификатор.
   */
  id: number;

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
  created: string;

  /**
   * Комментарии к статье
   */
  comments: Comment[];
}
