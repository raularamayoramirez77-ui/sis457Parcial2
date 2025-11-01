using CadParcial2Jrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ClnParcial2Lfms
{
    public class CanalCln
    {
        public static List<Canal> listar()
        {
            using (var context = new Parcial2JrarEntities())
            {
                return context.Canal
                    .Where(x => x.estado == 1)
                    .OrderBy(x => x.nombre)
                    .ToList();
            }
        }
    }
}