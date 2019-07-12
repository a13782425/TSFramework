
using UnityEngine;
using UnityEditor;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/// <summary>
/// Helper class that takes care of loading BMFont's glyph information from the specified byte array.
/// This functionality is not a part of BMFont anymore because Flash export option can't handle System.IO functions.
/// </summary>

public static class BMFontReader
{
    /// <summary>
    /// Helper function that retrieves the string value of the key=value pair.
    /// </summary>

    static string GetString(string s)
    {
        int idx = s.IndexOf('=');
        return (idx == -1) ? "" : s.Substring(idx + 1);
    }

    static int GetXmlInt(string text)
    {
        int.TryParse(text, out int val);
        return val;
    }

    /// <summary>
    /// Helper function that retrieves the integer value of the key=value pair.
    /// </summary>

    static int GetInt(string s)
    {
        int val = 0;
        string text = GetString(s);
#if UNITY_FLASH
		try { val = int.Parse(text); } catch (System.Exception) { }
#else
        int.TryParse(text, out val);
#endif
        return val;
    }

    /// <summary>
    /// Reload the font data.
    /// </summary>

    static public void Load(BMFont font, string name, byte[] bytes)
    {
        font.Clear();

        if (bytes != null)
        {
            ByteReader reader = new ByteReader(bytes);
            char[] separator = new char[] { ' ' };

            while (reader.canRead)
            {
                string line = reader.ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                string[] split = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                int len = split.Length;

                if (split[0] == "char")
                {
                    // Expected data style:
                    // char id=13 x=506 y=62 width=3 height=3 xoffset=-1 yoffset=50 xadvance=0 page=0 chnl=15

                    int channel = (len > 10) ? GetInt(split[10]) : 15;

                    if (len > 9 && GetInt(split[9]) > 0)
                    {
                        Debug.LogError("Your font was exported with more than one texture. Only one texture is supported by NGUI.\n" +
                            "You need to re-export your font, enlarging the texture's dimensions until everything fits into just one texture.");
                        break;
                    }

                    if (len > 8)
                    {
                        int id = GetInt(split[1]);
                        BMGlyph glyph = font.GetGlyph(id, true);

                        if (glyph != null)
                        {
                            glyph.x = GetInt(split[2]);
                            glyph.y = GetInt(split[3]);
                            glyph.width = GetInt(split[4]);
                            glyph.height = GetInt(split[5]);
                            glyph.offsetX = GetInt(split[6]);
                            glyph.offsetY = GetInt(split[7]);
                            glyph.advance = GetInt(split[8]);
                            glyph.channel = channel;
                        }
                        else Debug.Log("Char: " + split[1] + " (" + id + ") is NULL");
                    }
                    else
                    {
                        Debug.LogError("Unexpected number of entries for the 'char' field (" + name + ", " + split.Length + "):\n" + line);
                        break;
                    }
                }
                else if (split[0] == "kerning")
                {
                    // Expected data style:
                    // kerning first=84 second=244 amount=-5 

                    if (len > 3)
                    {
                        int first = GetInt(split[1]);
                        int second = GetInt(split[2]);
                        int amount = GetInt(split[3]);

                        BMGlyph glyph = font.GetGlyph(second, true);
                        if (glyph != null) glyph.SetKerning(first, amount);
                    }
                    else
                    {
                        Debug.LogError("Unexpected number of entries for the 'kerning' field (" +
                            name + ", " + split.Length + "):\n" + line);
                        break;
                    }
                }
                else if (split[0] == "common")
                {
                    // Expected data style:
                    // common lineHeight=64 base=51 scaleW=512 scaleH=512 pages=1 packed=0 alphaChnl=1 redChnl=4 greenChnl=4 blueChnl=4

                    if (len > 5)
                    {
                        font.charSize = GetInt(split[1]);
                        font.baseOffset = GetInt(split[2]);
                        font.texWidth = GetInt(split[3]);
                        font.texHeight = GetInt(split[4]);

                        int pages = GetInt(split[5]);

                        if (pages != 1)
                        {
                            Debug.LogError("Font '" + name + "' must be created with only 1 texture, not " + pages);
                            break;
                        }
                    }
                    else
                    {
                        Debug.LogError("Unexpected number of entries for the 'common' field (" +
                            name + ", " + split.Length + "):\n" + line);
                        break;
                    }
                }
                else if (split[0] == "page")
                {
                    // Expected data style:
                    // page id=0 file="textureName.png"

                    if (len > 2)
                    {
                        font.spriteName = GetString(split[2]).Replace("\"", "");
                        font.spriteName = font.spriteName.Replace(".png", "");
                        font.spriteName = font.spriteName.Replace(".tga", "");
                    }
                }
            }
        }
    }

