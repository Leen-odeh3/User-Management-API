﻿using GPMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPMS.Core.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
