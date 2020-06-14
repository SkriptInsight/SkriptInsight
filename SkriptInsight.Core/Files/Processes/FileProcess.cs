using System.ComponentModel;
using SkriptInsight.Core.Extensions;

namespace SkriptInsight.Core.Files.Processes
{
    [Description("")]
    public abstract class FileProcess
    {
        public virtual bool ReportProgress => true;
        
        public virtual string ReportProgressTitle => "Executing Process";
        
        public virtual string ReportProgressMessage => this.GetClassDescription();
        
        public abstract void DoWork(SkriptFile file, int lineNumber, string rawContent, FileParseContext context);
    }
}