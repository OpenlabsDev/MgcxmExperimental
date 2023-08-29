using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Common.Framework;

public sealed class EndpointSubpartAttribute : Attribute
{
    public EndpointSubpartAttribute(string controller)
    {
        Controller = controller;
    }

    public string Controller { get; set; }
}