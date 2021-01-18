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

namespace WPF_MindMap.Resources.scripts
{
    public class MindTextBox : StackPanel
    {
        private List<MindTextBox> childBoxs = null;
        private MindTextBox parentBox = null;
        Line _line;

        private bool isRoot = false;
        private bool _isMoving;

        private TextBox m_textBox;
        private Point? _buttonPosition;

        private double deltaX;
        private double deltaY;
        private TranslateTransform _currentTT;

        private int child_count;



        public MindTextBox(double _pointX = 0, double _pointY = 0, string txt = "", bool isRoot = false)
        {
            childBoxs = new List<MindTextBox>();
            this.isRoot = isRoot;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            
            TextBoxInit(_pointX, _pointY, txt);

            this.Children.Add(m_textBox);
        }

        private void TextBoxInit(double _pointX, double _pointY,string txt)
        {
            m_textBox = new TextBox();
            m_textBox.Margin = new Thickness(_pointX,_pointY,0,0);
            m_textBox.Background = Brushes.Yellow;

            m_textBox.VerticalContentAlignment = VerticalAlignment.Center;

            m_textBox.Text = txt;
            m_textBox.TextAlignment = TextAlignment.Center;
            m_textBox.TextWrapping = TextWrapping.Wrap;
            m_textBox.FontSize = 20;
            m_textBox.Width = 150;
            m_textBox.Height = 75;


            m_textBox.Foreground = Brushes.Black;
            m_textBox.KeyDown += OnKeyDownHandler;
            m_textBox.PreviewMouseLeftButtonDown += OnPreviewMouseDown;
            m_textBox.PreviewMouseMove += OnPreviewMouseMove;
            m_textBox.PreviewMouseLeftButtonUp += OnPreviewMouseUp;
        }

        private void CreateChild()
        {
            Point _pos = m_textBox.TransformToAncestor((Grid)this.Parent).Transform(new Point(0, 0));
            if (this.child_count < Utils.MAX_CHILD_COUNT)
            {
                child_count++;
                Grid draw_panel = (Grid)this.Parent;
                MindTextBox box = new MindTextBox(this.m_textBox.Margin.Left + Utils.ROOTS_CHILD_POINT_VALUE[child_count - 1].X, this.m_textBox.Margin.Top + Utils.ROOTS_CHILD_POINT_VALUE[child_count - 1].Y);
                box.parentBox = this;
                this.childBoxs.Add(box);

                draw_panel.Children.Add(box);

                /*                box.Focusable = true;
                                box.Focus();*/

                _line = new Line();
                Point _point1 = this.m_textBox.TransformToAncestor(this)
                          .Transform(new Point(0, 0));
                Point _point2 = box.m_textBox.TransformToAncestor(this)
                          .Transform(new Point(0, 0));

                _line.X1 = _point1.X;
                _line.Y1 = _point1.Y;
                _line.X2 = _point1.X + Utils.ROOTS_CHILD_POINT_VALUE[child_count-1].X;
                _line.Y2 = _point1.Y + Utils.ROOTS_CHILD_POINT_VALUE[child_count - 1].Y;
                System.Diagnostics.Debug.WriteLine(_line.X1);
                System.Diagnostics.Debug.WriteLine(_line.X2);

                SolidColorBrush blackBrush = new SolidColorBrush();
                blackBrush.Color = Colors.Black;
                _line.StrokeThickness = 3;
                _line.Stroke = blackBrush;
                draw_panel.Children.Add(_line);

                System.Diagnostics.Debug.WriteLine("DRAW LINE");
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
                Point _pos = this.parentBox.m_textBox.TransformToAncestor((Grid)this.Parent).Transform(new Point(0, 0));
                if (this.parentBox.child_count < Utils.MAX_CHILD_COUNT)
                {

                    this.parentBox.child_count++;
                    Grid draw_panel = (Grid)this.Parent;
                    MindTextBox box = new MindTextBox(this.parentBox.m_textBox.Margin.Left + Utils.ROOTS_CHILD_POINT_VALUE[this.parentBox.child_count - 1].X, this.parentBox.m_textBox.Margin.Top + Utils.ROOTS_CHILD_POINT_VALUE[this.parentBox.child_count - 1].Y);
                    
                    box.parentBox = this.parentBox;
                    draw_panel.Children.Add(box);

                    box.Focusable = true;
                    box.Focus();
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
                m_textBox.BorderThickness = new Thickness(3,3,3,3);
//                m_textBox.IsReadOnly = true;
                if (!m_textBox.Text.Equals(""))
                {

                    CreateChild();
                }
            }else if(args.Key == Key.Enter)
            {
                m_textBox.BorderThickness = new Thickness(3, 3, 3, 3);
//                m_textBox.IsReadOnly = true;
                if (!m_textBox.Text.Equals(""))
                {
                    CreateBrother();
                }
            }
        }



        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_buttonPosition == null)
                _buttonPosition = m_textBox.TransformToAncestor((Grid)this.Parent).Transform(new Point(0, 0));
            var mousePosition = Mouse.GetPosition((Grid)this.Parent);
            deltaX = mousePosition.X - _buttonPosition.Value.X;
            deltaY = mousePosition.Y - _buttonPosition.Value.Y;
            _isMoving = true;
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _currentTT = m_textBox.RenderTransform as TranslateTransform;
            _isMoving = false;
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;

            var mousePoint = Mouse.GetPosition((Grid)this.Parent);

            var offsetX = (_currentTT == null ? _buttonPosition.Value.X : _buttonPosition.Value.X - _currentTT.X) + deltaX - mousePoint.X;
            var offsetY = (_currentTT == null ? _buttonPosition.Value.Y : _buttonPosition.Value.Y - _currentTT.Y) + deltaY - mousePoint.Y;

            this.m_textBox.RenderTransform = new TranslateTransform(-offsetX, -offsetY);
        }



    }
}
