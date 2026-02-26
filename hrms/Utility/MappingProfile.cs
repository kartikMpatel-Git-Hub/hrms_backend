using AutoMapper;
using hrms.Dto.Request.BookingSlot;
using hrms.Dto.Request.Game;
using hrms.Dto.Request.Post;
using hrms.Dto.Request.Post.Comment;
using hrms.Dto.Request.Post.Tag;
using hrms.Dto.Response.BookingSlot;
using hrms.Dto.Response.DailyCelebration;
using hrms.Dto.Response.Department;
using hrms.Dto.Response.Expense;
using hrms.Dto.Response.Expense.Category;
using hrms.Dto.Response.Expense.Proof;
using hrms.Dto.Response.Game;
using hrms.Dto.Response.Game.offere;
using hrms.Dto.Response.Job;
using hrms.Dto.Response.Notification;
using hrms.Dto.Response.Other;
using hrms.Dto.Response.Post;
using hrms.Dto.Response.Post.Comment;
using hrms.Dto.Response.Post.Tag;
using hrms.Dto.Response.Referred;
using hrms.Dto.Response.Share;
using hrms.Dto.Response.Travel;
using hrms.Dto.Response.Travel.Document;
using hrms.Dto.Response.Traveler;
using hrms.Dto.Response.User;
using hrms.Model;

namespace hrms.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Department, DepartmentResponseDto>();
            CreateMap<User, UserResponseDto>()
                .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => GetRoleName(src.Role)));

            CreateMap<User, UserProfileResponseDto>()
                .ForMember(
                dest => dest.Team,
                opt => opt.MapFrom(src => src.Employees))
                .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => GetRoleName(src.Role)));
            
            CreateMap<Travel, TravelResponseDto>()
                .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Desciption));
            CreateMap<Travel, TravelWithTravelerResponseDto>()
                .ForMember(
                dest => dest.Description,
                opt => opt.MapFrom(src => src.Desciption));
            CreateMap<Traveler, TravelerDto>();
            CreateMap<ExpenseCategory, ExpenseCategoryResponseDto>();
            CreateMap<Expense, ExpenseResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));
            CreateMap<ExpenseProof, ExpenseProofResponseDto>();
            CreateMap<TravelDocument, TravelDocumentResponseDto>();
            CreateMap<Notification, NotificationResponseDto>();
            CreateMap<Job, JobResponseDto>();
            CreateMap<Job, JobResponseWithReviewersDto>();
            CreateMap<JobReviewer, JobReviewerResponseDto>();
            CreateMap<JobReferral, ReferralResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)))
                .ForMember(
                dest => dest.Referer,
                opt => opt.MapFrom(src => src.Referer.Email)
                );
            CreateMap<JobShared, SharedJobResponseDto>()
                .ForMember(
                dest => dest.Shared,
                opt => opt.MapFrom(src => src.Shared.Email)
                );

            CreateMap<BookingSlotCreateDto, BookingSlot>();
            CreateMap<BookingSlot, BookingSlotResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));

            CreateMap<BookingSlot, BookingSlotWithPlayerResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));

            CreateMap<SlotOffere, SlotOffereResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));

            CreateMap<BookingPlayer, BookingPlayerResponseDto>();

            CreateMap<PostCreateDto, Post>();
            CreateMap<PostUpdateDto, Post>();
            CreateMap<Post, PostResponseDto>()
                .ForMember(
                dest => dest.LikeCount,
                opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(
                dest => dest.CommentCount,
                opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.created_at))
                .ForMember(
                dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.updated_at));

            CreateMap<Post, PostDetailResponseDto>()
                .ForMember(
                dest => dest.LikeCount,
                opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(
                dest => dest.CommentCount,
                opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.created_at))
                .ForMember(
                dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.updated_at));

            CreateMap<TagCreateDto, Tag>();
            CreateMap<Tag, TagResponseDto>();

            CreateMap<CommentCreateDto, PostComment>();
            CreateMap<CommentUpdateDto, PostComment>();
            CreateMap<PostComment, CommentResponseDto>()
                .ForMember(
                dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.created_at))
                .ForMember(
                dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.updated_at));
            CreateMap<User, UserMinimalResponseDto>();

            CreateMap<GameCreateDto, Game>();
            CreateMap<GameUpdateDto, Game>();
            CreateMap<Game, GameResponseDto>();
            CreateMap<Game, GameResponseWithSlot>();

            CreateMap<GameOperationWindowCreateDto, GameOperationWindow>();
            CreateMap<GameOperationWindow, GameOperationWindowResponseDto>();

            CreateMap<GameSlotWaiting, GameSlotWaitinglistResponseDto>();
            CreateMap<GameSlot, GameSlotResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));
            CreateMap<GameSlot, GameSlotDetailResponseDto>();
            CreateMap<GameSlotPlayer, GameSlotPlayerResponseDto>();
            CreateMap<GameSlotWaiting, GameSlotWaitinglistResponseDto>();
            CreateMap<GameSlotWaitingPlayer, WaitlistPlayerResponseDto>();
            CreateMap<DailyCelebration, DailyCelebrationResponseDto>();
            CreateMap<GameSlot, UpcomingBookingSlotResponseDto>()
                .ForMember(
                dest => dest.PlayerCount,
                opt => opt.MapFrom(src => src.Players != null ? src.Players.Count : 0)
                )
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => GetStatus(src.Status)));

            CreateMap(typeof(PagedReponseOffSet<>), typeof(PagedReponseDto<>));
        }

        private object GetStatus(SlotOfferStatus status)
        {
            switch (status)
            {
                case SlotOfferStatus.Accepted:
                    return "Accepted";
                case SlotOfferStatus.InProcess:
                    return "InProcess";
                case SlotOfferStatus.Pending:
                    return "Pending";
                case SlotOfferStatus.Expired:
                    return "Expired";
                default:
                    return "Unknown";
            }
        }

        private object GetStatus(GameSlotStatus status)
        {
            switch (status)
            {
                case GameSlotStatus.WAITING:
                    return "Waiting";
                case GameSlotStatus.BOOKED:
                    return "Booked";
                case GameSlotStatus.AVAILABLE:
                    return "Available";
                case GameSlotStatus.COMPLETED:
                    return "Completed";
                default:
                    return "Unknown";
            }
        }

        private object GetStatus(ReferralStatus status)
        {
            switch (status)
            {
                case ReferralStatus.Pending:
                    return "Pending";
                case ReferralStatus.Interview:
                    return "Interview";
                case ReferralStatus.Rejected:
                    return "Rejected";
                case ReferralStatus.Hired:
                    return "Hired";
                case ReferralStatus.Shortlisted:
                    return "Shortlisted";
                default:
                    return "Unknown";
            }
        }

        private object GetStatus(ExpenseStatus status)
        {
            switch (status)
            {
                case ExpenseStatus.APPROVED:
                    return "Approved";
                case ExpenseStatus.REJECTED:
                    return "Rejected";
                case ExpenseStatus.PENDING:
                    return "Pending";
                default:
                    return "Unknown";
            }
        }

        private object GetRoleName(UserRole role)
        {
            switch (role)
            {
                case UserRole.ADMIN:
                    return "Admin";
                case UserRole.HR:
                    return "Hr";
                case UserRole.MANAGER:
                    return "Manager";
                case UserRole.EMPLOYEE:
                    return "Employee";
                default:
                    return "Unknown";
            }
        }
    }
}
