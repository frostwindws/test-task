using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using Articles.Models;
using Articles.Services.Models;
using AutoMapper;
using Nelibur.Sword.Extensions;
using Serilog;

namespace Articles.Services
{
    /// <summary>
    /// Сервис работы со статьями
    /// </summary>
    public class ArticlesService : IArticlesService, IDisposable
    {
        /// <summary>
        /// Контекст работы с данными
        /// </summary>
        private IDataContext Context { get; }

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="context">Используемый контекст</param>
        public ArticlesService(IDataContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Формирование результат успешной операции
        /// </summary>
        /// <typeparam name="T">Тип передаваемых данных</typeparam>
        /// <param name="data">передаваемые в результате данные</param>
        /// <returns>Результат операции с флагом успешного выполнения</returns>
        private ResultDto<T> SuccessResult<T>(T data)
        {
            return new ResultDto<T>
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Формирование результат об ошибке выполнения операции
        /// </summary>
        /// <typeparam name="T">Тип передаваемых данных</typeparam>
        /// <param name="exception">Текст сообщения об ошибке</param>
        /// <returns>Результат операции с флагом провала выполнения</returns>
        private ResultDto<T> FaultResult<T>(string exception)
        {
            return new ResultDto<T>
            {
                Success = false,
                Message = exception
            };
        }

        /// <summary>
        /// Оббертка для выполнения операция со стандартным набором обработчиков ошибок
        /// </summary>
        /// <typeparam name="T">Тип передаваемых на выходе данных</typeparam>
        /// <param name="function">Выполняемая функция</param>
        /// <param name="callerName">Имя вызывающего метода (используется для логирования)</param>
        /// <returns>Результат выполнения операции</returns>
        private ResultDto<T> SaveExecute<T>(Func<ResultDto<T>> function, [CallerMemberName] string callerName = "")
        {
            const string ErrorLogTemplate = "Error accured while executing \"ArticleService\".{MethodName}";

            try
            {
                return function.Invoke();
            }
            catch (TimeoutException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return FaultResult<T>("Database connection timeout. Please try again later");
            }
            catch (DbException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return FaultResult<T>($"Unexpected database exception: {e.GetBaseException().Message}.\r\nPlease try again later");
            }
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto[]> GetAll()
        {
            return SaveExecute(() =>
            {
                IEnumerable<Article> articles = Context.Articles.GetCollection().OrderByDescending(a => a.Created);
                return SuccessResult(Mapper.Map<ArticleDto[]>(articles));
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Get(long id)
        {
            return SaveExecute(() =>
            {
                Article article = Context.Articles.Get(id);
                ArticleDto data = Mapper.Map<ArticleDto>(article);
                if (article == null)
                {
                    return FaultResult<ArticleDto>("Article wasn't found");
                }

                IEnumerable<Comment> comments = Context.Comments.GetForArticle(article.Id);
                data.Comments = Mapper.Map<CommentDto[]>(comments);
                return SuccessResult(data);
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Create(ArticleDto article)
        {
            return SaveExecute(() =>
            {
                ResultDto<ArticleDto> result = article
                    .ToOption()
                    .DoOnEmpty(() => result = FaultResult<ArticleDto>("The article is empty"))
                    .Map(a => Mapper.Map<Article>(article))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = a.Validate();
                        return errors.Any()
                            ? FaultResult<ArticleDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}")
                            : SuccessResult(Mapper.Map<ArticleDto>(Context.Articles.Create(a)));
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Update(ArticleDto article)
        {
            return SaveExecute(() => {
                ResultDto<ArticleDto> result = article
                    .ToOption()
                    .DoOnEmpty(() => result = FaultResult<ArticleDto>("The article is empty"))
                    .Map(a => Mapper.Map<Article>(article))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = a.Validate();
                        return errors.Any()
                            ? FaultResult<ArticleDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}")
                            : SuccessResult(Mapper.Map<ArticleDto>(Context.Articles.Update(a)));
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<ArticleDto> Delete(long id)
        {
            return SaveExecute(() => {
                Context.Articles.Delete(id);
                return SuccessResult<ArticleDto>(null);
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Context is IDisposable disposableContext)
            {
                disposableContext.Dispose();
            }
        }
    }
}
