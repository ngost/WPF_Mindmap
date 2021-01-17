using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace WPF_MindMap.Resources.scripts
{
    public class MindTextBox : StackPanel
    {
        private TextBox m_textBox;
        private double _startX;
        private double _startY;
        private bool _isMoving;

        public MindTextBox()
        {
/*            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Center;*/
            
            TextBoxInit();

            this.Children.Add(m_textBox);
        }

        private void TextBoxInit()
        {
            m_textBox = new TextBox();
            m_textBox.Background = Brushes.Yellow;
/*            m_textBox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            m_textBox.VerticalContentAlignment = VerticalAlignment.Center;*/
            m_textBox.TextAlignment = TextAlignment.Center;
            m_textBox.TextWrapping = TextWrapping.Wrap;
            m_textBox.FontSize = 20;
            m_textBox.Width = 150;
            //m_textBox.Height = 100;


            m_textBox.Foreground = Brushes.Black;
            m_textBox.KeyDown += OnKeyDownHandler;
            m_textBox.PreviewMouseLeftButtonDown += BoxMouseDown;
            m_textBox.PreviewMouseMove += BoxMouseMove;
            m_textBox.PreviewMouseUp += BoxMouseUp;
        }

        private void OnKeyDownHandler(Object sender, KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                DockPanel p_panel =  (DockPanel)this.Parent;
                m_textBox.BorderThickness = new Thickness(2,2,2,2);
                m_textBox.IsReadOnly = false;
            }
        }

        private void BoxMouseDown(Object sender, EventArgs args)
        {

            _startX = Mouse.GetPosition((DockPanel)this.Parent).X;
            _startY = Mouse.GetPosition((DockPanel)this.Parent).Y;
            _isMoving = true;
        }
        private void BoxMouseUp(Object sender, EventArgs args)
        {
            _isMoving = false;
        }
        private void BoxMouseMove(Object sender, EventArgs args)
        {
            if (!_isMoving) return;

            TranslateTransform transform = new TranslateTransform();
            transform.X = Mouse.GetPosition((DockPanel)this.Parent).X - _startX;
            transform.Y = Mouse.GetPosition((DockPanel)this.Parent).Y - _startY;
            this.RenderTransform = transform;
        }



    }
}
