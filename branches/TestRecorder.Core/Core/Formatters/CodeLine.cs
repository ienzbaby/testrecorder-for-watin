using TestRecorder.Core.Actions;

namespace TestRecorder.Core.Formatters
{
    public class CodeLine
    {
        public bool NoModel = false;
        public string ModelPath;
        public string ModelVariable;
        public string ModelLocalProperty;
        public string ModelClassProperty;
        public string ModelFunction;
        public FindAttributeCollection Attributes;
        public FindAttributeCollection Frames;
        public string FullLine;
    }
}
