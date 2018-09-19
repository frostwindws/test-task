using System;
using System.Collections.Generic;
using ArticlesService;
using ArticlesWeb.Clients.Rabbit.Commands;
using ArticlesWeb.Commands.Articles;
using ArticlesWeb.Commands.Comments;

namespace ArticlesWeb.Commands
{
    /// <summary>
    /// Исполнитель команд обновления данных.
    /// </summary>
    public class AnnounceCommandsInvoker
    {
        /// <summary>
        /// Набор команд для статей.
        /// </summary>
        private static readonly Dictionary<string, IRequestCommand<ArticleDto>> articleCommands = new Dictionary<string, IRequestCommand<ArticleDto>>
        {
            { CommandNames.CreateArticle, new CreateArticleCommand()},
            { CommandNames.UpdateArticle, new UpdateArticleCommand()},
            { CommandNames.DeleteArticle, new DeleteArticleCommand()},
        };

        /// <summary>
        /// Набор команд для комментариев.
        /// </summary>
        private static readonly Dictionary<string, IRequestCommand<CommentDto>> commentCommands = new Dictionary<string, IRequestCommand<CommentDto>>
        {
            { CommandNames.CreateComment, new CreateCommentCommand()},
            { CommandNames.UpdateComment, new UpdateCommentCommand()},
            { CommandNames.DeleteComment, new DeleteCommentCommand()},
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
        /// Выполнение метода для статьи.
        /// </summary>
        /// <param name="name">Имя выполняемой команды.</param>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="article">Данные обновленной статьи.</param>
        public void ExecuteForArticle(string name, ViewDataContainer viewData, ArticleDto article)
        {
            if (!articleCommands.TryGetValue(name, out IRequestCommand<ArticleDto> command))
            {
                throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
            }

            command.Execute(viewData, article);

        }

        /// <summary>
        /// Выполнение метода для комментария.
        /// </summary>
        /// <param name="name">Имя выполняемой команды.</param>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="comment">Данные обновленного комментария.</param>
        public void ExecuteForComment(string name, ViewDataContainer viewData, CommentDto comment)
        {
            if (!commentCommands.TryGetValue(name, out IRequestCommand<CommentDto> command))
            {
                throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
            }

            command.Execute(viewData, comment);
        }
    }
}