using Microsoft.EntityFrameworkCore;
using RestarRapsodya.Data;
using System.Security.Cryptography;
using System.Text;
using RestarRapsodya.Services;
using RestarRapsodya.Models;
using Markdig;

namespace RestarRapsodya.Models
{
    public class ServiciosRapsodya
    {
        private readonly RestarRapsodyaContext _context;
        private readonly IEmailService _emailService;

        public ServiciosRapsodya(RestarRapsodyaContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public ServiciosRapsodya(RestarRapsodyaContext context)
        {
            _context = context;
        }

        public string CifrarContraseña(string contraseña)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convertir la contraseña en un arreglo de bytes
                byte[] bytesContraseña = Encoding.UTF8.GetBytes(contraseña);

                // Calcular el hash de la contraseña
                byte[] hashContraseña = sha256Hash.ComputeHash(bytesContraseña);

                // Convertir el hash en una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashContraseña.Length; i++)
                {
                    sb.Append(hashContraseña[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public Usuario comprobarContrasenas(string correo, string contrasena)
        {
            Usuario usuario = _context.Usuario.Include(u => u.Rol).FirstOrDefault(u => u.Correo == correo);
            if (usuario != null && contrasena != null)
            {
                contrasena = CifrarContraseña(contrasena);
                if (contrasena == usuario.password) return usuario;
                else return null;
            }
            else return null;
        }

        public void MandarCorreo(string correo)
        {
            //Consultar el usuario en la BD con la informacion del Rol incluida
            Usuario usuario = _context.Usuario.Include(u => u.Rol).FirstOrDefault(u => u.Correo == correo);
            //Creacion del mensaje que se va a mandar con la informacion del usuario agregado 
            string fechaActual = DateTime.Now.ToString("yyyy-MM-dd");
            string cuerpo = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n" + "<style>\r\n" + "body {\r\n" + "font-family: Arial, sans-serif;\r\n" + "color: #333;\r\n}\r\n\r\n" +"h1 {\r\ncolor: #008080;\r\n}\r\n\r\n" +".content {\r\nmargin: 20px 0;\r\n}\r\n\r\n" +".footer{\r\ncolor: #888;\r\nfont-size: 12px;\r\nmargin-top: 30px;\r\n}\r\n</style>\r\n" +"</head>\r\n<body>\r\n<h1>Creación de un nuevo " + usuario.Rol.nombre_Rol + "</h1>\r\n" +"<div class=\"content\">\r\n<p>El <strong>" + fechaActual + "</strong>" +" Se registró un nuevo usuario en el restaurante Rapsodia." +"Sus datos personales son:</p>\r\n<ul>\r\n<li>" +"<strong>Nombre: </strong>" + usuario.Nombre + "</li>\r\n<li>" +"<strong>Correo: </strong>" + usuario.Correo + "</li>\r\n<li>" +"<strong>Teléfono: </strong>" + usuario.Telefono + "</li>\r\n</ul>\r\n</div>\r\n" +"<div class=\"footer\">\r\nCorreo notificado para el Administrador automáticamente" +"desde la página Rapsodia\r\n</div>\r\n</body>\r\n</html>";
            //Se crea un objeto de EmailDTO para dale la informacion que se va a mandar
            //con los servicios de Email creados
            EmailDTO solicitud = new EmailDTO();
            //Definimos a quien le mandamos el mensaje
            solicitud.Para = "Pardomiguel28@gmail.com";
            //Definimos el asunto del mensaje
            solicitud.Asunto = "Nuevo " + usuario.Rol.nombre_Rol + " Registrado";
            //Definimos el contenido del mensaje (el CuerpoHTML antes creado)
            solicitud.Contenido = cuerpo;
            _emailService.SendEmail(solicitud);
        }
    }
}
