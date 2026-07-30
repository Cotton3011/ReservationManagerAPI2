using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ReservationManagerAPI2.Exceptions;
using Microsoft.Extensions.Logging;

namespace ReservationManagerAPI2.Middlewares
{
	//アプリ全体の例外をHTTPエラーレスポンスへ変換する
	public class ExceptionHandlingMiddleware
	{
		readonly RequestDelegate _next;
		readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next,
			ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				//次のMiddleware、またはControllerを実行する
				await _next(context);
			}
			catch (AppException exception)
			{
				//業務例外は設定されたステータスコードで返す
				await WriteProblemDetailsAsync(
					context,
					exception.StatusCode,
					exception.Message);
			}
			catch(Exception exception)
			{
				//クライアントに詳細を返さず、サーバーログへ例外情報を残す
				_logger.LogError(
					exception,
					"予期しない例外が発生しました Method: {Method}, Path: {Path}",
					context.Request.Method,
					context.Request.Path);

				//想定外の例外の詳細はクライアントへ公開しない
				await WriteProblemDetailsAsync(
					context,
					StatusCodes.Status500InternalServerError,
					"予期しないエラーが発生しました");
			}
		}

		static async Task WriteProblemDetailsAsync(
			HttpContext context, 
			int statusCode, 
			string detail)
		{
			var problemDetails = new ProblemDetails()
			{
				Status = statusCode,
				Title = ReasonPhrases.GetReasonPhrase(statusCode),
				Detail = detail
			};

			context.Response.StatusCode = statusCode;

			//ASP.NET Core　標準のエラー形式でJSONを返す
			await context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}
