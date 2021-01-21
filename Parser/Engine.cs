using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public interface IEngine
    {
        void Execute (Lexem lexem);
    }

    public class UpnConverter : IEngine
    {
        public List<Lexem> Program { get; }

        public UpnConverter ()
        {
            Program = new List<Lexem> ();
        }

        public void Execute (Lexem lexem)
        {
            Program.Add (lexem);
        }

        public string GetSource ()
        {
            var sb = new StringBuilder ();
            for (var i = 0; i < Program.Count; i++)
            {
                if (i > 0)
                    sb.Append (' ');
                sb.Append (Program[i]);
            }
            return sb.ToString ();
        }
    }
}
