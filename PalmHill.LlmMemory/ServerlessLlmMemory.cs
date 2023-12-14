using Microsoft.KernelMemory;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama;
using System.Collections.Concurrent;

namespace PalmHill.LlmMemory
{
    public class ServerlessLlmMemory
    {
        public ServerlessLlmMemory(IKernelMemory kernelMemory)
        {
            KernelMemory = kernelMemory;
        }

        public IKernelMemory KernelMemory { get; }


        public ConcurrentDictionary<Guid, AttachmentInfo> AttachmentInfos { get; } = new ConcurrentDictionary<Guid, AttachmentInfo>();

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
                documentId = await KernelMemory.ImportDocumentAsync(stream,
                attachmentInfo.Name,
                attachmentInfo.Id.ToString(),
                tagCollection,
                attachmentInfo.ConversationId.ToString());
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
            var isDocReady = await KernelMemory.IsDocumentReadyAsync(attachmentInfo.Id.ToString(), attachmentInfo.ConversationId.ToString());

            if (attachmentInfo != null && attachmentInfo?.Status != AttachmentStatus.Failed)
            {
                attachmentInfo!.Status = isDocReady ? AttachmentStatus.Uploaded : AttachmentStatus.Pending;
            }
        }

        public async Task<bool> DeleteDocument(Guid conversationId, Guid attachmentId)
        {
            await KernelMemory.DeleteDocumentAsync(attachmentId.ToString(), conversationId.ToString());
            var removed = AttachmentInfos.Remove(attachmentId, out _);
            return removed;
        }

        public async Task<SearchResult> SearchAsync(string conversationId, string query)
        {
            var results = await KernelMemory.SearchAsync(query, conversationId);

            return results;
        }

        public async Task<MemoryAnswer> Ask(string conversationId, string query, CancellationToken cancellationToken)
        {
            var processedQuery = processQuery(query);
            Exception? exception;
            try
            {
                await Llama.ThreadLock.InferenceLock.WaitAsync(cancellationToken);
                var results = await KernelMemory.AskAsync(processedQuery, conversationId, cancellationToken: cancellationToken);
                return results;
            }
            catch (OperationCanceledException ex)
            {
                exception = ex;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                Llama.ThreadLock.InferenceLock.Release();
            }

            throw exception;


        }

        private string processQuery(string query)
        {
            var processedQuery = query.Trim();

            return processedQuery;
        }

    }
}
