using System;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Shared.Models
{
    public static class ObjectQueueExtensions
    {
        public static CloudQueueMessage AsQueueItem(this object target)
        {
            if(target == null)
                throw new ArgumentNullException(nameof(target));

            return new CloudQueueMessage(JsonConvert.SerializeObject(target));
        }
    }
}
