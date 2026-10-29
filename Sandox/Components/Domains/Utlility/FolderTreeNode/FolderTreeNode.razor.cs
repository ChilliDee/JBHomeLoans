using Microsoft.AspNetCore.Components;

namespace FE.Components.Domains.Utlility.FolderTreeNode
{
    public partial class FolderTreeNode : ComponentBase
    {
        [Parameter] public Folder CurrentFolder { get; set; } // TODO: swap in your actual folder DTO type

        [Parameter] public Dictionary<string, List<Folder>> FoldersByParentKey { get; set; }

        [Parameter] public Func<string, string> GetFolderRowCss { get; set; }
        [Parameter] public Func<string, bool> IsFolderExpanded { get; set; }
        [Parameter] public Func<string, bool> AreChildFoldersLoaded { get; set; }
        [Parameter] public Func<string, Task> GetChildFoldersAsync { get; set; }
        [Parameter] public Action<string> ApplyChildFoldersToTree { get; set; }
        [Parameter] public Action<string> ToggleFolderExpanded { get; set; }
        [Parameter] public Action<string> SelectFolder { get; set; }
        [Parameter] public Func<Task> OnStateUpdate { get; set; }

        private async Task ToggleAsync()
        {
            if (!AreChildFoldersLoaded(CurrentFolder.Id))
            {
                await GetChildFoldersAsync(CurrentFolder.Id);
                ApplyChildFoldersToTree(CurrentFolder.Id);
            }

            ToggleFolderExpanded(CurrentFolder.Id);
            await OnStateUpdate();
        }

        private async Task SelectAsync()
        {
            SelectFolder(CurrentFolder.Id);
            await OnStateUpdate();
        }

        public class Folder
        {
            public string Id { get; set; }

            public string DisplayName { get; set; }

            public string ParentFolderId { get; set; }

            public int ChildFolderCount { get; set; }

            public int UnreadItemCount { get; set; }

            public int TotalItemCount { get; set; }
        }
    }
}
