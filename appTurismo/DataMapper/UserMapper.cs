using appTurismo.Models;
using appTurismo.Services;

namespace appTurismo.DataMapper
{
    public class UserMapper
    {
        //Might delete the async if it proves unnecessary at final release, but currently no issues so it stays

        private IStorageService _storageService;
        public UserMapper(IStorageService storageService)
        {
            _storageService = storageService;
        }
        public async Task<UserDTO> UserToDTO(Models.Supabase.User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),
                    "user cannot be null");
            }

            return new UserDTO()
            {
                IdUsuario = user.Id_usuario,

                Nombre = user.Nombre,

                ApellidoPaterno = user.Apellido_paterno,

                ApellidoMaterno = user.Apellido_materno,

                CorreoElectronico = user.Correo_electronico,

                Telefono = user.Telefono,

                IdRol = user.Id_rol,

                UltimaLatitud = user.Ultima_latitud,

                UltimaLongitud = user.Ultima_longitud,

                UltimaActualizacion = user.Ultima_actualizacion,

                CreatedAt = user.Created_at
            };
        }
    }
}