using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toarminas
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ThankYouPopUp : ContentView
    {
        public ThankYouPopUp()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                (this.Parent as InputAlertDialogBase<string>).PageClosedTaskCompletionSource.SetResult("Thank You");
            });
        }
    }
}