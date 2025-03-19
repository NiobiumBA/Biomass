using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace Localization
{
    public static class XmlTranslationParser
    {
        public const string XmlNodeSeparator = ".";

        public static string CombineNodesName(params string[] nodeNames)
        {
            return CombineNodesNameInternal(nodeNames);
        }

        public static Language LoadLanguage(string name)
        {
            string path = Path.Combine(Translation.LanguagesPath, name);
            TextAsset asset = Resources.Load<TextAsset>(path);
            string text = asset.text;

            XmlDocument xmlDocument = new();
            xmlDocument.LoadXml(text);

            XmlElement xRoot = xmlDocument.DocumentElement;

            Dictionary<string, string> translations = new();
            ParseXmlTranslation(translations, xRoot, new List<string>());
            
            return new Language(name, translations);
        }

        private static void ParseXmlTranslation(Dictionary<string, string> translations, XmlNode xNode, List<string> nodeNames)
        {
            foreach (XmlNode childNode in xNode.ChildNodes)
            {
                if (childNode.HasChildNodes && childNode.NodeType == XmlNodeType.Element)
                {
                    nodeNames.Add(childNode.Name);

                    ParseXmlTranslation(translations, childNode, nodeNames);
                }
                else if (childNode.NodeType == XmlNodeType.Text)
                {
                    string id = CombineNodesNameInternal(nodeNames);
                    translations.Add(id, childNode.InnerText);
                }
            }

            nodeNames.Remove(xNode.Name);
        }

        private static string CombineNodesNameInternal(IEnumerable<string> nodeNames)
        {
            return string.Join(XmlNodeSeparator, nodeNames);
        }
    }
}