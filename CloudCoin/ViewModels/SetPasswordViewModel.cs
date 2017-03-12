using GalaSoft.MvvmLight;
using System.Windows.Input;
using Xamarin.Forms;
using System;

namespace CloudCoin
{
	public class SetPasswordViewModel : ViewModelBase
	{
		public string Password { get; set; }
		public string VerifyPassword { get; set; }
		public bool ShortPasswordLabel { get; set; }
		public bool NotMatchLabel { get; set; }

		public ICommand OKButton { get; private set; }

		public SetPasswordViewModel()
		{
			ShortPasswordLabel = false;
			NotMatchLabel = false;
			Application.Current.MainPage.Navigation.PushModalAsync(new SetPasswordView());
			OKButton = new Command(ExecuteOKButton);
		}

		async void ExecuteOKButton()
		{
			
			if (Password.Length <= 5)
				ShortPasswordLabel = true;
			else if (Password != VerifyPassword)
				NotMatchLabel = true;
			else
			{
				Safe.password = Password;
				await Application.Current.MainPage.Navigation.PopModalAsync();
			}
		}
	}
}
