using System.Linq;


namespace McGiv.TextKey
{
    public interface ITextRepository
    {
        IQueryable<TextItem> All();

        TextItem FindByKey(string key);

        string FindValueByKey(string key);
    }
}
