using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using System.Text;


namespace iZine.Helpers
{
    public static class JavaScriptExtensions
    {
        public static string CascadingDropDownList(this HtmlHelper helper, string name, string associatedDropDownList)
        {
            var sb = new StringBuilder();
            
            // render select tag
            sb.AppendFormat("<select name='{0}' id='{0}'></select>", name);
            sb.AppendLine();

            // render data array
            sb.AppendLine("<script type='text/javascript'>");
            var data = (CascadingSelectList)helper.ViewDataContainer.ViewData[name];
            var listItems = data.GetListItems();
            var colArray = new List<string>();
            foreach (var item in listItems)
                colArray.Add(String.Format("{{key:'{0}',value:'{1}',text:'{2}'}}", item.Key, item.Value, item.Text));
            var jsArray = String.Join(",", colArray.ToArray());
            sb.AppendFormat("$get('{0}').allOptions=[{1}];", name, jsArray);
            sb.AppendLine();
            sb.AppendFormat("$addHandler($get('{0}'), 'change', Function.createCallback(bindDropDownList, $get('{1}')));", associatedDropDownList, name);
            sb.AppendLine();
            sb.AppendLine("</script>");

            return sb.ToString();
            
        }
    }

    public class CascadingSelectList
    {
        private IEnumerable _items;
        private string _dataKeyField;
        private string _dataValueField;
        private string _dataTextField;

        public CascadingSelectList(IEnumerable items, string dataKeyField, string dataValueField, string dataTextField)
        {
            _items = items;
            _dataKeyField = dataKeyField;
            _dataValueField = dataValueField;
            _dataTextField = dataTextField;
        }

        public List<CascadingListItem> GetListItems()
        {
            var listItems = new List<CascadingListItem>();
            if(_items != null)
            {
                foreach (var item in _items)
                {
                    var key = DataBinder.GetPropertyValue(item, _dataKeyField).ToString();
                    var value = DataBinder.GetPropertyValue(item, _dataValueField).ToString();
                    var text = DataBinder.GetPropertyValue(item, _dataTextField).ToString();
                    listItems.Add(new CascadingListItem(key, value, text));
                }
            }
            return listItems;
        }
    }

    public class CascadingListItem
    {
        public CascadingListItem(string key, string value, string text)
        {
            this.Key = key;
            this.Value = value;
            this.Text = text;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
    }

}
