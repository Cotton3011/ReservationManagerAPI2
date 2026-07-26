using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ReservationManagerAPI2.Exceptions;

namespace ReservationManagerAPI2.Middlewares
{
	//アプリ全体の例外をHTTPエラーレスポンスへ変換する
	public class ExceptionHandlingMiddleware
	{
		readonly RequestDelegate _next;

		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
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
			catch(Exception)
			{
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
