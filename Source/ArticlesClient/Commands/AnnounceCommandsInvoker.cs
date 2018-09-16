using Articles.Services.Executors.Articles;
using Articles.Services.Executors.Comments;
using ArticlesClient.ArticlesService;
using ArticlesClient.Clients.Rabbit;
using ArticlesClient.Models;
using System.Collections.Generic;

namespace Articles.Services.Executors
{
    /// <summary>
    /// Исполнитель команд обновления данных.
    /// </summary>
    internal class AnnounceCommandsInvoker
    {
        /// <summary>
        /// Набор команд для статей.
        /// </summary>
        private static Dictionary<string, IRequestCommand<ArticleDto>> ArticleCommands = new Dictionary<string, IRequestCommand<ArticleDto>>
        {
            { CommandNames.CreateArticle, new CreateArticleCommand()},
            { CommandNames.UpdateArticle, new UpdateArticleCommand()},
            { CommandNames.DeleteArticle, new DeleteArticleCommand()},
        };

        /// <summary>
        /// Набор команд для комментариев.
        /// </summary>
        private static Dictionary<string, IRequestCommand<CommentDto>> CommentCommands = new Dictionary<string, IRequestCommand<CommentDto>>
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
            return ArticleCommands.ContainsKey(name);
        }

        /// <summary>
        /// Проверка принадлежности команды к к группе команд для комментариев по ее имени.
        /// </summary>
        /// <param name="name">имя команды.</param>
        /// <returns>True, если команда относится к командам для комментариев</returns>
        public bool IsCommentCommand(string name)
        {
            return CommentCommands.ContainsKey(name);
        }

        /// <summary>
        /// Выполнение метода для статьи.
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Данные обновленной статьи.</param>
        public void ExecuteForArticle(string name, ViewDataContainer viewData, ArticleDto article)
        {
            if (ArticleCommands.TryGetValue(name, out IRequestCommand<ArticleDto> command))
            {
                command.Execute(viewData, article);
            }

            throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
        }

        /// <summary>
        /// Выполнение метода для комментария.
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Данные обновленного комментария.</param>
        public void ExecuteForComment(string name, ViewDataContainer viewData, CommentDto article)
        {
            if (CommentCommands.TryGetValue(name, out IRequestCommand<CommentDto> command))
            {
                command.Execute(viewData, article);
            }

            throw new KeyNotFoundException($"Received unknown command type message \"{name}\"");
        }
    }
}