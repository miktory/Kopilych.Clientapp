using CommunityToolkit.Maui.Alerts;
using FluentValidation;
using Kopilych.Application.Common.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Kopilych.Mobile.Middleware
{
	public static class CustomExceptionHandlerMiddleware
	{

		public static void Handle(Exception ex)
		{
			var code = HttpStatusCode.InternalServerError;
			var result = string.Empty;
			switch (ex)
			{
				case FluentValidation.ValidationException validationException:
					code = HttpStatusCode.BadRequest;
					result = JsonSerializer.Serialize(validationException.Errors);
					break;

				case NotFoundException notFoundException:
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
                    });
                    break;

                case AccessDeniedException notFriendsException:
                    code = HttpStatusCode.Forbidden;
                    break;

                case TimeoutException:
					code = HttpStatusCode.ServiceUnavailable;
					break;


				case OperationCanceledException:
					code = HttpStatusCode.ServiceUnavailable;
					break;

                case AlreadyExistsException:
                    code = HttpStatusCode.Conflict;
                    break;

                default:
                    break;

			}

			if (result == string.Empty)
			{
				result = JsonSerializer.Serialize(new { error = ex.Message });
			}

            Device.BeginInvokeOnMainThread(async () =>
            {
                var toast = Toast.Make(ex.Message);
				toast.Show();
            });
        }
	}
}
