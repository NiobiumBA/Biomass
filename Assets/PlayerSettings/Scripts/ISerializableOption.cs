namespace PlayerSettings
{
    public interface ISerializableOption
    {
        string OptionName { get; set; }

        byte[] Serialize();

        void Deserialize(byte[] bytes);
    }
}