using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using ReactiveUI;
using System;
using System.Collections.Generic;
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

namespace SOD.Keyboard
{
    /// <summary>
    /// Логика взаимодействия для Keyboard.xaml
    /// </summary>
    public partial class Keyboard : UserControl
    {
        private Lazy<LetterKeyboard> letterKeyboard = new Lazy<LetterKeyboard>();
        private Lazy<SymbolKeyboard> symbolKeyboard = new Lazy<SymbolKeyboard>();
        private Lazy<NumberKeyboard> numberKeyboard = new Lazy<NumberKeyboard>();
        private InputSimulator inputSimulator = new InputSimulator();
        public Keyboard()
        {
            InitializeComponent();
            CurrentKeyboard = letterKeyboard.Value;

            EnterKey = ReactiveCommand.Create<string>(key =>
            {
                TextBoxKeyboard.Focus();
                if (letterKeyboard.Value.IsUpper) key = key.ToUpper();
                inputSimulator.Keyboard.TextEntry(key);
            });

            BackSpace = ReactiveCommand.Create(() =>
            {
                TextBoxKeyboard.Focus();
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            });

            Space = ReactiveCommand.Create(() =>
            {
                TextBoxKeyboard.Focus();
                inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            });

            Enter = ReactiveCommand.Create(() =>
            {
                VirtualKeyboard.Instance.OnClose(TextBoxKeyboard.Text, true);
                /*FocusManager.AddGotFocusHandler(this, (s, e) => { });
                var a = System.Windows.Input.Keyboard.FocusedElement;*/
            });

            SwitchKeyboard = ReactiveCommand.Create<string>(keyboardType =>
            {
                if (keyboardType == null) return;
                if (keyboardType == "Letter")
                {
                    CurrentKeyboard = letterKeyboard.Value;
                }
                else if (keyboardType == "Symbol")
                {
                    CurrentKeyboard = symbolKeyboard.Value;
                }
            });
        }

        public void ShowNumberKeyboard()
        {
            CurrentKeyboard = numberKeyboard.Value;
            TextBoxKeyboard.SelectionStart = TextBoxKeyboard.Text.Length;
        }

        public void ShowLetterKeyboard()
        {
            CurrentKeyboard = letterKeyboard.Value;
            TextBoxKeyboard.SelectionStart = TextBoxKeyboard.Text.Length;
        }

        public Control CurrentKeyboard
        {
            get { return (LetterKeyboard)GetValue(CurrentKeyboardProperty); }
            set { SetValue(CurrentKeyboardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LetterKeyboard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentKeyboardProperty =
            DependencyProperty.Register("CurrentKeyboard", typeof(Control), typeof(Keyboard), new PropertyMetadata(null));

        public ICommand EnterKey { get; }
        public ICommand BackSpace { get;}
        public ICommand Space { get; }
        public ICommand Enter { get; }
        public ICommand SwitchKeyboard { get; set; }

    }
}
