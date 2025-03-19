namespace PlayerSettings
{
    public abstract class Option<T> : BaseOption
    {
        public abstract T Value { get; set; }
    }
}