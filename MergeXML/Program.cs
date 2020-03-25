namespace MergeXML
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    //using System.Threading.Tasks;

    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            if (args.Count() == 2)
            {
                MergeXML.Merge(args[0], args[1]);
            }
            else if (args.Count() == 3)
            {
                MergeXML.Merge(args[0], args[1], args[2]);
            }
            else
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("How To Use MergeXML.exe");
                Console.WriteLine("<args[0] = 'your source document'>");
                Console.WriteLine("<args[1] = 'your document to merge'>");
                Console.WriteLine("<args[2] = 'the destination document'>");
                Console.WriteLine("If args[2] is not provided, args[0] will be the destination document");
                Console.WriteLine("If destination file not exists(args[2]), file is created");
                Console.WriteLine("-----------------------------------------------------------");
                Console.ReadLine();
            }
        }
    }
}
