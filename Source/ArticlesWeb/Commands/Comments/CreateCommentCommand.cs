using System;
using System.Collections.ObjectModel;
using ArticlesService;
using ArticlesWeb.Models;
using AutoMapper;

namespace ArticlesWeb.Commands.Comments
{
    /// <summary>
    /// Команда обработки оповещения о добавлении комментария.
    /// </summary>
    internal class CreateCommentCommand : IRequestCommand<CommentDto>
    {
        /// <summary>
        /// Выполняемый метод (Действие при оповещении о добавлении комментария).
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Добавленный комментарий.</param>
        public void Execute(ViewDataContainer viewData, CommentDto record)
        {
            if (viewData.CurrentArticle?.Id == record.ArticleId)
            {
                Comment addedComment = Mapper.Map<Comment>(record);
                viewData.CurrentArticle.Comments.Add(addedComment);
                viewData.CurrentArticle.Comments = new ObservableCollection<Comment>(viewData.CurrentArticle.Comments.OrderByDescending(c => c.Created));
            }
        }
    }
}