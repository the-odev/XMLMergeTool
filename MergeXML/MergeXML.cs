namespace MergeXML
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Merge XML
    /// </summary>
    public static class MergeXML
    {
        #region Public Methods

        /// <summary>
        /// Merges the specified document source.
        /// </summary>
        /// <param name="documentSource">The document source.</param>
        /// <param name="documentToMerge">The document to merge.</param>
        public static void Merge(string documentSource, string documentToMerge)
        {
            try
            {
                XmlDocument sourceFile = new XmlDocument();
                XmlDocument fileToMerge = new XmlDocument();

                sourceFile.Load(documentSource);
                fileToMerge.Load(documentToMerge);

                HandleMerge(documentSource, sourceFile, fileToMerge);
            }
            catch (Exception ex)
            {
                int ERROR_CODE = 999;
                if (ex is FileNotFoundException)
                {
                    ERROR_CODE = 5;
                }
                if (ex is XmlException)
                {
                    ERROR_CODE = 10;
                }
                Environment.Exit(ERROR_CODE);
            }
        }

        /// <summary>
        /// Merges the specified documents.
        /// </summary>
        /// <param name="documentSource">The document source.</param>
        /// <param name="documentToMerge">The document to merge.</param>
        /// <param name="documentDestination">The document destination.</param>
        public static void Merge(string documentSource, string documentToMerge, string documentDestination)
        {
            try
            {
                XmlDocument sourceFile = new XmlDocument();
                XmlDocument fileToMerge = new XmlDocument();

                sourceFile.Load(documentSource);
                fileToMerge.Load(documentToMerge);
                
                HandleMerge(documentDestination, sourceFile, fileToMerge);
            }
            catch (Exception ex)
            {
                int ERROR_CODE = 0;
                if (ex is FileNotFoundException)
                {
                    ERROR_CODE = 2;
                }
                if (ex is XmlException)
                {
                    ERROR_CODE = 1;
                }
                else
                {
                    //UNKNOW ERROR
                    ERROR_CODE = 999;
                }
                Environment.Exit(ERROR_CODE);
            }            
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Handles the merge.
        /// </summary>
        /// <param name="documentDestination">The document destination.</param>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="fileToMerge">The file to merge.</param>
       private static void HandleMerge(string documentDestination, XmlDocument sourceFile, XmlDocument fileToMerge)
        {
           //on recupere l'element racine
            XmlElement root = fileToMerge.DocumentElement;

           //On parcours ces noeud enfants
           foreach (XmlNode nodeApresRoot in root.ChildNodes)
            {
                if (nodeApresRoot.NodeType != XmlNodeType.Comment)
                {
                    //On les créés ou mise à jour
                    CreateNode(sourceFile, fileToMerge, nodeApresRoot);

                    //Parcours de l'ensemble des noeuds enfant (récursif)
                    XmlNodeList listDeNode = GetListOfChildNodes(nodeApresRoot);
                    foreach (XmlNode node in listDeNode)
                    {
                        CreateNode(sourceFile, fileToMerge, node);
                    }
                }
            }
            sourceFile.Normalize();
            sourceFile.Save(documentDestination);
        }

       /// <summary>
       /// Creates the node.
       /// </summary>
       /// <param name="sourceFile">The source file.</param>
       /// <param name="fileToMerge">The file to merge.</param>
       /// <param name="node">The node.</param>
       private static void CreateNode(XmlDocument sourceFile, XmlDocument fileToMerge, XmlNode node)
        {
            string nodeXPathWithId = FindXPath(node);
            string nodeXPath = Regex.Replace(nodeXPathWithId, @"\[[0-9]+\]", string.Empty);

           
           XmlNode nodeToImport;
           if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Text)
           {
               nodeToImport = sourceFile.ImportNode(node, true);
           }
           else
           {
               nodeToImport = sourceFile.ImportNode(node, false);
           }
            
           string parentNodeXPathwithId = FindXPath(node.ParentNode);
           string parentNodeXPath = Regex.Replace(parentNodeXPathwithId, @"\[[0-9]+\]", string.Empty);
           if (nodeXPath == string.Empty)
           {
               if (parentNodeXPath != string.Empty)
               {
                   sourceFile.SelectSingleNode(parentNodeXPath).AppendChild(nodeToImport);
               }
           }
            else if (sourceFile.SelectSingleNode(nodeXPath) == null)
            {
                //Si le noeud n'est pas trouvé dans la destination
                //on l'ajoute
                sourceFile.SelectSingleNode(parentNodeXPath).AppendChild(nodeToImport);
            }
            else if (node.Attributes.Count != 0)
            {
                //Si trouvé mais qu'il possède des attributs
                string xpath = nodeXPath + "[@" + nodeToImport.Attributes[0].Name + "='" + nodeToImport.Attributes[0].Value + "']";
                XmlNode nodeFromSource = sourceFile.SelectSingleNode(xpath);
                if (nodeFromSource == null)
                {
                    //Noeud avec attributs non trouvé
                    sourceFile.SelectSingleNode(parentNodeXPath).AppendChild(nodeToImport);
                }
                else
                {
                    //Noeud avec attributs trouvé, on remplace le noeud dans le fichier de destination
                    sourceFile.SelectSingleNode(parentNodeXPath).ReplaceChild(nodeToImport, nodeFromSource);
                }
            }
        }

       /// <summary>
       /// Gets the list of child nodes.
       /// </summary>
       /// <param name="node">The node.</param>
       /// <returns>the list of child nodes</returns>
       private static XmlNodeList GetListOfChildNodes(XmlNode node)
        {
            XmlNode nodeRecupere = node.SelectSingleNode(FindXPath(node));
            XmlNodeList listDeNode = nodeRecupere.ChildNodes;
            foreach (XmlNode nodeEnfant in listDeNode)
            {
                if (nodeEnfant.HasChildNodes && nodeEnfant.FirstChild.NodeType != XmlNodeType.Text)
                {
                    listDeNode = GetListOfChildNodes(nodeEnfant);
                }
            }
            return listDeNode;
        }

       /// <summary>
       /// Determines whether [is document loaded] [the specified document].
       /// </summary>
       /// <param name="document">The document.</param>
       /// <param name="file">The file.</param>
       /// <returns>true if document is loaded; otherwise false</returns>
       private static bool IsDocumentLoaded(XmlDocument document, string file)
        {
           try
           {
               document.Load(file);
               return true;
           }
           catch (Exception)
           {
               return false;
           }
        }

       /// <summary>
       /// Finds the x path.
       /// </summary>
       /// <param name="node">The node.</param>
       /// <returns>the xPath of the node with id</returns>
       /// <exception cref="System.ArgumentException">
       /// Only elements and attributes are supported
       /// or
       /// Node was not in a document
       /// </exception>
       private static string FindXPath(XmlNode node)
       {
           StringBuilder builder = new StringBuilder();
           while (node != null)
           {
               switch (node.NodeType)
               {
                   case XmlNodeType.Attribute:
                       builder.Insert(0, "/@" + node.Name);
                       node = ((XmlAttribute)node).OwnerElement;
                       break;
                   case XmlNodeType.Element:
                       int index = FindElementIndex((XmlElement)node);
                       builder.Insert(0, "/" + node.Name + "[" + index + "]");
                       node = node.ParentNode;
                       break;
                   case XmlNodeType.Document:
                       return builder.ToString();
                   case XmlNodeType.Text:
                       builder.Insert(0, "/" + node.ParentNode.Name + "[" + 1 + "]");
                       node = node.ParentNode;
                       break;
                   case XmlNodeType.Comment:
                       node = null;
                       //TODO TD : mettre ici le code pour gérer les commentaires
                       break;
                   default:
                       throw new ArgumentException("Only elements and attributes are supported: NodeType = " + node.NodeType.ToString());
               }
           }
           return string.Empty;
       }

       /// <summary>
       /// Finds the index of the element.
       /// </summary>
       /// <param name="element">The element.</param>
       /// <returns>the index of the node</returns>
       /// <exception cref="System.ArgumentException">Couldn't find element within parent</exception>
       private static int FindElementIndex(XmlElement element)
       {
           XmlNode parentNode = element.ParentNode;
           if (parentNode is XmlDocument)
           {
               return 1;
           }

           XmlElement parent = (XmlElement)parentNode;
           int index = 1;
           foreach (XmlNode candidate in parent.ChildNodes)
           {
               if (candidate is XmlElement && candidate.Name == element.Name)
               {
                   if (candidate == element)
                   {
                       return index;
                   }

                   index++;
               }
           }

           throw new ArgumentException("Couldn't find element within parent");
       }

        #endregion Private Methods
    }
}
