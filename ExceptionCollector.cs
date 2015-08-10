using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Entity.Validation;
using System.Web.Mvc;

namespace Anvil.Repository
{
    public class ExceptionCollector : CollectionBase
    {

        #region Constructors

        public ExceptionCollector()
        { }

        public ExceptionCollector(Exception e)
        {
            if(e.GetType() == typeof(DbEntityValidationException))
            {
                foreach (var v in ((DbEntityValidationException)e).EntityValidationErrors)
                {
                    foreach (var ve in v.ValidationErrors)
                    {
                        Add(ve.ErrorMessage);
                    }
                }
            }
            else
                Add(e);
        }

        public ExceptionCollector(String msg)
        {
            Add(msg);
        }


        public ExceptionCollector(ICollection<ModelState> modelStates)
        {
            foreach (ModelState m in modelStates)
            {
                foreach (ModelError error in m.Errors)
                {
                    Add(error.Exception);
                }
            }
        }

        #endregion

        #region Add

        public void Add(String e)
        { List.Add(e); }


        public void Add(Exception e)
        {
            List<String> m = new List<string>();
            EnumerateInnerExceptions(e, ref m);

            AddRange(m.ToArray());
        }

        public void AddRange(string[] items)
        {
            foreach (string s in items)
                List.Add(s);
        }

        public void Insert(int index, string message)
        {
            List.Insert(index, message);
        }

        #endregion

        #region Remove

        public void Remove(string item)
        {
            List.Remove(item);
        }

        #endregion

        #region Item

        public string this[int index]
        {
            get { return List[index].ToString(); }
            set { List[index] = value; }
        }

        #endregion


        #region Array

        public string[] ToArray()
        {
            string[] items = new string[this.Count];
            List.CopyTo(items, 0);
            return items;
        }

        public string ToUL()
        {
            string ul = "<ul>\n";
            string[] items = ToArray();
            string[] lis = items.Select(x => "<li>" + x + "</li>\n").ToArray();
            ul += string.Join("", lis);
            ul += "</ul>";
            return ul;
        }

        public string ToLineBreakString()
        {
            string[] items = ToArray();
            return string.Join("\n", items);
        }

        #endregion

        #region Inner Exceptions

        public static void EnumerateInnerExceptions(Exception e, ref List<String> messages)
        {
            messages.Add(e.Message);

            if (e.InnerException != null)
            {
                EnumerateInnerExceptions(e.InnerException, ref messages);
            }
        }


        #endregion

        #region Exception

        public Exception ToException()
        {
            if (Count == 0)
                return null;

            if (Count == 1)
                return new Exception(this[0]);
            else
            {
                Exception ex = new Exception(this[0]);
                for (int i = 1; i <= this.Count - 1; i++)
                {
                    AddInnerException(ref ex, this[i]);
                }
                return ex;
            }
        }

        private void AddInnerException(ref Exception e, String innerException)
        {
            Exception _last = GetLast(ref e);
            FieldInfo f = (from FieldInfo x in e.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                           where x.Name.ToLower() == "_innerexception"
                           select x).SingleOrDefault();
            if (f != null)
            {
                f.SetValue(_last, new Exception(innerException));
            }
        }


        private Exception GetLast(ref Exception e)
        {
            if (e.InnerException == null)
            {
                return e;
            }
            else
            {
                Exception i = e.InnerException;
                return GetLast(ref i);
            }
        }
        #endregion

    }
}
