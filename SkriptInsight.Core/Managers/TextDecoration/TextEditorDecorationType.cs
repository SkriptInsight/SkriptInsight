using JetBrains.Annotations;

namespace SkriptInsight.Core.Managers.TextDecoration
{
    [UsedImplicitly]
    public class TextEditorDecorationType
    {
        public TextEditorDecorationType(string key)
        {
            Key = key;
        }

        [UsedImplicitly]
        public string Key { get; }

        public void Dispose()
        {
            WorkspaceManager.CurrentHost.SendRawNotification("insight/disposeDecoration", this);
        }
    }
}