using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GotScreen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Point firstposition;
        System.Windows.Point secondposition;
        System.Windows.Shapes.Rectangle rectangle;
        Bitmap img;

        public MainWindow()
        {
            InitializeComponent();
        }
        void OnMouseLeftButtonDownHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (rectangle == null)
            {
                System.Windows.Point position = Mouse.GetPosition(this);

                position.X *= GetWindowsScaling();
                position.Y *= GetWindowsScaling();

                SolidColorBrush mySolidColorBrush = new()
                {
                    Color = System.Windows.Media.Color.FromArgb(255, 255, 255, 0)
                };

                Ellipse myEllipse = new()
                {
                    Fill = mySolidColorBrush,
                    StrokeThickness = 2,
                    Stroke = System.Windows.Media.Brushes.White,
                    Width = 1,
                    Height = 1
                };

                Canvas.SetTop(myEllipse, position.Y);
                Canvas.SetLeft(myEllipse, position.X);
                canvas.Children.Add(myEllipse);

                if (firstposition.X == 0 && firstposition.X == 0)
                {
                    firstposition = position;
                }
                else
                {
                    secondposition = position;

                    mySolidColorBrush = new SolidColorBrush
                    {
                        Color = System.Windows.Media.Color.FromArgb(255, 255, 255, 0)
                    };

                    System.Windows.Shapes.Rectangle temprectangle = new()
                    {
                        Fill = mySolidColorBrush,
                        Opacity = 0.2,
                        Width = Math.Abs(firstposition.X - secondposition.X),
                        Height = Math.Abs(firstposition.Y - secondposition.Y),
                    };
                    rectangle = temprectangle;

                    Canvas.SetTop(rectangle, (firstposition.Y < secondposition.Y) ? firstposition.Y : secondposition.Y);
                    Canvas.SetLeft(rectangle, (firstposition.X < secondposition.X) ? firstposition.X : secondposition.X);
                    canvas.Children.Add(rectangle);


                    Bitmap cutImg = Cut(img, (int)firstposition.X, (int)firstposition.Y, (int)secondposition.X, (int)secondposition.Y);
                    cutImg.Save("cut.jpg", ImageFormat.Jpeg);
                    System.Windows.Clipboard.SetImage(new BitmapImage(new Uri(@"C:\Users\User\source\repos\Other\GotScreen\GotScreen\bin\Debug\net7.0-windows\cut.jpg")));
                    this.Close();
                }
            }
        }

        // raised when mouse cursor leaves the area occupied by the element
        void OnMouseRightButtonDownHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            canvas.Children.Clear();
            firstposition = default;
            secondposition = default;
            rectangle = null;
        }
        public Bitmap Cut(Bitmap bmp, int x1, int y1, int x2, int y2)
        {
            var img = bmp;

            if (x2 < x1)
            {
                (x1, x2) = (x2, x1);
            }
            if (y2 < y1)
            {
                (y1, y2) = (y2, y1);
            }
            int width = x2 - x1 + 1;
            int height = y2 - y1 + 1;

            var result = new Bitmap(width, height);

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    result.SetPixel(i - x1, j - y1, img.GetPixel(i, j));
                }
            }
            return result;
        }
        public Bitmap GetBitmap()
        {
            System.Drawing.Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
            }
            bitmap.Save("test.jpg", ImageFormat.Jpeg);
            return bitmap;
        }

        private void TakeScreenshotHandler(object sender, RoutedEventArgs e)
        {
            Bitmap bitmap = GetBitmap();
            img = bitmap;

            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            canvas.Height = bitmap.Height;
            canvas.Width = bitmap.Width;
            //Bitmap cropbitmap = bitmap.Clone(cropArea, bitmap.PixelFormat);
            System.Windows.Controls.Image BodyImage = new()
            {
                Width = bitmap.Width / GetWindowsScaling(),
                Height = bitmap.Height / GetWindowsScaling(),
                Source = new BitmapImage(new Uri(@"C:\Users\User\source\repos\Other\GotScreen\GotScreen\bin\Debug\net7.0-windows\test.jpg")),
            };
            Canvas.SetTop(BodyImage, 0);
            Canvas.SetLeft(BodyImage, 0);
            canvas.Children.Add(BodyImage);
        }
        public static double GetWindowsScaling()
        {
            return Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth;
        }
    }
}
