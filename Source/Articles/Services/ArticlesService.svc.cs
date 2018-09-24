using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Articles.Models;
using Articles.Services.Commands;
using Articles.Services.Models;
using AutoMapper;
using Serilog;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы со статьями.
    /// </summary>
    public class ArticlesService : IArticlesService, IDisposable
    {
        /// <summary>
        /// Валидатор статей.
        /// </summary>
        private readonly IModelValidator<Article> validator;

        /// <summary>
        /// Контекст работы с данными.
        /// </summary>
        private readonly IDataContext context;

        /// <summary>
        /// Исполнитель команд обновления данных
        /// </summary>
        private readonly UpdateCommandsInvoker commandsInvoker;

        /// <summary>
        /// Конструктор сервиса.
        /// </summary>
        /// <param name="context">Используемый контекст.</param>
        /// <param name="validator">Используемый валидатор статей.</param>
        public ArticlesService(IDataContext context, IModelValidator<Article> validator)
        {
            this.context = context;
            this.validator = validator;
            commandsInvoker = new UpdateCommandsInvoker();
        }

        /// <summary>
        /// Оббертка для выполнения операция со стандартным набором обработчиков ошибок.
        /// </summary>
        /// <typeparam name="T">Тип передаваемых на выходе данных.</typeparam>
        /// <param name="function">Выполняемая функция.</param>
        /// <param name="callerName">Имя вызывающего метода (используется для логирования).</param>
        /// <returns>Результат выполнения операции.</returns>
        private ResultDto<T> SafeExecute<T>(Func<ResultDto<T>> function, [CallerMemberName] string callerName = "")
        {
            const string ErrorLogTemplate = "Error accured while executing \"ArticleService\".{MethodName}";

            try
            {
                return function.Invoke();
            }
            catch (TimeoutException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>("Database connection timeout.\r\nPlease try again later");
            }
            catch (ArgumentException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>(e.GetBaseException().Message);
            }
            catch (Exception e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>($"Unexpected exception: {e.GetBaseException().Message}.\r\nPlease try again later");
            }
        }

        /// <summary>
        /// Получить список статей.
        /// </summary>
        /// <returns>Возвращает массив всех имеющихся статей.</returns>
        public ResultDto<ArticleDto[]> GetAll()
        {
            return SafeExecute(() =>
            {
                IEnumerable<Article> articles = context.Articles.GetCollection().OrderByDescending(a => a.Created);
                return ResultBuilder.Success(Mapper.Map<ArticleDto[]>(articles));
            });
        }

        /// <summary>
        /// Получить отдельную статью.
        /// </summary>
        /// <param name="id">Идентификатор статьи.</param>
        /// <returns>Возвращает объект запрашиваемой статьи.</returns>
        public ResultDto<ArticleDto> Get(long id)
        {
            return SafeExecute(() =>
            {
                Article article = context.Articles.Get(id);
                ArticleDto data = Mapper.Map<ArticleDto>(article);
                if (article == null)
                {
                    return ResultBuilder.Fault<ArticleDto>("Article wasn't found");
                }

                IEnumerable<Comment> comments = context.Comments.GetForArticle(article.Id).OrderByDescending(c => c.Created);
                data.Comments = Mapper.Map<CommentDto[]>(comments);
                return ResultBuilder.Success(data);
            });
        }

        /// <summary>
        /// Создать новую статью.
        /// </summary>
        /// <param name="article">Объект создаваемой стаьи.</param>
        public ResultDto<ArticleDto> Create(ArticleDto article)
        {
            return SafeExecute(() => {
                Article dbArticle = Mapper.Map<Article>(article);
                dbArticle =  commandsInvoker.ExecuteForArticle("article-create", context, dbArticle, validator);
                return ResultBuilder.Success(Mapper.Map<ArticleDto>(dbArticle));
            });
        }

        /// <summary>
        /// Обновить имеющуюся статью.
        /// </summary>
        /// <param name="article">Объект обновляемой статьи.</param>
        public ResultDto<ArticleDto> Update(ArticleDto article)
        {
            return SafeExecute(() => {
                Article dbArticle = Mapper.Map<Article>(article);
                dbArticle = commandsInvoker.ExecuteForArticle("article-update", context, dbArticle, validator);
                return ResultBuilder.Success(Mapper.Map<ArticleDto>(dbArticle));
            });
        }

        /// <summary>
        /// Удалить статью.
        /// </summary>
        /// <param name="id">Идентификатор удаляемой статьи.</param>
        public ResultDto<ArticleDto> Delete(long id)
        {
            return SafeExecute(() => {
                Article dbArticle = new Article() { Id = id };
                commandsInvoker.ExecuteForArticle("article-delete", context, dbArticle, validator);
                return ResultBuilder.Success<ArticleDto>(null);
            });
        }

        /// <summary>
        /// Освобождение ресурсов.
        /// </summary>
        public void Dispose()
        {
            context?.Dispose();
        }
    }
}
