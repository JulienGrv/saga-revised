namespace Saga
{
    public interface IPlugin
    {
        void LoadPlugin(IHostPlugin host);

        void UnloadPlugin(IHostPlugin host);
    }
}