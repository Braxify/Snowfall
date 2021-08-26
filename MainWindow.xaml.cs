using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Example2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer MyTimer;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Тик таймера выглядит следующим образом.
        private void MyTimer_Tick(object sender, EventArgs e)
        {
            Update();
        }

        //Обработчик события Loaded будет выглядеть следующим образом.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            GenerateSnowflakes();
            // Подготоваливаем таймер к работа
            MyTimer = new DispatcherTimer();
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Interval = new TimeSpan(100000);

        }
        void GenerateSnowflakes()
        {
            for (int i = 0; i < 20; i++)
            {
                CreateNewSnowflake();
            }
        }

        void PushToView(SnowDrop d)
        {
            viewport.Children.Add(d.model);
        }

        void CreateNewSnowflake()
        {
            var d = new SnowDrop();

            Vector3D v;
            if (drops.Count % 2 == 0)
            {
                v = new Vector3D(-1 + drops.Count * 0.1, 0.5, -1.05);
            }
            else
            {
                v = new Vector3D(-1 + drops.Count * 0.1, 0.63, -1.05);
            }
            d.Transform.OffsetX += v.X;
            d.Transform.OffsetY += v.Y;
            d.Transform.OffsetZ += v.Z;

            drops.Add(d);
            PushToView(d);
        }
        void Update()
        {
            int disappearedSnowflakes = 0;
            int snowflakesCount = drops.Count;
            for (int i = drops.Count - 1; i >= 0; i--)
            {
                var rand = rnd.NextDouble();
                drops[i].Transform.OffsetY -= 0.005 + 0.01 * rand;
                drops[i].Friction += 0.0005 * (0.5 - rand);
                drops[i].Transform.OffsetX += drops[i].Friction;

                if (drops[i].Transform.OffsetY < -0.5)
                {
                    viewport.Children.Remove(drops[i].model);
                    drops.RemoveAt(i);
                    disappearedSnowflakes++;
                }
            }
            if (disappearedSnowflakes == snowflakesCount)
            {
                GenerateSnowflakes();
            }


        }

        List<SnowDrop> drops = new List<SnowDrop>();
        static Random rnd = new Random();
        class SnowDrop
        {

            public ModelVisual3D model = synthMesh();

            public TranslateTransform3D Transform;

            public AxisAngleRotation3D Axis;

            public double Friction;

            public SnowDrop()
            {
                var randAngle = rnd.NextDouble();
                Axis = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 30 * randAngle);
                var tr = new TranslateTransform3D(0, 0, 0);
                var tgr = new Transform3DGroup();
                var rtg = new RotateTransform3D(Axis);

                tgr.Children.Add(rtg);

                Transform = tr;

                tgr.Children.Add(tr);
                model.Transform = tgr;
            }

            static ModelVisual3D synthMesh()
            {
                var pt = new List<Point3D>() {
                    new Point3D(0,0,0),
                    new Point3D(0.2,0,0),
                    new Point3D(0,0.2,0),
                };

                ModelVisual3D model = new ModelVisual3D { };
                var mesh = new MeshGeometry3D
                {
                    Positions = new Point3DCollection(pt),
                };
                mesh.TextureCoordinates.Add(new Point(0, 0));
                mesh.TextureCoordinates.Add(new Point(0, 0.1));
                mesh.TextureCoordinates.Add(new Point(0.1, 0));
                mesh.TextureCoordinates.Add(new Point(0.1, 0.1));



                var mat = new MaterialGroup();

                var snowflake = new ImageBrush();
                snowflake.ImageSource = new BitmapImage(new Uri("snowflake.png", UriKind.Relative));
                mat.Children.Add(new DiffuseMaterial(snowflake));

                model.Content = new GeometryModel3D(mesh, mat);


                return model;
            }
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MyTimer.Start();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            MyTimer.Stop();
        }


    }
}