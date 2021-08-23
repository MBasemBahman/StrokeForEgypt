using AutoMapper;
using Microsoft.AspNetCore.Http;
using StrokeForEgypt.BaseRepository;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Entity.BookingEntity;
using System.Threading.Tasks;

namespace StrokeForEgypt.Repository.BookingEntityRepository
{
    public class BookingMemberAttachmentRepository : AppBaseRepository<BookingMemberAttachment>
    {
        private readonly BaseDBContext DBContext;
        private readonly IMapper _Mapper;

        public BookingMemberAttachmentRepository(BaseDBContext DBContext, IMapper Mapper) : base(DBContext)
        {
            this.DBContext = DBContext;
            _Mapper = Mapper;
        }

        public async Task UploudFile(int Id, IFormFile File, string FolderURL = "Uploud/BookingMemberAttachment")
        {
            if (File != null)
            {
                ImgManager ImgManager = new(AppMainData.WebRootPath);

                string FileURL = await ImgManager.UploudImage(AppMainData.DomainName, Id.ToString(), File, FolderURL);

                if (!string.IsNullOrEmpty(FileURL))
                {
                    FileURL = FileURL.Replace("wwwroot/", "");

                    CreateEntity(new BookingMemberAttachment
                    {
                        Fk_BookingMember = Id,
                        FileURL = FileURL,
                        FileLength = File.Length,
                        FileName = File.FileName,
                        FileType = File.ContentType,
                    });

                    await Save();
                }
            }
        }
    }
}
