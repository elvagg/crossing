using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;

namespace crossing
{
    public class Policeman
    {
        public ObservableCollection<Car> carsList = new ObservableCollection<Car>();
        private object _carCollectionLock = new object();

        public Policeman(ObservableCollection<Car> _cars, object _lock)
        {
            carsList = _cars;
            _carCollectionLock = _lock;
            carsList.CollectionChanged += HandleChange;
        }

        public void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                // Now show the NEW items that were inserted.
                Console.WriteLine("The new car has started at the location:");
                foreach (Car p in e.NewItems)
                {
                    Console.WriteLine(p.myRect.Location);
                }
            }

            foreach (Car c in carsList)
            {
                Console.WriteLine("Current position is: " + c.myRectangleGeometry.Rect.Location);
            }
        }

        public void Observe()
        {
            Console.WriteLine("ID aktualnego wątku: " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
