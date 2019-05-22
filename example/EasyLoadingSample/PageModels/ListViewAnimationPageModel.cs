using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace EasyLoadingSample.PageModels
{
    class ListViewAnimationPageModel : FreshMvvm.FreshBasePageModel
    {
        public ObservableCollection<Order> Orders { get; set; }
        public bool IsLoading { get; set; }
        public ICommand ToggleLoadingCommand { get; set; }

        public ListViewAnimationPageModel()
        {
            ToggleLoadingCommand = new Command(async (x) => {
                IsLoading = true;
                await Task.Delay(3000);
                IsLoading = false;
              
               
            });

            Orders = new ObservableCollection<Order>
            {
                new Order
                {
                    AmountOfBoxes = 3,
                    OrderNumber = 1072191,
                    AmountOfProducts = 92,
                    DeliveryDate = DateTime.Now.AddDays(30),
                    Price = 389.29m
                },
                new Order
                {
                    AmountOfBoxes = 62,
                    OrderNumber = 664362,
                    AmountOfProducts = 56,
                    DeliveryDate = DateTime.Now.AddDays(23),
                    Price = 430.31m
                },
                new Order
                {
                    AmountOfBoxes = 96,
                    OrderNumber = 329953,
                    AmountOfProducts = 39,
                    DeliveryDate = DateTime.Now.AddDays(12),
                    Price = 59.24m
                }
            };
        }
    }

  
}
