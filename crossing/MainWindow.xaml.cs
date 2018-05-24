using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace crossing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int car_id = 0;
        public ObservableCollection<Car> cars = new ObservableCollection<Car>();
        public Policeman police;
        Thread policeThread;
        public delegate void RegisterNameDel();
        public RegisterNameDel myDelegate;

        private object _carCollectionLock = new object();


        public MainWindow()
        {
            InitializeComponent();
            police = new Policeman(cars, _carCollectionLock);
            policeThread = new Thread(new ThreadStart(police.Observe));
            policeThread.Start();
            BindingOperations.EnableCollectionSynchronization(cars, _carCollectionLock);
            Console.WriteLine("Wypisuje do konsoli");

            NameScope.SetNameScope(this, new NameScope());
            Content = containerCanvas;
        }


        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            //Thread carThread = new Thread(() => createCar(this));
            //carThread.SetApartmentState(ApartmentState.STA);
            //carThread.Start();
            Thread carThread = new Thread(createCar);
            carThread.SetApartmentState(ApartmentState.STA);
            carThread.Start();
        }

        public void createCar()
        {
           // lock (mainWin._carCollectionLock)
           // {
                cars.Add(new Car(298, 0, 100, 'u', car_id, _carCollectionLock, this));
                cars[car_id].DriveCar(this);
                containerCanvas.Children.Add(cars[car_id++].path);
                Console.WriteLine("ID aktualnego wątku: " + Thread.CurrentThread.ManagedThreadId);
         //   }
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            lock (_carCollectionLock)
            {
                cars.Add(new Car(700, 298, 100, 'l', car_id, _carCollectionLock, this));
                cars[car_id].DriveCar(this);
                containerCanvas.Children.Add(cars[car_id++].path);
            }
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            lock (_carCollectionLock)
            {
                cars.Add(new Car(367, 700, 100, 'd', car_id, _carCollectionLock, this));
                cars[car_id].DriveCar(this);
                containerCanvas.Children.Add(cars[car_id++].path);
            }
        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            lock (_carCollectionLock)
            {
                cars.Add(new Car(0, 367, 100, 'r', car_id, _carCollectionLock, this));
                cars[car_id].DriveCar(this);
                containerCanvas.Children.Add(cars[car_id++].path);
            }
        }
    }
}
