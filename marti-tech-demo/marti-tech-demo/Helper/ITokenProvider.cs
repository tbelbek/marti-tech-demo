namespace marti_tech_demo.Helper
{
    /// <summary>
    /// Token validasyonu için gereken metodları içerir.
    /// </summary>
    public interface ITokenProvider
    {
        bool TokenValidator(string token);
        string TokenGenerator();
    }
}