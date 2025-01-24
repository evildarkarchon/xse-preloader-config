using System;
using System.Xml.Linq;

namespace xse_preloader_config.Utils;

/// <summary>
/// Provides reusable utility methods for handling XML file operations.
/// </summary>
public static class XmlHelper
{
    /// <summary>
    /// Loads an XML file from the specified file path and retrieves its root element.
    /// </summary>
    /// <param name="filePath">The path to the XML file to be loaded.</param>
    /// <returns>The root element of the loaded XML document, or null if the document is empty.</returns>
    /// <exception cref="Exception">Thrown when the XML file cannot be loaded due to an error.</exception>
    public static XElement? LoadXml(string filePath)
    {
        try
        {
            var document = XDocument.Load(filePath);
            return document.Root;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load XML: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Saves the specified root XML element to a file at the given file path.
    /// </summary>
    /// <param name="filePath">The path where the XML file will be saved.</param>
    /// <param name="root">The root element of the XML document to be saved.</param>
    /// <exception cref="Exception">Thrown when the XML file cannot be saved due to an error.</exception>
    public static void SaveXml(string filePath, XElement root)
    {
        try
        {
            var document = new XDocument(root);
            document.Save(filePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save XML: {ex.Message}", ex);
        }
    }
}