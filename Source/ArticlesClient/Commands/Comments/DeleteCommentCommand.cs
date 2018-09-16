using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using System.Linq;

namespace Articles.Services.Executors.Comments
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
                }
            }
        }
    }
}