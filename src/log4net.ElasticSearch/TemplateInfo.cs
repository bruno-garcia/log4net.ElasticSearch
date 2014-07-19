using System.IO;
using log4net.Core;
using log4net.Util;

namespace log4net.ElasticSearch
{
    public class TemplateInfo : IOptionHandler
    {
        private readonly IErrorHandler _errorHandler;

        public string Name { get; set; }
        public string FileName { get; set; }
        public bool IsValid { get; private set; }

        public TemplateInfo()
        {
            IsValid = false;
            _errorHandler = new OnlyOnceErrorHandler(this.GetType().Name);
        }

        public void ActivateOptions()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(FileName))
            {
                _errorHandler.Error("Template name or fileName is empty!");
            }

            if (!File.Exists(FileName))
            {
                _errorHandler.Error(string.Format("Could not find template file: {0}", FileName));
            }

            IsValid = true;
        }
    }
}