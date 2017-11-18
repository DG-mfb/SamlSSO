using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Shared.Federtion.Forms
{
    public class SAMLForm
    {
        private static string htmlFormTemplate = "<html xmlns=\"http://www.w3.org/1999/xhtml\"><body onload=\"document.forms.samlform.submit()\"><noscript><p><strong>Note:</strong> Since your browser does not support Javascript, you must press the Continue button once to proceed.</p></noscript><form id=\"samlform\" action=\"{0}\" method=\"post\"><div>{1}</div><noscript><div><input type=\"submit\" value=\"Continue\"/></div></noscript></form></body></html>";
        private IDictionary<string, string> hiddenControls = (IDictionary<string, string>)new Dictionary<string, string>();
        private const string defaultHTMLFormTemplate = "<html xmlns=\"http://www.w3.org/1999/xhtml\"><body onload=\"document.forms.samlform.submit()\"><noscript><p><strong>Note:</strong> Since your browser does not support Javascript, you must press the Continue button once to proceed.</p></noscript><form id=\"samlform\" action=\"{0}\" method=\"post\"><div>{1}</div><noscript><div><input type=\"submit\" value=\"Continue\"/></div></noscript></form></body></html>";
        private string actionURL;

        public static string HTMLFormTemplate
        {
            get
            {
                return SAMLForm.htmlFormTemplate;
            }
            set
            {
                SAMLForm.htmlFormTemplate = value;
            }
        }

        public string ActionURL
        {
            get
            {
                return this.actionURL;
            }
            set
            {
                this.actionURL = value;
            }
        }

        public IDictionary<string, string> HiddenControls
        {
            get
            {
                return this.hiddenControls;
            }
            set
            {
                this.hiddenControls = value;
            }
        }

        public void AddHiddenControl(string controlName, string controlValue)
        {
            this.hiddenControls.Add(controlName, controlValue);
        }

        public void Write(Stream stream)
        {
            string str = this.ToString();
            
            StreamWriter streamWriter = new StreamWriter(stream);
            streamWriter.Write(str);
            streamWriter.Flush();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            foreach (string key in (IEnumerable<string>)this.hiddenControls.Keys)
            {
                string hiddenControl = this.hiddenControls[key];
                stringBuilder1.AppendFormat("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", (object)key, HttpUtility.HtmlEncode(hiddenControl));
            }
            StringBuilder stringBuilder2 = new StringBuilder();
            stringBuilder2.AppendFormat(SAMLForm.htmlFormTemplate, (object)this.actionURL, (object)stringBuilder1.ToString());
            return stringBuilder2.ToString();
        }
    }
}