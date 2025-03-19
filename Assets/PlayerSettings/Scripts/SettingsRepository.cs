using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PlayerSettings
{
    public class SettingsRepository : MonoBehaviour
    {
        [SerializeField] private string m_key;
        [SerializeField] private Section[] m_sections;

        private bool m_sectionsIsCreated = false;

        public string Key => m_key;

        public IReadOnlyCollection<Section> Sections
        {
            get
            {
                if (m_sectionsIsCreated == false)
                {
                    foreach (Section section in m_sections)
                    {
                        section.Create();
                    }

                    m_sectionsIsCreated = true;
                }

                return m_sections;
            }
        }

        private Encoding FileEncoding => Encoding.ASCII;

        public void Save()
        {
            using FileStream fileStream = FileRepository.GetFileStream(Key, FileAccess.Write);
            fileStream.SetLength(0); // Clear file

            BinaryWriter writer = new(fileStream, FileEncoding);
            SettingsSerialization.Serialize(writer, Sections);
        }

        public void Load()
        {
            using FileStream fileStream = FileRepository.GetFileStream(Key, FileAccess.Read);
            BinaryReader reader = new(fileStream, FileEncoding);

            try
            {
                SettingsSerialization.Deserialize(reader, Sections);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                fileStream.Close();
                Save();
            }
        }

        public void Apply()
        {
            foreach (Section section in m_sections)
            {
                section.Apply();
            }
        }
    }
}