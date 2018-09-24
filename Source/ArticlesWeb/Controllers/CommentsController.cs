using System;
using ArticlesWeb.Clients;
using ArticlesWeb.Clients.Rabbit.Commands;
using ArticlesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
// ReSharper disable UnusedMember.Global

namespace ArticlesWeb.Controllers
{
    /// <summary>
    /// Контроллер обращений к модели комментария.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase, IDisposable
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
        public Result Post([FromBody] Comment comment)
        {
            try
            {
                updateContext.SendUpdateForComment(CommandNames.CreateComment, comment);
                return new Result();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while processing Comment Post request");
                return new Result(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Редактирование существующего комментария.
        /// PUT: api/Comments/5.
        /// </summary>
        /// <param name="comment">Обновляемые данные комментария.</param>
        [HttpPut]
        public Result Put([FromBody] Comment comment)
        {
            try
            {
                updateContext.SendUpdateForComment(CommandNames.UpdateComment, comment);
                return new Result();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while processing Comment Put request");
                return new Result(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Удаление комментария.
        /// DELETE: api/Comments/5.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого комментария.</param>
        [HttpDelete("{id}")]
        public Result Delete(long id)
        {
            try
            {
                updateContext.SendUpdateForComment(CommandNames.DeleteComment, new Comment { Id = id });
                return new Result();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while processing Comment Delete request");
                return new Result(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            updateContext?.Dispose();
        }
    }
}
