using System;

namespace ArticlesWeb.Clients.Rabbit.Commands
{
    /// <summary>
    /// Допустимые типы запросов для отправки на RabbitMQ.
    /// </summary>
    public static class CommandNames
    {
        /// <summary>
        /// Запрос на добавление статьи.
        /// </summary>
        public const string CreateArticle = "article-create";

        /// <summary>
        /// Запрос на редактирование статьи.
        /// </summary>
        public const string UpdateArticle = "article-update";

        /// <summary>
        /// Запрос на удаление статьи.
        /// </summary>
        public const string DeleteArticle = "article-delete";

        /// <summary>
        /// Запрос на добавление комментария.
        /// </summary>
        public const string CreateComment = "comment-create";

        /// <summary>
        /// Запрос на редактирование комментария.
        /// </summary>
        public const string UpdateComment = "comment-update";

        /// <summary>
        /// Запрос на удаление комментария.
        /// </summary>
        public const string DeleteComment = "comment-delete";


        /// <summary>
        /// Проверка принадлежности команды к к группе команд для статей по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для статей</returns>
        public static bool IsArticleCommand(string name)
        {
            return name.StartsWith("article-");
        }

        /// <summary>
        /// Проверка принадлежности команды к к группе команд для комментариев по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для комментариев</returns>
        public static bool IsCommentCommand(string name)
        {
            return name.StartsWith("comment-");
        }
    }
}
