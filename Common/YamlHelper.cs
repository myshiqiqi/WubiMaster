using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WubiMaster.Common;

public static class YamlHelper
{
    private static IDeserializer _deserializer;
    private static IDeserializer _jsondeserializer;
    private static ISerializer _jsonserializer;
    private static ISerializer _serializer;

    static YamlHelper()
    {
        _serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        _deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        _jsondeserializer = new DeserializerBuilder().Build();
        _jsonserializer = new SerializerBuilder().JsonCompatible().Build();
    }

    /// <summary>
    /// 从文件中反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static T DeserializeFromFile<T>(string filePath)
    {
        var yaml = File.ReadAllText(filePath, Encoding.UTF8);
        return Deserizlize<T>(yaml);
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="yaml"></param>
    /// <returns></returns>
    public static T Deserizlize<T>(string yaml)
    {
        return _deserializer.Deserialize<T>(yaml);
    }

    /// <summary>
    /// 序列化
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string Serialize(object target)
    {
        return _serializer.Serialize(target);
    }

    /// <summary>
    /// 序列化到文件
    /// </summary>
    /// <param name="target"></param>
    /// <param name="filePath"></param>
    public static void SerizlizerToFile(object target, string filePath)
    {
        var content = Serialize(target);
        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    /// <summary>
    /// 写入Yaml文件
    /// </summary>
    /// <param name="target"></param>
    /// <param name="filePath"></param>
    public static void WriteYaml(object target, string filePath)
    {
        StreamWriter yamlWriter = File.CreateText(filePath);

        Serializer yamlSerializer = new Serializer();
        yamlSerializer.Serialize(yamlWriter, target);
        yamlWriter.Close();

        StringBuilder YamlHeaderStr = new StringBuilder();
        YamlHeaderStr.Append("# Rime settings\n");
        YamlHeaderStr.Append("# encoding: utf-8\n");
        YamlHeaderStr.Append("# author: 空山明月\n");
        YamlHeaderStr.Append("# modify: 中书君\n");
        YamlHeaderStr.Append("# time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\n");

        string[] yamlText = File.ReadAllLines(filePath);
        List<string> _list = yamlText.ToList();
        _list.Insert(0, YamlHeaderStr.ToString());
        File.WriteAllLines(filePath, _list.ToArray(), encoding: Encoding.UTF8);
    }

    /// <summary>
    /// yaml转json
    /// </summary>
    /// <param name="yaml"></param>
    /// <returns></returns>
    public static string YamlToJson(string yaml)
    {
        var yamlObject = _jsondeserializer.Deserialize(yaml);
        var json = _jsonserializer.Serialize(yamlObject);

        return json;
    }
}