using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;

namespace crossing
{
    public class Policeman
    {
       // public ObservableCollection<Car> carsList = new ObservableCollection<Car>();
        public List<Car> carsList = new List<Car>();
        private object _carCollectionLock = new object();
        public static Mutex mut = new Mutex();
        MainWindow mainw;

        //public Policeman(ObservableCollection<Car> _cars, object _lock)
        //public Policeman(List<Car> _cars, object _lock)
        public Policeman(List<Car> _cars, Mutex _mut, MainWindow mw)

        {
            carsList = _cars;
            mainw = mw;
            mut = _mut;
        }

        //public void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        //    {
        //        // Now show the NEW items that were inserted.
        //        Console.WriteLine("The new car has started at the location:");
        //        foreach (Car p in e.NewItems)
        //        {
        //            Console.WriteLine(p.myRect.Location);
        //        }
        //    }

        //    foreach (Car c in carsList)
        //    {
        //        Console.WriteLine("Current position of car {0} is {1}: ", c.id, c.myRectangleGeometry.Rect.Location);
        //        // c.myRectAnimation.SpeedRatio += 3;    
        //        //   c.myControllableClock.Controller.SpeedRatio += 3;
        //       // c.myRectAnimation.SpeedRatio +=3;
        //        c.driveStoryboard.SpeedRatio += 3;
        //        Console.WriteLine("Nowa predkosc samochodu {0} to {1}", c.id, c.myRectAnimation.SpeedRatio/*, c.myControllableClock.Controller.SpeedRatio*/);
        //    }
        //}

        public void Observe()
        {
            BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += worker_DoWork;
                    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                    worker.RunWorkerAsync(carsList);
                    worker.WorkerSupportsCancellation = true;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            List<Car> arg = (List<Car>)e.Argument;
            while (!worker.CancellationPending)
            {
                for (int i = 0; i < carsList.Count; i++)
                {
                    mainw.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                        delegate ()
                        {
                            for (int j = arg.Count - 1; j >= 0; j--)
                            {
                                if (arg[j].HasFinished == true)
                                {
                                    arg.RemoveAt(j);
                                    Console.WriteLine("Usunieto samochod na pozycji " + j);
                                    mainw.car_id--;
                                }
                            }

                        // Console.WriteLine("Current position of car {0} is {1}: ", i, arg[i].myRectangleGeometry.Rect.Location);
                        if (arg.All(n => n.HasFinished == true))
                            {
                                worker.CancelAsync();
                            }
                        }));
                }

                Thread.Sleep(500);

            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("All cars arrived at the destination.");
        }


    }
}
