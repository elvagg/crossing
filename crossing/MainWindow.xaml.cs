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
        public int car_id = 0;
        // public ObservableCollection<Car> cars = new ObservableCollection<Car>();
        public List<Car> cars = new List<Car>();
        public delegate void UpdateCanvas();
        public Policeman police;
        Thread policeThread;
        public delegate void RegisterNameDel(); //First, a delegate is created that takes no arguments
        public RegisterNameDel myDelegate;
        public static Mutex mut = new Mutex();
        private object _carCollectionLock = new object();


        public MainWindow()
        {
            InitializeComponent();
            //myDelegate = new RegisterNameDel(RegisterNameMethod);
          ///  police = new Policeman(cars, _carCollectionLock);
            police = new Policeman(cars, mut, this);
            police.Observe();
            ///  
           // policeThread = new Thread(new ThreadStart(police.Observe));
            //policeThread.Start();
            BindingOperations.EnableCollectionSynchronization(cars, _carCollectionLock);
            Console.WriteLine("Wypisuje do konsoli");

            NameScope.SetNameScope(this, new NameScope());
            Content = containerCanvas;
        }

        //public void RegisterNameMethod()
        //{
        //    lock (carLock)
        //    {
        //        // Assign the geometry a name so that
        //        // it can be targeted by a Storyboard.
        //        this.RegisterName(
        //            "RectGeometryNr" + id.ToString(), myRectangleGeometry);
        //    }
        //}


        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            lock (_carCollectionLock)
            {
                createCar('u');
            }
        }

        public double CountSpeed(char dir)
        {
            double current_max_speed = 100.0, speed = 0.0, s=0.0;

            if (cars.Count == 0) current_max_speed = 5.0;
            else
            {
                foreach (Car c in cars)
                {
                    switch(c.dir)
                    {
                        case 'l':
                            s = Math.Abs(410 - c.myRectangleGeometry.Rect.Location.X);
                            break;
                        case 'r':
                            s = Math.Abs(-280 + c.myRectangleGeometry.Rect.Location.X); 
                            break;
                        case 'u':
                            s = Math.Abs(400 - c.myRectangleGeometry.Rect.Location.Y);
                            break;
                        case 'd':
                            s = Math.Abs(-280 + c.myRectangleGeometry.Rect.Location.Y);
                            break;
                    }

                    Console.WriteLine("Wartosc predkosci auta {0} wynosi {1}", c.id, c.myRectAnimation.SpeedRatio);
                    if(s > 0)
                    {
                        if (dir == 'l' || dir == 'r')
                            speed = 250 * c.myRectAnimation.SpeedRatio / s;
                        else
                            speed = 250 * c.myRectAnimation.SpeedRatio / s;
                    }


                    if (current_max_speed > speed && speed != 0.0)
                        current_max_speed = speed;
                }

                Console.WriteLine("Obliczony speed dla nowego auta: "+ speed);
                    
                } 
            return current_max_speed;
        }


        public void createCar(char dir)
        {
            //lock (_carCollectionLock)
            //{
            mut.WaitOne();
            double max_speed = CountSpeed(dir);
            mut.ReleaseMutex();

            mut.WaitOne();
            Console.WriteLine("ID aktualnego wątku: " + Thread.CurrentThread.ManagedThreadId);
                switch (dir)
                {
                    case 'l':
                         cars.Add(new Car(0, 367, max_speed, dir, car_id, _carCollectionLock, this));
                        break;
                    case 'r':
                         cars.Add(new Car(700, 298, max_speed, dir, car_id, _carCollectionLock, this));
                         break;
                    case 'u':
                        cars.Add(new Car(298, 0, max_speed, dir, car_id, _carCollectionLock, this));
                        break;
                    case 'd':
                        cars.Add(new Car(367, 700, max_speed, dir , car_id, _carCollectionLock, this));
                        break;
                }
                cars[car_id].DriveCar(this, containerCanvas); 
            
                containerCanvas.Children.Add(cars[car_id++].path);
            mut.ReleaseMutex();
            Console.WriteLine("Predkosc nowego samochodu wynosi: " + max_speed);
           // }
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
         createCar('r');
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
             createCar('d');

        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            createCar('l');
        }
    }
}
