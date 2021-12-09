using Microsoft.AspNetCore.Identity;
using Provis.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provis.Core.Entities
{
    public class User : IdentityUser, IBaseEntity
    {
        
    }
}
