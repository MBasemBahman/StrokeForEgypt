﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StrokeForEgypt.Entity.CommonEntity
{
    public class AttachmentEntity : BaseEntity
    {
        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("File Type")]
        public string FileType { get; set; }

        [DisplayName("File Size")]
        [DefaultValue(0)]
        public double FileLength { get; set; }

        [DisplayName("File URL")]
        [DataType(DataType.ImageUrl)]
        [Url]
        public string FileURL { get; set; }
    }
}
