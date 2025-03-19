using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PlayerSettings
{
    public static class SettingsSerialization
    {
        public static void Serialize(BinaryWriter writer, IEnumerable<Section> sections)
        {
            foreach (Section section in sections)
            {
                foreach (ISerializableOption option in section.Options)
                {
                    try
                    {
                        SerializeOption(writer, option);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        public static void Deserialize(BinaryReader reader, IEnumerable<Section> sections)
        {
            Dictionary<string, ISerializableOption> options = new();

            foreach (Section section in sections)
            {
                foreach (ISerializableOption option in section.Options)
                {
                    string optionName = option.OptionName;

                    if (options.ContainsKey(optionName))
                        throw new ArgumentException($"Two options named {optionName} were found", nameof(sections));

                    options.Add(optionName, option);
                }
            }

            while (Remainder(reader) > 0)
            {
                (string name, byte[] data) = DeserializeOption(reader);

                if (options.TryGetValue(name, out ISerializableOption option))
                    option.Deserialize(data);
                else
                    Debug.Log($"An option with name {name} does not exists");
            }
        }

        private static long Remainder(BinaryReader reader)
        {
            return reader.BaseStream.Length - reader.BaseStream.Position;
        }

        private static void SerializeOption(BinaryWriter writer, ISerializableOption option)
        {
            byte[] data = option.Serialize();

            writer.Write(option.OptionName);
            SerializeBytesArray(writer, data);
        }

        private static (string name, byte[] data) DeserializeOption(BinaryReader reader)
        {
            string name = reader.ReadString();
            byte[] data = DeserializeBytesArray(reader);

            return (name, data);
        }

        private static void SerializeBytesArray(BinaryWriter writer, byte[] data)
        {
            writer.Write((int)data.Length);
            writer.Write(data);
        }

        private static byte[] DeserializeBytesArray(BinaryReader reader)
        {
            int length = reader.ReadInt32();

            byte[] data = new byte[length];

            reader.Read(data, 0, length);

            return data;
        }
    }
}