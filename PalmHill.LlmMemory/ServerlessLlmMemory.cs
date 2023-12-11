using Microsoft.KernelMemory;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama;
using System.Collections.Concurrent;
using System.Threading;

namespace PalmHill.LlmMemory
{
    public class ServerlessLlmMemory
    {
        public ServerlessLlmMemory(IKernelMemory kernelMemory)
        {
            KernelMemory = kernelMemory;
        }

        public IKernelMemory KernelMemory { get; }


        public ConcurrentDictionary<string, AttachmentInfo> AttachmentInfos { get; } = new ConcurrentDictionary<string, AttachmentInfo>();

        public async Task<AttachmentInfo> ImportDocumentAsync(
            AttachmentInfo attachmentInfo,
            TagCollection? tagCollection = null
            )
        {
            if (attachmentInfo.FileBytes == null)
            {
                throw new InvalidOperationException("FileBytes is null");
            }

            if (!AttachmentInfos.TryAdd(attachmentInfo.Id, attachmentInfo))
            {
                throw new Exception("Failed to add attachment to memory");
            };

            attachmentInfo.Size = attachmentInfo.FileBytes.LongLength;

            await ThreadLock.InferenceLock.WaitAsync();

            var stream = new MemoryStream(attachmentInfo.FileBytes);
            var documentId = string.Empty;
            try
            {
              documentId   = await KernelMemory.ImportDocumentAsync(stream,
              attachmentInfo.Name,
              attachmentInfo.Id,
              tagCollection,
              attachmentInfo.ConversationId);
            }
            catch (Exception ex)
            {
                attachmentInfo.Status = AttachmentStatus.Failed;
                Console.WriteLine(ex);
            }
            finally
            {
                ThreadLock.InferenceLock.Release();
            }



            if (documentId == null)
            {
                attachmentInfo.Status = AttachmentStatus.Failed;
            }


            while (attachmentInfo.Status == AttachmentStatus.Pending)
            {
                await UpdateAttachmentStatus(attachmentInfo);

                if (
                    attachmentInfo.Status == AttachmentStatus.Uploaded
                    ||
                    attachmentInfo.Status == AttachmentStatus.Failed
                   )
                {
                    break;
                }

                System.Threading.Thread.Sleep(100);
            }

            return attachmentInfo;
        }

        public async Task UpdateAttachmentStatus(AttachmentInfo attachmentInfo)
        {
            var isDocReady = await KernelMemory.IsDocumentReadyAsync(attachmentInfo.Id, attachmentInfo.ConversationId);

            if (attachmentInfo != null && attachmentInfo?.Status != AttachmentStatus.Failed)
            {
                attachmentInfo!.Status = isDocReady ? AttachmentStatus.Uploaded : AttachmentStatus.Pending;
            }
        }

        public async Task<bool> DeleteDocument(string conversationId, string attachmentId)
        {
            await KernelMemory.DeleteDocumentAsync(attachmentId, conversationId);
            var removed = AttachmentInfos.Remove(attachmentId, out _);


            return true;
        }

        public async Task<SearchResult> SearchAsync(string conversationId, string query)
        {
            var results = await KernelMemory.SearchAsync(query, conversationId);

            return results;
        }

        public async Task<MemoryAnswer> Ask(string conversationId, string query)
        {
            var processedQuery = processQuery(query);
            var results = await KernelMemory.AskAsync(processedQuery, conversationId);

            return results;
        }

        private string processQuery(string query)
        {
            var processedQuery = query.Trim();

            return processedQuery;
        }

    }
}
