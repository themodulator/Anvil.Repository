#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

#endregion

namespace System.Web.Mvc.Html
{
    public static class AnvilHtmlHelper
    {
        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            return DescriptionFor<TModel, TValue>(helper, expression, null);
        }

        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            string text = metaData.DisplayName ?? metaData.PropertyName ?? htmlFieldName.Split('.').Last();


            if (String.IsNullOrEmpty(text))
                return MvcHtmlString.Empty;

            var span = new TagBuilder("span");
            if (!string.IsNullOrEmpty(metaData.Description))
                span.SetInnerText(metaData.Description);

            if (htmlAttributes != null)
            {
                foreach (PropertyInfo p in htmlAttributes.GetType().GetProperties())
                {
                    object v = p.GetValue(htmlAttributes, null);
                    if (v != null)
                        span.Attributes.Add(p.Name, v.ToString());
                }
            }
            return MvcHtmlString.Create(span.ToString());
        }

        public static MvcHtmlString LabelWithTooltip<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression)
        {
            return LabelWithTooltip(helper, expression, null);
        }

        public static MvcHtmlString LabelWithTooltip<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {

            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            string labelText = metaData.DisplayName ?? metaData.PropertyName ?? htmlFieldName.Split('.').Last();



            if (String.IsNullOrEmpty(labelText))

                return MvcHtmlString.Empty;



            var label = new TagBuilder("label");

            label.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));



            if (!string.IsNullOrEmpty(metaData.Description))
                label.Attributes.Add("title", metaData.Description);

            if (htmlAttributes != null)
            {
                foreach (PropertyInfo p in htmlAttributes.GetType().GetProperties())
                {
                    object v = p.GetValue(htmlAttributes, null);
                    if (v != null)
                        label.Attributes.Add(p.Name, v.ToString());
                }
            }

            label.SetInnerText(labelText);

            return MvcHtmlString.Create(label.ToString());

        }

        public static MvcHtmlString UlStringListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);


            if (metaData.Model != null)
            {

                List<string> l = (List<string>)metaData.Model;



                TagBuilder ul = new TagBuilder("ul");

                foreach (string s in l)
                {
                    TagBuilder li = new TagBuilder("li");
                    li.SetInnerText(s);
                    ul.InnerHtml += li.ToString();
                }

                return MvcHtmlString.Create(ul.ToString());
            }

            return null;

        }

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(
            this IEnumerable<T> items,
            Func<T, string> nameSelector,
            Func<T, string> valueSelector
            )
        {
            return items.OrderBy(item => nameSelector(item))
                   .Select(item =>
                           new SelectListItem
                           {
                               Text = nameSelector(item),
                               Value = valueSelector(item)
                           });
        }
    }
}
