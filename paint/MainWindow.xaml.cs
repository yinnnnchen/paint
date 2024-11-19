using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start = new Point { X = 0, Y = 0 };
        Point dest = new Point { X = 0, Y = 0 };
        Color strokeColor = Colors.Red;
        Color fillColor = Colors.Aqua;
        int strokeThickness = 1;
        string shapeType = "line";
        string actionType = "draw";

        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        private void MyCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            if (actionType == "erase") myCanvas.Cursor = Cursors.Hand;
            else myCanvas.Cursor = Cursors.Pen;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myCanvas.Cursor = Cursors.Cross;
            start = e.GetPosition(myCanvas);

            if (actionType == "draw")
            {
                switch (shapeType)
                {
                    case "line":
                        Line line = new Line
                        {
                            X1 = start.X,
                            Y1 = start.Y,
                            X2 = dest.X,
                            Y2 = dest.Y,
                            StrokeThickness = 1,
                            Stroke = Brushes.Gray
                        };
                        myCanvas.Children.Add(line);
                        break;

                    case "rectangle":
                        Rectangle rect = new Rectangle
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        myCanvas.Children.Add(rect);
                        rect.SetValue(Canvas.LeftProperty, start.X);
                        rect.SetValue(Canvas.TopProperty, start.Y);
                        break;

                    case "ellipse":
                        Ellipse ellipse = new Ellipse
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        myCanvas.Children.Add(ellipse);
                        ellipse.SetValue(Canvas.LeftProperty, start.X);
                        ellipse.SetValue(Canvas.TopProperty, start.Y);
                        break;

                    case "polyline":
                        Polyline polyline = new Polyline
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        myCanvas.Children.Add(polyline);
                        break;
                }
            }

            DisplayStatus();
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            dest = e.GetPosition(myCanvas);

            switch (actionType)
            {
                case "draw":
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point origin;
                        origin.X = Math.Min(start.X, dest.X);
                        origin.Y = Math.Min(start.Y, dest.Y);
                        double width = Math.Abs(start.X - dest.X);
                        double height = Math.Abs(start.Y - dest.Y);

                        switch (shapeType)
                        {
                            case "line":
                                var line = myCanvas.Children.OfType<Line>().LastOrDefault();
                                line.X2 = dest.X;
                                line.Y2 = dest.Y;
                                break;

                            case "rectangle":
                                var rect = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                                rect.Width = width;
                                rect.Height = height;
                                rect.SetValue(Canvas.LeftProperty, origin.X);
                                rect.SetValue(Canvas.TopProperty, origin.Y);
                                break;

                            case "ellipse":
                                var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                                ellipse.Width = width;
                                ellipse.Height = height;
                                ellipse.SetValue(Canvas.LeftProperty, origin.X);
                                ellipse.SetValue(Canvas.TopProperty, origin.Y);
                                break;

                            case "polyline":
                                var polyline = myCanvas.Children.OfType<Polyline>().LastOrDefault();
                                polyline.Points.Add(dest);
                                break;
                        }
                    }
                    break;

                case "erase":
                    var shape = e.OriginalSource as Shape;
                    myCanvas.Children.Remove(shape);
                    if (myCanvas.Children.Count == 0)
                        myCanvas.Cursor = Cursors.Arrow;
                    break;
            }

            DisplayStatus();
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Brush strokeBrush = new SolidColorBrush(strokeColor);
            Brush fillBrush = new SolidColorBrush(fillColor);

            switch (actionType)
            {
                case "draw":
                    switch (shapeType)
                    {
                        case "line":
                            var line = myCanvas.Children.OfType<Line>().LastOrDefault();
                            line.Stroke = strokeBrush;
                            line.StrokeThickness = strokeThickness;
                            break;

                        case "rectangle":
                            var rect = myCanvas.Children.OfType<Rectangle>().LastOrDefault();
                            rect.Stroke = strokeBrush;
                            rect.Fill = fillBrush;
                            rect.StrokeThickness = strokeThickness;
                            break;

                        case "ellipse":
                            var ellipse = myCanvas.Children.OfType<Ellipse>().LastOrDefault();
                            ellipse.Stroke = strokeBrush;
                            ellipse.Fill = fillBrush;
                            ellipse.StrokeThickness = strokeThickness;
                            break;

                        case "polyline":
                            var polyline = myCanvas.Children.OfType<Polyline>().LastOrDefault();
                            polyline.Stroke = strokeBrush;
                            polyline.Fill = fillBrush;
                            polyline.StrokeThickness = strokeThickness;
                            break;
                    }
                    break;

                case "erase":
                    break;
            }
        }

        private void DisplayStatus()
        {
            pointLabel.Content = $"({Convert.ToInt32(start.X)}, {Convert.ToInt32(start.Y)}) -  ({Convert.ToInt32(dest.X)}, {Convert.ToInt32(dest.Y)})";
            shapeLabel.Content = shapeType;
            int lineCount = myCanvas.Children.OfType<Line>().Count();
            int rectCount = myCanvas.Children.OfType<Rectangle>().Count();
            int ellipseCount = myCanvas.Children.OfType<Ellipse>().Count();
            int polylineCount = myCanvas.Children.OfType<Polyline>().Count();

            statusLabel.Content = $"工作模式：{actionType}, Line:{lineCount}, Rectangle:{rectCount}, Ellipse:{ellipseCount}, Polyline:{polylineCount}";
        }

        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = Convert.ToInt32(strokeThicknessSlider.Value);
        }

        private void ShapeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            shapeType = targetRadioButton.Tag.ToString();
            actionType = "draw";
            DisplayStatus();
        }

        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = strokeColorPicker.SelectedColor.Value;
        }

        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = fillColorPicker.SelectedColor.Value;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();
            DisplayStatus();
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "erase";
            if (myCanvas.Children.Count > 0)
            {
                myCanvas.Cursor = Cursors.Hand;
            }
            DisplayStatus();
        }
    }
}