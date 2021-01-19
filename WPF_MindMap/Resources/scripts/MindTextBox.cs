using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Shapes;
using System.Diagnostics;

namespace WPF_MindMap.Resources.scripts
{
    public class MindTextBox : TextBox
    {
        Canvas canvas;
        private List<MindTextBox> childBoxs = null;
        private MindTextBox parentBox = null;
        private List<Line> child_lines = new List<Line>();
        private Line parent_line;

        private bool isRoot = false;
        private bool _isMoving;

        private Point? _buttonPosition;

        private double deltaX;
        private double deltaY;
        private TranslateTransform _currentTT;

        private int child_count;



        public MindTextBox(Canvas canvas, string txt = "", bool isRoot = false)
        {
            this.VerticalAlignment = VerticalAlignment.Bottom;
            this.HorizontalAlignment = HorizontalAlignment.Right;
            
            this.canvas = canvas;
            childBoxs = new List<MindTextBox>();
            this.isRoot = isRoot;
            
            TextBoxInit(txt);

        }

        private void TextBoxInit(string txt)
        {
            
            //           m_textBox.Margin = new Thickness(_pointX,_pointY,0,0);
            
            this.Background = Brushes.Yellow;

            this.VerticalContentAlignment = VerticalAlignment.Center;

            this.Text = txt;
            this.TextAlignment = TextAlignment.Center;
            this.TextWrapping = TextWrapping.Wrap;
            this.FontSize = 20;
            this.Width = 150;
            this.Height = 75;

            this.Foreground = Brushes.Black;
            this.KeyDown += OnKeyDownHandler;
            this.PreviewMouseLeftButtonDown += OnPreviewMouseDown;
            this.PreviewMouseMove += OnPreviewMouseMove;
            this.PreviewMouseLeftButtonUp += OnPreviewMouseUp;
        }

        public void SetActualPosition(double left,double top)
        {
            //Debug.WriteLine(left);
            //Debug.WriteLine(top);
            
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            Canvas.SetZIndex(this, 1);

        }

        private Line DrawingLine(Point _point1,Point _point2,double thickness = 1)
        {
            Line line = new Line();

            //Debug.WriteLine("draw_point :" + _point1);
            //Debug.WriteLine("draw_point :" + _point2);

            line.X1 = _point1.X + (this.Width / 2);
            line.Y1 = _point1.Y + (this.Height / 2);
            line.X2 = _point2.X + (this.Width / 2);
            line.Y2 = _point2.Y + (this.Height / 2);

            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;
            line.StrokeThickness = thickness;
            line.Stroke = blackBrush;

            return line;
        }
        private void Connected_Child_Line_Update(double thickness = 0.2)
        {
            //1. remove parent line
            if (this.child_lines.Count == 0)
                return;

            foreach(var child_line in (this.child_lines))
            {
                canvas.Children.Remove(child_line);

            }
            foreach(MindTextBox child_box in this.childBoxs)
            {
                child_box.parent_line = null;
                Line line = DrawingLine(this.TranslatePoint(new Point(0, 0), canvas)
    , child_box.TranslatePoint(new Point(0, 0), canvas), thickness);
                child_box.parent_line = line;
                this.child_lines.Add(line);
                canvas.Children.Add(line);
            }

        }
        private void Connected_Parent_Line_Update(double thickness = 0.2)
        {
            if (this.parentBox == null)
            {
                Debug.WriteLine("parentLine null");
                return;

            }
            //1. remove parent line
            canvas.Children.Remove(this.parent_line);
            this.parentBox.child_lines.Remove(this.parent_line);

            //2. new draw

            Line line = DrawingLine(this.parentBox.TranslatePoint(new Point(0,0),canvas)
                , this.TranslatePoint(new Point(0, 0), canvas), thickness);

            Canvas.SetZIndex(line, 0);
            canvas.Children.Add(line);

            this.parent_line = line;
            this.parentBox.child_lines.Add(line);


        }

