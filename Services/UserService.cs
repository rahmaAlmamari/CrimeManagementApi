using AutoMapper;
using BCrypt.Net;
using CrimeManagementApi.Data;
using CrimeManagementApi.DTOs;
using CrimeManagementApi.Models;
using CrimeManagementApi.Models.Enums;
using CrimeManagementApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrimeManagementApi.Services
{
    /// <summary>Handles user CRUD and authentication.</summary>
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> CreateAsync(CreateUserDto dto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists) return null;

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> UpdateAsync(int id, CreateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return null;

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.ClearanceLevel = Enum.TryParse<ClearanceLevel>(dto.ClearanceLevel, true, out var level)? level : ClearanceLevel.Low;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserDto?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return _mapper.Map<UserDto>(user);
        }
    }
}
