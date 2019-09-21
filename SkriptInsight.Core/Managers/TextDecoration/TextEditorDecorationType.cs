namespace SkriptInsight.Core.Managers.TextDecoration
{
    public class TextEditorDecorationType
    {
        public TextEditorDecorationType(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public void Dispose()
        {
            WorkspaceManager.Instance.Current.Server.SendNotification("insight/disposeDecoration", this);
        }
    }
}