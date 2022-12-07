using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class EnumEditor
{
    public static void WriteToFile(string name, string path)
    {
        List<string> data = File.ReadAllLines(path).ToList();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Contains("}"))
            {
                name = name.Replace(" ", "");

                var firstCharater = char.ToUpper(name[0]).ToString();

                name = firstCharater + (name.Length >= 2 ? name.Substring(1) : "");
                data.Insert(i, "\t" + name + ",");
                break;
            }
        }

        File.WriteAllLines(path, data, Encoding.Default);
    }

    public static bool TryRemoveFromFile(string name, string path)
    {
        List<string> data = File.ReadAllLines(path).ToList();

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].Contains(name))
            {
                data.RemoveAt(i);
                File.WriteAllLines(path, data, Encoding.Default);
                return true;
            }
        }

        return false;
    }
}
