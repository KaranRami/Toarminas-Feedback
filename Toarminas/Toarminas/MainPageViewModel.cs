using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Toarminas.DependencyInterface;
using Xamarin.Forms;

namespace Toarminas
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public MainPageViewModel(ContentPage view)
        {
            m_View = view;
            Task.Run(() =>
            {
                RotateOfferImage();
            });

        }

        private void RotateOfferImage()
        {
            string[] offers = DependencyService.Get<IServiceAPI>().GetImages();
            if (offers == null || offers.Length == 0)
                return;
            int i = 0;
            while (true)
            {
                CurrentOfferImage = offers[i];
                i++;
                if (i == offers.Length)
                    i = 0;
                Thread.Sleep(10000);
            }
        }

        public ICommand SubscribeCommand { get { return new Command(async () => await SubscribeCommandEvent()); } }
        private async Task SubscribeCommandEvent()
        {
            await SingleRun(async () =>
            {
                if (Validate())
                {
                    DependencyService.Get<IServiceAPI>().AddCustomerDetail(new Model.ContactModel()
                    {
                        Email = Email,
                        Phone = Phone,
                        SubScriptionTime = DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss"),
                    });
                    IsTCAccepted = false;
                    Phone = string.Empty;
                    Email = string.Empty;
                    string result = await LaunchPopup<string>(new ThankYouPopUp(),true);
                }
            });
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(Phone) && string.IsNullOrEmpty(Email))
            {
                m_View.DisplayAlert(Constants.AppName, Constants.BlankContancts, "Ok");
                return false;
            }
            if (!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email, Constants.EmailRegex))
            {
                m_View.DisplayAlert(Constants.AppName, Constants.WrongEmail, "Ok");
                return false;
            }
            if (!IsTCAccepted)
            {
                m_View.DisplayAlert(Constants.AppName, Constants.TCNotAccepted, "Ok");
                return false;
            }
            return true;
        }

        #region Property
        private string phone;
        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        private bool isTCAccepted;
        public bool IsTCAccepted
        {
            get { return isTCAccepted; }
            set { SetProperty(ref isTCAccepted, value); }
        }

        private ImageSource currentOfferImage;
        public ImageSource CurrentOfferImage
        {
            get { return currentOfferImage; }
            set { SetProperty(ref currentOfferImage, value); }
        }


        #endregion

        #region Base
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyName)));
        }

        protected bool SetProperty<T>(ref T storage, T value, bool checkEquality = false, [CallerMemberName]string propertyName = null)
        {
            if (checkEquality && EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }
        public async Task SingleRun(Func<Task> operation)
        {
            object _lock = new object();
            lock (_lock)
            {
                if (CommandInitiated)
                    return;

                CommandInitiated = true;
            }

            try
            {
                await operation();
            }
            catch (Exception e)
            {
                WriteExceptionToConsole(e);
            }
            CommandInitiated = false;
        }
        public async Task<T> LaunchPopup<T>(ContentView view, bool overrideBackButton = false)
        {
            var popup = new InputAlertDialogBase<T>(view, overrideBackButton);

            // Push the page to Navigation Stack
            await PopupNavigation.Instance.PushAsync(popup);

            // await for the user to press Button
            var result = await popup.PageClosedTask;

            // Pop the page from Navigation Stack
            await PopupNavigation.Instance.PopAsync();

            // return user pressed button text value
            return result;
        }

        public void WriteExceptionToConsole(Exception e, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string callerfile = null, [CallerMemberName] string caller = null)
        {
            Debug.WriteLine("Exception thrown in: " + callerfile + " at position- Line No:" + lineNumber + ", Caller Member:" + caller + ", Message:" + e.Message);
            Debug.WriteLine(e.StackTrace);
        }

        public ContentPage m_View { get; set; }
        public bool CommandInitiated { get; set; }
        #endregion
    }
}
