using System;
using ArticlesWeb.Clients;
using ArticlesWeb.Clients.Rabbit.Commands;
using ArticlesWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace ArticlesWeb.Controllers
{
    /// <summary>
    /// Контроллер обращений к модели комментария.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IUpdateContext updateContext;

        public CommentsController(IUpdateContext updateContext)
        {
            this.updateContext = updateContext;
        }

        /// <summary>
        /// Добавление нового комментария.
        /// POST: api/Comments.
        /// </summary>
        /// <param name="comment"></param>
        [HttpPost]
        public void Post([FromBody] Comment comment)
        {
            updateContext.SendUpdateForComment(CommandNames.CreateComment, comment);
        }

        /// <summary>
        /// Редактирование существующего комментария.
        /// PUT: api/Comments/5.
        /// </summary>
        /// <param name="comment">Обновляемые данные комментария.</param>
        [HttpPut]
        public void Put([FromBody] Comment comment)
        {
            updateContext.SendUpdateForComment(CommandNames.UpdateComment, comment);
        }

        /// <summary>
        /// Удаление комментария.
        /// DELETE: api/Comments/5.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого комментария.</param>
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            updateContext.SendUpdateForComment(CommandNames.DeleteComment, new Comment { Id = id });
        }
    }
}
