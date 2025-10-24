using AutoMapper;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Models.Enums;

namespace CrimeManagementApi.Mappings
{
    /// <summary>
    /// Central AutoMapper configuration profile for the Crime Management API.
    /// Defines all entity-to-DTO and DTO-to-entity mappings used across the system.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ======================================================
            // 🔹 USER MANAGEMENT
            // ======================================================
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();

            // ======================================================
            // 🔹 AUTHENTICATION
            // ======================================================
            CreateMap<LoginDto, User>();
            CreateMap<User, AuthResponseDto>();

            // ======================================================
            // 🔹 CASES
            // ======================================================
            CreateMap<Case, CaseDto>()
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.FullName));
            CreateMap<CreateCaseDto, Case>();

            // ======================================================
            // 🔹 CASE ASSIGNEES
            // ======================================================
            CreateMap<CaseAssignee, CaseAssigneeDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown"))
                .ForMember(dest => dest.AssignedRole, opt => opt.MapFrom(src => src.User != null ? src.User.Role : "N/A"))
                .ForMember(dest => dest.ClearanceLevel,opt => opt.MapFrom(src => src.User != null? src.User.ClearanceLevel.ToString(): ClearanceLevel.Low.ToString()));
            CreateMap<CreateCaseAssigneeDto, CaseAssignee>();

            // ======================================================
            // 🔹 CASE PARTICIPANTS
            // ======================================================
            CreateMap<CreateCaseParticipantDto, CaseParticipant>();
            CreateMap<CaseParticipant, CaseParticipantDto>();

            // ======================================================
            // 🔹 CASE REPORTS
            // ======================================================
            CreateMap<CaseReport, CaseReportDto>()
                .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.Case.Name))
                .ForMember(dest => dest.ReportTitle, opt => opt.MapFrom(src => src.Report.Title))
                .ForMember(dest => dest.AreaCity, opt => opt.MapFrom(src => src.Report.AreaCity));
            CreateMap<CreateCaseReportDto, CaseReport>();
            CreateMap<UpdateCaseReportDto, CaseReport>();

            // ======================================================
            // 🔹 CRIME REPORTS
            // ======================================================
            CreateMap<CrimeReport, CrimeReportDto>();
            CreateMap<CreateCrimeReportDto, CrimeReport>();

            // ======================================================
            // 🔹 PARTICIPANTS
            // ======================================================
            CreateMap<Participant, ParticipantDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role)) //  Fixed
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.AddedByUserId, opt => opt.MapFrom(src => src.AddedByUserId))
                .ForMember(dest => dest.AddedOn, opt => opt.MapFrom(src => src.AddedOn));

            CreateMap<CreateParticipantDto, Participant>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.AddedByUserId, opt => opt.MapFrom(src => src.AddedByUserId))
                .ForMember(dest => dest.AddedOn, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateParticipantDto, Participant>()
                .ForMember(dest => dest.FullName, opt => opt.Condition(src => src.FullName != null))
                .ForMember(dest => dest.Role, opt => opt.Condition(src => src.Role != null))
                .ForMember(dest => dest.Phone, opt => opt.Condition(src => src.Phone != null))
                .ForMember(dest => dest.Notes, opt => opt.Condition(src => src.Notes != null));

            // ======================================================
            // 🔹 EVIDENCE
            // ======================================================
            CreateMap<Evidence, EvidenceDto>()
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.AddedByUserId, opt => opt.MapFrom(src => src.AddedByUserId))
                .ForMember(dest => dest.AddedByUserName, opt => opt.MapFrom(src => src.AddedByUser != null ? src.AddedByUser.FullName : "System"))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.MimeType))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.AddedAt))
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.AuditLogs, opt => opt.MapFrom(src => src.AuditLogs))
                .ReverseMap();

            CreateMap<CreateEvidenceDto, Evidence>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.AuditLogs, opt => opt.Ignore());

            // ======================================================
            // 🔹 EVIDENCE AUDIT LOGS
            // ======================================================
            CreateMap<EvidenceAuditLog, EvidenceAuditDto>()
                .ForMember(dest => dest.EvidenceId, opt => opt.MapFrom(src => src.EvidenceId))
                .ForMember(dest => dest.ActedByUserId, opt => opt.MapFrom(src => src.ActedByUserId))
                .ForMember(dest => dest.ActedByUserName, opt => opt.MapFrom(src => src.ActedByUser != null ? src.ActedByUser.FullName : "System"))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details))
                .ForMember(dest => dest.ActedAt, opt => opt.MapFrom(src => src.ActedAt))
                .ReverseMap();

            CreateMap<EvidenceAuditDto, EvidenceAuditLog>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ActedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.ActedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // ======================================================
            // 🔹 COMMENTS (CASE-SPECIFIC)
            // ======================================================
            CreateMap<CreateCommentDto, CaseComment>();
            CreateMap<CaseComment, CommentDto>();
            CreateMap<UpdateCaseCommentDto, CaseComment>();

            // ======================================================
            // 🔹 GENERAL COMMENTS
            // ======================================================
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.User.Role));
            CreateMap<CreateCommentDto, Comment>();
            CreateMap<UpdateCommentDto, Comment>();

            // ======================================================
            // 🔹 DELETION STATUS
            // ======================================================
            CreateMap<DeletionStatus, DeletionStatusDto>().ReverseMap();
            CreateMap<StartDeletionRequestDto, DeletionStatus>();
            CreateMap<DeletionResultDto, DeletionStatus>();

            // ======================================================
            // 🔹 EMAIL SETTINGS
            // ======================================================
            CreateMap<EmailSettings, EmailSettingsDto>().ReverseMap();
            CreateMap<EmailMessageDto, EmailSettings>();
            CreateMap<EmailNotificationDto, EmailSettings>();
        }
    }
}
