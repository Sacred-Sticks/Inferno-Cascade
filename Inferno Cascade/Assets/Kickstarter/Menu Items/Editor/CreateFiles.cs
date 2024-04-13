using UnityEngine;
using UnityEditor;
using System.IO;

namespace Kickstarter.MenuItems
{
    public class CreateFiles
    {
        [MenuItem("Assets/Create/Kickstarter/Blend File")]
        public static void CreateBlendFile()
        {
            string selectedFolderPath = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                selectedFolderPath = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(selectedFolderPath))
                {
                    selectedFolderPath = Path.GetDirectoryName(selectedFolderPath);
                }
                break;
            }

            string fileName = "New Model.blend";

            string filePath = Path.Combine(selectedFolderPath, fileName);

            FileStream fileStream = File.Create(filePath);
            fileStream.Close();

            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/Kickstarter/UI Document")]
        public static void CreateUIDocument()
        {
            string selectedFolderPath = "Assets";

            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                selectedFolderPath = AssetDatabase.GetAssetPath(obj);
                if (File.Exists(selectedFolderPath))
                {
                    selectedFolderPath = Path.GetDirectoryName(selectedFolderPath);
                }
                break;
            }

            string documentName = "NewDocument.uxml";
            string stylesheetName = "NewStylesheet.uss";
            string folderName = "NewUI";
            string newFolderPath = Path.Combine(selectedFolderPath, folderName);
            if (!AssetDatabase.IsValidFolder(newFolderPath))
            {
                AssetDatabase.CreateFolder(selectedFolderPath, folderName);
            }

            string uxmlPath = Path.Combine(newFolderPath, documentName);
            string ussPath = Path.Combine(newFolderPath, stylesheetName);

            // Create uxml file and write default data
            string defaultUXMLData = "<engine:UXML xmlns:engine=\"UnityEngine.UIElements\">\n</engine:UXML>";
;
            File.WriteAllText(uxmlPath, defaultUXMLData);

            // Create uss file and write default data
            string defaultUSSData = "VisualElement \r\n{\r\n\r\n}";
            File.WriteAllText(ussPath, defaultUSSData);

            AssetDatabase.Refresh();
        }
    }
}