    public static void Load(BMFont font, string name, string text)
    {
        font.Clear();
        string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines[0].Contains("xml"))
        {
            //XML
            ReadXml(font, name, text);
        }
        else
        {
            ReadText(font, name, lines);
        }
    }

    private static void ReadXml(BMFont font, string name, string text)
    {

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(text);
        XmlNode commonNode = doc.SelectSingleNode("/font/common");
        if (commonNode != null)
        {
            font.charSize = GetXmlInt(commonNode.Attributes["lineHeight"].Value);
            font.baseOffset = GetXmlInt(commonNode.Attributes["base"].Value);
            font.texWidth = GetXmlInt(commonNode.Attributes["scaleW"].Value);
            font.texHeight = GetXmlInt(commonNode.Attributes["scaleH"].Value);

            int pages = GetXmlInt(commonNode.Attributes["pages"].Value);
            if (pages != 1)
            {
                Debug.LogError("Font '" + name + "' must be created with only 1 texture, not " + pages);
                return;
            }
        }
        XmlNode pageNode = doc.SelectSingleNode("/font/pages/page");
        if (pageNode != null)
        {
            font.spriteName = pageNode.Attributes["file"].Value.Replace(".png", "");
        }
        XmlNodeList charNodes = doc.SelectNodes("/font/chars/char");

        foreach (XmlNode node in charNodes)
        {
            string chnl = node.Attributes["chnl"].Value;
            int channel = string.IsNullOrWhiteSpace(chnl) ? 15 : GetXmlInt(chnl);
            int id = GetXmlInt(node.Attributes["id"].Value);
            BMGlyph glyph = font.GetGlyph(id, true);
            if (glyph != null)
            {
                glyph.x = GetXmlInt(node.Attributes["x"].Value);
                glyph.y = GetXmlInt(node.Attributes["y"].Value);
                glyph.width = GetXmlInt(node.Attributes["width"].Value);
                glyph.height = GetXmlInt(node.Attributes["height"].Value);
                glyph.offsetX = GetXmlInt(node.Attributes["xoffset"].Value);
                glyph.offsetY = GetXmlInt(node.Attributes["yoffset"].Value);
                glyph.advance = GetXmlInt(node.Attributes["xadvance"].Value);
                glyph.channel = channel;
            }
            else Debug.Log("Char: " + node.Attributes["id"] + " (" + id + ") is NULL");
        }

    }

    private static void ReadText(BMFont font, string name, string[] lines)
    {
        char[] separator = new char[] { ' ' };
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] split = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
            int len = split.Length;
            if (split[0] == "char")
            {
                // Expected data style:
                // char id=13 x=506 y=62 width=3 height=3 xoffset=-1 yoffset=50 xadvance=0 page=0 chnl=15

                int channel = (len > 10) ? GetInt(split[10]) : 15;

                if (len > 9 && GetInt(split[9]) > 0)
                {
                    Debug.LogError("Your font was exported with more than one texture. Only one texture is supported by NGUI.\n" +
                        "You need to re-export your font, enlarging the texture's dimensions until everything fits into just one texture.");
                    break;
                }

                if (len > 8)
                {
                    int id = GetInt(split[1]);
                    BMGlyph glyph = font.GetGlyph(id, true);

                    if (glyph != null)
                    {
                        glyph.x = GetInt(split[2]);
                        glyph.y = GetInt(split[3]);
                        glyph.width = GetInt(split[4]);
                        glyph.height = GetInt(split[5]);
                        glyph.offsetX = GetInt(split[6]);
                        glyph.offsetY = GetInt(split[7]);
                        glyph.advance = GetInt(split[8]);
                        glyph.channel = channel;
                    }
                    else Debug.Log("Char: " + split[1] + " (" + id + ") is NULL");
                }
                else
                {
                    Debug.LogError("Unexpected number of entries for the 'char' field (" + name + ", " + split.Length + "):\n" + line);
                    break;
                }
            }
            else if (split[0] == "kerning")
            {
                // Expected data style:
                // kerning first=84 second=244 amount=-5 

                if (len > 3)
                {
                    int first = GetInt(split[1]);
                    int second = GetInt(split[2]);
                    int amount = GetInt(split[3]);

                    BMGlyph glyph = font.GetGlyph(second, true);
                    if (glyph != null) glyph.SetKerning(first, amount);
                }
                else
                {
                    Debug.LogError("Unexpected number of entries for the 'kerning' field (" +
                        name + ", " + split.Length + "):\n" + line);
                    break;
                }
            }
            else if (split[0] == "common")
            {
                // Expected data style:
                // common lineHeight=64 base=51 scaleW=512 scaleH=512 pages=1 packed=0 alphaChnl=1 redChnl=4 greenChnl=4 blueChnl=4

                if (len > 5)
                {
                    font.charSize = GetInt(split[1]);
                    font.baseOffset = GetInt(split[2]);
                    font.texWidth = GetInt(split[3]);
                    font.texHeight = GetInt(split[4]);

                    int pages = GetInt(split[5]);

                    if (pages != 1)
                    {
                        Debug.LogError("Font '" + name + "' must be created with only 1 texture, not " + pages);
                        break;
                    }
                }
                else
                {
                    Debug.LogError("Unexpected number of entries for the 'common' field (" +
                        name + ", " + split.Length + "):\n" + line);
                    break;
                }
            }
            else if (split[0] == "page")
            {
                // Expected data style:
                // page id=0 file="textureName.png"

                if (len > 2)
                {
                    font.spriteName = GetString(split[2]).Replace("\"", "");
                    font.spriteName = font.spriteName.Replace(".png", "");
                    font.spriteName = font.spriteName.Replace(".tga", "");
                }
            }
        }
    }
}
