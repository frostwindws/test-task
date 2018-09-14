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
    /// Сервис работы с комментариями к статьям.
    /// </summary>
    public class CommentsService : ICommentsService, IDisposable
    {
        /// <summary>
        /// Валидатор комментариев.
        /// </summary>
        private readonly IModelValidator<Comment> validator;

        /// <summary>
        /// Контекст работы с данными.
        /// </summary>
        private readonly IDataContext context;

        /// <summary>
        /// Конструктор сервиса.
        /// </summary>
        /// <param name="context">Используемый контекст.</param>
        /// <param name="validator">Валидатор для комментариев.</param>
        public CommentsService(IDataContext context, IModelValidator<Comment> validator)
        {
            this.context = context;
            this.validator = validator;
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
            const string ErrorLogTemplate = "Error accured while executing \"CommentsService\".{MethodName}";
            try
            {
                return function.Invoke();
            }
            catch (TimeoutException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>("Database connection timeout. Please try again later");
            }
            catch (DbException e)
            {
                Log.Error(e, ErrorLogTemplate, callerName);
                return ResultBuilder.Fault<T>($"Unexpected database exception: {e.GetBaseException().Message}.\r\nPlease try again later");
            }
        }

        /// <inheritdoc />
        public ResultDto<CommentDto[]> GetAll()
        {
            return SafeExecute(() =>
            {
                IOrderedEnumerable<Comment> comments = context.Comments.GetCollection().OrderByDescending(a => a.Created);
                return ResultBuilder.Success(Mapper.Map<CommentDto[]>(comments));
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Get(long id)
        {
            return SafeExecute(() =>
            {
                Comment comment = context.Comments.Get(id);
                CommentDto data = Mapper.Map<CommentDto>(comment);
                return comment == null
                    ? ResultBuilder.Fault<CommentDto>("Comment wasn't found")
                    : ResultBuilder.Success(data);
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto[]> GetForArticle(long articleid)
        {
            return SafeExecute(() =>
            {
                IEnumerable<Comment> comments = context.Comments.GetForArticle(articleid).OrderByDescending(a => a.Created);
                return ResultBuilder.Success(Mapper.Map<CommentDto[]>(comments));
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Create(CommentDto comment)
        {
            return SafeExecute(() =>
            {
                ResultDto<CommentDto> result = comment
                    .ToOption()
                    .DoOnEmpty(() => ResultBuilder.Fault<CommentDto>("The comment is empty"))
                    .Map(c => Mapper.Map<Comment>(c))
                    .Map(c =>
                    {
                        IEnumerable<string> errors = validator.GetErrors(context.Comments, c);
                        if (errors.Any())
                        {
                            return ResultBuilder.Fault<CommentDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                        }
                        else
                        {
                            c.Article = new Article { Id = c.ArticleId };
                            var createdComment = context.Comments.Create(c);
                            context.Commit();
                            return ResultBuilder.Success(Mapper.Map<CommentDto>(createdComment));
                        }
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Update(CommentDto comment)
        {
            return SafeExecute(() =>
            {
                ResultDto<CommentDto> result = comment
                    .ToOption()
                    .DoOnEmpty(() => result = ResultBuilder.Fault<CommentDto>("The comment is empty"))
                    .Map(a => Mapper.Map<Comment>(comment))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = validator.GetErrors(context.Comments, a);
                        if (errors.Any())
                        {
                            return ResultBuilder.Fault<CommentDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}");
                        }
                        else
                        {
                            var updatedComment = context.Comments.Update(a);
                            context.Commit();
                            return ResultBuilder.Success(Mapper.Map<CommentDto>(updatedComment));
                        }
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Delete(long id)
        {
            return SafeExecute(() =>
            {
                context.Comments.Delete(id);
                context.Commit();
                return ResultBuilder.Success<CommentDto>(null);
            });
        }

        /// <inheritdoc />
        public void Dispose()
        {
            context?.Dispose();

        }
    }
}
