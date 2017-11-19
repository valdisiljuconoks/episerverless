namespace Shared.Models
{
    public class SettingsMessage

    {
        public SettingsMessage(string storageUrl, string doneQueueName)
        {
            StorageUrl = storageUrl;
            DoneQueueName = doneQueueName;
        }

        public string DoneQueueName { get; }
        public string StorageUrl { get; }
        public string RequestFunctionUri { get; set; }
    }
}
