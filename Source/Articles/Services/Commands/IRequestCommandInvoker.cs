using Articles.Models;

namespace Articles.Services.Executors
{
    /// <summary>
    /// Интерфейс провайдера исполнителей.
    /// </summary>
    public interface IRequestCommandInvoker
    {

        /// <summary>
        /// Проверка принадлежности команды к к группе команд для статей по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для статей</returns>
        bool IsArticleCommand(string name);

        /// <summary>
        /// Проверка принадлежности команды к к группе команд для комментариев по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для комментариев</returns>
        bool IsCommentCommand(string name);

        /// <summary>
        /// Выполнение команды для статьи.
        /// </summary>
        /// <param name="name">Имя команды.</param>
        /// <param name="context">Контекст выполнения.</param>
        /// <param name="record">Данные статьи для команды.</param>
        /// <param name="validator">Валидатор статьи.</param>
        /// <returns>Статья, результат выполнения команды.</returns>
        Article ExecuteForArticle(string name, IDataContext context, Article article, IModelValidator<Article> validator);

        /// <summary>
        /// Выполнение команды для комментария.
        /// </summary>
        /// <param name="name">Имя команды.</param>
        /// <param name="context">Контекст выполнения.</param>
        /// <param name="record">Данные коммантария для команды.</param>
        /// <param name="validator">Валидатор комментария.</param>
        /// <returns>комментарий, результат выполнения команды.</returns>
        Comment ExecuteForComment(string name, IDataContext context, Comment comment, IModelValidator<Comment> validator);
    }
}
