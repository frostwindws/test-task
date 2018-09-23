/**
 * Модель комментария к статье
 */
export class ArticleComment {

  /**
   * Идентификатор.
   */
  id: number = 0;

  /**
   * Идентификатор статьи.
   */
  articleId: number;

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
}
