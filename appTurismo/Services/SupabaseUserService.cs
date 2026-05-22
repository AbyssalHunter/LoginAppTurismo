using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using appTurismo.DataMapper;
using appTurismo.Models;

namespace appTurismo.Services
{
    public class SupabaseUserService : IUserService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly UserMapper _userMapper;

        public SupabaseUserService(Supabase.Client supabaseClient, UserMapper userMapper)
        {
            _supabaseClient = supabaseClient;
            _userMapper = userMapper;
        }

        public bool IsUserLoggedIn() => _supabaseClient.Auth.CurrentSession != null;

        public async Task<string?> LoginAsync(string email, string password)
        {
            try
            {
                // 1. Autenticación normal contra Supabase Auth
                var session = await _supabaseClient.Auth.SignIn(email, password);
                if (session?.User == null) return null;

                // 2. 💡 CAMBIO AQUÍ: Añadimos <string> al Rpc para recibir el texto limpio
                var userRole = await _supabaseClient.Rpc<string>("get_user_role", new Dictionary<string, object>
        {
            { "user_uuid", Guid.Parse(session.User.Id) }
        });

                // 3. Regresa el rol en minúsculas y sin comillas extrañas ("guia" o "turista")
                return userRole?.ToLower().Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Login Error]: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string email, string password, Models.Supabase.User profileData)
        {
            return await RegisterWithRoleAsync(email, password, profileData, "turista");
        }

        public async Task<bool> RegisterWithRoleAsync(string email, string password, Models.Supabase.User profileData, string roleName)
        {
            try
            {
                // 1. Obtener ID del Rol usando el RPC tipado PRIMERO para asegurar datos antes del SignUp
                var resolvedRoleIdStr = await _supabaseClient.Rpc<string>("get_role_id_by_name", new Dictionary<string, object>
        {
            { "role_name", roleName.ToLower().Trim() }
        });

                if (string.IsNullOrEmpty(resolvedRoleIdStr))
                {
                    Debug.WriteLine($"[REGISTRO]: El RPC no encontró ningún ID para el rol '{roleName}'.");
                    return false;
                }
                Guid resolvedRoleId = Guid.Parse(resolvedRoleIdStr);

                // 2. Registro en el sistema de Autenticación de Supabase
                var response = await _supabaseClient.Auth.SignUp(email, password);
                if (response?.User == null)
                {
                    Debug.WriteLine("[REGISTRO]: Supabase Auth no devolvió un usuario válido.");
                    return false;
                }

                // 3. Poblamos el perfil de manera explícita y segura
                profileData.Id_usuario = Guid.Parse(response.User.Id);
                profileData.Correo_electronico = email;
                profileData.Id_rol = resolvedRoleId;
                profileData.Created_at = DateTime.UtcNow;

                // Forzamos que los valores geográficos vayan como nulos explícitos si no se usan
                profileData.Ultima_latitud = null;
                profileData.Ultima_longitud = null;
                profileData.Ultima_actualizacion = null;

                // 4. Inserción en la tabla pública usuarios
                await CreateUserAsync(profileData);
                return true;
            }
            catch (Supabase.Postgrest.Exceptions.PostgrestException pgEx)
            {
                Debug.WriteLine("==================================================");
                Debug.WriteLine($"[ERROR POSTGRESQL]: {pgEx.Message}");
                Debug.WriteLine("==================================================");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR GENERAL]: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Models.UserDTO>> GetUsersAsync()
        {
            try
            {
                var response = await _supabaseClient.From<Models.Supabase.User>().Get();
                var users = response.Models;
                var usersDTO = new List<Models.UserDTO>();
                foreach (var user in users)
                {
                    usersDTO.Add(await _userMapper.UserToDTO(user));
                }
                return usersDTO;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving users: {ex.Message}");
                return new List<Models.UserDTO>();
            }
        }

        public async Task CreateUserAsync(Models.Supabase.User user) =>
            await _supabaseClient.From<Models.Supabase.User>().Insert(user);

        public async Task UpdateUserAsync(Models.Supabase.User user)
        {
            await _supabaseClient
                .From<Models.Supabase.User>()
                .Where(x => x.Id_usuario == user.Id_usuario)
                .Set(x => x.Nombre, user.Nombre)
                .Set(x => x.Apellido_paterno, user.Apellido_paterno)
                .Set(x => x.Apellido_materno, user.Apellido_materno)
                .Set(x => x.Correo_electronico, user.Correo_electronico)
                .Set(x => x.Telefono, user.Telefono)
                .Update();
        }

        public async Task DeleteUserAsync(Guid id) =>
            await _supabaseClient.From<Models.Supabase.User>().Where(x => x.Id_usuario == id).Delete();

        public async Task LogoutAsync()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Logout Error]: {ex.Message}");
            }
        }
    }
}