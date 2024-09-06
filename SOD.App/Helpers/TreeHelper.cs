using SOD.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOD.App.Helpers
{
    public static class TreeHelper
    {
        public static IEnumerable<IBranch<T>> FindAllBranch<T>(IBranch<T> root)
        {
            var branch = new List<IBranch<T>>();
            for (int i = 0; i < root.Childrens.Count; i++)
            {
                if (root.Childrens[i].Childrens.Count == 0)
                    branch.Add(root.Childrens[i]);
                else
                {
                    branch.Add(root.Childrens[i]);
                    branch.AddRange(FindAllBranch<T>(root.Childrens[i]));
                }
            }
            return branch;
        }

        public static IBranch<T> FindOwnerBranch<T>(IBranch<T> root, IBranch<T> children)
        {
            if (children == null) return null;
            for (int i = 0; i < root.Childrens.Count; i++)
            {
                if (root.Childrens[i].Id == children.Id) return root;
                if (root.Childrens[i].Childrens.Count>0)
                {
                    var nextOwner = FindOwnerBranch(root.Childrens[i], children);
                    if (nextOwner != null) return nextOwner;
                }
            }
            return null;
        }
    }
}
