using Microsoft.EntityFrameworkCore;
using RestarRapsodya.Data;
using RestarRapsodya.Models;
using RestarRapsodya.Services;

namespace RestarRapsodya.Models
{
    public class SeedData
    {
    
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new RestarRapsodyaContext(
                serviceProvider.GetRequiredService<DbContextOptions<RestarRapsodyaContext>>()))
            {
                ServiciosRapsodya servicios = new ServiciosRapsodya(context);
                if (context.Estado.Any() || context.Usuario.Any() || context.Rol.Any())
                {
                    return;
                }
                context.Estado.AddRange(
                    new Estado
                    {
                        estado = "Libre"
                    },
                    new Estado
                    {
                        estado = "Ocupado"
                    }
                );
                context.SaveChanges();
                context.Rol.AddRange(
                    new Rol
                    {
                        nombre_Rol = "Administrador"
                    },
                    new Rol
                    {
                        nombre_Rol = "Cliente"
                    }
                    );
                context.SaveChanges();
                context.Usuario.Add(new Usuario
                {
                    Nombre = "Miguel Pardo",
                    Correo = "pardomiguel28@gmail.com",
                    password = servicios.CifrarContraseña("12345678"),
                    Telefono = "3007478159",
                    id_Rol = 1
                });
                context.SaveChanges();

            }
        }
    }
}
