﻿using System;
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
    /// Сервис работы с комментариями к статьям
    /// </summary>
    public class CommentsService : ICommentsService, IDisposable
    {
        /// <summary>
        /// Контекст работы с данными
        /// </summary>
        private IDataContext Context { get; }

        /// <summary>
        /// Конструктор сервиса
        /// </summary>
        /// <param name="context">Используемый контекст</param>
        public CommentsService(IDataContext context)
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
            const string ErrorLogTemplate = "Error accured while executing \"CommentsService\".{MethodName}";
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
        public ResultDto<CommentDto[]> GetAll()
        {
            return SaveExecute(() =>
            {
                IOrderedEnumerable<Comment> comments = Context.Comments.GetCollection().OrderByDescending(a => a.Created);
                return SuccessResult(Mapper.Map<CommentDto[]>(comments));
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Get(long id)
        {
            return SaveExecute(() =>
            {
                Comment comment = Context.Comments.Get(id);
                CommentDto data = Mapper.Map<CommentDto>(comment);
                return comment == null 
                    ? FaultResult<CommentDto>("Comment wasn't found") 
                    : SuccessResult(data);
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto[]> GetForArticle(long articleid)
        {
            return SaveExecute(() =>
            {
                IEnumerable<Comment> comments = Context.Comments.GetForArticle(articleid).OrderByDescending(a => a.Created);
                return SuccessResult(Mapper.Map<CommentDto[]>(comments));
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Create(CommentDto comment)
        {
            return SaveExecute(() =>
            {
                ResultDto<CommentDto> result = comment
                    .ToOption()
                    .DoOnEmpty(() => result = FaultResult<CommentDto>("The comment is empty"))
                    .Map(a => Mapper.Map<Comment>(comment))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = a.Validate();
                        return errors.Any()
                            ? FaultResult<CommentDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}")
                            : SuccessResult(Mapper.Map<CommentDto>(Context.Comments.Create(a)));
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Update(CommentDto comment)
        {
            return SaveExecute(() => {
                ResultDto<CommentDto> result = comment
                    .ToOption()
                    .DoOnEmpty(() => result = FaultResult<CommentDto>("The comment is empty"))
                    .Map(a => Mapper.Map<Comment>(comment))
                    .Map(a =>
                    {
                        IEnumerable<string> errors = a.Validate();
                        return errors.Any()
                            ? FaultResult<CommentDto>($"Validation failure:\r\n\t{string.Join(";\r\n\t", errors)}")
                            : SuccessResult(Mapper.Map<CommentDto>(Context.Comments.Update(a)));
                    }).Value;

                return result;
            });
        }

        /// <inheritdoc />
        public ResultDto<CommentDto> Delete(long id)
        {
            return SaveExecute(() => {
                Context.Comments.Delete(id);
                return SuccessResult<CommentDto>(null);
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
