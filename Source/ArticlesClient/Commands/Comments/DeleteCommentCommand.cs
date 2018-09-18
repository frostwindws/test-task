using System;
using System.Collections.ObjectModel;
using System.Linq;
using ArticlesClient.Connected_Services.ArticlesService;
using ArticlesClient.Models;

namespace ArticlesClient.Commands.Comments
{
    /// <summary>
    /// Команда обработки оповещения об удалении комментария.
    /// </summary>
    internal class DeleteCommentCommand : IRequestCommand<CommentDto>
    {
        /// <summary>
        /// Выполняемый метод (Действие при оповещении об удалении комментария).
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Удаленный комментарий.</param>
        public void Execute(ViewDataContainer viewData, CommentDto record)
        {
            if (viewData.CurrentArticle?.Id == record.ArticleId)
            {
                CommentView commentToDelete = viewData.CurrentArticle.Comments.FirstOrDefault(c => c.Id == record.Id);
                if (commentToDelete != null)
                {
                    viewData.CurrentArticle.Comments.Remove(commentToDelete);
                    viewData.CurrentArticle.Comments = new ObservableCollection<CommentView>(viewData.CurrentArticle.Comments);
                }
            }
        }
    }
}