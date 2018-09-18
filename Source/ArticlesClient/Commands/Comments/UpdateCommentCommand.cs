﻿using System;
using System.Linq;
using ArticlesClient.Connected_Services.ArticlesService;
using ArticlesClient.Models;

namespace ArticlesClient.Commands.Comments
{
    /// <summary>
    /// Команда обработки оповещения об обновлении комментария.
    /// </summary>
    internal class UpdateCommentCommand : IRequestCommand<CommentDto>
    {
        /// <summary>
        /// Выполняемый метод (Действие при оповещении об обновлении комментария).
        /// </summary>
        /// <param name="viewData">Контейнер отображаемых данных.</param>
        /// <param name="record">Обновленный комментарий.</param>
        public void Execute(ViewDataContainer viewData, CommentDto record)
        {
            if (viewData.CurrentArticle?.Id == record.ArticleId)
            {
                CommentView commentToUpdate = viewData.CurrentArticle.Comments.FirstOrDefault(c => c.Id == record.Id);
                if (commentToUpdate != null)
                {
                    commentToUpdate.Content = record.Content;
                }
            }
        }
    }
}