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
  created: Date = null;

  /**
   * Комментарии к статье
   */
  comments: Comment[];
}
