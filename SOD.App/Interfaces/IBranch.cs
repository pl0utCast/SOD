using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Interfaces
{
    public interface IBranch<T>
    {
        public int Id { get; }
        public IList<IBranch<T>> Childrens { get; }
        public bool CanAddChildren { get; }
    }
}
