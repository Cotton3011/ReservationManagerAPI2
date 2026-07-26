using Microsoft.Identity.Client;

namespace ReservationManagerAPI2.Exceptions
{
	//業務エラーと返すHTTPステータスをまとめて扱うための基底例外
	public class AppException : Exception
	{
		public int StatusCode { get; }
		public AppException(int statusCode, string message): base(message) 
		{
			StatusCode = statusCode;
		}
	}

	//入力値や業務ルールが不正な場合に使う
	public class BadRequestException : AppException
	{
		public BadRequestException(string message) : base(StatusCodes.Status400BadRequest, message)
		{
		}
	}


	//対象データが存在しない場合に使う
	public class NotFoundException : AppException
	{
		public NotFoundException(string message) : base(StatusCodes.Status404NotFound, message)
		{
		}
	}

	//重複予約など、現在の状態と競合する場合に使う
	public class ConflictException : AppException
	{
		public ConflictException(string message) : base(StatusCodes.Status409Conflict, message)
		{
		}
	}
}
