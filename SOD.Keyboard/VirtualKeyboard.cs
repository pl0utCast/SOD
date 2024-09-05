using SOD.Core.Infrastructure;
using SOD.Core.Settings;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SOD.Keyboard
{
    public class VirtualKeyboard
    {
        public static event EventHandler<bool> Close;
        public static event EventHandler Show;
        
        private TextBox currentTextBox;
        private PasswordBox currentPasswordBox;
        private bool isEnableVirtualKeyboard;
        private BindingExpression currentBinding;
        public VirtualKeyboard(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            Instance = this;
            Settings = settingsService.GetSettings("VirtualKeyboard", new Settings());
            IsEnableVirtualKeyboard = Settings.IsEnable;
        }
        public void RegisterElement(DependencyObject dependencyObject)
        {
            FocusManager.AddGotFocusHandler(dependencyObject,
            (s, e) =>
            {
                if (!IsEnableVirtualKeyboard) return;
                if (e.OriginalSource is TextBox textBox)
                {
                    if (textBox.Name != "TextBoxKeyboard")
                    {
                        Keyboard.TextBoxKeyboard.Text = textBox.Text;
                        currentBinding = textBox.GetBindingExpression(TextBox.TextProperty);

                        if (currentBinding != null && currentBinding.ResolvedSourcePropertyName != null)
                        {
                            var prop = currentBinding.ResolvedSource.GetType().GetProperty(currentBinding.ResolvedSourcePropertyName).PropertyType.Name;
                            if (prop == "String") Keyboard.ShowLetterKeyboard();
                            else Keyboard.ShowNumberKeyboard();
                        }
                        else Keyboard.ShowLetterKeyboard();
                        currentPasswordBox = null;
                        currentTextBox = textBox;

                        Show?.Invoke(Keyboard, new EventArgs());
                    }
                }
                else if (e.OriginalSource is PasswordBox passwordBox)
                {
                    currentTextBox = null;
                    Keyboard.TextBoxKeyboard.Text = passwordBox.Password;
                    Keyboard.ShowLetterKeyboard();
                    currentPasswordBox = passwordBox;

                    Show?.Invoke(Keyboard, new EventArgs());
                }
            });
        }
        public void OnClose(string text, bool isEnter)
        {
            if (currentTextBox != null)
            {
                if (isEnter)
                {
                    currentTextBox.Text = text;
                    currentBinding?.UpdateSource();
                }
                
            }
            else if (currentPasswordBox != null)
            {
                if (isEnter) currentPasswordBox.Password = text;
            }
            Close?.Invoke(Keyboard, isEnter);
        }

        private bool isOpen;
        private readonly ISettingsService settingsService;

        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                if (!isOpen) OnClose(null, false);
            }
        }
        
        
        [ApplicationSettings]
        public bool IsEnableVirtualKeyboard
        {
            get { return isEnableVirtualKeyboard; }
            set 
            { 
                isEnableVirtualKeyboard = value;
                //Settings.IsEnable = value;
                //settingsService.SaveSettings("VirtualKeyboard", Settings);
            }
        }
        public Settings Settings { get; set; }

        public static VirtualKeyboard Instance { get; private set; }
        public static Keyboard Keyboard { get; } = new Keyboard();
    }
}
