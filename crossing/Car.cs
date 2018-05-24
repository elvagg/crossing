using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Threading;

namespace crossing
{
    public class Car : INotifyPropertyChanged
    {
        MainWindow mainWin;
        public RectangleGeometry myRectangleGeometry = new RectangleGeometry();
        Storyboard driveStoryboard = new Storyboard();
        public Rect myRect = new Rect();
        public Path path = new Path();
        public int velo;
        public char dir;
        public int id;
        public bool HasFinished;
        public event PropertyChangedEventHandler PropertyChanged;
        private object carLock = new object();


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(String id)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(id));
            }
        }

        public Car(int _pos_x, int _pos_y, int _velo, char _dir, int _id, object _lock, MainWindow win)
        {
            mainWin = win;
            HasFinished = false;
            myRect.X = _pos_x;
            myRect.Y = _pos_y;
            if (_dir == 'l' || _dir == 'r')
            {
                myRect.Width = 50;
                myRect.Height = 25;
            } else
            {
                myRect.Width = 25;
                myRect.Width = 50;
            }
            myRectangleGeometry.Rect = myRect;
            Console.WriteLine("Pozycja samochodu {0}:", myRect.Location);
            carLock = _lock;
            
            velo = _velo;
            dir = _dir;
            id = _id;
        }

        public void DriveCar(MainWindow mainWin)
        {

            path.Fill = Brushes.LemonChiffon;
            path.StrokeThickness = 1;
            path.Stroke = Brushes.Black;
            path.Data = this.myRectangleGeometry;

            RectAnimation myRectAnimation = new RectAnimation();
            myRectAnimation.Duration = TimeSpan.FromSeconds(10);
            myRectAnimation.FillBehavior = FillBehavior.HoldEnd;

            // Set the From and To properties of the animation.
            switch(dir)
            {
                case 'l':
                    myRectAnimation.From = new Rect(myRect.X, myRect.Y, 50, 25);
                    myRectAnimation.To = new Rect(-50, myRect.Y, 50, 25);
                    break;
                case 'r':
                    myRectAnimation.From = new Rect(myRect.X, myRect.Y, 50, 25);
                    myRectAnimation.To = new Rect(1000, myRect.Y, 50, 25);
                    break;
                case 'u':
                    myRectAnimation.From = new Rect(myRect.X, myRect.Y, 25, 50);
                    myRectAnimation.To = new Rect(myRect.X, 1000, 25, 50);
                    break;
                case 'd':
                    myRectAnimation.From = new Rect(myRect.X, myRect.Y, 25, 50);
                    myRectAnimation.To = new Rect(myRect.X, -50, 25, 50);
                    break;                    
            }


            // Set the animation to target the Rect property
            // of the object named "MyAnimatedRectangleGeometry."
            Storyboard.SetTargetName(myRectAnimation, "RectGeometryNr" + id.ToString());
            Storyboard.SetTargetProperty(
                myRectAnimation, new PropertyPath(RectangleGeometry.RectProperty));

            // Create a storyboard to apply the animation.
            
            driveStoryboard.Children.Add(myRectAnimation);

            // Start the storyboard when the Path loads.
            path.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                driveStoryboard.Begin(mainWin);
            };

            mainWin.Invoke(mainWin.myDelegate);

            lock (carLock)
            {
                // Assign the geometry a name so that
                // it can be targeted by a Storyboard.
                mainWin.RegisterName(
                    "RectGeometryNr" + id.ToString(), myRectangleGeometry);
            }


            driveStoryboard.Completed += new EventHandler(Story_Completed);
            Console.WriteLine("ID aktualnego wątku: " + Thread.CurrentThread.ManagedThreadId);
        }

        private void Story_Completed(object sender, EventArgs e)
        {
            // tutaj nalezy dodać usuwanie animacji
            HasFinished = true;
        }
    }
}
