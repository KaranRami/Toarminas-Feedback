using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toarminas.DependencyInterface;
using Toarminas.Droid.ServiceHelper;
using Toarminas.Model;

[assembly: Xamarin.Forms.Dependency(typeof(ServiceAPI))]
namespace Toarminas.Droid.ServiceHelper
{
    public class ServiceAPI : IServiceAPI
    {
        CustomerSerive.CustomerSerive service = new CustomerSerive.CustomerSerive();
        public void AddCustomerDetail(ContactModel contact)
        {
            Task.Run(() =>
            {
                try
                {
                    service.addCustomerDetails(contact.Email, contact.Phone, contact.SubScriptionTime);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            });
        }
        public string[] GetImages()
        {
            return service.getImages();
        }
    }
}