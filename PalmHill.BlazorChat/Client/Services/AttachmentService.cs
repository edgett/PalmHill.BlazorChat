using Microsoft.AspNetCore.Components.Forms;
using PalmHill.BlazorChat.ApiClient;
using PalmHill.BlazorChat.Shared.Models;
using Refit;

namespace PalmHill.BlazorChat.Client.Services
{
    public class AttachmentService
    {
        public AttachmentService(
            BlazorChatApi blazorChatApi,
            Guid conversationId
            )
        {
            _blazorChatApi = blazorChatApi;
            ConversationId = conversationId;

        }

        private readonly BlazorChatApi _blazorChatApi;

        public Guid ConversationId { get; set; }

        /// <summary>
        /// The list of files that have been uploaded for Chat.
        /// </summary>
        public List<AttachmentInfo> UploadedFiles { get; private set; } = new List<AttachmentInfo>();

        /// <summary>
        /// Whether the chat is in Attachment mode. Will only reference attached douments.
        /// </summary>
        public bool AttachmentsEnabled { get; set; } = false;
        /// <summary>
        /// Show the attachment panel.
        /// </summary>
        public bool AttachmentsVisible { get; set; } = false;
        /// <summary>
        /// The list of files that have been selected for Chat. This is non-functional for now.
        /// </summary>
        public List<AttachmentInfo> SelectedFiles { get; private set; } = new List<AttachmentInfo>();


        /// <summary>
        /// Uploads files to the server when the user selects them.
        /// Adds to the <see cref="Controller"/>'s UploadedFiles list.
        /// </summary>
        /// <param name="e">The selected files.</param>
        public async Task UploadAttachments(InputFileChangeEventArgs e)
        {

            var files = e.GetMultipleFiles();
            var uploadedCount = 0;
            var uploadTasks = new List<Task>();

            foreach (var file in files)
            {
                var attachmentInfo = new AttachmentInfo();
                attachmentInfo.ConversationId = ConversationId;
                attachmentInfo.Name = file.Name;
                attachmentInfo.Size = file.Size;
                attachmentInfo.ContentType = file.ContentType;
                attachmentInfo.Status = AttachmentStatus.Pending;

                UploadedFiles.Add(attachmentInfo);

                var uploadTask = new Task(async () =>
                {
                    if (attachmentInfo?.ConversationId is null)
                    {
                        attachmentInfo!.Status = AttachmentStatus.Failed;
                        return;
                    }

                    var stream = file.OpenReadStream(10000000);
                    var streamPart = new StreamPart(stream, file.Name, file.ContentType);
                    var apiResponse = await _blazorChatApi.Attachment.AddAttachment(attachmentInfo.ConversationId.Value, attachmentInfo.Id, streamPart);
                    uploadedCount++;

                    if (!apiResponse.IsSuccessStatusCode)
                    {
                        attachmentInfo.Status = AttachmentStatus.Failed;
                    }
                });

                uploadTasks.Add(uploadTask);
            }

            foreach (var uploadTask in uploadTasks)
            {
                uploadTask.Start();
                await uploadTask;
            }
        }

        /// <summary>
        /// Toggles the <see cref="AttachmentsVisible"/> property.
        /// This does not toggle the <see cref="AttachmentsEnabled"/> property.
        /// </summary>
        public void ToggleAttachmentsVisible()
        {
            AttachmentsVisible = !AttachmentsVisible;
            StateHasChanged();
        }

        /// <summary>
        /// Shows the <see cref="Client.Components.Attachment.AttachmentManager"/>.
        /// </summary>
        public void ShowAttachments()
        {
            AttachmentsVisible = true;
            StateHasChanged();
        }

        /// <summary>
        /// Hides the <see cref="Client.Components.Attachment.AttachmentManager"/>.
        /// </summary>
        public void HideAttachments()
        {
            AttachmentsVisible = false;
            StateHasChanged();
        }

        /// <summary>
        /// Invokes the <see cref="OnStateChange"/> event.
        /// </summary>
        private void StateHasChanged()
        {
            OnStateChange?.Invoke(this, true);
        }

        /// <summary>
        /// Event handler for when the state changes.
        /// </summary>
        public event EventHandler<bool>? OnStateChange;

    }
}
