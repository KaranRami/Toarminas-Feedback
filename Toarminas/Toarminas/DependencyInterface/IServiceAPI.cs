using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Toarminas.Model;

namespace Toarminas.DependencyInterface
{
    public interface IServiceAPI
    {
        void AddCustomerDetail(ContactModel contact);
        string[] GetImages();
    }
}
