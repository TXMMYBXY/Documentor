using System.Threading.Tasks;

namespace Documentor.Application.Api;

public interface IGeneralClient
{
    /// <summary>
    /// Базовый метод для эндпоинтов Patch
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="uri">эндпоинт</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>TResponse</returns>
    Task<TResponse?> PatchResponseAsync<TRequest, TResponse>(TRequest request, string uri);
    
    /// <summary>
    /// Базовый метод для эндпоинтов Put
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="uri">эндпоинт</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>TResponse</returns>
    Task<TResponse?> PutResponseAsync<TRequest, TResponse>(TRequest request, string uri);
    
    /// <summary>
    /// Базовый метод для эндпоинтов Post
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="uri">эндпоинт</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>TResponse</returns>
    Task<TResponse?> PostResponseAsync<TRequest, TResponse>(TRequest request, string uri);

    /// <summary>
    /// Базовый метод для множественного удаления 
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="uri">эндпоинт</param>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>TResponse</returns>
    Task<TResponse?> MultipleDeletionResponseAsync<TRequest, TResponse>(TRequest request, string uri);    
    
    /// <summary>
    /// Базовый метод для эндпоинтов Delete
    /// </summary>
    /// <param name="uri">эндпоинт с id сущности</param>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>TResponse</returns>
    Task<TResponse?> DeleteResponseAsync<TResponse>(string uri);
    
    /// <summary>
    /// Базовый метод для эндпоинтов Get
    /// </summary>
    /// <param name="uri">эндпоинт</param>
    /// <typeparam name="TResponse">Тип ответа</typeparam>
    /// <returns>TResponse</returns>
    Task<TResponse?> GetResponseAsync<TResponse>(string uri);
}