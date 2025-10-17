namespace FreshlyBackendNew.Constants
{
    /// <summary>
    /// Contains constant GUIDs for order statuses
    /// </summary>
    public static class OrderStatuses
    {
        // Status GUIDs from your database
        public static readonly Guid Delivered = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
        public static readonly Guid OrderPlaced = Guid.Parse("..."); // Add your actual GUID
        public static readonly Guid OrderPickedUp = Guid.Parse("..."); // Add your actual GUID
        public static readonly Guid ProcessingInLaundry = Guid.Parse("..."); // Add your actual GUID
        public static readonly Guid FinishedProcessing = Guid.Parse("..."); // Add your actual GUID
        public static readonly Guid OutForDelivery = Guid.Parse("..."); // Add your actual GUID
    }
}