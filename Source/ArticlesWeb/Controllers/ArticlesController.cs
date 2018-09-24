﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArticlesWeb.Clients;
using ArticlesWeb.Clients.Rabbit.Commands;
using ArticlesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

// ReSharper disable UnusedMember.Global

namespace ArticlesWeb.Controllers
{
    /// <summary>
    /// Контроллер обращений к модели статьи.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase, IDisposable
    {
        private readonly IReadContext readContext;
        private readonly IUpdateContext updateContext;

        public ArticlesController(IReadContext readContext, IUpdateContext updateContext)
        {
            this.readContext = readContext;
            this.updateContext = updateContext;
        }

        /// <summary>
        /// Запрос получение списка всех статей.
        /// GET: api/Articles.
        /// </summary>
        /// <returns>перечиселение статей, зарегистрированных на сервере.</returns>
        [HttpGet]
        public async Task<IEnumerable<Article>> Get()
        {
            return await readContext.Articles.GetAllAsync();
        }

        /// <summary>
        /// Запрос на получение статьи по ее идентификатору.
        /// GET: api/Articles/{id}.
        /// </summary>
        /// <param name="id">Идентификатор статьи.</param>
        /// <returns>Запрашиваемая статья.</returns>
        [HttpGet("{id}", Name = "Get")]
        public async Task<Article> Get(long id)
        {
            return await readContext.Articles.GetAsync(id);
        }

        /// <summary>
        /// Добавление новой статьи.
        /// POST: api/Articles.
        /// </summary>
        /// <param name="article">Добавляемая статья.</param>
        [HttpPost]
        public Result Post([FromBody] Article article)
        {
            try
            {
                updateContext.SendUpdateForArticle(CommandNames.CreateArticle, article);
                return new Result();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while processing Article Post request");
                return new Result(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Обновление имеющейся статьи.
        /// PUT: api/Articles/{id}.
        /// </summary>
        /// <param name="article">Одновленные данные статьи.</param>
        public Result Put([FromBody] Article article)
        {
            try
            {
                updateContext.SendUpdateForArticle(CommandNames.UpdateArticle, article);
                return new Result();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while processing Article Put request");
                return new Result(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Удаление статьи.
        /// DELETE: api/Articles/{id}.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой статьи.</param>
        [HttpDelete("{id}")]
        public Result Delete(long id)
        {
            try
            {
                updateContext.SendUpdateForArticle(CommandNames.DeleteArticle, new Article { Id = id });
                return new Result();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error was occured while processing Article Delete request");
                return new Result(e.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            readContext?.Dispose();
            updateContext?.Dispose();
        }
    }
}
