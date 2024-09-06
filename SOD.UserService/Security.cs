using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SOD.UserService
{
    public class Security : DependencyObject
    {


        public static UserRole? GetRole(DependencyObject obj)
        {
            return (UserRole?)obj.GetValue(RoleProperty);
        }

        public static void SetRole(DependencyObject obj, UserRole? value)
        {
            obj.SetValue(RoleProperty, value);
        }

        // Using a DependencyProperty as the backing store for Role.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(UserRole?), typeof(Security), new PropertyMetadata(null));


    }
}
