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
    /// Логика взаимодействия для LetterKeyboard.xaml
    /// </summary>
    public partial class LetterKeyboard : UserControl
    {
        private string currentKeyboardPrefix;
        private Dictionary<string, UserControl> keyboardCache = new Dictionary<string, UserControl>();
        public LetterKeyboard()
        {
            InitializeComponent();

            ToUpperLower = ReactiveCommand.Create(() =>
            {
                IsUpper = !IsUpper;
            });
            SwitchLanguage = ReactiveCommand.Create(() =>
            {
                if (currentKeyboardPrefix == "eng")
                {
                    currentKeyboardPrefix = "rus";
                    if (keyboardCache.ContainsKey(currentKeyboardPrefix))
                    {
                        CurrentKeyboard = keyboardCache[currentKeyboardPrefix];
                    }
                    else
                    {
                        CurrentKeyboard = new RusKeyboard();
                        keyboardCache.Add(currentKeyboardPrefix, CurrentKeyboard);

                    }
                }
                else if (currentKeyboardPrefix == "rus")
                {
                    currentKeyboardPrefix = "eng";
                    if (keyboardCache.ContainsKey(currentKeyboardPrefix))
                    {
                        CurrentKeyboard = keyboardCache[currentKeyboardPrefix];
                    }
                    else
                    {
                        CurrentKeyboard = new EngKeyboard();
                        keyboardCache.Add(currentKeyboardPrefix, CurrentKeyboard);
                    }
                }
                UpperLower(IsUpper, this);
            });

            CurrentKeyboard = new EngKeyboard();
            currentKeyboardPrefix = "eng";
            keyboardCache.Add(currentKeyboardPrefix, CurrentKeyboard);
        }

        public bool IsUpper
        {
            get { return (bool)GetValue(IsUpperProperty); }
            set { SetValue(IsUpperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShift.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUpperProperty =
            DependencyProperty.Register("IsUpper", typeof(bool), typeof(LetterKeyboard),
                new PropertyMetadata(false,
                (s, e) =>
                {
                    var keyboard = s as LetterKeyboard;
                    UpperLower((bool)e.NewValue, keyboard);
                }));

        public UserControl CurrentKeyboard
        {
            get { return (UserControl)GetValue(CurrentKeyboardProperty); }
            set { SetValue(CurrentKeyboardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentKeyboard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentKeyboardProperty =
            DependencyProperty.Register("CurrentKeyboard", typeof(UserControl), typeof(LetterKeyboard), new PropertyMetadata(null));

        private static void UpperLower(bool isUpper, LetterKeyboard keyboard)
        {
            foreach (var children in ((Grid)((UserControl)keyboard.LangKeyBoard.Content).Content).Children)
            {
                if (children is StackPanel stackPanel)
                {
                    foreach (var spChildren in stackPanel.Children)
                    {
                        if (spChildren is Button button && button.Content is string content)
                        {
                            if (isUpper) button.Content = content.ToUpper();
                            else button.Content = content.ToLower();
                        }
                    }
                }
            }
        }



        public ICommand ToUpperLower { get; }
        public ICommand SwitchLanguage { get; }
    }
}
