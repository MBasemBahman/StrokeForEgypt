namespace StrokeForEgypt.AdminApp.ViewModel
{
    public class ImageViewModel
    {
        public string LabelName { get; set; } = "Image Uploud";
        public string ImageName { get; set; } = "ImageFile";
        public string OldImageURL { get; set; } = "ImageFile";
        public string ImageTitle { get; set; }
    }

    public class MultiImageViewModel
    {
        public string Name { get; set; }
        public string UploudAction { get; set; }
        public int Id { get; set; }
    }

}
