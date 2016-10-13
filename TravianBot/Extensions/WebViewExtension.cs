using EO.WebBrowser;
using EO.WebBrowser.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianBot.Core.Extensions
{
    static class WebViewExtension
    {
        public static object EvalScriptAndWait(this EO.WebBrowser.Wpf.WebView webView, 
            string script, bool throwOnError)
        {
            var task = Task<object>.Factory.StartNew(() =>
            {
                return webView.EvalScript(script, throwOnError);
            });

            task.Start();
            task.Wait();
            return new object();

            
        }
    }
}
