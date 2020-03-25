namespace TestMergeXML
{
    using System.IO;
    using NUnit.Framework;
    using TestMergeXML.Properties;

    /// <summary>
    /// UnitTest1
    /// </summary>
    [TestFixture]
    public class UnitTest1
    {
        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            File.Delete(@"C:\SVN\Tools\Project\XMLMergeTool\TestMergeXML\TestsFiles\sourceFile.xml");
            File.WriteAllText(@"C:\SVN\Tools\Project\XMLMergeTool\TestMergeXML\TestsFiles\sourceFile.xml", Resources.SourceFile.ToString());
        }


        /// <summary>
        /// Tests the merge.
        /// </summary>
        [Test]
        public void TestMerge()
        {
            MergeXML.MergeXML.Merge(@"C:\SVN\Tools\Project\XMLMergeTool\TestMergeXML\TestsFiles\sourceFile.xml", @"C:\SVN\Tools\Project\XMLMergeTool\TestMergeXML\TestsFiles\FileToMerge.xml");
            string textGenere = File.ReadAllText(@"C:\SVN\Tools\Project\XMLMergeTool\TestMergeXML\TestsFiles\sourceFile.xml");
            string textAttendu = File.ReadAllText(@"C:\SVN\Tools\Project\XMLMergeTool\TestMergeXML\TestsFiles\MergedFileExpected.xml");
            NUnit.Framework.Assert.IsTrue(textAttendu == textGenere);
        }
    }
}