        private void CreateChild()
        {
            if (this.child_count < Utils.MAX_CHILD_COUNT)
            {
                child_count++;
                //MindTextBox box = new MindTextBox(canvas,Canvas.GetLeft(this)+ Utils.ROOTS_CHILD_POINT_VALUE[child_count - 1].X, Canvas.GetTop(this) + Utils.ROOTS_CHILD_POINT_VALUE[child_count - 1].Y);
                MindTextBox box = new MindTextBox(canvas);
                box.parentBox = this;
                this.childBoxs.Add(box);
                canvas.Children.Add(box);
                box.SetActualPosition(this.TranslatePoint(new Point(0, 0), canvas).X + Utils.ROOTS_CHILD_POINT_VALUE[this.child_count - 1].X,
                    this.TranslatePoint(new Point(0, 0), canvas).Y + Utils.ROOTS_CHILD_POINT_VALUE[this.child_count - 1].Y);

                Line line = DrawingLine(this.TranslatePoint(new Point(0, 0), canvas)
                    , new Point(Canvas.GetLeft(box),Canvas.GetTop(box)));
                //line setting
                canvas.Children.Add(line);
                Canvas.SetZIndex(line, 0);
                Canvas.SetZIndex(box, 1);

                //set parent line of new box
                box.parent_line = line;
                //set child line of this
                this.child_lines.Add(line);
                
            }
            else
            {
                MessageBox.Show("7개 이상 생성이 불가합니다.");
            }
        }

        private void CreateBrother()
        {
            if (isRoot)
            {
                CreateChild();
            }
            else
            {
                if (this.parentBox.child_count < Utils.MAX_CHILD_COUNT)
                {

                    this.parentBox.child_count++;
                    MindTextBox box = new MindTextBox(canvas);
//                    ,this.parentBox.Margin.Left + Utils.ROOTS_CHILD_POINT_VALUE[this.parentBox.child_count - 1].X, this.parentBox.Margin.Top + Utils.ROOTS_CHILD_POINT_VALUE[this.parentBox.child_count - 1].Y

                    box.parentBox = this.parentBox;
                    this.parentBox.childBoxs.Add(box);
                    canvas.Children.Add(box);
                    
                    box.SetActualPosition(Canvas.GetLeft(this.parentBox) + Utils.ROOTS_CHILD_POINT_VALUE[this.parentBox.child_count - 1].X, Canvas.GetTop(this.parentBox) + Utils.ROOTS_CHILD_POINT_VALUE[this.parentBox.child_count - 1].Y);

                    Line line = DrawingLine(box.parentBox.TranslatePoint(new Point(0, 0), canvas),
                        new Point(Canvas.GetLeft(box), Canvas.GetTop(box)));
                    //line setting
                    canvas.Children.Add(line);
                    Canvas.SetZIndex(line, 0);
                    Canvas.SetZIndex(box, 1);

                    //set parent line of new box
                    box.parent_line = line;
                    //set child line of this
                    this.child_lines.Add(line);

                }
                else
                {
                    MessageBox.Show("7개 이상 생성이 불가합니다.");
                }
            }
        }
        

        private void OnKeyDownHandler(Object sender, KeyEventArgs args)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && args.Key == Key.Enter)
            {
                this.BorderThickness = new Thickness(3,3,3,3);
//                m_textBox.IsReadOnly = true;
                if (!this.Text.Equals(""))
                {

                    CreateChild();
                }
            }else if(args.Key == Key.Enter)
            {
                this.BorderThickness = new Thickness(3, 3, 3, 3);
//                m_textBox.IsReadOnly = true;
                if (!this.Text.Equals(""))
                {
                    CreateBrother();
                }
            }
        }



        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_buttonPosition == null)
                _buttonPosition = this.TransformToAncestor((Canvas)this.Parent).Transform(new Point(0, 0));
            var mousePosition = Mouse.GetPosition((Canvas)this.Parent);
            deltaX = mousePosition.X - _buttonPosition.Value.X;
            deltaY = mousePosition.Y - _buttonPosition.Value.Y;
            _isMoving = true;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _currentTT = this.RenderTransform as TranslateTransform;
            _isMoving = false;

            Connected_Parent_Line_Update(1);
            Connected_Child_Line_Update(1);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;

            var mousePoint = Mouse.GetPosition((Canvas)this.Parent);

            var offsetX = (_currentTT == null ? _buttonPosition.Value.X : _buttonPosition.Value.X - _currentTT.X) + deltaX - mousePoint.X;
            var offsetY = (_currentTT == null ? _buttonPosition.Value.Y : _buttonPosition.Value.Y - _currentTT.Y) + deltaY - mousePoint.Y;

            this.RenderTransform = new TranslateTransform(-offsetX, -offsetY);
           


            
            Connected_Parent_Line_Update();
            Connected_Child_Line_Update();
        }



    }
}
