using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Simple3DEngine;

public class GLTFToPolygonConverter
{
    public static void ConvertGLTFToPolygon(string inputPath, string outputPath)
    {
        string gltfContent = File.ReadAllText(inputPath);
        List<Triangle3D> triangles = ExtractTriangles(gltfContent);

        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using Simple3DEngine;");
            writer.WriteLine();
            writer.WriteLine("public class GLTFMesh");
            writer.WriteLine("{");
            writer.WriteLine("    public static List<Triangle3D> GetMesh()");
            writer.WriteLine("    {");
            writer.WriteLine("        List<Triangle3D> triangles = new List<Triangle3D>();");
            writer.WriteLine();

            foreach (var triangle in triangles)
            {
                writer.WriteLine($"        triangles.Add(new Triangle3D(");
                writer.WriteLine($"            new Vector3D({triangle.Vertices[0].X}f, {triangle.Vertices[0].Y}f, {triangle.Vertices[0].Z}f),");
                writer.WriteLine($"            new Vector3D({triangle.Vertices[1].X}f, {triangle.Vertices[1].Y}f, {triangle.Vertices[1].Z}f),");
                writer.WriteLine($"            new Vector3D({triangle.Vertices[2].X}f, {triangle.Vertices[2].Y}f, {triangle.Vertices[2].Z}f)");
                writer.WriteLine("        ));");
            }

            writer.WriteLine();
            writer.WriteLine("        return triangles;");
            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        Console.WriteLine($"Conversion complete. Output written to {outputPath}");
    }

    private static List<Triangle3D> ExtractTriangles(string gltfContent)
    {
        List<Triangle3D> triangles = new List<Triangle3D>();
        List<Vector3D> vertices = ExtractVertices(gltfContent);
        List<int> indices = ExtractIndices(gltfContent);

        for (int i = 0; i < indices.Count; i += 3)
        {
            Vector3D v1 = vertices[indices[i]];
            Vector3D v2 = vertices[indices[i + 1]];
            Vector3D v3 = vertices[indices[i + 2]];
            triangles.Add(new Triangle3D(v1, v2, v3));
        }

        return triangles;
    }

    private static List<Vector3D> ExtractVertices(string gltfContent)
    {
        List<Vector3D> vertices = new List<Vector3D>();
        string pattern = @"""POSITION""[^]]+\[([^\]]+)\]";
        Match match = Regex.Match(gltfContent, pattern);
        if (match.Success)
        {
            string[] values = match.Groups[1].Value.Split(',');
            for (int i = 0; i < values.Length; i += 3)
            {
                float x = float.Parse(values[i]);
                float y = float.Parse(values[i + 1]);
                float z = float.Parse(values[i + 2]);
                vertices.Add(new Vector3D(x, y, z));
            }
        }
        return vertices;
    }

    private static List<int> ExtractIndices(string gltfContent)
    {
        List<int> indices = new List<int>();
        string pattern = @"""indices""[^]]+\[([^\]]+)\]";
        Match match = Regex.Match(gltfContent, pattern);
        if (match.Success)
        {
            string[] values = match.Groups[1].Value.Split(',');
            foreach (string value in values)
            {
                indices.Add(int.Parse(value));
            }
        }
        return indices;
    }
}