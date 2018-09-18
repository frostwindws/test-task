using System;
using System.Collections.Generic;
using Articles.Models;
using Articles.Services.Commands.Articles;
using Articles.Services.Commands.Comments;

namespace Articles.Services.Commands
{
    /// <summary>
    /// Исполнитель команд обновления данных
    /// </summary>
    public class UpdateCommandsInvoker : IRequestCommandInvoker
    {
        /// <summary>
        /// Набор команд для статей
        /// </summary>
        private static readonly Dictionary<string, IRequestCommand<Article>> articleCommands = new Dictionary<string, IRequestCommand<Article>>
        {
            { "article-create", new CreateArticleCommand()},
            { "article-update", new UpdateArticleCommand()},
            { "article-delete", new DeleteArticleCommand()},
        };

        /// <summary>
        /// Набор команд для комментариев
        /// </summary>
        private static readonly Dictionary<string, IRequestCommand<Comment>> commentCommands = new Dictionary<string, IRequestCommand<Comment>>
        {
            { "comment-create", new CreateCommentCommand()},
            { "comment-update", new UpdateCommentCommand()},
            { "comment-delete", new DeleteCommentCommand()},
        };

        /// <summary>
        /// Проверка принадлежности команды к к группе команд для статей по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для статей</returns>
        public bool IsArticleCommand(string name)
        {
            return articleCommands.ContainsKey(name);
        }

        /// <summary>
        /// Проверка принадлежности команды к к группе команд для комментариев по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для комментариев</returns>
        public bool IsCommentCommand(string name)
        {
            return commentCommands.ContainsKey(name);
        }

        /// <summary>
        /// Выполнение команды для статьи.
        /// </summary>
        /// <param name="name">Имя команды.</param>
        /// <param name="context">Контекст выполнения.</param>
        /// <param name="article">Данные статьи для команды.</param>
        /// <param name="validator">Валидатор статьи.</param>
        /// <returns>Статья, результат выполнения команды.</returns>
        public Article ExecuteForArticle(string name, IDataContext context, Article article, IModelValidator<Article> validator)
        {
            if (articleCommands.TryGetValue(name, out IRequestCommand<Article> command))
            {
                return command.Execute(context, article, validator);
            }

            throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
        }

        /// <summary>
        /// Выполнение команды для комментария.
        /// </summary>
        /// <param name="name">Имя команды.</param>
        /// <param name="context">Контекст выполнения.</param>
        /// <param name="comment">Данные коммантария для команды.</param>
        /// <param name="validator">Валидатор комментария.</param>
        /// <returns>комментарий, результат выполнения команды.</returns>
        public Comment ExecuteForComment(string name, IDataContext context, Comment comment, IModelValidator<Comment> validator)
        {
            if (commentCommands.TryGetValue(name, out IRequestCommand<Comment> command))
            {
                return command.Execute(context, comment, validator);
            }

            throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
        }
    }
}