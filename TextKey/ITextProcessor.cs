
namespace McGiv.TextKey
{
    public interface ITextProcessor
    {
        string Process(string key);
    }


    public interface IKeyProcessor : ITextProcessor
    {
        string Argument { get; set; }

        string Key { get; }
    }
}
