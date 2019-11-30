namespace marti_tech_demo.Helper
{
    public interface ITokenProvider
    {
        bool TokenValidator(string token);
        string TokenGenerator();
    }
}