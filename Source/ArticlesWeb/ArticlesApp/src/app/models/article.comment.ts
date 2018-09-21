/**
 * Модель комментария к статье
 */
export class ArticleComment {
  /**
   * Идентификатор.
   */
  id: number;

  /**
   * Идентификатор статьи.
   */
  articleId: string;

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
}
