using ArticlesClient.ArticlesService;
using ArticlesClient.Models;
using AutoMapper;
using System.Collections.ObjectModel;
using System.Linq;

namespace Articles.Services.Executors.Comments
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
                CommentView addedComment = Mapper.Map<CommentView>(record);
                viewData.CurrentArticle.Comments.Add(addedComment);
                viewData.CurrentArticle.Comments = new ObservableCollection<CommentView>(viewData.CurrentArticle.Comments.OrderByDescending(c => c.Created));
            }
        }
    }
}